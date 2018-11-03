using System;
using System.Collections.Generic;

namespace SexyTerminal
{
    class LargeInteger
    {
        public static readonly short[] Bcd = {
            0, 0, 0, 0,
            1, 0, 0, 0,
            0, 1, 0, 0,
            1, 1, 0, 0,
            0, 0, 1, 0,
            1, 0, 1, 0,
            0, 1, 1, 0,
            1, 1, 1, 0,
            0, 0, 0, 1,
            1, 0, 0, 1
        };

        public static void BinaryAdd(short[] accumulator, short[] inputBuffer)
        {
            var carry = 0;

            for (var i = 0; i < 112; i++)
            {
                var done = false;

                if (inputBuffer[i] == 0 && accumulator[i] == 0 && carry == 0 && done == false)
                {
                    accumulator[i] = 0;
                    carry = 0;
                    done = true;
                }
                if (inputBuffer[i] == 0 && accumulator[i] == 0 && carry == 1 && done == false)
                {
                    accumulator[i] = 1;
                    carry = 0;
                    done = true;
                }
                if (inputBuffer[i] == 0 && accumulator[i] == 1 && carry == 0 && done == false)
                {
                    accumulator[i] = 1;
                    carry = 0;
                    done = true;
                }
                if (inputBuffer[i] == 0 && accumulator[i] == 1 && carry == 1 && done == false)
                {
                    accumulator[i] = 0;
                    carry = 1;
                    done = true;
                }
                if (inputBuffer[i] == 1 && accumulator[i] == 0 && carry == 0 && done == false)
                {
                    accumulator[i] = 1;
                    carry = 0;
                    done = true;
                }
                if (inputBuffer[i] == 1 && accumulator[i] == 0 && carry == 1 && done == false)
                {
                    accumulator[i] = 0;
                    carry = 1;
                    done = true;
                }
                if (inputBuffer[i] == 1 && accumulator[i] == 1 && carry == 0 && done == false)
                {
                    accumulator[i] = 0;
                    carry = 1;
                    done = true;
                }

                if (inputBuffer[i] != 1 || accumulator[i] != 1 || carry != 1 || done)
                {
                    continue;
                }

                accumulator[i] = 1;
                carry = 1;
                done = true;
            }
        }

        public static void BinarySubtract(short[] accumulator, short[] inputBuffer)
        {
            var subtractionBuffer = new short[112];

            for (var index = 0; index < 112; index++)
            {
                if (inputBuffer[index] == 0)
                {
                    subtractionBuffer[index] = 1;
                }
                else
                {
                    subtractionBuffer[index] = 0;
                }
            }

            BinaryAdd(accumulator, subtractionBuffer);
            subtractionBuffer[0] = 1;

            for (var index = 1; index < 112; index++)
            {
                subtractionBuffer[index] = 0;
            }

            BinaryAdd(accumulator, subtractionBuffer);
        }

        public static void ShiftDown(IList<short> buffer)
        {
            buffer[102] = 0;
            buffer[103] = 0;

            for (var i = 0; i < 102; i++)
            {
                buffer[i] = buffer[i + 1];
            }
        }

        public static void ShiftUp(IList<short> buffer)
        {
            for (var index = 102; index > 0; index--)
            {
                buffer[index] = buffer[index - 1];
            }

            buffer[0] = 0;
        }

        public static short IsLarger(short[] accum, short[] reg)
        {
            var latch = 0;
            var i = 103;
            short larger = 0;

            do
            {
                if (accum[i] == 1 && reg[i] == 0)
                {
                    latch = 1;
                    larger = 1;
                }
                if (accum[i] == 0 && reg[i] == 1)
                {
                    latch = 1;
                }
                i--;
            } while (latch == 0 && i >= -1);

            return larger;
        }

        void BinaryLoad(short[] _reg, string _data, int src_len)
        {
            var temp = new short[112];

            for (var index = 0; index < 112; index++)
            {
                _reg[index] = 0;
            }

            for (var read = 0; read < src_len; read++)
            {
                for (var i = 0; i < 112; i++)
                {
                    temp[i] = _reg[i];
                }

                for (var i = 0; i < 9; i++)
                {
                    BinaryAdd(_reg, temp);
                }

                temp[0] = Bcd[Common.ctoi(_data[read]) * 4];
                temp[1] = Bcd[Common.ctoi(_data[read]) * 4 + 1];
                temp[2] = Bcd[Common.ctoi(_data[read]) * 4 + 2];
                temp[3] = Bcd[Common.ctoi(_data[read]) * 4 + 3];
                for (var i = 4; i < 112; i++)
                {
                    temp[i] = 0;
                }

                BinaryAdd(_reg, temp);
            }
        }

        public static void HexDump(IReadOnlyList<short> inputBuffer)
        {
            int i;

            var byteSpace = 1;
            for (i = 100; i >= 0; i -= 4)
            {
                var digit = 0;
                digit += 1 * inputBuffer[i];
                digit += 2 * inputBuffer[i + 1];
                digit += 4 * inputBuffer[i + 2];
                digit += 8 * inputBuffer[i + 3];

                switch (digit)
                {
                case 0: Console.Write("0"); break;
                case 1: Console.Write("1"); break;
                case 2: Console.Write("2"); break;
                case 3: Console.Write("3"); break;
                case 4: Console.Write("4"); break;
                case 5: Console.Write("5"); break;
                case 6: Console.Write("6"); break;
                case 7: Console.Write("7"); break;
                case 8: Console.Write("8"); break;
                case 9: Console.Write("9"); break;
                case 10: Console.Write("A"); break;
                case 11: Console.Write("B"); break;
                case 12: Console.Write("C"); break;
                case 13: Console.Write("D"); break;
                case 14: Console.Write("E"); break;
                case 15: Console.Write("F"); break;
                }
                if (byteSpace == 1)
                {
                    byteSpace = 0;
                }
                else
                {
                    byteSpace = 1;
                    Console.Write(" ");
                }
            }
            Console.Write("\n");
        }
    }
}
