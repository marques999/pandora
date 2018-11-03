using System.Runtime.CompilerServices;

namespace VgmPlayer
{
    /// <summary>
    /// </summary>
    internal static class Ym2612Constants
    {
        internal const int EnvelopeBits = 10;

        internal const int EnvelopeLength = 1 << EnvelopeBits;
        internal const int MAX_ATT_INDEX = EnvelopeLength - 1;
        internal const float EnvelopeStepGx = 128.0f / EnvelopeLength;
        internal const int RATE_STEPS = (8);

        /// <summary>
        /// </summary>
        internal static readonly byte[] DtTable =
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 1, 1, 1,
            1, 1, 1, 1, 2, 2, 2, 2,
            2, 3, 3, 3, 4, 4, 4, 5,
            5, 6, 6, 7, 8, 8, 8, 8,
            1, 1, 1, 1, 2, 2, 2, 2,
            2, 3, 3, 3, 4, 4, 4, 5,
            5, 6, 6, 7, 8, 8, 9, 10,
            11, 12, 13, 14, 16, 16, 16, 16,
            2, 2, 2, 2, 2, 3, 3, 3,
            4, 4, 4, 5, 5, 6, 6, 7,
            8, 8, 9, 10, 11, 12, 13, 14,
            16, 17, 19, 20, 22, 22, 22, 22
        };


        internal static readonly byte[] eg_inc = {
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            1,
            1,
            0,
            1, /* rates 00..11 1 */
            /* 2 */
            0,
            1,
            1,
            1,
            0,
            1,
            1,
            1, /* rates 00..11 2 */
            /* 3 */
            0,
            1,
            1,
            1,
            1,
            1,
            1,
            1, /* rates 00..11 3 */

            /* 4 */
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1, /* rate 12 0 (increment by 1) */
            /* 5 */
            1,
            1,
            1,
            2,
            1,
            1,
            1,
            2, /* rate 12 1 */
            /* 6 */
            1,
            2,
            1,
            2,
            1,
            2,
            1,
            2, /* rate 12 2 */
            /* 7 */
            1,
            2,
            2,
            2,
            1,
            2,
            2,
            2, /* rate 12 3 */

            /* 8 */
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2, /* rate 13 0 (increment by 2) */
            /* 9 */
            2,
            2,
            2,
            4,
            2,
            2,
            2,
            4, /* rate 13 1 */
            /*10 */
            2,
            4,
            2,
            4,
            2,
            4,
            2,
            4, /* rate 13 2 */
            /*11 */
            2,
            4,
            4,
            4,
            2,
            4,
            4,
            4, /* rate 13 3 */

            /*12 */
            4,
            4,
            4,
            4,
            4,
            4,
            4,
            4, /* rate 14 0 (increment by 4) */
            /*13 */
            4,
            4,
            4,
            8,
            4,
            4,
            4,
            8, /* rate 14 1 */
            /*14 */
            4,
            8,
            4,
            8,
            4,
            8,
            4,
            8, /* rate 14 2 */
            /*15 */
            4,
            8,
            8,
            8,
            4,
            8,
            8,
            8, /* rate 14 3 */

            /*16 */
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8, /* rates 15 0, 15 1, 15 2, 15 3 (increment by 8) */
            /*17 */
            16,
            16,
            16,
            16,
            16,
            16,
            16,
            16, /* rates 15 2, 15 3 for attack */
            /*18 */
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0, /* infinity rates for attack and decay(s) */
        };

        /// <summary>
        /// </summary>
        internal static readonly uint[] SlTable =
        {
            SC(0),
            SC(1),
            SC(2),
            SC(3),
            SC(4),
            SC(5),
            SC(6),
            SC(7),
            SC(8),
            SC(9),
            SC(10),
            SC(11),
            SC(12),
            SC(13),
            SC(14),
            SC(31)
        };

        internal static readonly byte[] OpnFkTable =
        {
            0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 3, 3, 3, 3, 3, 3
        };

        internal static readonly uint[] LfoSamplesPerStep = new uint[8]
        {
            108, 77, 71, 67, 62, 44, 8, 5
        };

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint SC(int db)
        {
            return (uint)(db * (4.0f / EnvelopeStepGx));
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte CalculateEgRateShift(byte a) => (byte)(a * 1);

        /// <summary>
        /// </summary>
        internal static readonly byte[] EgRateShift =
        {
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(11),
            CalculateEgRateShift(10),
            CalculateEgRateShift(10),
            CalculateEgRateShift(10),
            CalculateEgRateShift(10),
            CalculateEgRateShift(9),
            CalculateEgRateShift(9),
            CalculateEgRateShift(9),
            CalculateEgRateShift(9),
            CalculateEgRateShift(8),
            CalculateEgRateShift(8),
            CalculateEgRateShift(8),
            CalculateEgRateShift(8),
            CalculateEgRateShift(7),
            CalculateEgRateShift(7),
            CalculateEgRateShift(7),
            CalculateEgRateShift(7),
            CalculateEgRateShift(6),
            CalculateEgRateShift(6),
            CalculateEgRateShift(6),
            CalculateEgRateShift(6),
            CalculateEgRateShift(5),
            CalculateEgRateShift(5),
            CalculateEgRateShift(5),
            CalculateEgRateShift(5),
            CalculateEgRateShift(4),
            CalculateEgRateShift(4),
            CalculateEgRateShift(4),
            CalculateEgRateShift(4),
            CalculateEgRateShift(3),
            CalculateEgRateShift(3),
            CalculateEgRateShift(3),
            CalculateEgRateShift(3),
            CalculateEgRateShift(2),
            CalculateEgRateShift(2),
            CalculateEgRateShift(2),
            CalculateEgRateShift(2),
            CalculateEgRateShift(1),
            CalculateEgRateShift(1),
            CalculateEgRateShift(1),
            CalculateEgRateShift(1),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0),
            CalculateEgRateShift(0)
        };
    }
}