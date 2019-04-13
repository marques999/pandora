using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using SmoothDownloader.Mkv;

namespace SmoothDownloader.Smooth
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class VideoTrackInfo : TrackInfo
    {
        /// <summary>
        /// </summary>
        /// <param name="bitmapWidth"></param>
        /// <param name="bitmapHeight"></param>
        /// <param name="bitmapPlanes"></param>
        /// <param name="bitmapDepth"></param>
        /// <param name="bitmapCompression"></param>
        /// <param name="biSizeImage"></param>
        /// <param name="biXPelsPerMeter"></param>
        /// <param name="biYPelsPerMeter"></param>
        /// <param name="biClrUsed"></param>
        /// <param name="biClrImportant"></param>
        /// <param name="privateData"></param>
        /// <returns></returns>
        private static byte[] GetBitmapInfoHeaderBytes(int bitmapWidth, int bitmapHeight, ushort bitmapPlanes, ushort bitmapDepth,
            uint bitmapCompression, uint biSizeImage, int biXPelsPerMeter, int biYPelsPerMeter,
            uint biClrUsed, uint biClrImportant, byte[] privateData)
        {
            var bufferLength = 40 + privateData.Length;
            var buffer = new byte[bufferLength];

            Buffer.BlockCopy(BitConverter.GetBytes(bufferLength), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(bitmapWidth), 0, buffer, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(bitmapHeight), 0, buffer, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(bitmapPlanes), 0, buffer, 12, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bitmapDepth), 0, buffer, 14, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bitmapCompression), 0, buffer, 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biSizeImage), 0, buffer, 20, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biXPelsPerMeter), 0, buffer, 24, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biYPelsPerMeter), 0, buffer, 28, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biClrUsed), 0, buffer, 32, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biClrImportant), 0, buffer, 36, 4);
            Buffer.BlockCopy(privateData, 0, buffer, 40, privateData.Length);

            return buffer;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributes"></param>
        /// <param name="position"></param>
        /// <param name="stream"></param>
        public VideoTrackInfo(XmlNode element, IDictionary<string, string> attributes, uint position, StreamInfo stream) : base(element, position, stream)
        {
            var pixelWidth = 0u;
            var pixelHeight = 0u;
            var displayWidth = 0u;
            var displayHeight = 0u;

            if (Attributes.ContainsKey("MaxWidth"))
            {
                pixelWidth = Parser.UInt32Attribute(Attributes, "MaxWidth");
            }
            else if (Attributes.ContainsKey("Width"))
            {
                pixelWidth = Parser.UInt32Attribute(Attributes, "Width");
            }
            else if (attributes.ContainsKey("MaxWidth"))
            {
                pixelWidth = Parser.UInt32Attribute(attributes, "MaxWidth");
            }

            if (pixelWidth == 0u)
            {
                throw new Exception();
            }

            if (Attributes.ContainsKey("MaxHeight"))
            {
                pixelHeight = Parser.UInt32Attribute(Attributes, "MaxHeight");
            }
            else if (Attributes.ContainsKey("Height"))
            {
                pixelHeight = Parser.UInt32Attribute(Attributes, "Height");
            }
            else if (attributes.ContainsKey("MaxHeight"))
            {
                pixelHeight = Parser.UInt32Attribute(attributes, "MaxHeight");
            }

            if (pixelHeight == 0u)
            {
                throw new Exception();
            }

            if (attributes.ContainsKey("DisplayWidth"))
            {
                displayWidth = Parser.UInt32Attribute(attributes, "DisplayWidth");
            }

            if (displayWidth == 0u)
            {
                displayWidth = pixelWidth;
            }

            if (attributes.ContainsKey("DisplayHeight"))
            {
                displayHeight = Parser.UInt32Attribute(attributes, "DisplayHeight");
            }

            if (displayHeight == 0u)
            {
                displayHeight = pixelHeight;
            }

            var videoInfoBytes = MkvUtils.GetVideoInfoBytes(pixelWidth, pixelHeight, displayWidth, displayHeight);

            byte[] codecPrivateData = null;

            if (Attributes.ContainsKey("CodecPrivateData"))
            {
                codecPrivateData = Parser.HexStringAttribute(Attributes, "CodecPrivateData");
            }

            if (codecPrivateData == null)
            {
                throw new Exception();
            }

            string fourcc = null;

            if (Attributes.ContainsKey("FourCC"))
            {
                fourcc = Parser.StringAttribute(Attributes, "FourCC");
            }
            else if (attributes.ContainsKey("FourCC"))
            {
                fourcc = Parser.StringAttribute(attributes, "FourCC");
            }

            switch (fourcc)
            {
            case "WVC1":

                TrackEntry = new TrackEntry(
                    MkvTrackType.Video, videoInfoBytes, MkvCodec.VideoMs, GetVfWCodecPrivate(
                        pixelWidth, pixelHeight, fourcc, codecPrivateData));
                break;

            case "H264":

                ushort nalUnitLengthField = 4;

                if (Attributes.ContainsKey("NALUnitLengthField"))
                {
                    nalUnitLengthField = Parser.UInt16Attribute(Attributes, "NALUnitLengthField");
                }

                TrackEntry = new TrackEntry(MkvTrackType.Video, videoInfoBytes, MkvCodec.VideoAvc, GetAvcCodecPrivate(codecPrivateData, nalUnitLengthField));

                break;

            case null:

                throw new Exception();

            default:

                throw new Exception();
            }

            if (Attributes.ContainsKey("Name"))
            {
                TrackEntry.Name = Parser.StringAttribute(attributes, "Name");
            }

            TrackEntry.Language = LanguageId.Hungarian;
            Description = $"{fourcc} {pixelWidth}x{pixelHeight} ({displayWidth}x{displayHeight}) @ {Bitrate / 1000u} kbps";
        }

        /// <summary>
        /// </summary>
        /// <param name="privateData"></param>
        /// <param name="unitLengthField"></param>
        /// <returns></returns>
        private static byte[] GetAvcCodecPrivate(byte[] privateData, ushort unitLengthField)
        {
            if (unitLengthField != 1 && unitLengthField != 2 && unitLengthField != 4)
            {
                throw new Exception();
            }

            var contents = Utils.HexEncodeString(privateData);

            if (string.IsNullOrEmpty(contents))
            {
                throw new Exception();
            }

            var array1 = contents.Split(new[] { "00000001" }, 0);

            if (array1.Length != 3)
            {
                throw new Exception();
            }

            var array1Decoded = Utils.HexDecodeString(array1[1]);

            if (array1Decoded == null || array1Decoded.Length < 3)
            {
                throw new Exception();
            }

            var array2Decoded = Utils.HexDecodeString(array1[2]);

            if (array2Decoded == null)
            {
                throw new Exception();
            }

            return GetAvcDecoderConfigurationBytes(
                array1Decoded[1],
                array1Decoded[2],
                array1Decoded[3],
                (byte)(unitLengthField - 1),
                new[] { array1Decoded },
                new[] { array2Decoded }
            );
        }

        /// <summary>
        /// </summary>
        /// <param name="avcProfileIndication"></param>
        /// <param name="profileCompatibility"></param>
        /// <param name="avcLevelIndication"></param>
        /// <param name="lengthSizeMinusOne"></param>
        /// <param name="sequenceParameterSetNalUnits"></param>
        /// <param name="pictureParameterSetNalUnits"></param>
        /// <returns></returns>
        private static byte[] GetAvcDecoderConfigurationBytes(
            byte avcProfileIndication,
            byte profileCompatibility,
            byte avcLevelIndication,
            byte lengthSizeMinusOne,
            IReadOnlyList<byte[]> sequenceParameterSetNalUnits,
            IReadOnlyList<byte[]> pictureParameterSetNalUnits
        )
        {
            if (lengthSizeMinusOne != 0 && lengthSizeMinusOne != 1 && lengthSizeMinusOne != 3)
            {
                throw new Exception();
            }

            if (sequenceParameterSetNalUnits.Count > 31)
            {
                throw new Exception();
            }

            if (pictureParameterSetNalUnits.Count > 255)
            {
                throw new Exception();
            }

            var position = 7;
            var sMaximum = sequenceParameterSetNalUnits.Count;

            for (var offset = 0; offset < sMaximum; ++offset)
            {
                position += 2 + sequenceParameterSetNalUnits[offset].Length;
            }

            var pMaximum = pictureParameterSetNalUnits.Count;

            for (var offset = 0; offset < pMaximum; ++offset)
            {
                position += 2 + pictureParameterSetNalUnits[offset].Length;
            }

            var buffer = new byte[position];

            position = 6;
            buffer[0] = 0x01;
            buffer[1] = avcProfileIndication;
            buffer[2] = profileCompatibility;
            buffer[3] = avcLevelIndication;
            buffer[4] = (byte)(0xFC ^ lengthSizeMinusOne);
            buffer[5] = (byte)(0xE0 ^ sMaximum);

            for (var offset = 0; offset < sMaximum; ++offset)
            {
                var length = sequenceParameterSetNalUnits[offset].Length;

                buffer[position] = (byte)(length >> 8);
                buffer[position + 1] = (byte)(length & 0xFF);
                position += 2;
                Buffer.BlockCopy(sequenceParameterSetNalUnits[offset], 0, buffer, position, length);
                position += length;
            }

            buffer[position++] = (byte)pMaximum;

            for (var offset = 0; offset < pMaximum; ++offset)
            {
                var length = pictureParameterSetNalUnits[offset].Length;
                buffer[position] = (byte)(length >> 8);
                buffer[position + 1] = (byte)(length & 255);
                position += 2;
                Buffer.BlockCopy(pictureParameterSetNalUnits[offset], 0, buffer, position, length);
                position += length;
            }

            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fourCc"></param>
        /// <param name="codecPrivate"></param>
        /// <returns></returns>
        private static byte[] GetVfWCodecPrivate(uint width, uint height, string fourCc, byte[] codecPrivate)
        {
            if (width > 2147483647u || height > 2147483647u)
            {
                throw new Exception();
            }

            if (fourCc.Length != 4)
            {
                throw new Exception();
            }

            return GetBitmapInfoHeaderBytes(
                (int)width,
                (int)height, 1, 24,
                BitConverter.ToUInt32(Encoding.ASCII.GetBytes(fourCc), 0),
                width * height * 24u / 8u,
                0, 0, 0u, 0u,
                codecPrivate
            );
        }
    }
}