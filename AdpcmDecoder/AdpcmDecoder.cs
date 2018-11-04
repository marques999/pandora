using System;
using System.Collections.Generic;

namespace AdpcmDecoder
{
    /// <summary>
    /// </summary>
    internal class AdpcmDecoder
    {
        /// <summary>
        /// </summary>
        private static readonly short[] StepSize =
        {
            16, 17, 19, 21, 23, 25, 28, 31, 34,
            37, 41, 45, 50, 55, 60, 66, 73, 80,
            88, 97, 107, 118, 130, 143, 157, 173,
            190, 209, 230, 253, 279, 307, 337, 371,
            408, 449, 494, 544, 598, 658, 724, 796,
            876, 963, 1060, 1166, 1282, 1411, 1552
        };

        /// <summary>
        /// </summary>
        private static readonly int[] StepAdjustment =
        {
            -1, -1, -1, -1, 2, 5, 7, 9,
            -1, -1, -1, -1, 2, 5, 7, 9
        };

        /// <summary>
        /// </summary>
        private static readonly int[] AdpcmDecodeTableB1 =
        {
            1, 3, 5, 7, 9, 11, 13, 15,
            -1, -3, -5, -7, -9, -11, -13, -15
        };

        /// <summary>
        /// </summary>
        private static readonly short[] AdpcmDecodeTableB2 =
        {
            57, 57, 57, 57, 77, 102, 128, 153,
            57, 57, 57, 57, 77, 102, 128, 153
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
        private int _bufferPosition;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public AdpcmDecoder()
        {
            _jediTable = new int[0x310];

            for (var i = 0; i < 0x31; i++)
            {
                for (var j = 0; j < 0x10; j++)
                {
                    var step = (2 * (j & 0x07) + 1) * StepSize[i] / 8;

                    if ((j & 8) == 0)
                    {
                        _jediTable[(i << 4) + j] = step;
                    }
                    else
                    {
                        _jediTable[(i << 4) + j] = -step;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="adpcmData"></param>
        /// <returns></returns>
        public short[] ConvertAdpcmA(IReadOnlyCollection<byte> adpcmData)
        {
            return Convert(adpcmData, DecodeApdcmTypeA);
        }

        /// <summary>
        /// </summary>
        /// <param name="adpcmData"></param>
        /// <param name="adpcmFrequency"></param>
        /// <returns></returns>
        public short[] ConvertAdpcmB(IReadOnlyCollection<byte> adpcmData, int adpcmFrequency)
        {
            if (adpcmFrequency >= 1800 && adpcmFrequency <= 55500)
            {
                return Convert(adpcmData, DecodeApdcmTypeB);
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="adpcmData"></param>
        /// <param name="adpcmHandler"></param>
        /// <returns></returns>
        private short[] Convert(IReadOnlyCollection<byte> adpcmData, Func<byte, short> adpcmHandler)
        {
            _decrement = 0;
            _accumulator = 0;
            _adpcmDelta = 0x7F;
            _bufferPosition = 0;
            _buffer = new short[adpcmData.Count << 1];

            foreach (var nibble in adpcmData)
            {
                _buffer[_bufferPosition++] = adpcmHandler((byte)((nibble & 0xF0) >> 4));
                _buffer[_bufferPosition++] = adpcmHandler((byte)(nibble & 0x0F));
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
                _accumulator |= -4096;
            }

            _decrement += StepAdjustment[nibble & 0x07] << 4;

            if (_decrement < 0x0000)
            {
                _decrement = 0x0000;
            }

            if (_decrement > 0x0300)
            {
                _decrement = 0x0300;
            }

            return (short)_accumulator;
        }

        /// <summary>
        /// </summary>
        /// <param name="nibble"></param>
        /// <returns></returns>
        private short DecodeApdcmTypeB(byte nibble)
        {
            _accumulator += AdpcmDecodeTableB1[nibble] * (_adpcmDelta >> 3);

            if (_accumulator > 0x7FFF)
            {
                _accumulator = 0x7FFF;
            }
            else if (_accumulator < -32768)
            {
                _accumulator = -32768;
            }

            _adpcmDelta = _adpcmDelta * AdpcmDecodeTableB2[nibble] / 64;

            if (_adpcmDelta > 0x6000)
            {
                _adpcmDelta = 0x6000;
            }
            else if (_adpcmDelta < 0x007F)
            {
                _adpcmDelta = 0x007F;
            }

            return (short)_accumulator;
        }
    }
}