using System;
using System.Collections.Generic;

using SmoothDownloader.Mkv;

namespace SmoothDownloader.Download
{
    internal class DownloadingMediaDataSource : IMediaDataSource, IStoppable
    {
        private readonly DisplayDuration _displayDuration;
        private readonly bool _isLive;
        private readonly string _manifestParentPath;
        private readonly ulong _minStartTime = ulong.MaxValue;
        private readonly ulong _stopAfter;
        private readonly ulong _timeScale;

        private readonly ulong _totalTicks; // For ETA calculation.

        private readonly MediaDataBlock[] _trackFirstBlocks;
        private readonly byte[][] _trackFirstFileDatas;

        // Usually Tracks has 2 elements: one for the video track and one for the audio track.
        private readonly IList<Track> _tracks;

        // Usually TrackSamples has 2 elements: one for the video track and one for the audio track.
        private readonly IList<MediaSample>[] _trackSamples;
        private readonly int[] _trackSampleStartIndexes;
        private IChunkStartTimeReceiver _chunkStartTimeReceiver;
        private ulong _totalDuration;
        public volatile bool IsStopped;

        /// <summary>
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="manifestParentPath"></param>
        /// <param name="timeScale"></param>
        /// <param name="isLive"></param>
        /// <param name="stopAfter"></param>
        /// <param name="totalTicks"></param>
        /// <param name="displayDuration"></param>
        public DownloadingMediaDataSource(IList<Track> tracks, string manifestParentPath,
            ulong timeScale, bool isLive, ulong stopAfter, ulong totalTicks,
            DisplayDuration displayDuration)
        {
            _tracks = tracks;
            _manifestParentPath = manifestParentPath;
            _timeScale = timeScale;
            _displayDuration = displayDuration;
            _isLive = isLive;
            _stopAfter = stopAfter;

            for (var i = 0; i < tracks.Count; ++i)
            {
                var chunkStartTime = tracks[i].NextStartTime;
                if (_minStartTime > chunkStartTime) _minStartTime = chunkStartTime;
            }

            _totalTicks = totalTicks;
            _trackSamples = new IList<MediaSample>[tracks.Count];

            for (var i = 0; i < tracks.Count; ++i) _trackSamples[i] = new List<MediaSample>();

            _trackSampleStartIndexes = new int[tracks.Count]; // Items initialized to 0.
            _trackFirstBlocks = new MediaDataBlock[tracks.Count]; // Items initialized to null.
            _trackFirstFileDatas = new byte[tracks.Count][];
            _chunkStartTimeReceiver = null;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public int GetTrackCount()
        {
            return _trackFirstBlocks.Length;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <returns></returns>
        public ulong GetTrackEndTime(int trackIndex)
        {
            return _tracks[trackIndex].NextStartTime;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="startTimeReceiver"></param>
        public void StartChunks(IChunkStartTimeReceiver startTimeReceiver)
        {
            _chunkStartTimeReceiver = startTimeReceiver;

            for (var trackIndex = 0; trackIndex < _tracks.Count; ++trackIndex)
            {
                _chunkStartTimeReceiver.SetChunkStartTime(trackIndex, _tracks[trackIndex].DownloadedChunkCount, _tracks[trackIndex].NextStartTime);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <returns></returns>
        public MediaDataBlock PeekBlock(int trackIndex)
        {
            if (_trackFirstBlocks[trackIndex] != null)
            {
                return _trackFirstBlocks[trackIndex];
            }

            var mediaSamples = _trackSamples[trackIndex];
            var sampleIndex = _trackSampleStartIndexes[trackIndex];

            if (sampleIndex >= mediaSamples.Count)
            {
                if (DownloadNextChunk(trackIndex))
                {
                    sampleIndex = 0;
                }
                else
                {
                    return null;
                }
            }

            var mediaSample = mediaSamples[sampleIndex];

            mediaSamples[sampleIndex] = null;

            return _trackFirstBlocks[trackIndex] = new MediaDataBlock(
                new ArraySegment<byte>(_trackFirstFileDatas[trackIndex], (int)mediaSample.Offset, mediaSample.Length),
                mediaSample.StartTime,
                mediaSample.IsKeyFrame
            );
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        public void ConsumeBlock(int trackIndex)
        {
            if (_trackFirstBlocks[trackIndex] == null && PeekBlock(trackIndex) == null)
            {
                throw new Exception();
            }

            _trackFirstBlocks[trackIndex] = null;

            if (++_trackSampleStartIndexes[trackIndex] < _trackSamples[trackIndex].Count)
            {
                return;
            }

            _trackSamples[trackIndex].Clear();
            _trackFirstFileDatas[trackIndex] = null;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <param name="startTime"></param>
        public void ConsumeBlocksUntil(int trackIndex, ulong startTime)
        {
            var startIndex = _trackSampleStartIndexes[trackIndex];
            var mediaSamples = _trackSamples[trackIndex];
            var mediaSampleCount = mediaSamples.Count;

            if (_trackFirstBlocks[trackIndex] != null)
            {
                if (_trackFirstBlocks[trackIndex].StartTime > startTime)
                {
                    return;
                }

                _trackFirstBlocks[trackIndex] = null;

                if (startIndex < mediaSampleCount && mediaSamples[startIndex].StartTime > startTime)
                {
                    throw new Exception();
                }
            }

            var track = _tracks[trackIndex];

            if (startIndex < mediaSampleCount && _chunkStartTimeReceiver.GetChunkStartTime(trackIndex, track.DownloadedChunkCount) > startTime)
            {
                for (; startIndex < mediaSampleCount; ++startIndex)
                {
                    if (mediaSamples[startIndex].StartTime <= startTime)
                    {
                        continue;
                    }

                    _trackSampleStartIndexes[trackIndex] = startIndex;

                    return;
                }
            }

            mediaSamples.Clear();
            _trackFirstFileDatas[trackIndex] = null;
            _trackSampleStartIndexes[trackIndex] = 0;

            ulong nextNextChunkStartTime;

            while ((nextNextChunkStartTime = _chunkStartTimeReceiver.GetChunkStartTime(trackIndex, track.DownloadedChunkCount + 1)) <= startTime)
            {
                track.NextStartTime = nextNextChunkStartTime;
                ++track.DownloadedChunkCount;
            }

            while (DownloadNextChunk(trackIndex))
            {
                mediaSampleCount = mediaSamples.Count;

                if (mediaSampleCount == 0)
                {
                    throw new Exception();
                }

                for (startIndex = 0; startIndex < mediaSampleCount; ++startIndex)
                {
                    if (mediaSamples[startIndex].StartTime <= startTime)
                    {
                        continue;
                    }

                    _trackSampleStartIndexes[trackIndex] = startIndex;

                    return;
                }

                mediaSamples.Clear();
            }

            _trackSamples[trackIndex].Clear();
            _trackFirstFileDatas[trackIndex] = null;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Stop()
        {
            IsStopped = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <returns></returns>
        private bool DownloadNextChunk(int trackIndex)
        {
            var mediaSamples = _trackSamples[trackIndex];
            var track = _tracks[trackIndex];

            if (track.DownloadedChunkCount >= track.TrackInfo.Stream.ChunkCount && _isLive == false || IsStopped)
            {
                return false;
            }

            _trackSampleStartIndexes[trackIndex] = 0;
            mediaSamples.Clear();

            while (mediaSamples.Count == 0)
            {
                var chunkIndex = track.DownloadedChunkCount;

                if (chunkIndex >= track.TrackInfo.Stream.ChunkCount && !_isLive || IsStopped)
                {
                    return false;
                }

                var chunkStartTime = track.NextStartTime;

                if (_isLive && chunkStartTime * 1e7 / _timeScale >= _stopAfter)
                {
                    return false;
                }

                if (track.TrackInfo.Stream.ChunkList.Count > chunkIndex && chunkStartTime != track.TrackInfo.Stream.ChunkList[chunkIndex].StartTime)
                {
                    throw new Exception();
                }

                var contents = Downloader.DownloadChunk(track.TrackInfo, mediaSamples, chunkStartTime, _manifestParentPath, _isLive, out track.NextStartTime);

                if (contents == null)
                {
                    IsStopped = true;
                    throw new Exception();
                }

                ++track.DownloadedChunkCount;

                if (track.TrackInfo.Stream.ChunkList.Count > chunkIndex && track.NextStartTime != chunkStartTime + track.TrackInfo.Stream.ChunkList[chunkIndex].Duration)
                {
                    throw new Exception();
                }

                _chunkStartTimeReceiver.SetChunkStartTime(trackIndex, track.DownloadedChunkCount, track.NextStartTime);
                _trackFirstFileDatas[trackIndex] = contents;

                var trackTotalDuration = track.NextStartTime - _minStartTime;

                if (_totalDuration >= trackTotalDuration)
                {
                    continue;
                }

                _totalDuration = trackTotalDuration;
                _displayDuration((ulong)(_totalDuration * 1e7 / _timeScale), _totalTicks);
            }

            return true;
        }
    }
}