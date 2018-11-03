using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SexyTerminal
{
    class Common
    {
        void ustrcpy(byte[] target, byte[] source)
        {
            var terminator = source.Length - 1;

            for (var i = 0; i < terminator; i++)
            {
                target[i] = source[i];
            }

            target[terminator] = (byte)'\0';
        }

        public static void concat(char[] dest, char[] source)
        {

            var j = dest.Length - 1;
            for (var i = 0; i <= source.Length - 1; i++)
            {
                dest[i + j] = source[i];
            }
        }

        public static void uconcat(byte[] dest, byte[] source)
        {
            var j = dest.Length - 1;

            for (var i = 0; i <= source.Length - 1; i++)
            {
                dest[i + j] = source[i];
            }
        }

        public static int ctoi(char source)
        {
            if (source >= '0' && (source <= '9'))
            {
                return (source - '0');
            }

            return (source - 'A' + 10);
        }

        public static char itoc(int source)
        {
            if ((source >= 0) && (source <= 9))
            {
                return (char)('0' + source);
            }

            return (char)('A' + (source - 10));
        }

        public static void toUpper(byte[] source)
        {
            for (var i = 0; i < source.Length; i++)
            {
                if ((source[i] >= 'a') && (source[i] <= 'z'))
                {
                    source[i] = (byte)(source[i] - 'a' + 'A');
                }
            }
        }
        /*
            int is_sane(char test_string[], uint8_t source[], int length)
            {
                uint latch;
                uint lt = strlen(test_string);

                for (unsigned int i = 0; i < length; i++)
                {
                    latch = FALSE;
                    for (unsigned int j = 0; j < lt; j++)
                    {
                        if (source[i] == test_string[j])
                        {
                            latch = TRUE;
                            break;
                        }
                    }
                    if (!latch)
                    {
                        return ZERROR_INVALID_DATA;
                    }
                }

                return 0;
            }

            int posn(char set_string[], char data)
            {
                unsigned int n = strlen(set_string);

                for (unsigned int i = 0; i < n; i++)
                    if (data == set_string[i])
                        return i;
                return 0;
            }
            void lookup(char set_string[], const char* table[], char data, char dest[])
            {
                unsigned int n = strlen(set_string);

                for (unsigned int i = 0; i < n; i++)
                    if (data == set_string[i])
                        concat(dest, table[i]);
            }

            int module_is_set(struct zint_symbol * symbol, int y_coord, int x_coord)
        {
            return (symbol->encoded_data[y_coord][x_coord / 7] >> (x_coord % 7)) & 1;
        }

        void set_module(struct zint_symbol * symbol, int y_coord, int x_coord)
        {
            symbol->encoded_data[y_coord][x_coord / 7] |= 1 << (x_coord % 7);
        }

        void unset_module(struct zint_symbol * symbol, int y_coord, int x_coord)
        {
            symbol->encoded_data[y_coord][x_coord / 7] &= ~(1 << (x_coord % 7));
        }

        void expand(struct zint_symbol * symbol, char data[])
        {

            unsigned int reader, n = strlen(data);
        int writer;
        char latch;

        writer = 0;
            latch = '1';

            for(reader = 0; reader<n; reader++) {
                for(int i = 0; i<ctoi(data[reader]); i++) {
                    if(latch == '1') { set_module(symbol, symbol->rows, writer); }
                    writer++;
                }

                latch = (latch == '1' ? '0' : '1');
            }

            if(symbol->symbology != BARCODE_PHARMA) {
                if(writer > symbol->width) {
                    symbol->width = writer;
                }
            } else {
                if(writer > symbol->width + 2) {
                    symbol->width = writer - 2;
                }
            }
            symbol->rows = symbol->rows + 1;
        }

        int is_stackable(int symbology)
        {
            if (symbology < BARCODE_PDF417) { return 1; }
            if (symbology == BARCODE_CODE128B) { return 1; }
            if (symbology == BARCODE_ISBNX) { return 1; }
            if (symbology == BARCODE_EAN14) { return 1; }
            if (symbology == BARCODE_NVE18) { return 1; }
            if (symbology == BARCODE_KOREAPOST) { return 1; }
            if (symbology == BARCODE_PLESSEY) { return 1; }
            if (symbology == BARCODE_TELEPEN_NUM) { return 1; }
            if (symbology == BARCODE_ITF14) { return 1; }
            if (symbology == BARCODE_CODE32) { return 1; }

            return 0;
        }

        int is_extendable(int symbology)
        {
            if (symbology == BARCODE_EANX) { return 1; }
            if (symbology == BARCODE_UPCA) { return 1; }
            if (symbology == BARCODE_UPCE) { return 1; }
            if (symbology == BARCODE_ISBNX) { return 1; }
            if (symbology == BARCODE_UPCA_CC) { return 1; }
            if (symbology == BARCODE_UPCE_CC) { return 1; }
            if (symbology == BARCODE_EANX_CC) { return 1; }

            return 0;
        }

        int roundup(float input)
        {
            var integer_part = (int)input;
            var remainder = input - integer_part;

            if (remainder > 0.1)
            {
                integer_part++;
            }

            return integer_part;
        }

        int istwodigits(uint8_t source[], int position)
        {
            if ((source[position] >= '0') && (source[position] <= '9'))
            {
                if ((source[position + 1] >= '0') && (source[position + 1] <= '9'))
                {
                    return 1;
                }
            }

            return 0;
        }

        float froundup(float input)
        {
            float fraction, output = 0.0;

            fraction = input - (int)input;
            if (fraction > 0.01) { output = (input - fraction) + 1.0; } else { output = input; }

            return output;
        }

        int latin1_process(struct zint_symbol * symbol, uint8_t source[], uint8_t preprocessed[], int* length)
        {
            int j = 0, i = 0, next;

            do {
                next = -1;
                if(source[i] < 128) {
                    preprocessed[j] = source[i];
                    j++;
                    next = i + 1;
                } else {
                    if(source[i] == 0xC2) {
                        preprocessed[j] = source[i + 1];
                        j++;
                        next = i + 2;
                    }
                    if(source[i] == 0xC3) {
                        preprocessed[j] = source[i + 1] + 64;
                        j++;
                        next = i + 2;
                    }
                }
                if(next == -1) {
                    strcpy(symbol->errtxt, "error: Invalid character in input string (only Latin-1 characters supported)");
                    return ZERROR_INVALID_DATA;
                }
                i = next;
            } while(i< *length);
            preprocessed[j] = '\0';

            * length = j;

            return 0;
        }

        int utf8toutf16(struct zint_symbol * symbol, uint8_t source[], int vals[], int* length)
        {
            int bpos, jpos, error_number;
        int next;

        bpos = 0;
            jpos = 0;
            error_number = 0;
            next = 0;

            do {
                if(source[bpos] <= 0x7f) {
                    vals[jpos] = source[bpos];
                    next = bpos + 1;
                    jpos++;
                } else {
                    if((source[bpos] >= 0x80) && (source[bpos] <= 0xbf)) {
                        strcpy(symbol->errtxt, "Corrupt Unicode data");
                        return ZERROR_INVALID_DATA;
                    }
                    if((source[bpos] >= 0xc0) && (source[bpos] <= 0xc1)) {
                        strcpy(symbol->errtxt, "Overlong encoding not supported");
                        return ZERROR_INVALID_DATA;
                    }

                    if((source[bpos] >= 0xc2) && (source[bpos] <= 0xdf)) {

                        vals[jpos] = ((source[bpos] & 0x1f) << 6) + (source[bpos + 1] & 0x3f);
                        next = bpos + 2;
                        jpos++;
                    } else
                    if((source[bpos] >= 0xe0) && (source[bpos] <= 0xef)) {

                        vals[jpos] = ((source[bpos] & 0x0f) << 12) + ((source[bpos + 1] & 0x3f) << 6) + (source[bpos + 2] & 0x3f);
                        next = bpos + 3;
                        jpos ++;
                    } else
                    if(source[bpos] >= 0xf0) {
                        strcpy(symbol->errtxt, "Unicode sequences of more than 3 bytes not supported");
                        return ZERROR_INVALID_DATA;
                    }
                }

                bpos = next;

            } while(bpos< *length);

            * length = jpos;

            return error_number;
        }*/

    }
}