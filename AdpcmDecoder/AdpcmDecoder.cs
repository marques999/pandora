using System;
using System.Collections.Generic;
using Math = System.Math;

namespace AdpcmDecoder
{
    /// <summary>
    /// </summary>
    internal class AdpcmDecoder
    {
        /// <summary>
        /// </summary>
        private static readonly short[] AdpcmLengthTable =
        {
            0x10, 0x11, 0x13, 0x15, 0x17, 25, 28, 31, 34,
            37, 41, 45, 50, 55, 60, 66, 73, 80,
            0x058, 0x061, 0x06B, 0x076, 0x082, 0x08F, 0x09D, 0x0AD,
            0x0BE, 0x0D1, 0x0E6, 0x0FD, 0x117, 0x133, 0x151, 0x173,
            0x198, 0x1C1, 0x1EE, 0x220, 0x256, 0x292, 0x2D4, 0x31C,
            0x36C, 0x3C3, 0x424, 0x48E, 0x502, 0x583, 0x610
        };

        /// <summary>
        /// </summary>
        private static readonly int[] AdpcmStepTable =
        {
            -1, -1, -1, -1, 2, 5, 7, 9, -1, -1, -1, -1, 2, 5, 7, 9
        };

        /// <summary>
        /// </summary>
        private static readonly int[] AdpcmDecodeTableB1 =
        {
            1, 3, 5, 7, 9, 11, 13, 15, -1, -3, -5, -7, -9, -11, -13, -15
        };

        /// <summary>
        /// </summary>
        private static readonly short[] AdpcmDecodeTableB2 =
        {
            0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99,
            0x39, 0x39, 0x39, 0x39, 0x4D, 0x66, 0x80, 0x99
        };

        /// <summary>
        /// </summary>
        private static int[] _jediTable;

        /// <summary>
        /// </summary>
        private int _accumulator;

        /// <summary>
        /// </summary>
        private int _adpcmDelta;

        /// <summary>
        /// </summary>
        private int _decrement;

        /// <summary>
        /// </summary>
        private short[] _buffer;

        /// <summary>
        /// </summary>
        private int _position;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public AdpcmDecoder()
        {
            _jediTable = new int[784];

            for (var i = 0; i < 49; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    var step = (((j & 0x07) << 1) + 1) * AdpcmLengthTable[i] / 8;

                    if ((j & 0x08) == 0x08)
                    {
                        _jediTable[(i << 4) + j] = -step;
                    }
                    else
                    {
                        _jediTable[(i << 4) + j] = step;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="apdmcBytes"></param>
        /// <returns></returns>
        public short[] ConvertAdpcmA(IReadOnlyCollection<byte> apdmcBytes)
        {
            return Convert(apdmcBytes, DecodeApdcmTypeA);
        }

        /// <summary>
        /// </summary>
        /// <param name="adpcmBytes"></param>
        /// <param name="adpcmFrequency"></param>
        /// <returns></returns>
        public short[] ConvertAdpcmB(IReadOnlyCollection<byte> adpcmBytes, int adpcmFrequency)
        {
            if (adpcmFrequency >= 1800 && adpcmFrequency <= 55500)
            {
                return Convert(adpcmBytes, DecodeAdpcmTypeB);
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="adpcmBytes"></param>
        /// <param name="adpcmHandler"></param>
        /// <returns></returns>
        private short[] Convert(IReadOnlyCollection<byte> adpcmBytes, Func<byte, short> adpcmHandler)
        {
            _position = 0;
            _decrement = 0;
            _accumulator = 0;
            _adpcmDelta = 0x7F;
            _buffer = new short[adpcmBytes.Count << 1];

            foreach (var nibble in adpcmBytes)
            {
                _buffer[_position++] = adpcmHandler((byte)((nibble & 0xF0) >> 4));
                _buffer[_position++] = adpcmHandler((byte)(nibble & 0x0F));
            }

            return _buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="nibble"></param>
        /// <returns></returns>
        private short DecodeApdcmTypeA(byte nibble)
        {
            _accumulator += _jediTable[_decrement + nibble];

            if ((_accumulator & -2048) == 0)
            {
                _accumulator &= 0x0FFF;
            }
            else
            {
                _accumulator |= -0x1000;
            }

            _decrement += AdpcmStepTable[nibble & 0x07] << 4;
            _decrement = Math.Max(0, Math.Min(_decrement, 768));

            return (short)_accumulator;
        }

        /// <summary>
        /// </summary>
        /// <param name="nibble"></param>
        /// <returns></returns>
        private short DecodeAdpcmTypeB(byte nibble)
        {
            _accumulator += AdpcmDecodeTableB1[nibble] * (_adpcmDelta >> 3);

            if (_accumulator > short.MaxValue)
            {
                _accumulator = short.MaxValue;
            }
            else if (_accumulator < short.MinValue)
            {
                _accumulator = short.MinValue;
            }

            _adpcmDelta = (_adpcmDelta * AdpcmDecodeTableB2[nibble]) >> 6;
            _adpcmDelta = Math.Max(0x7F, Math.Min(_adpcmDelta, 0x6000));

            return (short)_accumulator;
        }
    }
}