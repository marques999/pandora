using System;
using System.Collections.Generic;
using System.Xml;

using SmoothDownloader.Mkv;

namespace SmoothDownloader.Smooth
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal partial class AudioTrackInfo : TrackInfo
    {
        /// <summary>
        /// </summary>
        /// <param name="audioTag"></param>
        /// <returns></returns>
        private static string GetCodecNameForAudioTag(ushort audioTag)
        {
            switch (audioTag)
            {
            case 1:
                return "LPCM";
            case 85:
                return "MP3";
            case 255:
            case 5633:
                return "AAC";
            case 353:
                return "WMA2";
            case 354:
                return "WMAP";
            case 65534:
                return "Vendor-extensible format";
            default:
                throw new Exception();
            }
        }

        /// <summary>
        /// </summary>
        private static readonly uint[] Mp4SamplingRate =
        {
            96000u, 88200u, 64000u, 48000u, 44100u,
            32000u, 24000u, 22050u, 16000u, 12000u,
            11025u, 8000u, 7350u, 0u, 0u, 0u
        };

        /// <summary>
        /// </summary>
        private const string Mp4Channels = "\x00\x01\x02\x03\x04\x05\x06\x08";

        /// <summary>
        /// </summary>
        /// <param name="samplingRate"></param>
        /// <param name="numberOfChannels"></param>
        /// <returns></returns>
        private static byte[] GetAudioSpecificConfigBytes(uint samplingRate, byte numberOfChannels)
        {
            ushort value = 2 << 11;
            var accumulator = 0;

            while (Mp4SamplingRate[accumulator] != samplingRate && accumulator < Mp4SamplingRate.Length)
            {
                accumulator++;
            }

            if (accumulator > Mp4SamplingRate.Length)
            {
                throw new Exception();
            }

            value += (ushort)((ushort)accumulator << 7);
            accumulator = 0;

            while (Mp4Channels[accumulator] != numberOfChannels && accumulator < Mp4Channels.Length)
            {
                accumulator++;
            }

            if (accumulator > Mp4Channels.Length)
            {
                throw new Exception();
            }

            return Utils.InplaceReverseBytes(BitConverter.GetBytes(value + (ushort)((ushort)accumulator << 3)));
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="streamAttributes"></param>
        /// <param name="position"></param>
        /// <param name="stream"></param>
        public AudioTrackInfo(XmlNode element, IDictionary<string, string> streamAttributes, uint position, StreamInfo stream) : base(element, position, stream)
        {
            WaveFormatEx waveFormatEx;

            if (Attributes.ContainsKey("WaveFormatEx"))
            {
                waveFormatEx = new WaveFormatEx(Parser.HexStringAttribute(Attributes, "WaveFormatEx"));
            }
            else
            {
                var wFormatTag = Parser.UInt16Attribute(Attributes, "AudioTag");
                var nChannels = Parser.UInt16Attribute(Attributes, "Channels");
                var nSamplesPerSec = Parser.UInt32Attribute(Attributes, "SamplingRate");
                var num = Parser.UInt32Attribute(Attributes, "Bitrate");
                var nBlockAlign = Parser.UInt16Attribute(Attributes, "PacketSize");
                var wBitsPerSample = Parser.UInt16Attribute(Attributes, "BitsPerSample");
                var decoderSpecificData = Parser.HexStringAttribute(Attributes, "CodecPrivateData");
                waveFormatEx = new WaveFormatEx(wFormatTag, nChannels, nSamplesPerSec, num / 8u, nBlockAlign, wBitsPerSample, decoderSpecificData);
            }

            var audioInfoBytes = MkvUtils.GetAudioInfoBytes(waveFormatEx.NSamplesPerSec, waveFormatEx.NChannels, waveFormatEx.WBitsPerSample);

            switch (waveFormatEx.WFormatTag)
            {
            case 0x0161:
            case 0x0162:

                TrackEntry = new TrackEntry(
                    MkvTrackType.Audio,
                    audioInfoBytes,
                    MkvCodec.AudioMs,
                    waveFormatEx.GetBytes()
                );

                break;

            case 0x00FF:
            case 0x1601:

                TrackEntry = new TrackEntry(
                    MkvTrackType.Audio,
                    audioInfoBytes,
                    MkvCodec.AudioAac,
                    GetAudioSpecificConfigBytes(waveFormatEx.NSamplesPerSec, (byte)waveFormatEx.NChannels)
                );

                break;

            default:

                throw new Exception();
            }

            if (Attributes.ContainsKey("Name"))
            {
                TrackEntry.Name = Parser.StringAttribute(streamAttributes, "Name");
            }

            TrackEntry.Language = LanguageId.Hungarian;
            Description = $"{GetCodecNameForAudioTag(waveFormatEx.WFormatTag)} {waveFormatEx.NChannels} channels {waveFormatEx.NSamplesPerSec} Hz @ {Bitrate / 1000u} kbps";
        }
    }
}