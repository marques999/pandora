using System.Collections.Generic;
using System.Text;

namespace MemcardRex
{
    /// <summary>
    /// </summary>
    internal class ShiftJisConverter
    {
        /// <summary>
        /// </summary>
        private static readonly Dictionary<int, char> ShiftJisTable = new Dictionary<int, char>
        {
            [0x8143] = ',',
            [0x8144] = '.',
            [0x8145] = '·',
            [0x8146] = ':',
            [0x8147] = ';',
            [0x8148] = '?',
            [0x8149] = '!',
            [0x814F] = '^',
            [0x8151] = '_',
            [0x815B] = '-',
            [0x815C] = '-',
            [0x815D] = '-',
            [0x815E] = '/',
            [0x815F] = '\\',
            [0x8160] = '~',
            [0x8161] = '|',
            [0x8168] = '\"',
            [0x8169] = '(',
            [0x816A] = ')',
            [0x816D] = '[',
            [0x816E] = ']',
            [0x816F] = '{',
            [0x8170] = '}',
            [0x817B] = '+',
            [0x817C] = '-',
            [0x817D] = '±',
            [0x817E] = '*',
            [0x8180] = '÷',
            [0x8181] = '=',
            [0x8183] = '<',
            [0x8184] = '>',
            [0x818A] = '°',
            [0x818B] = '\'',
            [0x818C] = '\"',
            [0x8190] = '$',
            [0x8193] = '%',
            [0x8194] = '#',
            [0x8195] = '&',
            [0x8196] = '*',
            [0x8197] = '@',
            [0x824F] = '0',
            [0x8250] = '1',
            [0x8251] = '2',
            [0x8252] = '3',
            [0x8253] = '4',
            [0x8254] = '5',
            [0x8255] = '6',
            [0x8256] = '7',
            [0x8257] = '8',
            [0x8258] = '9',
            [0x8260] = 'A',
            [0x8261] = 'B',
            [0x8262] = 'C',
            [0x8263] = 'D',
            [0x8264] = 'E',
            [0x8265] = 'F',
            [0x8266] = 'G',
            [0x8267] = 'H',
            [0x8268] = 'I',
            [0x8269] = 'J',
            [0x826A] = 'K',
            [0x826B] = 'L',
            [0x826C] = 'M',
            [0x826D] = 'N',
            [0x826E] = 'O',
            [0x826F] = 'P',
            [0x8270] = 'Q',
            [0x8271] = 'R',
            [0x8272] = 'S',
            [0x8273] = 'T',
            [0x8274] = 'U',
            [0x8275] = 'V',
            [0x8276] = 'W',
            [0x8277] = 'X',
            [0x8278] = 'Y',
            [0x8279] = 'Z',
            [0x8281] = 'a',
            [0x8282] = 'b',
            [0x8283] = 'c',
            [0x8284] = 'd',
            [0x8285] = 'e',
            [0x8286] = 'f',
            [0x8287] = 'g',
            [0x8288] = 'h',
            [0x8289] = 'i',
            [0x828A] = 'j',
            [0x828B] = 'k',
            [0x828C] = 'l',
            [0x828D] = 'm',
            [0x828E] = 'n',
            [0x828F] = 'o',
            [0x8290] = 'p',
            [0x8291] = 'q',
            [0x8292] = 'r',
            [0x8293] = 's',
            [0x8294] = 't',
            [0x8295] = 'u',
            [0x8296] = 'v',
            [0x8297] = 'w',
            [0x8298] = 'x',
            [0x8299] = 'y',
            [0x829A] = 'z'
        };

        /// <summary>
        /// </summary>
        public string ConvertShiftJisToAscii(byte[] buffer)
        {
            var stringBuilder = new StringBuilder();

            for (var position = 0; position < buffer.Length; position += 2)
            {
                var keyCode = (buffer[position] << 8) | buffer[position + 1];

                switch (keyCode)
                {
                case 0x0000:

                    return stringBuilder.ToString();

                case 0x8140:

                    stringBuilder.Append("  ");

                    break;

                default:

                    if (ShiftJisTable.TryGetValue(keyCode, out var value))
                    {
                        stringBuilder.Append(value);
                    }

                    break;

                }
            }

            return stringBuilder.ToString();
        }
    }
}