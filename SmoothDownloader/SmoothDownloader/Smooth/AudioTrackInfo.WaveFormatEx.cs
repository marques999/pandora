using System;

namespace SmoothDownloader.Smooth
{
    internal partial class AudioTrackInfo
    {
        private class WaveFormatEx
        {
            public readonly ushort WFormatTag;
            public readonly ushort NChannels;
            public readonly uint NSamplesPerSec;
            private readonly uint _nAvgBytesPerSec;
            private readonly ushort _nBlockAlign;
            public readonly ushort WBitsPerSample;
            private readonly byte[] _decoderSpecificData;
            public WaveFormatEx(byte[] data)
            {
                if (data == null || data.Length < 18)
                {
                    throw new Exception("Invalid WaveFormatEx data!");
                }
                var num = BitConverter.ToUInt16(data, 16);
                if (data.Length != 18 + num)
                {
                    throw new Exception("Invalid cbSize value!");
                }
                WFormatTag = BitConverter.ToUInt16(data, 0);
                NChannels = BitConverter.ToUInt16(data, 2);
                NSamplesPerSec = BitConverter.ToUInt16(data, 4);
                _nAvgBytesPerSec = BitConverter.ToUInt16(data, 8);
                _nBlockAlign = BitConverter.ToUInt16(data, 12);
                WBitsPerSample = BitConverter.ToUInt16(data, 14);
                _decoderSpecificData = new byte[num];
                Buffer.BlockCopy(data, 18, _decoderSpecificData, 0, _decoderSpecificData.Length);
            }
            public WaveFormatEx(ushort wFormatTag, ushort nChannels, uint nSamplesPerSec, uint nAvgBytesPerSec, ushort nBlockAlign,
                ushort wBitsPerSample, byte[] decoderSpecificData)
            {
                if (decoderSpecificData != null && decoderSpecificData.Length > 65535)
                {
                    throw new Exception("DecoderSpecificData too long.");
                }
                WFormatTag = wFormatTag;
                NChannels = nChannels;
                NSamplesPerSec = nSamplesPerSec;
                _nAvgBytesPerSec = nAvgBytesPerSec;
                _nBlockAlign = nBlockAlign;
                WBitsPerSample = wBitsPerSample;
                _decoderSpecificData = decoderSpecificData;
            }
            public byte[] GetBytes()
            {
                var array = new byte[18 + _decoderSpecificData.Length];
                Buffer.BlockCopy(BitConverter.GetBytes(WFormatTag), 0, array, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(NChannels), 0, array, 2, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(NSamplesPerSec), 0, array, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(_nAvgBytesPerSec), 0, array, 8, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(_nBlockAlign), 0, array, 12, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(WBitsPerSample), 0, array, 14, 2);
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)_decoderSpecificData.Length), 0, array, 16, 2);
                if (array.Length != 18)
                {
                    Buffer.BlockCopy(_decoderSpecificData, 0, array, 18, _decoderSpecificData.Length);
                }
                return array;
            }
        }
    }
}