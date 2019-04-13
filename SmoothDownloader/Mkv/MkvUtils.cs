using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using SmoothDownloader.Smooth;

namespace SmoothDownloader.Mkv
{
    public class MkvUtils
    {
        private const ulong DataSizeMaxValue = 72057594037927934uL;
        public static byte[] GetDataSizeBytes(ulong value)
        {
            if (value > DataSizeMaxValue)
            {
                throw new Exception();
            }
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            var b = 1;
            while (value > (1uL << (7 * b)) - 2)
            {
                ++b;
            }
            var array = new byte[b];
            Buffer.BlockCopy(bytes, 8 - b, array, 0, b);
            array[0] += (byte)(1 << (8 - b));
            return array;
        }

        private static byte[] GetDataSizeEightBytes(ulong value)
        {
            if (value > DataSizeMaxValue)
            {
                throw new Exception();
            }

            var bytes = BitConverter.GetBytes(value);

            Array.Reverse(bytes);
            bytes[0] = 1;

            return bytes;
        }

        public static byte[] GetVintBytes(ulong value)
        {
            var offset = 0;
            var bytes = BitConverter.GetBytes(value);

            Array.Reverse(bytes);

            while (bytes[offset] == 0 && offset + 1 < bytes.Length)
            {
                ++offset;
            }

            var contents = new byte[bytes.Length - offset];

            for (var index = 0; index < contents.Length; index++)
            {
                contents[index] = bytes[offset + index];
            }

            return contents;
        }
        public static byte[] GetFloatBytes(float value)
        {
            return Utils.InplaceReverseBytes(BitConverter.GetBytes(value));
        }
        private static readonly DateTime MinDateTimeValue = DateTime.Parse("2001-01-01").ToUniversalTime();
        public static byte[] GetDateTimeBytes(DateTime dateTime)
        {
            var dateTime2 = dateTime.ToUniversalTime();

            if (dateTime2 < MinDateTimeValue)
            {
                throw new Exception();
            }

            return Utils.InplaceReverseBytes(BitConverter.GetBytes(Convert.ToUInt64(dateTime2.Subtract(MinDateTimeValue).TotalMilliseconds * 1000000.0)));
        }

        public static byte[] GetEeBytes(MkvIdentifier id, byte[] contents)
        {
            return Utils.CombineBytes(GetDataSizeBytes((ulong)id), GetDataSizeBytes((ulong)contents.Length), contents);
        }

        private static byte[] GetEbmlHeaderBytes()
        {
            return GetEeBytes(MkvIdentifier.Ebml, Utils.CombineByteArrays(new List<byte[]>
            {
                GetEeBytes(MkvIdentifier.EbmlVersion, GetVintBytes(1uL)),
                GetEeBytes(MkvIdentifier.EbmlReadVersion, GetVintBytes(1uL)),
                GetEeBytes(MkvIdentifier.EbmlMaxIdLength, GetVintBytes(4uL)),
                GetEeBytes(MkvIdentifier.EbmlMaxSizeLength, GetVintBytes(8uL)),
                GetEeBytes(MkvIdentifier.DocType, Encoding.ASCII.GetBytes("matroska")),
                GetEeBytes(MkvIdentifier.DocTypeVersion, GetVintBytes(1uL)),
                GetEeBytes(MkvIdentifier.DocTypeReadVersion, GetVintBytes(1uL))
            }));
        }
        public static string GetStringForCodecId(MkvCodec mkvCodec)
        {
            switch (mkvCodec)
            {
            case MkvCodec.VideoAvc: { return "V_MPEG4/ISO/AVC"; }
            case MkvCodec.VideoMs: { return "V_MS/VFW/FOURCC"; }
            case MkvCodec.AudioAac: { return "A_AAC"; }
            case MkvCodec.AudioMs: { return "A_MS/ACM"; }
            default: { throw new Exception($"CodecID '{mkvCodec}' is invalid!"); }
            }
        }
        public static byte[] GetVideoInfoBytes(ulong pixelWidth, ulong pixelHeight, ulong displayWidth, ulong displayHeight)
        {
            if (pixelWidth == 0uL)
            {
                throw new Exception("PixelWidth must be greater than 0!");
            }
            if (pixelHeight == 0uL)
            {
                throw new Exception("PixelHeight must be greater than 0!");
            }
            if (displayWidth == 0uL)
            {
                throw new Exception("DisplayWidth must be greater than 0!");
            }
            if (displayHeight == 0uL)
            {
                throw new Exception("DisplayHeight must be greater than 0!");
            }

            var list = new List<byte[]>
            {
                GetEeBytes(MkvIdentifier.FlagInterlaced, GetVIntForFlag(false)),
                GetEeBytes(MkvIdentifier.PixelWidth, GetVintBytes(pixelWidth)),
                GetEeBytes(MkvIdentifier.PixelHeight, GetVintBytes(pixelHeight))
            };
            if (displayWidth != pixelWidth)
            {
                list.Add(GetEeBytes(MkvIdentifier.DisplayWidth, GetVintBytes(displayWidth)));
            }
            if (displayHeight != pixelHeight)
            {
                list.Add(GetEeBytes(MkvIdentifier.DisplayHeight, GetVintBytes(displayHeight)));
            }
            return GetEeBytes(MkvIdentifier.Video, Utils.CombineByteArrays(list));
        }
        public static byte[] GetAudioInfoBytes(float samplingFrequency, ulong channels, ulong bitDepth)
        {
            if (samplingFrequency <= 0f)
            {
                throw new Exception();
            }
            if (channels == 0uL)
            {
                throw new Exception();
            }

            var list = new List<byte[]>
            {
                GetEeBytes(MkvIdentifier.SamplingFrequency, GetFloatBytes(samplingFrequency)),
                GetEeBytes(MkvIdentifier.Channels, GetVintBytes(channels))
            };
            if (bitDepth != 0uL)
            {
                list.Add(GetEeBytes(MkvIdentifier.BitDepth, GetVintBytes(bitDepth)));
            }
            return GetEeBytes(MkvIdentifier.Audio, Utils.CombineByteArrays(list));
        }
        private static readonly byte[] VintFalse = { 0 };
        private static readonly byte[] VintTrue = { 1 };
        public static byte[] GetVIntForFlag(bool flag)
        {
            return flag ? VintTrue : VintFalse;
        }
        private static byte[] GetDurationBytes(ulong duration, ulong timeScale)
        {
            var floatDuration = Convert.ToSingle(duration * 1000.0 / timeScale);
            if (floatDuration <= 0f) floatDuration = 0.125f;  // .mkv requires a positive duration.
            var bytes = BitConverter.GetBytes(floatDuration);  // 4 bytes.
            Array.Reverse(bytes);
            return bytes;
        }

        private static byte[] GetSegmentInfoBytes(ulong duration, ulong timeScale, bool isDeterministic)
        {
            var name = Assembly.GetEntryAssembly().GetName();
            var muxingApp = name.Name + " v" + name.Version;
            var writingApp = muxingApp;
            byte[] segmentUid;
            if (isDeterministic)
            {
                segmentUid = new byte[]
                {
                    110, 104, 17, 204, 142, 130, 251, 240, 218, 112, 216, 160, 143, 114, 2, 237
                };
            }
            else
            {
                segmentUid = new byte[16];
                new Random().NextBytes(segmentUid);
            }
            var list = new List<byte[]>();

            list.Add(GetEeBytes(MkvIdentifier.Duration, GetDurationBytes(duration, timeScale)));
            if (!string.IsNullOrEmpty(muxingApp))
            {
                list.Add(GetEeBytes(MkvIdentifier.MuxingApp, Encoding.ASCII.GetBytes(muxingApp)));
            }
            if (!string.IsNullOrEmpty(writingApp))
            {
                list.Add(GetEeBytes(MkvIdentifier.WritingApp, Encoding.ASCII.GetBytes(writingApp)));
            }
            list.Add(GetEeBytes(MkvIdentifier.SegmentUid, segmentUid));

            var dateBytes = isDeterministic ? new byte[] { 4, 242, 35, 97, 249, 143, 0, 192 }
                : GetDateTimeBytes(DateTime.UtcNow);
            list.Add(GetEeBytes(MkvIdentifier.DateUtc, dateBytes));
            list.Add(GetEeBytes(MkvIdentifier.TimecodeScale, GetVintBytes(timeScale / 10uL)));
            return GetEeBytes(MkvIdentifier.Info, Utils.CombineByteArrays(list));
        }

        private static byte[] GetTrackEntriesBytes(IList<TrackEntry> trackEntries)
        {
            var byteArrays = new byte[trackEntries.Count][];
            for (var i = 0; i < trackEntries.Count; i++)
            {
                byteArrays[i] = trackEntries[i].GetBytes();
            }
            return GetEeBytes(MkvIdentifier.Tracks, Utils.CombineByteArrays(byteArrays));
        }

        private struct SeekBlock
        {
            public readonly MkvIdentifier Id;
            public readonly ulong Offset;
            public SeekBlock(MkvIdentifier id, ulong offset)
            {
                Id = id;
                Offset = offset;
            }
        }

        private static byte[] GetVoidBytes(ulong length)
        {
            if (length < 9uL)
            {
                throw new Exception();
            }

            length -= 9;

            return Utils.CombineBytes(GetDataSizeBytes((ulong)MkvIdentifier.Void), GetDataSizeEightBytes(length), new byte[length]);
        }

        private static byte[] GetSeekBytes(IList<SeekBlock> seekBlocks, int desiredSize)
        {
            var seekBlockCount = seekBlocks.Count;
            var byteArrays = new byte[4 * seekBlockCount + 3][];

            byteArrays[0] = GetDataSizeBytes((ulong)MkvIdentifier.SeekHead);

            for (int i = 0, j = 2; i < seekBlockCount; ++i, j += 4)
            {
                byteArrays[j] = GetDataSizeBytes((ulong)MkvIdentifier.Seek);
                byteArrays[j + 2] = GetEeBytes(MkvIdentifier.SeekId, GetDataSizeBytes((ulong)seekBlocks[i].Id));
                byteArrays[j + 3] = GetEeBytes(MkvIdentifier.SeekPosition, GetVintBytes(seekBlocks[i].Offset));
                byteArrays[j + 1] = GetDataSizeBytes((ulong)(byteArrays[j + 2].Length + byteArrays[j + 3].Length));
            }

            var dataSize = 0;
            var voidIndex = byteArrays.Length - 1;

            for (var i = 2; i < voidIndex; ++i)
            {
                dataSize += byteArrays[i].Length;
            }

            byteArrays[1] = GetDataSizeBytes((ulong)dataSize);
            byteArrays[voidIndex] = new byte[] { };

            if (desiredSize < 0)
            {
                return Utils.CombineByteArrays(byteArrays);
            }

            dataSize += byteArrays[0].Length + byteArrays[1].Length;

            if (desiredSize == dataSize)
            {
                return Utils.CombineByteArrays(byteArrays);
            }

            if (desiredSize <= dataSize + 9)
            {
                throw new Exception();
            }

            byteArrays[voidIndex] = GetVoidBytes((ulong)(desiredSize - dataSize));

            return Utils.CombineByteArrays(byteArrays);
        }

        private const int DesiredSeekSize = 90;

        private static byte[] GetSegmentBytes(ulong duration, ulong mediaEndOffsetMs,
            ulong seekHeadOffsetMs, ulong cuesOffsetMs,
            ulong timeScale, IList<TrackEntry> trackEntries, bool isDeterministic)
        {
            var byteArrays = new byte[5][];

            byteArrays[0] = GetDataSizeBytes((ulong)MkvIdentifier.Segment);  // 4 bytes.
            byteArrays[1] = GetDataSizeEightBytes(mediaEndOffsetMs);
            byteArrays[3] = GetSegmentInfoBytes(duration, timeScale, isDeterministic);
            byteArrays[4] = GetTrackEntriesBytes(trackEntries);

            IList<SeekBlock> seekBlocks = new List<SeekBlock>();
            seekBlocks.Add(new SeekBlock(MkvIdentifier.Info, DesiredSeekSize));
            seekBlocks.Add(new SeekBlock(MkvIdentifier.Tracks, (ulong)(DesiredSeekSize + byteArrays[3].Length)));
            if (seekHeadOffsetMs > 0) seekBlocks.Add(new SeekBlock(MkvIdentifier.SeekHead, seekHeadOffsetMs));
            if (cuesOffsetMs > 0) seekBlocks.Add(new SeekBlock(MkvIdentifier.Cues, cuesOffsetMs));

            byteArrays[2] = GetSeekBytes(seekBlocks, DesiredSeekSize);

            return Utils.CombineByteArrays(byteArrays);
        }

        private const ulong InitialMediaEndOffsetMs = ulong.MaxValue >> 9;
        private const ulong InitialSeekHeadOffsetMs = 0;
        private const ulong InitialCuesOffsetMs = 0;
        private const ulong KeepOriginalDuration = ulong.MaxValue - 1;

        /// <summary>
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="prefixSize"></param>
        /// <param name="segmentOffset"></param>
        /// <param name="mediaEndOffsetMs"></param>
        /// <param name="seekHeadOffsetMs"></param>
        /// <param name="cuesOffsetMs"></param>
        /// <param name="duration"></param>
        /// <param name="timeScale"></param>
        /// <returns></returns>
        private static int UpdatePrefix(byte[] prefix, int prefixSize,
            ulong segmentOffset, ulong mediaEndOffsetMs, ulong seekHeadOffsetMs, ulong cuesOffsetMs,
            ulong duration, ulong timeScale)
        {
            Buffer.BlockCopy(Utils.InplaceReverseBytes(BitConverter.GetBytes(mediaEndOffsetMs)), 1,
                prefix, (int)segmentOffset - 7, 7);
            FindDurationAndAfterInfoOffset(prefix, (int)segmentOffset, prefixSize, out var durationOffset, out var afterInfoOffset);
            if (duration != KeepOriginalDuration)
            {
                Buffer.BlockCopy(GetDurationBytes(duration, timeScale), 0, prefix, durationOffset, 4);
            }
            IList<SeekBlock> seekBlocks = new List<SeekBlock>();
            seekBlocks.Add(new SeekBlock(MkvIdentifier.Info, DesiredSeekSize));
            seekBlocks.Add(new SeekBlock(MkvIdentifier.Tracks, (ulong)afterInfoOffset - segmentOffset));
            if (seekHeadOffsetMs > 0) seekBlocks.Add(new SeekBlock(MkvIdentifier.SeekHead, seekHeadOffsetMs));
            if (cuesOffsetMs > 0) seekBlocks.Add(new SeekBlock(MkvIdentifier.Cues, cuesOffsetMs));
            var seekBytes = GetSeekBytes(seekBlocks, DesiredSeekSize);
            Buffer.BlockCopy(seekBytes, 0, prefix, (int)segmentOffset, seekBytes.Length);
            return durationOffset + 4;
        }

        private static int GetEbmlElementDataSize(byte[] bytes, ref int i)
        {
            if (bytes.Length <= i)
            {
                throw new Exception();
            }

            if ((bytes[i] & 0x80) != 0)
            {
                return bytes[i++] & 0x7f;
            }

            if ((bytes[i] & 0x40) != 0)
            {
                i += 2;
                if (bytes.Length < i)
                {
                    throw new Exception();
                }
                return (bytes[i - 2] & 0x3f) << 8 | bytes[i - 1];
            }

            if (bytes[i] != 1)
            {
                throw new Exception();
            }

            i += 8;

            if (bytes.Length < i)
            {
                throw new Exception();
            }

            if (bytes[i - 5] != 0 || bytes[i - 6] != 0 || bytes[i - 7] != 0 || (bytes[i - 4] & 0x80) != 0)
            {
                throw new Exception();
            }

            return bytes[i - 1] | bytes[i - 2] << 8 | bytes[i - 3] << 16 | bytes[i - 4] << 24;
        }

        private static void FindDurationAndAfterInfoOffset(byte[] bytes, int segmentOffset, int j, out int durationOffset, out int afterInfoOffset)
        {
            var i = segmentOffset;

            if (i + 4 <= j && bytes[i] == 17 && bytes[i + 1] == 77 && bytes[i + 2] == 155 && bytes[i + 3] == 116)
            {
                i += 4;
                var n = GetEbmlElementDataSize(bytes, ref i);  // Doesn't work (i becomes 68 instead of 76) without a helper.
                i += n;
            }

            if (i < j && bytes[i] == 236)
            {
                ++i;
                var n = GetEbmlElementDataSize(bytes, ref i);  // Doesn't work (i becomes 68 instead of 76) without a helper.
                i += n;
            }
            // Detect ID.Info.
            if (!(i + 4 <= j && bytes[i] == 21 && bytes[i + 1] == 73 && bytes[i + 2] == 169 && bytes[i + 3] == 102))
            {
                throw new Exception();
            }
            i += 4;
            var infoSize = GetEbmlElementDataSize(bytes, ref i);
            afterInfoOffset = i + infoSize;
            if (j > i + infoSize) j = i + infoSize;

            if (!(i + 2 <= j && bytes[i] == 68 && bytes[i + 1] == 137))
            {
                throw new Exception();
            }

            i += 2;

            var durationSize = GetEbmlElementDataSize(bytes, ref i);

            if (durationSize != 4)
            {
                throw new Exception();
            }

            durationOffset = i;
        }

        private static int GetVideoTrackIndex(IList<TrackEntry> trackEntries, int defaultIndex)
        {
            var videoTrackIndex = 0;

            while (videoTrackIndex < trackEntries.Count && trackEntries[videoTrackIndex].TrackType != MkvTrackType.Video)
            {
                ++videoTrackIndex;
            }

            return (videoTrackIndex == trackEntries.Count) ? defaultIndex : videoTrackIndex;
        }

        private static IList<bool> GetIsAmsCodecs(IEnumerable<TrackEntry> trackEntries)
        {
            return trackEntries.Select(entry => entry.MkvCodec == MkvCodec.AudioMs).ToList();
        }

        private static byte[] GetSimpleBlockBytes(ulong trackNumber, short timeCode, bool isKeyFrame, bool isAmsCodec,
            int mediaDataBlockTotalSize)
        {
            var b = isAmsCodec ? (byte)4 : (byte)0;

            if (isKeyFrame)
            {
                b += 128;
            }

            var output = new List<byte[]>
            {
                GetDataSizeBytes((ulong) MkvIdentifier.SimpleBlock),
                null,
                GetDataSizeBytes(trackNumber),
                Utils.InplaceReverseBytes(BitConverter.GetBytes(timeCode)),
                new[] {b}
            };

            if (isAmsCodec) output.Add(new[] { (byte)1 });

            var totalSize = 0;

            for (var offset = 2; offset < output.Count; ++offset)
            {
                totalSize += output[offset].Length;
            }

            output[1] = GetDataSizeBytes((ulong)(totalSize + mediaDataBlockTotalSize));

            return Utils.CombineByteArrays(output);
        }

        public static byte[] GetCueBytes(IList<CuePoint> cuePoints)
        {
            var output = new byte[cuePoints.Count][];

            for (var index = 0; index < cuePoints.Count; index++)
            {
                output[index] = cuePoints[index].GetBytes();
            }

            return GetEeBytes(MkvIdentifier.Cues, Utils.CombineByteArrays(output));
        }

        private class StateChunkStartTimeReceiver : IChunkStartTimeReceiver
        {
            private MuxStateWriter _muxStateWriter;
            private readonly IList<ulong>[] _trackChunkStartTimes;
            private readonly int[] _trackChunkWrittenCounts;

            public StateChunkStartTimeReceiver(MuxStateWriter muxStateWriter, IList<ulong>[] trackChunkStartTimes)
            {
                _muxStateWriter = muxStateWriter;
                _trackChunkStartTimes = trackChunkStartTimes;
                _trackChunkWrittenCounts = new int[trackChunkStartTimes.Length];  // Initializes items to 0.

                for (var trackIndex = 0; trackIndex < trackChunkStartTimes.Length; ++trackIndex)
                {
                    var startTimes = trackChunkStartTimes[trackIndex];

                    if (startTimes == null)
                    {
                        trackChunkStartTimes[trackIndex] = new List<ulong>();
                    }
                    else
                    {
                        for (var chunkIndex = 1; chunkIndex < startTimes.Count; ++chunkIndex)
                        {
                            if (startTimes[chunkIndex - 1] >= startTimes[chunkIndex])
                            {
                                throw new Exception();
                            }
                        }
                    }

                    _trackChunkWrittenCounts[trackIndex] = trackChunkStartTimes[trackIndex].Count;
                }
            }
            /*implements*/
            public ulong GetChunkStartTime(int trackIndex, int chunkIndex)
            {
                var chunkStartTimes = _trackChunkStartTimes[trackIndex];
                return chunkIndex >= chunkStartTimes.Count ? ulong.MaxValue : chunkStartTimes[chunkIndex];
            }

            public void SetChunkStartTime(int trackIndex, int chunkIndex, ulong startTime)
            {
                var numberChunks = _trackChunkStartTimes[trackIndex].Count;

                if (chunkIndex == numberChunks)
                {
                    var startTimes = _trackChunkStartTimes[trackIndex];

                    if (numberChunks > 0)
                    {
                        var lastChunkStartTime = startTimes[numberChunks - 1];
                        if (lastChunkStartTime >= startTime)
                        {
                            throw new Exception(string.Concat("New chunk StartTime not larger: track=", trackIndex, " chunk=", chunkIndex, " last=", lastChunkStartTime, " new=", startTime));
                        }
                    }

                    startTimes.Add(startTime);
                    ++numberChunks;

                    var i = _trackChunkWrittenCounts[trackIndex];
                    var key = (char)('n' + trackIndex);
                    if (i == 0) _muxStateWriter.WriteUlong(key, startTimes[i++]);
                    for (; i < numberChunks; ++i)
                    {
                        _muxStateWriter.WriteUlong(key, startTimes[i] - startTimes[i - 1]);
                    }

                    _trackChunkWrittenCounts[trackIndex] = i;
                }
                else if (chunkIndex < numberChunks)
                {
                    if (startTime != _trackChunkStartTimes[trackIndex][chunkIndex])
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="segmentOffset"></param>
        /// <param name="videoTrackIndex"></param>
        /// <param name="isAmsCodecs"></param>
        /// <param name="mediaDataSource"></param>
        /// <param name="muxStateWriter"></param>
        /// <param name="cuePoints"></param>
        /// <param name="minStartTime"></param>
        /// <param name="timePosition"></param>
        /// <param name="seekHeadOffsetMs"></param>
        /// <param name="cuesOffsetMs"></param>
        private static void WriteClustersAndCues(FileStream fileStream,
            ulong segmentOffset,
            int videoTrackIndex,
            IList<bool> isAmsCodecs,
            IMediaDataSource mediaDataSource,
            MuxStateWriter muxStateWriter,
            IList<CuePoint> cuePoints,
            ref ulong minStartTime,
            ulong timePosition,
            out ulong seekHeadOffsetMs,
            out ulong cuesOffsetMs)
        {
            var trackCount = mediaDataSource.GetTrackCount();

            if (isAmsCodecs.Count != trackCount || trackCount > 13)
            {
                throw new Exception();
            }

            var ungetBlocks = new MediaDataBlock[trackCount];
            var minStartTime0 = minStartTime;

            if (timePosition == ulong.MaxValue)
            {
                timePosition = 0;

                var maximumStartTime = ulong.MaxValue;

                for (var i = 0; i < trackCount; ++i)
                {
                    if ((ungetBlocks[i] = mediaDataSource.PeekBlock(i)) != null)
                    {
                        if (maximumStartTime == ulong.MaxValue || maximumStartTime < ungetBlocks[i].StartTime)
                        {
                            maximumStartTime = ungetBlocks[i].StartTime;
                        }
                        mediaDataSource.ConsumeBlock(i);  // Since it was moved to ungetBlocks[i].
                    }
                }
                for (var i = 0; i < trackCount; ++i)
                {
                    var block = mediaDataSource.PeekBlock(i);
                    while (block != null && block.StartTime <= maximumStartTime)
                    {
                        ungetBlocks[i] = block;  // Takes ownership.
                        mediaDataSource.ConsumeBlock(i);
                    }
                    // We'll start each track (in ungetMediaSample[i]) from the furthest sample within maxStartTime.
                }

                var trackIndex2 = GetNextTrackIndex(mediaDataSource, ungetBlocks);

                if (trackIndex2 < 0)
                {
                    throw new Exception();
                }
                minStartTime = minStartTime0 = ungetBlocks[trackIndex2] != null ? ungetBlocks[trackIndex2].StartTime :
                    mediaDataSource.PeekBlock(trackIndex2).StartTime;
                muxStateWriter.WriteUlong('A', minStartTime0);
            }
            var output = new List<ArraySegment<byte>>();
            var lastOutputStartTimes = new ulong[trackCount];  // Items initialized to zero.
            int trackIndex;
            // timePosition is the beginning StartTime of the last output block written by fileStream.Write.
            while ((trackIndex = GetNextTrackIndex(mediaDataSource, ungetBlocks)) >= 0)
            {
                ulong timeCode;  // Will be set below.
                bool isKeyFrame;  // Will be set below.
                MediaDataBlock block0;  // Will be set below.
                MediaDataBlock block1 = null;  // May be set below.
                int mediaDataBlockTotalSize;  // Will be set below.
                {
                    if ((block0 = ungetBlocks[trackIndex]) == null &&
                        (block0 = mediaDataSource.PeekBlock(trackIndex)) == null)
                    {
                        throw new Exception();
                    }
                    // Some kind of time delta for this sample.
                    timeCode = block0.StartTime - timePosition - minStartTime0;
                    if (block0.StartTime < timePosition + minStartTime0)
                    {
                        throw new Exception();
                    }
                    isKeyFrame = block0.IsKeyFrame;
                    mediaDataBlockTotalSize = block0.Bytes.Count;
                    if (ungetBlocks[trackIndex] != null)
                    {
                        ungetBlocks[trackIndex] = null;
                    }
                    else
                    {
                        mediaDataSource.ConsumeBlock(trackIndex);
                    }
                }
                if (timeCode > 327670000uL)
                {
                    throw new Exception();
                }
                if (isAmsCodecs[trackIndex])
                {
                    block1 = ungetBlocks[trackIndex];

                    if (block1 != null)
                    {
                        mediaDataBlockTotalSize += block1.Bytes.Count;
                        ungetBlocks[trackIndex] = null;
                    }
                    else if ((block1 = mediaDataSource.PeekBlock(trackIndex)) != null)
                    {
                        mediaDataBlockTotalSize += block1.Bytes.Count;
                        mediaDataSource.ConsumeBlock(trackIndex);
                    }
                }
                // TODO: How can be timeCode so large at this point?
                if ((output.Count != 0 && trackIndex == videoTrackIndex && isKeyFrame) || timeCode > 327670000uL)
                {
                    var outputOffset = (ulong)fileStream.Position - segmentOffset;
                    cuePoints.Add(new CuePoint(timePosition / 10000uL, (ulong)(videoTrackIndex + 1), outputOffset));
                    muxStateWriter.WriteUlong('C', timePosition);
                    muxStateWriter.WriteUlong('D', outputOffset);

                    var totalSize = output.Sum(t => t.Count);

                    var bytes = Utils.CombineByteArraysAndArraySegments(
                        new[] { GetDataSizeBytes((ulong)MkvIdentifier.Cluster), GetDataSizeBytes((ulong)totalSize) }, output);

                    output.Clear();
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Flush();

                    for (var i = 0; i < trackCount; ++i)
                    {
                        muxStateWriter.WriteUlong((char)('a' + i), lastOutputStartTimes[i]);
                    }

                    muxStateWriter.WriteUlong('P', (ulong)bytes.Length);
                    muxStateWriter.Flush();
                }
                if (output.Count == 0)
                {
                    timePosition += timeCode;
                    timeCode = 0uL;
                    output.Add(new ArraySegment<byte>(
                        GetEeBytes(MkvIdentifier.Timecode, GetVintBytes(timePosition / 10000uL))));
                }
                output.Add(new ArraySegment<byte>(GetSimpleBlockBytes(
                    (ulong)(trackIndex + 1), (short)(timeCode / 10000uL), isKeyFrame, isAmsCodecs[trackIndex],
                    mediaDataBlockTotalSize)));
                output.Add(block0.Bytes);
                if (block1 != null) output.Add(block1.Bytes);
                lastOutputStartTimes[trackIndex] = block1?.StartTime ?? block0.StartTime;
            }

            {
                var outputOffset = (ulong)fileStream.Position - segmentOffset;
                cuePoints.Add(new CuePoint(timePosition / 10000uL, (ulong)(videoTrackIndex + 1), outputOffset));
                muxStateWriter.WriteUlong('C', timePosition);
                muxStateWriter.WriteUlong('D', outputOffset);

                if (output.Count == 0)
                {
                    throw new Exception("ASSERT: Expecting non-empty output at end of mixing.");
                }

                var totalSize = output.Sum(t => t.Count);
                var bytes = Utils.CombineByteArraysAndArraySegments(
                    new[] { GetDataSizeBytes((ulong)MkvIdentifier.Cluster), GetDataSizeBytes((ulong)totalSize) }, output);

                output.Clear();  // Save memory.
                cuesOffsetMs = outputOffset + (ulong)bytes.Length;

                var bytes2 = GetCueBytes(cuePoints);  // cues are about 1024 bytes per 2 minutes.

                seekHeadOffsetMs = cuesOffsetMs + (ulong)bytes2.Length;

                var seekBlocks = new SeekBlock[cuePoints.Count];

                for (var i = 0; i < cuePoints.Count; ++i)
                {
                    seekBlocks[i] = new SeekBlock(MkvIdentifier.Cluster, cuePoints[i].CueClusterPosition);
                }

                bytes = Utils.CombineBytes(bytes, bytes2, GetSeekBytes(seekBlocks, -1));
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        private static int GetNextTrackIndex(IMediaDataSource mediaDataSource, MediaDataBlock[] ungetBlocks)
        {
            var trackIndex = -1;
            var minUnconsumedStartTime = 0uL;
            var trackCount = ungetBlocks.Length;

            for (var position = 0; position < trackCount; ++position)
            {
                var block = ungetBlocks[position] ?? mediaDataSource.PeekBlock(position);

                if (block == null || trackIndex != -1 && minUnconsumedStartTime <= block.StartTime)
                {
                    continue;
                }

                trackIndex = position;
                minUnconsumedStartTime = block.StartTime;
            }

            return trackIndex;
        }

        private const ulong MuxStateVersion = 923840374526694867;

        private static ParsedMuxState ParseMuxState(byte[] muxState, ulong oldSize, byte[] prefix, int prefixSize,
            int videoTrackIndex, int trackCount)
        {
            var parsedMuxState = new ParsedMuxState();
            if (muxState == null)
            {
                parsedMuxState.Status = "no mux state";
                return parsedMuxState;
            }
            if (muxState.Length == 0)
            {
                parsedMuxState.Status = "empty mux state";
                return parsedMuxState;
            }
            if (oldSize == 0)
            {
                parsedMuxState.Status = "empty old file";
                return parsedMuxState;
            }
            if (prefixSize == 0)
            {
                parsedMuxState.Status = "empty old prefix";
                return parsedMuxState;
            }

            var end = 0;
            byte b;
            var i = muxState.Length;
            int j;

            if (i > 0 && (b = muxState[i - 1]) != '\n' && b != '\r')
            {
                while (i > 0 && (b = muxState[i - 1]) != '\n' && b != '\r')
                {
                    --i;
                }
                if (i > 0) --i;
            }

            for (; ; )
            {  // Traverse the lines backwards.
                while (i > 0 && ((b = muxState[i - 1]) == '\n' || b == '\r'))
                {
                    --i;
                }
                if (i == 0) break;
                j = i;
                while (i > 0 && (b = muxState[i - 1]) != '\n' && b != '\r')
                {
                    --i;
                }

                if (muxState[i] == 'Z' || muxState[i] == 'P')
                {  // Stop just after the last line starting with Z or P.
                    end = j + 1;  // +1 for the trailing newline.
                    break;
                }
            }
            if (end == 0)
            {
                parsedMuxState.Status = "truncated to useless";
                return parsedMuxState;
            }
            parsedMuxState.EndOffset = end;

            var outState = -4;  // Output block state: -1 before V,
            parsedMuxState.LastC = ulong.MaxValue;
            var lastD = ulong.MaxValue;
            i = 0;
            if (i >= end || muxState[i] != 'X')
            {
                parsedMuxState.Status = "expected key X in the beginning";
                return parsedMuxState;
            }
            while (i < end)
            {
                var key = muxState[i++];
                if (key == '\r' || key == '\n') continue;
                var doCheckDup = false;
                if (key == 'X' || key == 'S' || key == 'V' || key == 'A' || key == 'M' || key == 'Z' ||
                    key == 'C' || key == 'D' || key == 'P' || (uint)(key - 'a') < 26)
                {
                    ulong v = 0;
                    while (i < end && (b = muxState[i]) != '\n' && b != '\r')
                    {
                        if (((uint)b - '0') > 9)
                        {
                            parsedMuxState.Status = "expected ulong for key " + (char)key;
                            return parsedMuxState;
                        }
                        if (v > (ulong.MaxValue - (ulong)(b - '0')) / 10)
                        {
                            parsedMuxState.Status = "ulong overflow for key " + (char)key;
                            return parsedMuxState;
                        }
                        v = 10 * v + (ulong)(b - '0');
                        ++i;
                    }
                    if (i == end)
                    {
                        parsedMuxState.Status = "EOF in key " + (char)key;
                        return parsedMuxState;
                    }
                    if (key == 'X' && outState == -4)
                    {
                        doCheckDup = parsedMuxState.HasX; parsedMuxState.HasX = true; parsedMuxState.Vx = v;
                        parsedMuxState.IsXGood = (v == MuxStateVersion);
                        if (!parsedMuxState.IsXGood)
                        {
                            parsedMuxState.Status = "unsupported format version (X)";
                            return parsedMuxState;
                        }
                    }
                    else if (key == 'S' && outState == -4)
                    {
                        doCheckDup = parsedMuxState.HasS; parsedMuxState.HasS = true; parsedMuxState.Vs = v;
                    }
                    else if (key == 'V' && outState == -4)
                    {
                        doCheckDup = parsedMuxState.HasV; parsedMuxState.HasV = true; parsedMuxState.Vv = v;
                        outState = -3;
                    }
                    else if (key == 'A' && outState == -3)
                    {
                        doCheckDup = parsedMuxState.HasA; parsedMuxState.HasA = true; parsedMuxState.Va = v;
                        outState = -2;
                    }
                    else if (key == 'M' && outState == 0)
                    {
                        doCheckDup = parsedMuxState.HasM; parsedMuxState.HasM = true; parsedMuxState.Vm = v;
                        outState = -5;
                    }
                    else if (key == 'Z' && outState == -5)
                    {
                        doCheckDup = parsedMuxState.HasZ; parsedMuxState.HasZ = true; parsedMuxState.Vz = v;
                    }
                    else if (key == 'C' && outState == -2)
                    {
                        outState = -1;
                        parsedMuxState.LastC = v;
                    }
                    else if (key == 'D' && outState == -1)
                    {
                        outState = 0;
                        if (parsedMuxState.CuePoints == null)
                        {
                            parsedMuxState.CuePoints = new List<CuePoint>();
                        }
                        parsedMuxState.CuePoints.Add(new CuePoint(
                            parsedMuxState.LastC / 10000uL, (ulong)(videoTrackIndex + 1), v));
                        lastD = v;
                        if (parsedMuxState.TrackLastStartTimes == null)
                        {
                            parsedMuxState.TrackLastStartTimes = new ulong[trackCount];  // Initialized to 0. Good.
                        }
                    }
                    else if ((uint)(key - 'a') < 13 && outState < trackCount && outState == key - 'a')
                    {
                        if (v <= parsedMuxState.TrackLastStartTimes[outState])
                        {
                            parsedMuxState.Status = "trackLastStart time values must increase, got " + v +
                                                    ", expected > " + parsedMuxState.TrackLastStartTimes[outState];
                            return parsedMuxState;
                        }
                        parsedMuxState.TrackLastStartTimes[outState] = v;
                        ++outState;
                    }
                    else if ((uint)(key - 'n') < 13 && outState >= -3)
                    {
                        if (parsedMuxState.TrackChunkStartTimes == null)
                        {
                            parsedMuxState.TrackChunkStartTimes = new IList<ulong>[trackCount];
                            for (var ti = 0; ti < trackCount; ++ti)
                            {
                                parsedMuxState.TrackChunkStartTimes[ti] = new List<ulong>();
                            }
                        }
                        var trackIndex = key - 'n';
                        var chunkCount = parsedMuxState.TrackChunkStartTimes[trackIndex].Count;
                        if (chunkCount > 0)
                        {
                            var lastChunkStartTime =
                                parsedMuxState.TrackChunkStartTimes[trackIndex][chunkCount - 1];
                            v += lastChunkStartTime;
                            if (lastChunkStartTime >= v)
                            {
                                parsedMuxState.Status = string.Concat("trackChunkStartTime values must increase, got ", v, ", expected > ", lastChunkStartTime, " for track ", trackIndex);
                                return parsedMuxState;
                            }
                        }
                        parsedMuxState.TrackChunkStartTimes[trackIndex].Add(v);
                    }
                    else if (key == 'P' && outState == trackCount)
                    {
                        outState = -2;
                        parsedMuxState.LastOutOfs = v + parsedMuxState.Vs + lastD;
                        lastD = ulong.MaxValue;  // A placeholder to expose future bugs.
                    }
                    else
                    {
                        parsedMuxState.Status = "unexpected key " + (char)key + " in outState " + outState;
                        return parsedMuxState;
                    }
                }
                else if (key == 'H')
                {
                    if (i == end || muxState[i] != ':')
                    {
                        parsedMuxState.Status = "expected colon after hex key " + (char)key;
                        return parsedMuxState;
                    }
                    j = ++i;
                    while (i > 0 && (b = muxState[i]) != '\n' && b != '\r')
                    {
                        ++i;
                    }
                    var bytes = Utils.HexDecodeBytes(muxState, j, i);
                    if (bytes == null)
                    {
                        parsedMuxState.Status = "parse error in hex key " + (char)key;
                        return parsedMuxState;
                    }
                    if (key == 'H' && outState == -4)
                    {
                        doCheckDup = parsedMuxState.HasH; parsedMuxState.HasH = true; parsedMuxState.Vh = bytes;
                    }
                    else
                    {
                        parsedMuxState.Status = "unexpected key " + (char)key + " in outState " + outState;
                        return parsedMuxState;
                    }
                }
                else
                {
                    parsedMuxState.Status = "unknown key " + (char)key;
                    return parsedMuxState;
                }
                if (doCheckDup)
                {
                    parsedMuxState.Status = "duplicate key " + (char)key;
                    return parsedMuxState;
                }
            }
            if (outState != -5 && outState != -2)
            {
                parsedMuxState.Status = "unexpected final outState " + outState;
                return parsedMuxState;
            }
            if (!parsedMuxState.HasV)
            {
                parsedMuxState.Status = "missing video track index (V)";
                return parsedMuxState;
            }
            if (parsedMuxState.Vv != (ulong)videoTrackIndex)
            {
                parsedMuxState.Status = "video track index (V) mismatch, expected " + videoTrackIndex;
                return parsedMuxState;
            }
            if (!parsedMuxState.HasH)
            {
                parsedMuxState.Status = "missing hex file prefix (H)";
                return parsedMuxState;
            }
            if (parsedMuxState.Vh.Length < 10)
            {
                // This shouldn't happen, because we read 4096 bytes below, and the header is usually just 404 bytes long.
                parsedMuxState.Status = "hex file prefix (H) too short";
                return parsedMuxState;
            }
            if (parsedMuxState.Vh.Length > prefixSize)
            {
                parsedMuxState.Status = "hex file prefix (H) too long, maximum prefix size is " + prefixSize;
                return parsedMuxState;
            }
            if (!parsedMuxState.HasS)
            {
                parsedMuxState.Status = "missing segmentOffset (S)";
                return parsedMuxState;
            }
            if (parsedMuxState.Vs < 10 || parsedMuxState.Vs > oldSize)
            {
                parsedMuxState.Status = "bad video track index (V) range";
                return parsedMuxState;
            }
            if (!parsedMuxState.HasA)
            {
                parsedMuxState.Status = "missing minStartTime (A)";
                return parsedMuxState;
            }
            if (parsedMuxState.HasZ)
            {
                if (parsedMuxState.Vz != 1)
                {
                    parsedMuxState.Status = "bad end marker (Z) value, expected 1";
                    return parsedMuxState;
                }
                if (!parsedMuxState.HasM)
                {
                    parsedMuxState.Status = "missing key M";
                    return parsedMuxState;
                }
                if (parsedMuxState.Vm != oldSize)
                {
                    parsedMuxState.Status = "old file size (M) mismatch, expected " + oldSize;
                    return parsedMuxState;
                }
            }

            if (!Utils.ArePrefixBytesEqual(parsedMuxState.Vh, prefix, parsedMuxState.Vh.Length))
            {
                var prefixCompareSize = parsedMuxState.Vh.Length;  // Shortness is checked above.
                var prefix1 = new byte[prefixCompareSize];
                Buffer.BlockCopy(parsedMuxState.Vh, 0, prefix1, 0, prefixCompareSize);
                var prefix2 = new byte[prefixCompareSize];
                Buffer.BlockCopy(prefix, 0, prefix2, 0, prefixCompareSize);
                UpdatePrefix(  // TODO: Catch exception if this fails.
                    prefix2, prefixCompareSize, parsedMuxState.Vs,
                    InitialMediaEndOffsetMs, InitialSeekHeadOffsetMs, InitialCuesOffsetMs,
                    KeepOriginalDuration, /*timeScale:*/0);
                int afterInfoOffset;  // Ignored, dummy.
                // TODO: Catch exception if this fails.
                FindDurationAndAfterInfoOffset(prefix1, (int)parsedMuxState.Vs, prefixCompareSize,
                    out var durationOffset, out afterInfoOffset);
                Buffer.BlockCopy(prefix1, durationOffset, prefix2, durationOffset, 4);
                if (!Utils.ArePrefixBytesEqual(prefix1, prefix2, prefixCompareSize))
                {
                    parsedMuxState.Status = "hex file prefix (H) mismatch";
                    return parsedMuxState;
                }
            }

            if (parsedMuxState.HasZ)
            {
                parsedMuxState.IsComplete = true;
                parsedMuxState.Status = "complete";
            }
            else if (parsedMuxState.CuePoints == null || parsedMuxState.CuePoints.Count == 0)
            {
                parsedMuxState.Status = "no cue points";
            }
            else if (parsedMuxState.LastOutOfs <= parsedMuxState.Vs)
            {
                parsedMuxState.Status = "no downloaded media data";
            }
            else if (parsedMuxState.LastOutOfs > oldSize)
            {
                parsedMuxState.Status = "file shorter than lastOutOfs";
            }
            else if (parsedMuxState.TrackChunkStartTimes == null)
            {
                parsedMuxState.Status = "no chunk start times";
            }
            else
            {
                if (parsedMuxState.TrackLastStartTimes == null)
                {
                    throw new Exception();
                }

                for (i = 0; i < trackCount; ++i)
                {
                    if (parsedMuxState.TrackLastStartTimes[i] == 0)
                    {
                        throw new Exception();
                    }
                }

                parsedMuxState.IsContinuable = true;
                parsedMuxState.Status = "continuable";
            }

            return parsedMuxState;
        }

        public static void WriteMkv(string mkvPath,
            IList<TrackEntry> trackEntries,
            IMediaDataSource mediaDataSource,
            ulong maxTrackEndTimeHint,
            ulong timeScale,
            bool isDeterministic,
            byte[] oldMuxState,
            MuxStateWriter muxStateWriter)
        {
            if (trackEntries.Count != mediaDataSource.GetTrackCount())
            {
                throw new Exception();
            }

            var doParseOldMuxState = oldMuxState != null && oldMuxState.Length > 0;
            var fileMode = doParseOldMuxState ? FileMode.OpenOrCreate : FileMode.Create;
            using (var fileStream = new FileStream(mkvPath, fileMode))
            {
                var oldSize = doParseOldMuxState ? (ulong)fileStream.Length : 0uL;
                var videoTrackIndex = GetVideoTrackIndex(trackEntries, 0);
                var isComplete = false;
                var isContinuable = false;
                ulong lastOutOfs = 0;
                ulong segmentOffset = 0;  // Will be overwritten below.
                IList<CuePoint> cuePoints = null;
                ulong minStartTime = 0;
                var timePosition = ulong.MaxValue;
                byte[] prefix = null; // Well be overwritten below.
                if (doParseOldMuxState && oldSize > 0)
                {
                    Console.WriteLine("Trying to use the old mux state to continue downloading.");
                    prefix = new byte[4096];
                    var prefixSize = fileStream.Read(prefix, 0, prefix.Length);
                    var parsedMuxState = ParseMuxState(
                        oldMuxState, oldSize, prefix, prefixSize, videoTrackIndex, trackEntries.Count);
                    if (parsedMuxState.IsComplete)
                    {
                        Console.WriteLine("The .mkv file is already fully downloaded.");
                        isComplete = true;
                        muxStateWriter.WriteRaw(oldMuxState, 0, oldMuxState.Length);
                    }
                    else if (parsedMuxState.IsContinuable)
                    {
                        Console.WriteLine("Continuing the .mkv file download.");
                        lastOutOfs = parsedMuxState.LastOutOfs;
                        segmentOffset = parsedMuxState.Vs;
                        cuePoints = parsedMuxState.CuePoints;
                        minStartTime = parsedMuxState.Va;
                        timePosition = parsedMuxState.LastC;
                        muxStateWriter.WriteRaw(oldMuxState, 0, parsedMuxState.EndOffset);
                        mediaDataSource.StartChunks(new StateChunkStartTimeReceiver(
                            muxStateWriter, parsedMuxState.TrackChunkStartTimes));
                        for (var i = 0; i < trackEntries.Count; ++i)
                        {
                            mediaDataSource.ConsumeBlocksUntil(i, parsedMuxState.TrackLastStartTimes[i]);
                        }
                        isContinuable = true;
                    }
                    else
                    {
                        Console.WriteLine("Could not use old mux state: " + parsedMuxState);
                    }
                }

                if (isComplete)
                {
                    return;
                }

                fileStream.SetLength((long)lastOutOfs);
                fileStream.Seek((long)lastOutOfs, 0);
                if (!isContinuable)
                {
                    prefix = GetEbmlHeaderBytes();
                    segmentOffset = (ulong)prefix.Length + 12;
                    muxStateWriter.WriteUlong('X', MuxStateVersion);  // Unique ID and version number.
                    muxStateWriter.WriteUlong('S', segmentOffset);  // About 52.
                    prefix = Utils.CombineBytes(prefix, GetSegmentBytes(
                        /*duration:*/maxTrackEndTimeHint,
                        InitialMediaEndOffsetMs, InitialSeekHeadOffsetMs, InitialCuesOffsetMs,
                        timeScale, trackEntries, isDeterministic));
                    fileStream.Write(prefix, 0, prefix.Length);  // Write the MKV header.
                    fileStream.Flush();
                    muxStateWriter.WriteBytes('H', prefix);  // About 405 bytes long.
                    muxStateWriter.WriteUlong('V', (ulong)videoTrackIndex);
                    cuePoints = new List<CuePoint>();
                    mediaDataSource.StartChunks(new StateChunkStartTimeReceiver(
                        muxStateWriter, new IList<ulong>[trackEntries.Count]));
                }

                WriteClustersAndCues(
                    fileStream, segmentOffset, videoTrackIndex, GetIsAmsCodecs(trackEntries),
                    mediaDataSource, muxStateWriter, cuePoints,
                    ref minStartTime, timePosition,
                    out var seekHeadOffsetMs,
                    out var cuesOffsetMs);

                fileStream.Flush();

                var maxTrackEndTime = 0uL;
                var mediaEndOffset = (ulong)fileStream.Position;

                muxStateWriter.WriteUlong('M', mediaEndOffset);

                for (var i = 0; i < mediaDataSource.GetTrackCount(); ++i)
                {
                    var trackEndTime = mediaDataSource.GetTrackEndTime(i);
                    if (maxTrackEndTime < trackEndTime) maxTrackEndTime = trackEndTime;
                }

                var seekOffset = (int)segmentOffset - 7;

                var updateOffset = UpdatePrefix(
                    prefix,
                    prefix.Length,
                    segmentOffset,
                    mediaEndOffset - segmentOffset,
                    seekHeadOffsetMs,
                    cuesOffsetMs,
                    maxTrackEndTime - minStartTime,
                    timeScale);

                fileStream.Seek(seekOffset, 0);
                fileStream.Write(prefix, seekOffset, updateOffset - seekOffset);
                fileStream.Flush();
                muxStateWriter.WriteUlong('Z', 1);
                muxStateWriter.Flush();
            }
        }
    }
}