using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

using SmoothDownloader.Mkv;
using SmoothDownloader.Smooth;

namespace SmoothDownloader.Download
{
    /// <summary>
    /// </summary>
    /// <param name="isLive"></param>
    /// <param name="stoppable"></param>
    public delegate void SetupStop(bool isLive, IStoppable stoppable);

    /// <summary>
    /// </summary>
    /// <param name="reachedTicks"></param>
    /// <param name="totalTicks"></param>
    public delegate void DisplayDuration(ulong reachedTicks, ulong totalTicks);

    /// <summary>
    /// </summary>
    public class Downloader
    {
        /// <summary>
        /// </summary>
        private const string LocalUrlPrefix = "http://local/";

        /// <summary>
        /// </summary>
        /// <param name="manifestUri"></param>
        /// <param name="manifestPath"></param>
        /// <param name="mkvPath"></param>
        /// <param name="isDeterministic"></param>
        /// <param name="stopAfter"></param>
        /// <param name="setupStop"></param>
        /// <param name="displayDuration"></param>
        public static void DownloadAndMux(Uri manifestUri, string manifestPath, string mkvPath, bool isDeterministic, TimeSpan stopAfter, SetupStop setupStop, DisplayDuration displayDuration)
        {
            string manifestParentPath = null;
            ManifestInfo manifestInfo;

            if (manifestPath != null)
            {
                manifestParentPath = Path.GetDirectoryName(manifestPath);
                Console.WriteLine($"Parsing local manifest file: {manifestPath}");

                using (var manifestStream = new FileStream(manifestPath, FileMode.Open))
                {
                    manifestInfo = ManifestInfo.ParseManifest(manifestStream, new Uri(LocalUrlPrefix));
                }
            }
            else
            {
                Console.WriteLine($"Downloading and parsing manifest: {manifestUri}");

                using (var manifestStream = new WebClient().OpenRead(manifestUri))
                {
                    manifestInfo = ManifestInfo.ParseManifest(manifestStream, manifestUri);
                }
            }

            Console.Write(manifestInfo.GetDescription());

            var tracks = (
                from streamInfo in manifestInfo.SelectedStreams
                from trackInfo in streamInfo.SelectedTracks
                select new Track(trackInfo)
            ).ToList();

            var trackEntries = new List<TrackEntry>();
            var trackSamples = new List<IList<MediaSample>>();

            for (var i = 0; i < tracks.Count; ++i)
            {
                trackEntries.Add(tracks[i].TrackInfo.TrackEntry);
                trackEntries[i].TrackNumber = (ulong)(i + 1);
                trackSamples.Add(new List<MediaSample>());
            }

            foreach (var track in tracks)
            {
                if (track.TrackInfo.Stream.ChunkList.Count == 0)
                {
                    track.NextStartTime = 0;
                }
                else
                {
                    track.NextStartTime = track.TrackInfo.Stream.ChunkList[0].StartTime;
                }
            }

            Console.WriteLine("Also muxing selected tracks to MKV: " + mkvPath);

            try
            {
                if (Directory.GetParent(mkvPath) != null &&
                    !Directory.GetParent(mkvPath).Exists)
                    Directory.GetParent(mkvPath).Create();
            }
            catch (IOException)
            {
                throw new Exception();
            }

            var maxTrackEndTimeHint = manifestInfo.Duration;

            foreach (var track in tracks)
            {
                var chunkInfos = track.TrackInfo.Stream.ChunkList;
                var lastIndex = chunkInfos.Count - 1;

                if (lastIndex < 0)
                {
                    continue;
                }

                var trackDuration = chunkInfos[lastIndex].StartTime + chunkInfos[lastIndex].Duration;

                if (maxTrackEndTimeHint < trackDuration)
                {
                    maxTrackEndTimeHint = trackDuration;
                }
            }

            var muxStatePath = Path.ChangeExtension(mkvPath, "muxstate");
            var muxStateOldPath = muxStatePath + ".old";
            byte[] previousMuxState = null;

            if (File.Exists(muxStatePath))
            {
                using (var stream = new FileStream(muxStatePath ?? throw new Exception(), FileMode.Open))
                {
                    previousMuxState = ReadFileStream(stream);
                }

                if (previousMuxState.Length > 0)
                {
                    try
                    {
                        File.Move(muxStatePath, muxStateOldPath);
                    }
                    catch (IOException)
                    {
                        File.Replace(muxStatePath, muxStateOldPath, null, true);
                    }
                }
            }

            var source = new DownloadingMediaDataSource(tracks, manifestParentPath, manifestInfo.TimeScale,
                manifestInfo.IsLive, (ulong)stopAfter.Ticks, manifestInfo.TotalTicks, displayDuration);
            setupStop(manifestInfo.IsLive, source);

            var muxStateWriter = new MuxStateWriter(new FileStream(muxStatePath, FileMode.Create));

            try
            {
                MkvUtils.WriteMkv(mkvPath, trackEntries, source, maxTrackEndTimeHint, manifestInfo.TimeScale, isDeterministic, previousMuxState, muxStateWriter);
            }
            finally
            {
                muxStateWriter.Close();
            }

            File.Delete(muxStatePath);

            if (File.Exists(muxStateOldPath))
            {
                File.Delete(muxStateOldPath);
            }
        }

        internal static byte[] DownloadChunk(TrackInfo trackInfo, IList<MediaSample> mediaSamples, ulong chunkStartTime,
            string manifestParentPath, bool isLive, out ulong nextStartTime)
        {
            nextStartTime = 0;
            var chunkUrl = trackInfo.Stream.GetChunkUrl(trackInfo.Bitrate, chunkStartTime);
            byte[] downloadedBytes;

            if (manifestParentPath != null)
            {
                if (!chunkUrl.StartsWith(LocalUrlPrefix))
                {
                    throw new Exception();
                }

                var chunkDownloadedPath = manifestParentPath + Path.DirectorySeparatorChar + chunkUrl.Substring(LocalUrlPrefix.Length).Replace('/', Path.DirectorySeparatorChar);

                using (var stream = new FileStream(chunkDownloadedPath, FileMode.Open))
                {
                    downloadedBytes = ReadFileStream(stream);
                }

                if (downloadedBytes.Length == 0)
                {
                    return null;
                }
            }
            else
            {
                var webClient = new WebClient();

                try
                {
                    downloadedBytes = webClient.DownloadData(chunkUrl);
                }
                catch
                {
                    Thread.Sleep(isLive ? 4000 : 2000);

                    try
                    {
                        downloadedBytes = webClient.DownloadData(chunkUrl);
                    }
                    catch
                    {
                        Thread.Sleep(isLive ? 6000 : 3000);

                        try
                        {
                            downloadedBytes = webClient.DownloadData(chunkUrl);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }

            if (downloadedBytes.Length == 0)
            {
                return null;
            }


            nextStartTime = ParseFragment(
                new Fragment(downloadedBytes, 0, downloadedBytes.Length),
                mediaSamples,
                trackInfo.Stream.Type,
                chunkStartTime
            );

            if (nextStartTime <= chunkStartTime)
            {
                throw new Exception();
            }

            return downloadedBytes;
        }

        private static byte[] ReadFileStream(Stream fileStream)
        {
            var streamLength = (int)fileStream.Length;

            if (streamLength <= 0)
            {
                return new byte[0];
            }

            var contents = new byte[streamLength];

            if (streamLength != fileStream.Read(contents, 0, streamLength))
            {
                throw new Exception();
            }

            if (fileStream.Read(contents, 0, 1) != 0)
            {
                throw new Exception();
            }

            return contents;
        }

        private static ulong ParseFragment(Fragment fragment, ICollection<MediaSample> samples, MediaStreamType type, ulong chunkStartTime)
        {
            var trackFragmentBox = fragment.MovieFragmentBox.TrackFragmentBox;

            if (trackFragmentBox.Tfxd != null)
            {
                chunkStartTime = trackFragmentBox.Tfxd.FragmentAbsoluteTime;
            }

            var nextStartTime = 0uL;

            if (trackFragmentBox.Tfrf != null && trackFragmentBox.Tfrf.Array.Length > 0u)
            {
                nextStartTime = trackFragmentBox.Tfrf.Array[0].FragmentAbsoluteTime;
            }

            long sampleOffset = fragment.MediaDataBox.Start;
            var defaultSampleSize = trackFragmentBox.Tfhd.DefaultSampleSize;
            var sampleSize = defaultSampleSize;
            var defaultSampleDuration = trackFragmentBox.Tfhd.DefaultSampleDuration;
            var duration = defaultSampleDuration;
            ulong totalDuration = 0;
            var sampleCount = trackFragmentBox.Trun.SampleCount;
            var array = defaultSampleSize == 0u || defaultSampleDuration == 0u ? trackFragmentBox.Trun.Array : null;

            for (uint i = 0; i < sampleCount; ++i)
            {
                if (defaultSampleSize == 0u) sampleSize = array[i].SampleSize;
                if (defaultSampleDuration == 0u) duration = array[i].SampleDuration;
                // We add a few dozen MediaSample entries for a chunk.
                samples.Add(new MediaSample(sampleOffset, (int)sampleSize, chunkStartTime,
                    /*isKeyFrame:*/i == 0 || type == MediaStreamType.Audio));
                chunkStartTime += duration;
                totalDuration += duration;
                sampleOffset += sampleSize;
            }

            return nextStartTime != 0uL ? nextStartTime : chunkStartTime;
        }
    }
}