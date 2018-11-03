using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VgmPlayer.Ym2612
{
    internal class Ym2612Core
    {
        private const int FreqSh = 16; /* 16.16 fixed point (frequency calculations) */
        private const int EgSh = 16; /* 16.16 fixed point (envelope generator timing) */
        private const int LfoSh = 24; /*  8.24 fixed point (LFO calculations)       */
        private const int TimerSh = 16; /* 16.16 fixed point (timers calculations)    */
        private const int FreqMask = (1 << FreqSh) - 1;
        private const int MinAttIndex = 0; /* 0 */
        private const int EgAtt = 4;
        private const int EgDec = 3;
        private const int EgSus = 2;
        private const int EgRel = 1;
        private const int EgOff = 0;
        private const int SinBits = 10;
        private const int SinLen = 1 << SinBits;
        private const int SinMaskGx = SinLen - 1;
        private const int TlResLen = 256; /* 8 bits addressing (real chip) */

        private static byte eg_rate_selectO(byte a)
        {
            return (byte)(a * Ym2612Constants.RATE_STEPS);
        }

        private static readonly byte[] EgRateSelect = new byte[32 + 64 + 32]
        {
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(18),
            eg_rate_selectO(0),
            eg_rate_selectO(0),
            eg_rate_selectO(0),
            eg_rate_selectO(0),
            eg_rate_selectO(2),
            eg_rate_selectO(2),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(0),
            eg_rate_selectO(1),
            eg_rate_selectO(2),
            eg_rate_selectO(3),
            eg_rate_selectO(4),
            eg_rate_selectO(5),
            eg_rate_selectO(6),
            eg_rate_selectO(7),
            eg_rate_selectO(8),
            eg_rate_selectO(9),
            eg_rate_selectO(10),
            eg_rate_selectO(11),
            eg_rate_selectO(12),
            eg_rate_selectO(13),
            eg_rate_selectO(14),
            eg_rate_selectO(15),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16),
            eg_rate_selectO(16)
        };

        private static readonly byte[] LfoAmsDepthShift = new byte[4]
        {
            8,
            3,
            1,
            0
        };

        private static readonly byte[,] LfoPmOutput = new byte[7 * 8, 8]
        {
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 1 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 2 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 3 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 4 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 5 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 6 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 7 */
            {
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1
            },

            /* FNUM BIT 5: 000 0010xxxx */
            /* DEPTH 0 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 1 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 2 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 3 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 4 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 5 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 6 */
            {
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1
            },
            /* DEPTH 7 */
            {
                0,
                0,
                1,
                1,
                2,
                2,
                2,
                3
            },

            /* FNUM BIT 6: 000 0100xxxx */
            /* DEPTH 0 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 1 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 2 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 3 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 4 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1
            },
            /* DEPTH 5 */
            {
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1
            },
            /* DEPTH 6 */
            {
                0,
                0,
                1,
                1,
                2,
                2,
                2,
                3
            },
            /* DEPTH 7 */
            {
                0,
                0,
                2,
                3,
                4,
                4,
                5,
                6
            },

            /* FNUM BIT 7: 000 1000xxxx */
            /* DEPTH 0 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 1 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 2 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                1
            },
            /* DEPTH 3 */
            {
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1
            },
            /* DEPTH 4 */
            {
                0,
                0,
                0,
                1,
                1,
                1,
                1,
                2
            },
            /* DEPTH 5 */
            {
                0,
                0,
                1,
                1,
                2,
                2,
                2,
                3
            },
            /* DEPTH 6 */
            {
                0,
                0,
                2,
                3,
                4,
                4,
                5,
                6
            },
            /* DEPTH 7 */
            {
                0,
                0,
                4,
                6,
                8,
                8,
                0xa,
                0xc
            },

            /* FNUM BIT 8: 001 0000xxxx */
            /* DEPTH 0 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 1 */
            {
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1
            },
            /* DEPTH 2 */
            {
                0,
                0,
                0,
                1,
                1,
                1,
                2,
                2
            },
            /* DEPTH 3 */
            {
                0,
                0,
                1,
                1,
                2,
                2,
                3,
                3
            },
            /* DEPTH 4 */
            {
                0,
                0,
                1,
                2,
                2,
                2,
                3,
                4
            },
            /* DEPTH 5 */
            {
                0,
                0,
                2,
                3,
                4,
                4,
                5,
                6
            },
            /* DEPTH 6 */
            {
                0,
                0,
                4,
                6,
                8,
                8,
                0xa,
                0xc
            },
            /* DEPTH 7 */
            {
                0,
                0,
                8,
                0xc,
                0x10,
                0x10,
                0x14,
                0x18
            },

            /* FNUM BIT 9: 010 0000xxxx */
            /* DEPTH 0 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 1 */
            {
                0,
                0,
                0,
                0,
                2,
                2,
                2,
                2
            },
            /* DEPTH 2 */
            {
                0,
                0,
                0,
                2,
                2,
                2,
                4,
                4
            },
            /* DEPTH 3 */
            {
                0,
                0,
                2,
                2,
                4,
                4,
                6,
                6
            },
            /* DEPTH 4 */
            {
                0,
                0,
                2,
                4,
                4,
                4,
                6,
                8
            },
            /* DEPTH 5 */
            {
                0,
                0,
                4,
                6,
                8,
                8,
                0xa,
                0xc
            },
            /* DEPTH 6 */
            {
                0,
                0,
                8,
                0xc,
                0x10,
                0x10,
                0x14,
                0x18
            },
            /* DEPTH 7 */
            {
                0,
                0,
                0x10,
                0x18,
                0x20,
                0x20,
                0x28,
                0x30
            },

            /* FNUM BIT10: 100 0000xxxx */
            /* DEPTH 0 */
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            /* DEPTH 1 */
            {
                0,
                0,
                0,
                0,
                4,
                4,
                4,
                4
            },
            /* DEPTH 2 */
            {
                0,
                0,
                0,
                4,
                4,
                4,
                8,
                8
            },
            /* DEPTH 3 */
            {
                0,
                0,
                4,
                4,
                8,
                8,
                0xc,
                0xc
            },
            /* DEPTH 4 */
            {
                0,
                0,
                4,
                8,
                8,
                8,
                0xc,
                0x10
            },
            /* DEPTH 5 */
            {
                0,
                0,
                8,
                0xc,
                0x10,
                0x10,
                0x14,
                0x18
            },
            /* DEPTH 6 */
            {
                0,
                0,
                0x10,
                0x18,
                0x20,
                0x20,
                0x28,
                0x30
            },
            /* DEPTH 7 */
            {
                0,
                0,
                0x20,
                0x30,
                0x40,
                0x40,
                0x50,
                0x60
            },
        };

        private const int EnvQuiet = TlTabLen >> 3;

        private const int Slot1 = 0;
        private const int Slot2 = 2;
        private const int Slot3 = 1;
        private const int Slot4 = 3;


        private const int TlTabLen = 13 * 2 * TlResLen;

        /***********************************************************/
        /* YM2612 chip                                                */
        /***********************************************************/
        private class Ym2612Data
        {
            public readonly Ym2612Channel[] Ch = new Ym2612Channel[6]; /* channel state */
            public byte Dacen; /* DAC Mode  */
            public long Dacout; /* DAC output */
            public readonly Ym2612Opn Opn = new Ym2612Opn(); /* OPN state */

            public Ym2612Data()
            {
                for (var i = 0; i < 6; i++)
                    Ch[i] = new Ym2612Channel();
            }
        };

        private readonly int[] _tlTab = new int[TlTabLen];

        /* sin waveform table in 'decibel' scale */
        private readonly uint[] _sinTab = new uint[SinLen];

        private readonly long[] _lfoPmTable = new long[128 * 8 * 32]; /* 128 combinations of 7 bits meaningful (of F-NUMBER), 8 LFO depths, 32 LFO output levels per one depth */

        /* emulated chip */
        private readonly Ym2612Data _ym2612 = new Ym2612Data();

        /* current chip state */
        private readonly LongPointer _m2 = new LongPointer();

        private readonly LongPointer _c1 = new LongPointer();
        private readonly LongPointer _c2 = new LongPointer(); /* Phase Modulation input for operators 2,3,4 */
        private readonly LongPointer _mem = new LongPointer(); /* one sample delay memory */
        private readonly LongPointer[] _outFm = new LongPointer[8]; /* outputs of working channels */

        public Ym2612Core()
        {
            for (var i = 0; i < 8; i++)
            {
                _outFm[i] = new LongPointer();
            }
        }

        private void FM_KEYON(Ym2612Channel ch, int s)
        {
            var slot = ch.Slot[s];

            if (slot.key == 0 && _ym2612.Opn.Sl3.KeyCsm == 0)
            {
                slot.phase = 0;
                slot.ssgn = 0;

                if (slot.ar + slot.ksr < 94)
                {
                    slot.state = (byte)(slot.volume <= MinAttIndex ? (slot.sl == MinAttIndex ? EgSus : EgDec) : EgAtt);
                }
                else
                {
                    slot.volume = MinAttIndex;
                    slot.state = (byte)(slot.sl == MinAttIndex ? EgSus : EgDec);
                }

                if ((slot.ssg & 0x08) != 0 && (slot.ssgn ^ (slot.ssg & 0x04)) != 0)
                {
                    slot.vol_out = ((uint)(0x200 - slot.volume) & Ym2612Constants.MAX_ATT_INDEX) + slot.tl;
                }
                else
                {
                    slot.vol_out = (uint)slot.volume + slot.tl;
                }
            }

            slot.key = 1;
        }

        private void FM_KEYOFF(Ym2612Channel ch, int s)
        {
            var slot = ch.Slot[s];

            if (slot.key > 0 && _ym2612.Opn.Sl3.KeyCsm == 0 && slot.state > EgRel)
            {
                slot.state = EgRel;

                if ((slot.ssg & 0x08) > 0)
                {
                    if ((slot.ssgn ^ (slot.ssg & 0x04)) != 0)
                    {
                        slot.volume = 0x200 - slot.volume;
                    }

                    if (slot.volume >= 0x200)
                    {
                        slot.volume = Ym2612Constants.MAX_ATT_INDEX;
                        slot.state = EgOff;
                    }

                    slot.vol_out = (uint)slot.volume + slot.tl;
                }
            }

            slot.key = 0;
        }

        private void FM_KEYON_CSM(Ym2612Channel channel, int s)
        {
            var slot = channel.Slot[s];

            if (slot.key == 0 && _ym2612.Opn.Sl3.KeyCsm == 0)
            {
                slot.phase = 0;
                slot.ssgn = 0;

                if (slot.ar + slot.ksr < 94 /*32+62*/)
                {
                    slot.state = (byte)(slot.volume <= MinAttIndex ? (slot.sl == MinAttIndex ? EgSus : EgDec) : EgAtt);
                }
                else
                {
                    slot.volume = MinAttIndex;
                    slot.state = (byte)(slot.sl == MinAttIndex ? EgSus : EgDec);
                }

                /* recalculate EG output */
                if ((slot.ssg & 0x08) != 0 && (slot.ssgn ^ (slot.ssg & 0x04)) != 0)
                    slot.vol_out = ((uint)(0x200 - slot.volume) & Ym2612Constants.MAX_ATT_INDEX) + slot.tl;
                else
                    slot.vol_out = (uint)slot.volume + slot.tl;
            }
        }

        private void FM_KEYOFF_CSM(Ym2612Channel channel, int slotIndex)
        {
            var slot = channel.Slot[slotIndex];

            if (slot.key != 0 || slot.state <= EgRel)
            {
                return;
            }

            slot.state = EgRel;

            if ((slot.ssg & 0x08) <= 0)
            {
                return;
            }

            if ((slot.ssgn ^ (slot.ssg & 0x04)) > 0)
            {
                slot.volume = 0x200 - slot.volume;
            }

            if (slot.volume >= 0x200)
            {
                slot.volume = Ym2612Constants.MAX_ATT_INDEX;
                slot.state = EgOff;
            }

            slot.vol_out = (uint)slot.volume + slot.tl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CsmKeyControll(Ym2612Channel channel)
        {
            FM_KEYON_CSM(channel, Slot1);
            FM_KEYON_CSM(channel, Slot2);
            FM_KEYON_CSM(channel, Slot3);
            FM_KEYON_CSM(channel, Slot4);
            _ym2612.Opn.Sl3.KeyCsm = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INTERNAL_TIMER_A()
        {
            if ((_ym2612.Opn.St.Mode & 0x01) == 0)
            {
                return;
            }

            if ((_ym2612.Opn.St.Tac -= _ym2612.Opn.St.TimerBase) > 0)
            {
                return;
            }

            if ((_ym2612.Opn.St.Mode & 0x04) != 0)
            {
                _ym2612.Opn.St.Status |= 0x01;
            }

            if (_ym2612.Opn.St.Tal != 0)
            {
                _ym2612.Opn.St.Tac += _ym2612.Opn.St.Tal;
            }
            else
            {
                _ym2612.Opn.St.Tac = _ym2612.Opn.St.Tal;
            }

            if ((_ym2612.Opn.St.Mode & 0xC0) == 0x80)
            {
                CsmKeyControll(_ym2612.Ch[2]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INTERNAL_TIMER_B(int step)
        {
            if ((_ym2612.Opn.St.Mode & 0x02) == 0)
            {
                return;
            }

            if ((_ym2612.Opn.St.Tbc -= _ym2612.Opn.St.TimerBase * step) > 0)
            {
                return;
            }

            if ((_ym2612.Opn.St.Mode & 0x08) != 0)
            {
                _ym2612.Opn.St.Status |= 0x02;
            }

            if (_ym2612.Opn.St.Tbl == 0)
            {
                _ym2612.Opn.St.Tbc = _ym2612.Opn.St.Tbl;
            }
            else
            {
                _ym2612.Opn.St.Tbc += _ym2612.Opn.St.Tbl;
            }
        }

        private void set_timers(int v)
        {
            if (((_ym2612.Opn.St.Mode ^ v) & 0xC0) != 0)
            {
                _ym2612.Ch[2].Slot[Slot1].Incr = -1;

                if ((v & 0xC0) != 0x80 && _ym2612.Opn.Sl3.KeyCsm != 0)
                {
                    FM_KEYOFF_CSM(_ym2612.Ch[2], Slot1);
                    FM_KEYOFF_CSM(_ym2612.Ch[2], Slot2);
                    FM_KEYOFF_CSM(_ym2612.Ch[2], Slot3);
                    FM_KEYOFF_CSM(_ym2612.Ch[2], Slot4);
                    _ym2612.Opn.Sl3.KeyCsm = 0;
                }
            }

            if ((v & 1) != 0 && (_ym2612.Opn.St.Mode & 1) == 0)
            {
                _ym2612.Opn.St.Tac = _ym2612.Opn.St.Tal;
            }

            if ((v & 2) != 0 && (_ym2612.Opn.St.Mode & 2) == 0)
            {
                _ym2612.Opn.St.Tbc = _ym2612.Opn.St.Tbl;
            }

            _ym2612.Opn.St.Status &= (byte)(~v >> 4);
            _ym2612.Opn.St.Mode = (uint)v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetupConnection(Ym2612Channel channel, int channelIndex)
        {
            var carrier = _outFm[channelIndex];

            switch (channel.Algorithm)
            {
            case 0:
                channel.Connect1 = _c1;
                channel.Connect2 = _mem;
                channel.Connect3 = _c2;
                channel.MemoryConnect = _m2;
                break;
            case 1:
                channel.Connect1 = _mem;
                channel.Connect2 = _mem;
                channel.Connect3 = _c2;
                channel.MemoryConnect = _m2;
                break;
            case 2:
                channel.Connect1 = _c2;
                channel.Connect2 = _mem;
                channel.Connect3 = _c2;
                channel.MemoryConnect = _m2;
                break;
            case 3:
                channel.Connect1 = _c1;
                channel.Connect2 = _mem;
                channel.Connect3 = _c2;
                channel.MemoryConnect = _c2;
                break;
            case 4:
                channel.Connect1 = _c1;
                channel.Connect2 = carrier;
                channel.Connect3 = _c2;
                channel.MemoryConnect = _mem;
                break;
            case 5:
                channel.Connect1 = null;
                channel.Connect2 = carrier;
                channel.Connect3 = carrier;
                channel.MemoryConnect = _m2;
                break;
            case 6:
                channel.Connect1 = _c1;
                channel.Connect2 = carrier;
                channel.Connect3 = carrier;
                channel.MemoryConnect = _mem;
                break;
            case 7:
                channel.Connect1 = carrier;
                channel.Connect2 = carrier;
                channel.Connect3 = carrier;
                channel.MemoryConnect = _mem;
                break;
            }

            channel.Connect4 = carrier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void set_det_mul(Ym2612Channel ch, Ym2612Slot slot, int v)
        {
            slot.mul = (uint)((v & 0x0f) != 0 ? (v & 0x0f) * 2 : 1);
            slot.DT = _ym2612.Opn.St.DetuneTable[(v >> 4) & 7];
            ch.Slot[Slot1].Incr = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetTl(Ym2612Slot slot, int v)
        {
            slot.tl = (uint)((v & 0x7f) << (Ym2612Constants.EnvelopeBits - 7));

            if ((slot.ssg & 0x08) != 0 && (slot.ssgn ^ (slot.ssg & 0x04)) != 0 && slot.state > EgRel)
            {
                slot.vol_out = ((uint)(0x200 - slot.volume) & Ym2612Constants.MAX_ATT_INDEX) + slot.tl;
            }
            else
            {
                slot.vol_out = (uint)slot.volume + slot.tl;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetArKsr(Ym2612Channel channel, Ym2612Slot slot, int v)
        {
            var oldKsr = slot.KSR;

            slot.ar = (uint)((v & 0x1f) != 0 ? 32 + ((v & 0x1f) << 1) : 0);
            slot.KSR = (byte)(3 - (v >> 6));

            if (slot.KSR != oldKsr)
            {
                channel.Slot[Slot1].Incr = -1;
            }

            if (slot.ar + slot.ksr < 32 + 62)
            {
                slot.eg_sh_ar = Ym2612Constants.EgRateShift[slot.ar + slot.ksr];
                slot.eg_sel_ar = EgRateSelect[slot.ar + slot.ksr];
            }
            else
            {
                /* verified by Nemesis on real hardware (Attack phase is blocked) */
                slot.eg_sh_ar = 0;
                slot.eg_sel_ar = 18 * Ym2612Constants.RATE_STEPS;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetDecayRate(Ym2612Slot slot, int v)
        {
            slot.d1r = (uint)((v & 0x1f) != 0 ? 32 + ((v & 0x1f) << 1) : 0);
            slot.eg_sh_d1r = Ym2612Constants.EgRateShift[slot.d1r + slot.ksr];
            slot.eg_sel_d1r = EgRateSelect[slot.d1r + slot.ksr];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetSustainRate(Ym2612Slot slot, int v)
        {
            if ((v & 0x1f) == 0)
            {
                slot.d2r = 0;
            }
            else
            {
                slot.d2r = (uint)(32 + ((v & 0x1f) << 1));
            }

            slot.eg_sh_d2r = Ym2612Constants.EgRateShift[slot.d2r + slot.ksr];
            slot.eg_sel_d2r = EgRateSelect[slot.d2r + slot.ksr];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetReleaseRate(Ym2612Slot slot, int v)
        {
            slot.sl = Ym2612Constants.SlTable[v >> 4];

            if (slot.state == EgDec && slot.volume >= (int)slot.sl)
            {
                slot.state = EgSus;
            }

            slot.rr = (uint)(34 + ((v & 0x0f) << 2));
            slot.eg_sh_rr = Ym2612Constants.EgRateShift[slot.rr + slot.ksr];
            slot.eg_sel_rr = EgRateSelect[slot.rr + slot.ksr];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void advance_lfo()
        {
            if (_ym2612.Opn.LfoTimerOverflow == 0)
            {
                return;
            }

            _ym2612.Opn.LfoTimer += _ym2612.Opn.LfoTimerStep;

            while (_ym2612.Opn.LfoTimer >= _ym2612.Opn.LfoTimerOverflow)
            {
                _ym2612.Opn.LfoTimer -= _ym2612.Opn.LfoTimerOverflow;
                _ym2612.Opn.LfoCount = (byte)((_ym2612.Opn.LfoCount + 1) & 127);

                if (_ym2612.Opn.LfoCount < 64)
                {
                    _ym2612.Opn.LfoAm = (uint)(_ym2612.Opn.LfoCount * 2);
                }
                else
                {
                    _ym2612.Opn.LfoAm = (uint)(126 - (_ym2612.Opn.LfoCount & 63) * 2);
                }

                _ym2612.Opn.LfoPm = (uint)(_ym2612.Opn.LfoCount >> 2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void advance_eg_channels()
        {
            var egCnt = _ym2612.Opn.EgCounter;
            uint i = 0;
            uint j;

            do
            {
                var curSlot = Slot1;
                var slot = _ym2612.Ch[i].Slot[curSlot];
                j = 4; /* four operators per channel */
                do
                {
                    if (slot.state == EgAtt)
                    {
                        if ((egCnt & ((1 << slot.eg_sh_ar) - 1)) == 0)
                        {
                            /* update attenuation level */
                            slot.volume +=
                                (~slot.volume *
                                 Ym2612Constants.eg_inc[slot.eg_sel_ar + ((egCnt >> slot.eg_sh_ar) & 7)]) >> 4;

                            /* check phase transition*/
                            if (slot.volume <= MinAttIndex)
                            {
                                slot.volume = MinAttIndex;
                                slot.state =
                                    (byte)(slot.sl == MinAttIndex ? EgSus : EgDec); /* special case where SL=0 */
                            }

                            /* recalculate EG output */
                            if ((slot.ssg & 0x08) != 0 && (slot.ssgn ^ (slot.ssg & 0x04)) != 0
                            ) /* SSG-EG Output Inversion */
                                slot.vol_out = ((uint)(0x200 - slot.volume) & Ym2612Constants.MAX_ATT_INDEX) + slot.tl;
                            else
                                slot.vol_out = (uint)slot.volume + slot.tl;
                        }
                    }
                    else if (slot.state == EgDec)
                    {
                        if ((egCnt & ((1 << slot.eg_sh_d1r) - 1)) == 0)
                        {
                            /* SSG EG type */
                            if ((slot.ssg & 0x08) != 0)
                            {
                                /* update attenuation level */
                                if (slot.volume < 0x200)
                                {
                                    slot.volume +=
                                        4 * Ym2612Constants.eg_inc[slot.eg_sel_d1r + ((egCnt >> slot.eg_sh_d1r) & 7)];

                                    /* recalculate EG output */
                                    if ((slot.ssgn ^ (slot.ssg & 0x04)) != 0) /* SSG-EG Output Inversion */
                                        slot.vol_out = ((uint)(0x200 - slot.volume) & Ym2612Constants.MAX_ATT_INDEX) +
                                                       slot.tl;
                                    else
                                        slot.vol_out = (uint)slot.volume + slot.tl;
                                }
                            }
                            else
                            {
                                /* update attenuation level */
                                slot.volume +=
                                    Ym2612Constants.eg_inc[slot.eg_sel_d1r + ((egCnt >> slot.eg_sh_d1r) & 7)];

                                /* recalculate EG output */
                                slot.vol_out = (uint)slot.volume + slot.tl;
                            }

                            /* check phase transition*/
                            if (slot.volume >= (int)slot.sl)
                                slot.state = EgSus;
                        }
                    }
                    else if (slot.state == EgSus)
                    {
                        if ((egCnt & ((1 << slot.eg_sh_d2r) - 1)) == 0)
                        {
                            /* SSG EG type */
                            if ((slot.ssg & 0x08) != 0)
                            {
                                /* update attenuation level */
                                if (slot.volume < 0x200)
                                {
                                    slot.volume +=
                                        4 * Ym2612Constants.eg_inc[slot.eg_sel_d2r + ((egCnt >> slot.eg_sh_d2r) & 7)];

                                    /* recalculate EG output */
                                    if ((slot.ssgn ^ (slot.ssg & 0x04)) != 0) /* SSG-EG Output Inversion */
                                        slot.vol_out = ((uint)(0x200 - slot.volume) & Ym2612Constants.MAX_ATT_INDEX) +
                                                       slot.tl;
                                    else
                                        slot.vol_out = (uint)slot.volume + slot.tl;
                                }
                            }
                            else
                            {
                                /* update attenuation level */
                                slot.volume +=
                                    Ym2612Constants.eg_inc[slot.eg_sel_d2r + ((egCnt >> slot.eg_sh_d2r) & 7)];

                                /* check phase transition*/
                                if (slot.volume >= Ym2612Constants.MAX_ATT_INDEX)
                                    slot.volume = Ym2612Constants.MAX_ATT_INDEX;
                                /* do not change SLOT.state (verified on real chip) */

                                /* recalculate EG output */
                                slot.vol_out = (uint)slot.volume + slot.tl;
                            }
                        }
                    }
                    else if (slot.state == EgRel)
                    {
                        if ((egCnt & ((1 << slot.eg_sh_rr) - 1)) == 0)
                        {
                            /* SSG EG type */
                            if ((slot.ssg & 0x08) != 0)
                            {
                                /* update attenuation level */
                                if (slot.volume < 0x200)
                                    slot.volume +=
                                        4 * Ym2612Constants.eg_inc[slot.eg_sel_rr + ((egCnt >> slot.eg_sh_rr) & 7)];

                                /* check phase transition */
                                if (slot.volume >= 0x200)
                                {
                                    slot.volume = Ym2612Constants.MAX_ATT_INDEX;
                                    slot.state = EgOff;
                                }
                            }
                            else
                            {
                                /* update attenuation level */
                                slot.volume += Ym2612Constants.eg_inc[slot.eg_sel_rr + ((egCnt >> slot.eg_sh_rr) & 7)];

                                /* check phase transition*/
                                if (slot.volume >= Ym2612Constants.MAX_ATT_INDEX)
                                {
                                    slot.volume = Ym2612Constants.MAX_ATT_INDEX;
                                    slot.state = EgOff;
                                }
                            }

                            /* recalculate EG output */
                            slot.vol_out = (uint)slot.volume + slot.tl;
                        }
                    }

                    curSlot += 1;
                    if (curSlot < _ym2612.Ch[i].Slot.Count())
                        slot = _ym2612.Ch[i].Slot[curSlot];
                    j--;
                } while (j != 0);

                i++;
            } while (i < 6); /* 6 channels */
        }

        private static void update_ssg_eg_channel(Ym2612Slot[] slots)
        {
            uint i = 4; /* four operators per channel */
            var curSlot = Slot1;
            var slot = slots[curSlot];
            do
            {
                if ((slot.ssg & 0x08) != 0 && slot.volume >= 0x200 && slot.state > EgRel)
                {
                    if ((slot.ssg & 0x01) != 0) /* bit 0 = hold SSG-EG */
                    {
                        /* set inversion flag */
                        if ((slot.ssg & 0x02) != 0)
                            slot.ssgn = 4;

                        /* force attenuation level during decay phases */
                        if (slot.state != EgAtt && (slot.ssgn ^ (slot.ssg & 0x04)) == 0)
                            slot.volume = Ym2612Constants.MAX_ATT_INDEX;
                    }
                    else /* loop SSG-EG */
                    {
                        /* toggle output inversion flag or reset Phase Generator */
                        if ((slot.ssg & 0x02) != 0)
                            slot.ssgn ^= 4;
                        else
                            slot.phase = 0;

                        /* same as Key ON */
                        if (slot.state != EgAtt)
                        {
                            if (slot.ar + slot.ksr < 94 /*32+62*/)
                            {
                                slot.state = (byte)(slot.volume <= MinAttIndex ? (slot.sl == MinAttIndex ? EgSus : EgDec) : EgAtt);
                            }
                            else
                            {
                                /* Attack Rate is maximal: directly switch to Decay or Substain */
                                slot.volume = MinAttIndex;
                                slot.state = (byte)(slot.sl == MinAttIndex ? EgSus : EgDec);
                            }
                        }
                    }

                    /* recalculate EG output */
                    if ((slot.ssgn ^ (slot.ssg & 0x04)) != 0)
                        slot.vol_out = ((uint)(0x200 - slot.volume) & Ym2612Constants.MAX_ATT_INDEX) + slot.tl;
                    else
                        slot.vol_out = (uint)slot.volume + slot.tl;
                }

                /* next slot */
                curSlot++;
                if (curSlot < slots.Count())
                    slot = slots[curSlot];
                i--;
            } while (i != 0);
        }

        private void update_phase_lfo_slot(Ym2612Slot slot, long pms, uint blockFnum)
        {
            var lfoFnTableIndexOffset = _lfoPmTable[(((blockFnum & 0x7f0) >> 4) << 8) + pms + _ym2612.Opn.LfoPm];

            if (lfoFnTableIndexOffset != 0) /* LFO phase modulation active */
            {
                byte blk;
                int kc, fc;

                blockFnum = (uint)(blockFnum * 2 + lfoFnTableIndexOffset);
                blk = (byte)((blockFnum & 0x7000) >> 12);
                blockFnum = blockFnum & 0xfff;
                kc = (blk << 2) | Ym2612Constants.OpnFkTable[blockFnum >> 8];
                fc = (int)((_ym2612.Opn.FnTable[blockFnum] >> (7 - blk)) + slot.DT[kc]);

                /* (frequency) phase overflow (credits to Nemesis) */
                if (fc < 0)
                    fc += (int)_ym2612.Opn.FnMax;

                /* update phase */
                slot.phase += (uint)((fc * slot.mul) >> 1);
            }
            else /* LFO phase modulation  = zero */
            {
                slot.phase += (uint)slot.Incr;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void update_phase_lfo_channel(Ym2612Channel ch)
        {
            var blockFnum = ch.BlockFnum;

            var lfoFnTableIndexOffset = _lfoPmTable[(((blockFnum & 0x7f0) >> 4) << 8) + ch.Pms + _ym2612.Opn.LfoPm];

            if (lfoFnTableIndexOffset != 0) /* LFO phase modulation active */
            {
                byte blk;
                int kc, fc, finc;

                blockFnum = (uint)(blockFnum * 2 + lfoFnTableIndexOffset);
                blk = (byte)((blockFnum & 0x7000) >> 12);
                blockFnum = blockFnum & 0xfff;

                /* keyscale code */
                kc = (blk << 2) | Ym2612Constants.OpnFkTable[blockFnum >> 8];

                /* (frequency) phase increment counter */
                fc = (int)(_ym2612.Opn.FnTable[blockFnum] >> (7 - blk));

                /* (frequency) phase overflow (credits to Nemesis) */
                finc = (int)(fc + ch.Slot[Slot1].DT[kc]);
                if (finc < 0)
                    finc += (int)_ym2612.Opn.FnMax;
                ch.Slot[Slot1].phase += (uint)((finc * ch.Slot[Slot1].mul) >> 1);

                finc = (int)(fc + ch.Slot[Slot2].DT[kc]);
                if (finc < 0)
                    finc += (int)_ym2612.Opn.FnMax;
                ch.Slot[Slot2].phase += (uint)((finc * ch.Slot[Slot2].mul) >> 1);

                finc = (int)(fc + ch.Slot[Slot3].DT[kc]);
                if (finc < 0)
                    finc += (int)_ym2612.Opn.FnMax;
                ch.Slot[Slot3].phase += (uint)((finc * ch.Slot[Slot3].mul) >> 1);

                finc = (int)(fc + ch.Slot[Slot4].DT[kc]);
                if (finc < 0)
                    finc += (int)_ym2612.Opn.FnMax;
                ch.Slot[Slot4].phase += (uint)((finc * ch.Slot[Slot4].mul) >> 1);
            }
            else /* LFO phase modulation  = zero */
            {
                ch.Slot[Slot1].phase += (uint)ch.Slot[Slot1].Incr;
                ch.Slot[Slot2].phase += (uint)ch.Slot[Slot2].Incr;
                ch.Slot[Slot3].phase += (uint)ch.Slot[Slot3].Incr;
                ch.Slot[Slot4].phase += (uint)ch.Slot[Slot4].Incr;
            }
        }

        /* update phase increment and envelope generator */
        private void refresh_fc_eg_slot(Ym2612Slot slot, int fc, int kc)
        {
            /* add detune value */
            fc += (int)slot.DT[kc];

            /* (frequency) phase overflow (credits to Nemesis) */
            if (fc < 0)
                fc += (int)_ym2612.Opn.FnMax;

            /* (frequency) phase increment counter */
            slot.Incr = (fc * slot.mul) >> 1;

            /* ksr */
            kc = kc >> slot.KSR;

            if (slot.ksr != kc)
            {
                slot.ksr = (byte)kc;

                /* recalculate envelope generator rates */
                if (slot.ar + kc < 32 + 62)
                {
                    slot.eg_sh_ar = Ym2612Constants.EgRateShift[slot.ar + kc];
                    slot.eg_sel_ar = EgRateSelect[slot.ar + kc];
                }
                else
                {
                    slot.eg_sh_ar = 0;
                    slot.eg_sel_ar = 18 * Ym2612Constants.RATE_STEPS;
                }

                slot.eg_sh_d1r = Ym2612Constants.EgRateShift[slot.d1r + kc];
                slot.eg_sel_d1r = EgRateSelect[slot.d1r + kc];
                slot.eg_sh_d2r = Ym2612Constants.EgRateShift[slot.d2r + kc];
                slot.eg_sel_d2r = EgRateSelect[slot.d2r + kc];
                slot.eg_sh_rr = Ym2612Constants.EgRateShift[slot.rr + kc];
                slot.eg_sel_rr = EgRateSelect[slot.rr + kc];
            }
        }

        /* update phase increment counters */
        private void refresh_fc_eg_chan(Ym2612Channel ch)
        {
            if (ch.Slot[Slot1].Incr == -1)
            {
                var fc = (int)ch.Fc;
                var kc = (int)ch.KeyCode;
                refresh_fc_eg_slot(ch.Slot[Slot1], fc, kc);
                refresh_fc_eg_slot(ch.Slot[Slot2], fc, kc);
                refresh_fc_eg_slot(ch.Slot[Slot3], fc, kc);
                refresh_fc_eg_slot(ch.Slot[Slot4], fc, kc);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint CalculateVolume(Ym2612Slot slot, uint am)
        {
            return slot.vol_out + (am & slot.AMmask);
        }

        private int op_calc(uint phase, uint env, int pm)
        {
            var p = (env << 3) + _sinTab[((int)((phase & ~FreqMask) + (pm << 15)) >> FreqSh) & SinMaskGx];

            if (p >= TlTabLen)
                return 0;

            return _tlTab[p];
        }

        private int op_calc1(uint phase, uint env, int pm)
        {
            var p = (env << 3) + _sinTab[((int)((phase & ~FreqMask) + pm) >> FreqSh) & SinMaskGx];

            if (p >= TlTabLen)
                return 0;

            return _tlTab[p];
        }

        private void chan_calc(Ym2612Channel ch)
        {
            var am = (uint)_ym2612.Opn.LfoAm >> ch.Ams;
            var egOut = CalculateVolume(ch.Slot[Slot1], am);

            _m2.Value = _c1.Value = _c2.Value = _mem.Value = 0;

            ch.MemoryConnect.Value = ch.MemoryValue; /* restore delayed sample (MEM) value to m2 or c2 */
            {
                var outVal = ch.Op1Output[0] + ch.Op1Output[1];
                ch.Op1Output[0] = ch.Op1Output[1];

                if (ch.Connect1 == null)
                {
                    /* algorithm 5  */
                    _mem.Value = _c1.Value = _c2.Value = ch.Op1Output[0];
                }
                else
                {
                    /* other algorithms */
                    ch.Connect1.Value += ch.Op1Output[0];
                }

                ch.Op1Output[1] = 0;
                if (egOut < EnvQuiet) /* SLOT 1 */
                {
                    if (ch.Feedback == 0)
                        outVal = 0;

                    ch.Op1Output[1] = op_calc1(ch.Slot[Slot1].phase, egOut, (int)(outVal << ch.Feedback));
                }
            }

            egOut = CalculateVolume(ch.Slot[Slot3], am);
            if (egOut < EnvQuiet) /* SLOT 3 */
                ch.Connect3.Value += op_calc(ch.Slot[Slot3].phase, egOut, (int)_m2.Value);

            egOut = CalculateVolume(ch.Slot[Slot2], am);
            if (egOut < EnvQuiet) /* SLOT 2 */
                ch.Connect2.Value += op_calc(ch.Slot[Slot2].phase, egOut, (int)_c1.Value);

            egOut = CalculateVolume(ch.Slot[Slot4], am);
            if (egOut < EnvQuiet) /* SLOT 4 */
                ch.Connect4.Value += op_calc(ch.Slot[Slot4].phase, egOut, (int)_c2.Value);

            /* store current MEM */
            ch.MemoryValue = _mem.Value;

            /* update phase counters AFTER output calculations */
            if (ch.Pms != 0)
            {
                /* add support for 3 slot Mode */
                if ((_ym2612.Opn.St.Mode & 0xC0) > 0 && ch == _ym2612.Ch[2])
                {
                    update_phase_lfo_slot(ch.Slot[Slot1], ch.Pms, _ym2612.Opn.Sl3.BlockFnum[1]);
                    update_phase_lfo_slot(ch.Slot[Slot2], ch.Pms, _ym2612.Opn.Sl3.BlockFnum[2]);
                    update_phase_lfo_slot(ch.Slot[Slot3], ch.Pms, _ym2612.Opn.Sl3.BlockFnum[0]);
                    update_phase_lfo_slot(ch.Slot[Slot4], ch.Pms, ch.BlockFnum);
                }
                else
                    update_phase_lfo_channel(ch);
            }
            else /* no LFO phase modulation */
            {
                ch.Slot[Slot1].phase += (uint)ch.Slot[Slot1].Incr;
                ch.Slot[Slot2].phase += (uint)ch.Slot[Slot2].Incr;
                ch.Slot[Slot3].phase += (uint)ch.Slot[Slot3].Incr;
                ch.Slot[Slot4].phase += (uint)ch.Slot[Slot4].Incr;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OpnWriteMode(int r, int v)
        {
            byte c;

            switch (r)
            {
            case 0x21: /* Test */
                break;

            case 0x22: /* LFO FREQ (YM2608/YM2610/YM2610B/ym2612) */
                if ((v & 8) != 0) /* LFO enabled ? */
                {
                    if (_ym2612.Opn.LfoTimerOverflow == 0)
                    {
                        /* restart LFO */
                        _ym2612.Opn.LfoCount = 0;
                        _ym2612.Opn.LfoTimer = 0;
                        _ym2612.Opn.LfoAm = 0;
                        _ym2612.Opn.LfoPm = 0;
                    }

                    _ym2612.Opn.LfoTimerOverflow = Ym2612Constants.LfoSamplesPerStep[v & 7] << LfoSh;
                }
                else
                {
                    _ym2612.Opn.LfoTimerOverflow = 0;
                }

                break;
            case 0x24: /* timer A High 8*/
                _ym2612.Opn.St.Ta = (_ym2612.Opn.St.Ta & 0x03) | ((long)v << 2);
                _ym2612.Opn.St.Tal = (1024 - _ym2612.Opn.St.Ta) << TimerSh;
                break;
            case 0x25: /* timer A Low 2*/
                _ym2612.Opn.St.Ta = (_ym2612.Opn.St.Ta & 0x3fc) | ((long)v & 3);
                _ym2612.Opn.St.Tal = (1024 - _ym2612.Opn.St.Ta) << TimerSh;
                break;
            case 0x26: /* timer B */
                _ym2612.Opn.St.Tb = v;
                _ym2612.Opn.St.Tbl = (256 - _ym2612.Opn.St.Tb) << (TimerSh + 4);
                break;
            case 0x27: /* Mode, timer control */
                set_timers(v);
                break;
            case 0x28: /* key on / off */
                c = (byte)(v & 0x03);
                if (c == 3)
                    break;

                if ((v & 0x04) != 0)
                    c += 3; /* CH 4-6 */
                var ch = _ym2612.Ch[c];

                if ((v & 0x10) != 0)
                    FM_KEYON(ch, Slot1);
                else
                    FM_KEYOFF(ch, Slot1);

                if ((v & 0x20) != 0)
                    FM_KEYON(ch, Slot2);
                else
                    FM_KEYOFF(ch, Slot2);

                if ((v & 0x40) != 0)
                    FM_KEYON(ch, Slot3);
                else
                    FM_KEYOFF(ch, Slot3);

                if ((v & 0x80) != 0)
                    FM_KEYON(ch, Slot4);
                else
                    FM_KEYOFF(ch, Slot4);
                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte GetOpnChannel(int n)
        {
            return (byte)(n & 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetOpnSlot(int n)
        {
            return (n >> 2) & 3;
        }

        /* write a OPN register (0x30-0xff) */
        private void OpnWriteReg(int r, int v)
        {
            Ym2612Channel ch;
            Ym2612Slot slot;

            var c = GetOpnChannel(r);

            if (c == 3)
                return; /* 0xX3,0xX7,0xXB,0xXF */

            if (r >= 0x100)
                c += 3;

            ch = _ym2612.Ch[c];

            slot = ch.Slot[GetOpnSlot(r)];

            switch (r & 0xf0)
            {
            case 0x30: /* DET , MUL */
                set_det_mul(ch, slot, v);
                break;

            case 0x40: /* TL */
                SetTl(slot, v);
                break;

            case 0x50: /* KS, AR */
                SetArKsr(ch, slot, v);
                break;

            case 0x60: /* bit7 = AM ENABLE, DR */
                SetDecayRate(slot, v);
                slot.AMmask = (uint)((v & 0x80) != 0 ? ~0 : 0);
                break;

            case 0x70: /*     SR */
                SetSustainRate(slot, v);
                break;

            case 0x80: /* SL, RR */
                SetReleaseRate(slot, v);
                break;

            case 0x90: /* SSG-EG */
                slot.ssg = (byte)(v & 0x0f);

                /* recalculate EG output */
                if (slot.state > EgRel)
                {
                    if ((slot.ssg & 0x08) != 0 && (slot.ssgn ^ (slot.ssg & 0x04)) != 0)
                        slot.vol_out = ((uint)(0x200 - slot.volume) & Ym2612Constants.MAX_ATT_INDEX) + slot.tl;
                    else
                        slot.vol_out = (uint)slot.volume + slot.tl;
                }

                break;

            case 0xa0:
                switch (GetOpnSlot(r))
                {
                case 0: /* 0xa0-0xa2 : FNUM1 */
                {
                    var fn = (uint)(((uint)(_ym2612.Opn.St.FnH & 7) << 8) + v);
                    var blk = (byte)(_ym2612.Opn.St.FnH >> 3);
                    /* keyscale code */
                    ch.KeyCode = (byte)((blk << 2) | Ym2612Constants.OpnFkTable[fn >> 7]);
                    /* phase increment counter */
                    ch.Fc = _ym2612.Opn.FnTable[fn * 2] >> (7 - blk);

                    /* store fnum in clear form for LFO PM calculations */
                    ch.BlockFnum = (uint)(((uint)blk << 11) | fn);

                    ch.Slot[Slot1].Incr = -1;
                    break;
                }
                case 1: /* 0xa4-0xa6 : FNUM2,BLK */
                    _ym2612.Opn.St.FnH = (byte)(v & 0x3f);
                    break;
                case 2: /* 0xa8-0xaa : 3CH FNUM1 */
                    if (r < 0x100)
                    {
                        var fn = (uint)(((uint)(_ym2612.Opn.Sl3.FnLatch & 7) << 8) + v);
                        var blk = (byte)(_ym2612.Opn.Sl3.FnLatch >> 3);
                        /* keyscale code */
                        _ym2612.Opn.Sl3.KeyCode[c] = (byte)((blk << 2) | Ym2612Constants.OpnFkTable[fn >> 7]);
                        /* phase increment counter */
                        _ym2612.Opn.Sl3.Fc[c] = _ym2612.Opn.FnTable[fn * 2] >> (7 - blk);
                        _ym2612.Opn.Sl3.BlockFnum[c] = (uint)(((uint)blk << 11) | fn);
                        _ym2612.Ch[2].Slot[Slot1].Incr = -1;
                    }
                    break;
                case 3: /* 0xac-0xae : 3CH FNUM2,BLK */
                    if (r < 0x100)
                        _ym2612.Opn.Sl3.FnLatch = (byte)(v & 0x3f);
                    break;
                }

                break;

            case 0xb0:
                switch (GetOpnSlot(r))
                {
                case 0: /* 0xb0-0xb2 : Feedback,Algorithm */
                {
                    var feedback = (v >> 3) & 7;
                    ch.Algorithm = (byte)(v & 7);
                    ch.Feedback = (byte)(feedback != 0 ? feedback + 6 : 0);
                    SetupConnection(ch, c);
                    break;
                }
                case 1: /* 0xb4-0xb6 : L , R , AMS , PMS (ym2612/YM2610B/YM2610/YM2608) */
                    /* b0-2 PMS */
                    ch.Pms = (v & 7) * 32; /* CH.Pms = PM depth * 32 (index in lfo_pm_table) */

                    /* b4-5 AMS */
                    ch.Ams = LfoAmsDepthShift[(v >> 4) & 0x03];

                    /* PAN :  b7 = L, b6 = R */
                    _ym2612.Opn.Pan[c * 2] = (uint)((v & 0x80) != 0 ? ~0 : 0);
                    _ym2612.Opn.Pan[c * 2 + 1] = (uint)((v & 0x40) != 0 ? ~0 : 0);
                    break;
                }

                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void init_timetables(double freqbase)
        {
            int i, d;

            /* DeTune table */
            for (d = 0; d <= 3; d++)
            {
                for (i = 0; i <= 31; i++)
                {
                    var rate = Ym2612Constants.DtTable[d * 32 + i] * freqbase * (1 << (FreqSh - 10));
                    _ym2612.Opn.St.DetuneTable[d][i] = (int)rate;
                    _ym2612.Opn.St.DetuneTable[d + 4][i] = -_ym2612.Opn.St.DetuneTable[d][i];
                }
            }

            for (i = 0; i < 4096; i++)
            {
                _ym2612.Opn.FnTable[i] = (uint)((double)i * 32 * freqbase * (1 << (FreqSh - 10))); /* -10 because chip works with 10.10 fixed point, while we use 16.16 */
            }

            /* maximal frequency is required for Phase overflow calculation, register size is 17 bits (Nemesis) */
            _ym2612.Opn.FnMax = (uint)((double)0x20000 * freqbase * (1 << (FreqSh - 10)));
        }

        /* prescaler set (and make time tables) */
        private void OpnSetPres(int pres)
        {
            var freqbase = _ym2612.Opn.St.Clock / _ym2612.Opn.St.Rate / pres;

            _ym2612.Opn.EgTimerAdd = (uint)((1 << EgSh) * freqbase);
            _ym2612.Opn.EgTimerOverflow = 3 * (1 << EgSh);
            _ym2612.Opn.LfoTimerStep = (uint)((1 << LfoSh) * freqbase);
            _ym2612.Opn.St.TimerBase = (int)((1 << TimerSh) * freqbase);
            init_timetables(freqbase);
        }

        /// <summary>
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="numberChannels"></param>
        private static void ResetChannels(IReadOnlyList<Ym2612Channel> channels, int numberChannels)
        {
            for (var channelIndex = 0; channelIndex < numberChannels; channelIndex++)
            {
                channels[channelIndex].MemoryValue = 0;
                channels[channelIndex].Op1Output[0] = 0;
                channels[channelIndex].Op1Output[1] = 0;

                for (var slotIndex = 0; slotIndex < 4; slotIndex++)
                {
                    channels[channelIndex].Slot[slotIndex].Incr = -1;
                    channels[channelIndex].Slot[slotIndex].key = 0;
                    channels[channelIndex].Slot[slotIndex].phase = 0;
                    channels[channelIndex].Slot[slotIndex].ssgn = 0;
                    channels[channelIndex].Slot[slotIndex].state = EgOff;
                    channels[channelIndex].Slot[slotIndex].volume = Ym2612Constants.MAX_ATT_INDEX;
                    channels[channelIndex].Slot[slotIndex].vol_out = Ym2612Constants.MAX_ATT_INDEX;
                }
            }
        }

        /* initialize generic tables */
        private void init_tables()
        {
            int i, x;
            int n;
            double o, m;

            /* DAC precision */
            var mask = (uint)~((1 << (14 - DacBits)) - 1);

            /* build Linear Power Table */
            for (x = 0; x < TlResLen; x++)
            {
                m = (1 << 16) / Math.Pow(2, (x + 1) * (Ym2612Constants.EnvelopeStepGx / 4.0) / 8.0);
                m = Math.Floor(m);
                n = (int)m; /* 16 bits here */
                n >>= 4; /* 12 bits here */

                if ((n & 1) != 0)
                {
                    n = (n >> 1) + 1;
                }
                else
                {
                    n = n >> 1;
                }

                n <<= 2;
                _tlTab[x * 2 + 0] = (int)(n & mask);
                _tlTab[x * 2 + 1] = (int)(-_tlTab[x * 2 + 0] & mask);

                for (i = 1; i < 13; i++)
                {
                    _tlTab[x * 2 + 0 + i * 2 * TlResLen] = (int)((_tlTab[x * 2 + 0] >> i) & mask);
                    _tlTab[x * 2 + 1 + i * 2 * TlResLen] = (int)(-_tlTab[x * 2 + 0 + i * 2 * TlResLen] & mask);
                }
            }

            /* build Logarithmic Sinus table */
            for (i = 0; i < SinLen; i++)
            {
                m = Math.Sin((i * 2 + 1) * Math.PI / SinLen); /* checked against the real chip */
                /* we never reach zero here due to ((i*2)+1) */

                if (m > 0.0)
                    o = 8 * Math.Log(1.0 / m) / Math.Log(2.0f); /* convert to 'decibels' */
                else
                    o = 8 * Math.Log(-1.0 / m) / Math.Log(2.0f); /* convert to 'decibels' */

                o = o / (Ym2612Constants.EnvelopeStepGx / 4);

                n = (int)(2.0 * o);
                if ((n & 1) != 0) /* round to nearest */
                    n = (n >> 1) + 1;
                else
                    n = n >> 1;

                _sinTab[i] = (uint)(n * 2 + (m >= 0.0 ? 0 : 1));
            }

            for (i = 0; i < 8; i++)
            {
                byte fnum;
                for (fnum = 0; fnum < 128; fnum++)
                {
                    byte step;
                    var offsetDepth = (uint)i;

                    for (step = 0; step < 8; step++)
                    {
                        byte value = 0;
                        uint bitTmp;
                        for (bitTmp = 0; bitTmp < 7; bitTmp++) /* 7 bits */
                        {
                            if ((fnum & (1 << (int)bitTmp)) != 0) /* only if bit "bit_tmp" is set */
                            {
                                var offsetFnumBit = bitTmp * 8;
                                value += LfoPmOutput[offsetFnumBit + offsetDepth, step];
                            }
                        }
                        /* 32 steps for LFO PM (sinus) */
                        _lfoPmTable[fnum * 32 * 8 + i * 32 + step + 0] = value;
                        _lfoPmTable[fnum * 32 * 8 + i * 32 + (step ^ 7) + 8] = value;
                        _lfoPmTable[fnum * 32 * 8 + i * 32 + step + 16] = -value;
                        _lfoPmTable[fnum * 32 * 8 + i * 32 + (step ^ 7) + 24] = -value;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Initialize(double clock, int rate)
        {
            init_tables();
            _ym2612.Opn.St.Clock = clock;
            _ym2612.Opn.St.Rate = (uint)rate;
            OpnSetPres(6 * 24); /* YM2612 prescaler is fixed to 1/6, one sample (6 mixed channels) is output for each 24 FM clocks */
        }

        public int DacBits { get; set; } = 14;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            int i;

            _ym2612.Opn.EgTimer = 0;
            _ym2612.Opn.EgCounter = 0;

            _ym2612.Opn.LfoTimerOverflow = 0;
            _ym2612.Opn.LfoTimer = 0;
            _ym2612.Opn.LfoCount = 0;
            _ym2612.Opn.LfoAm = 0;
            _ym2612.Opn.LfoPm = 0;

            _ym2612.Opn.St.Tac = 0;
            _ym2612.Opn.St.Tbc = 0;

            _ym2612.Opn.Sl3.KeyCsm = 0;

            _ym2612.Dacen = 0;
            _ym2612.Dacout = 0;

            set_timers(0x30);
            _ym2612.Opn.St.Tb = 0;
            _ym2612.Opn.St.Tbl = 256 << (TimerSh + 4);
            _ym2612.Opn.St.Ta = 0;
            _ym2612.Opn.St.Tal = 1024 << TimerSh;

            ResetChannels(_ym2612.Ch, 6);

            for (i = 0xb6; i >= 0xb4; i--)
            {
                OpnWriteReg(i, 0xc0);
                OpnWriteReg(i | 0x100, 0xc0);
            }
            for (i = 0xb2; i >= 0x30; i--)
            {
                OpnWriteReg(i, 0);
                OpnWriteReg(i | 0x100, 0);
            }
        }

        public void Write(uint a, uint v)
        {
            v &= 0xff;

            switch (a)
            {
            case 0: /* address port 0 */
                _ym2612.Opn.St.Address = (ushort)v;
                break;

            case 2: /* address port 1 */
                _ym2612.Opn.St.Address = (ushort)(v | 0x100);
                break;

            default: /* data port */
            {
                int addr = _ym2612.Opn.St.Address; /* verified by Nemesis on real YM2612 */
                switch (addr & 0x1f0)
                {
                case 0x20: /* 0x20-0x2f Mode */
                    switch (addr)
                    {
                    case 0x2a: /* DAC data (ym2612) */
                        _ym2612.Dacout = ((int)v - 0x80) << 6; /* level unknown (5 is too low, 8 is too loud) */
                        break;
                    case 0x2b: /* DAC Sel  (ym2612) */
                        /* b7 = dac enable */
                        _ym2612.Dacen = (byte)(v & 0x80);
                        break;
                    default: /* OPN section */
                        /* write register */
                        OpnWriteMode(addr, (int)v);
                        break;
                    }

                    break;
                default: /* 0x30-0xff OPN section */
                    /* write register */
                    OpnWriteReg(addr, (int)v);
                    break;
                }

                break;
            }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Ym2612Read()
        {
            return (uint)(_ym2612.Opn.St.Status & 0xff);
        }

        public void Update(int[] buffer, int length)
        {
            int i;
            int rt;

            refresh_fc_eg_chan(_ym2612.Ch[0]);
            refresh_fc_eg_chan(_ym2612.Ch[1]);

            if ((_ym2612.Opn.St.Mode & 0xC0) == 0)
            {
                refresh_fc_eg_chan(_ym2612.Ch[2]);
            }
            else
            {
                if (_ym2612.Ch[2].Slot[Slot1].Incr == -1)
                {
                    refresh_fc_eg_slot(_ym2612.Ch[2].Slot[Slot1], (int)_ym2612.Opn.Sl3.Fc[1], _ym2612.Opn.Sl3.KeyCode[1]);
                    refresh_fc_eg_slot(_ym2612.Ch[2].Slot[Slot2], (int)_ym2612.Opn.Sl3.Fc[2], _ym2612.Opn.Sl3.KeyCode[2]);
                    refresh_fc_eg_slot(_ym2612.Ch[2].Slot[Slot3], (int)_ym2612.Opn.Sl3.Fc[0], _ym2612.Opn.Sl3.KeyCode[0]);
                    refresh_fc_eg_slot(_ym2612.Ch[2].Slot[Slot4], (int)_ym2612.Ch[2].Fc, _ym2612.Ch[2].KeyCode);
                }
            }

            refresh_fc_eg_chan(_ym2612.Ch[3]);
            refresh_fc_eg_chan(_ym2612.Ch[4]);
            refresh_fc_eg_chan(_ym2612.Ch[5]);

            var bufferPos = 0;
            /* buffering */
            for (i = 0; i < length; i++)
            {
                /* clear outputs */
                _outFm[0].Value = 0;
                _outFm[1].Value = 0;
                _outFm[2].Value = 0;
                _outFm[3].Value = 0;
                _outFm[4].Value = 0;
                _outFm[5].Value = 0;

                /* update SSG-EG output */
                update_ssg_eg_channel(_ym2612.Ch[0].Slot);
                update_ssg_eg_channel(_ym2612.Ch[1].Slot);
                update_ssg_eg_channel(_ym2612.Ch[2].Slot);
                update_ssg_eg_channel(_ym2612.Ch[3].Slot);
                update_ssg_eg_channel(_ym2612.Ch[4].Slot);
                update_ssg_eg_channel(_ym2612.Ch[5].Slot);

                /* calculate FM */
                chan_calc(_ym2612.Ch[0]);
                chan_calc(_ym2612.Ch[1]);
                chan_calc(_ym2612.Ch[2]);
                chan_calc(_ym2612.Ch[3]);
                chan_calc(_ym2612.Ch[4]);
                if (_ym2612.Dacen == 0)
                {
                    chan_calc(_ym2612.Ch[5]);
                }
                else
                {
                    /* DAC Mode */
                    _outFm[5].Value = _ym2612.Dacout;
                }

                /* advance LFO */
                advance_lfo();

                /* advance envelope generator */
                _ym2612.Opn.EgTimer += _ym2612.Opn.EgTimerAdd;
                while (_ym2612.Opn.EgTimer >= _ym2612.Opn.EgTimerOverflow)
                {
                    _ym2612.Opn.EgTimer -= _ym2612.Opn.EgTimerOverflow;
                    _ym2612.Opn.EgCounter++;
                    advance_eg_channels();
                }

                /* 14-bit DAC inputs (range is -8192;+8192) */
                if (_outFm[0].Value > 8192)
                    _outFm[0].Value = 8192;
                else if (_outFm[0].Value < -8192)
                    _outFm[0].Value = -8192;
                if (_outFm[1].Value > 8192)
                    _outFm[1].Value = 8192;
                else if (_outFm[1].Value < -8192)
                    _outFm[1].Value = -8192;
                if (_outFm[2].Value > 8192)
                    _outFm[2].Value = 8192;
                else if (_outFm[2].Value < -8192)
                    _outFm[2].Value = -8192;
                if (_outFm[3].Value > 8192)
                    _outFm[3].Value = 8192;
                else if (_outFm[3].Value < -8192)
                    _outFm[3].Value = -8192;
                if (_outFm[4].Value > 8192)
                    _outFm[4].Value = 8192;
                else if (_outFm[4].Value < -8192)
                    _outFm[4].Value = -8192;
                if (_outFm[5].Value > 8192)
                    _outFm[5].Value = 8192;
                else if (_outFm[5].Value < -8192)
                    _outFm[5].Value = -8192;

                var lt = (int)(_outFm[0].Value & _ym2612.Opn.Pan[0]);
                rt = (int)(_outFm[0].Value & _ym2612.Opn.Pan[1]);
                lt += (int)(_outFm[1].Value & _ym2612.Opn.Pan[2]);
                rt += (int)(_outFm[1].Value & _ym2612.Opn.Pan[3]);
                lt += (int)(_outFm[2].Value & _ym2612.Opn.Pan[4]);
                rt += (int)(_outFm[2].Value & _ym2612.Opn.Pan[5]);
                lt += (int)(_outFm[3].Value & _ym2612.Opn.Pan[6]);
                rt += (int)(_outFm[3].Value & _ym2612.Opn.Pan[7]);
                lt += (int)(_outFm[4].Value & _ym2612.Opn.Pan[8]);
                rt += (int)(_outFm[4].Value & _ym2612.Opn.Pan[9]);
                lt += (int)(_outFm[5].Value & _ym2612.Opn.Pan[10]);
                rt += (int)(_outFm[5].Value & _ym2612.Opn.Pan[11]);

                buffer[bufferPos] = lt;
                bufferPos += 1;
                buffer[bufferPos] = rt;
                bufferPos += 1;
                _ym2612.Opn.Sl3.KeyCsm <<= 1;
                INTERNAL_TIMER_A();

                if ((_ym2612.Opn.Sl3.KeyCsm & 2) != 0)
                {
                    FM_KEYOFF_CSM(_ym2612.Ch[2], Slot1);
                    FM_KEYOFF_CSM(_ym2612.Ch[2], Slot2);
                    FM_KEYOFF_CSM(_ym2612.Ch[2], Slot3);
                    FM_KEYOFF_CSM(_ym2612.Ch[2], Slot4);
                    _ym2612.Opn.Sl3.KeyCsm = 0;
                }
            }

            INTERNAL_TIMER_B(length);
        }
    }
}