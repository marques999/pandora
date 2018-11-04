using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VgmPlayer.Ym2612
{
    internal class Ym2612Core
    {
        private const int EgAtt = 4;
        private const int EgDec = 3;
        private const int EgSus = 2;
        private const int EgRel = 1;
        private const int EgOff = 0;


        private static readonly byte[,] LfoPmOutput = new byte[7 * 8, 8]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
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
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 1, 1, 2, 2, 2, 3 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 1, 1, 2, 2, 2, 3 },
            { 0, 0, 2, 3, 4, 4, 5, 6 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 1 },
            { 0, 0, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 0, 1, 1, 1, 1, 2 },
            { 0, 0, 1, 1, 2, 2, 2, 3 },
            { 0, 0, 2, 3, 4, 4, 5, 6 },
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
            {  0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 0, 1, 1, 1, 2, 2 },
            { 0, 0, 1, 1, 2, 2, 3, 3 },
            { 0, 0, 1, 2, 2, 2, 3, 4 },
            { 0, 0, 2, 3, 4, 4, 5, 6 },
            { 0, 0, 4, 6, 8, 8, 0xA, 0xC },
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
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 2, 2, 2, 2 },
            { 0, 0, 0, 2, 2, 2, 4, 4 },
            { 0, 0, 2, 2, 4, 4, 6, 6 },
            { 0, 0, 2, 4, 4, 4, 6, 8 },
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
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 4, 4, 4, 4 },
            { 0, 0, 0, 4, 4, 4, 8, 8 },
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
            {
                0,
                0,
                0x20,
                0x30,
                0x40,
                0x40,
                0x50,
                0x60
            }
        };

        private const int Slot1 = 0;
        private const int Slot2 = 2;
        private const int Slot3 = 1;
        private const int Slot4 = 3;
        private const int EnvQuiet = 832;
        private const int TlTableLength = 6656;

        private class Ym2612Data
        {
            public readonly Ym2612Channel[] Ch =
            {
                new Ym2612Channel(),
                new Ym2612Channel(),
                new Ym2612Channel(),
                new Ym2612Channel(),
                new Ym2612Channel(),
                new Ym2612Channel()
            };
            public byte Dacen;
            public long Dacout;
            public readonly Ym2612Opn Opn = new Ym2612Opn();
        }

        private readonly int[] _tlTab = new int[TlTableLength];
        private readonly uint[] _sinTab = new uint[Ym2612Constants.SinTableLength];
        private readonly long[] _lfoPmTable = new long[128 * 8 * 32];

        private readonly Ym2612Data _ym2612 = new Ym2612Data();
        private readonly LongPointer _m2 = new LongPointer();
        private readonly LongPointer _c1 = new LongPointer();
        private readonly LongPointer _c2 = new LongPointer();
        private readonly LongPointer _mem = new LongPointer();
        private readonly LongPointer[] _outFm = new LongPointer[8];

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

            if (slot.Key == 0 && _ym2612.Opn.Sl3.KeyCsm == 0)
            {
                slot.Phase = 0;
                slot.SsgN = 0;

                if (slot.Ar + slot.ksr < 94)
                {
                    slot.State = (byte)(slot.Volume <= Ym2612Constants.MinimumAttenuation ? (slot.Sl == Ym2612Constants.MinimumAttenuation ? EgSus : EgDec) : EgAtt);
                }
                else
                {
                    slot.Volume = Ym2612Constants.MinimumAttenuation;
                    slot.State = (byte)(slot.Sl == Ym2612Constants.MinimumAttenuation ? EgSus : EgDec);
                }

                if ((slot.Ssg & 0x08) != 0 && (slot.SsgN ^ (slot.Ssg & 0x04)) != 0)
                {
                    slot.VolumeFinal = ((uint)(0x200 - slot.Volume) & Ym2612Constants.MaximumAttenuation) + slot.Tl;
                }
                else
                {
                    slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                }
            }

            slot.Key = 1;
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="slotIndex"></param>
        private void KeyOff(Ym2612Channel channel, int slotIndex)
        {
            var slot = channel.Slot[slotIndex];

            if (slot.Key > 0 && _ym2612.Opn.Sl3.KeyCsm == 0 && slot.State > EgRel)
            {
                slot.State = EgRel;

                if ((slot.Ssg & 0x08) > 0)
                {
                    if ((slot.SsgN ^ (slot.Ssg & 0x04)) != 0)
                    {
                        slot.Volume = 0x200 - slot.Volume;
                    }

                    if (slot.Volume >= 0x200)
                    {
                        slot.Volume = Ym2612Constants.MaximumAttenuation;
                        slot.State = EgOff;
                    }

                    slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                }
            }

            slot.Key = 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="slotIndex"></param>
        private void KeyOnCsm(Ym2612Channel channel, int slotIndex)
        {
            var slot = channel.Slot[slotIndex];

            if (slot.Key != 0 || _ym2612.Opn.Sl3.KeyCsm != 0)
            {
                return;
            }

            slot.Phase = 0;
            slot.SsgN = 0;

            if (slot.Ar + slot.ksr < 94)
            {
                slot.State = (byte)(slot.Volume <= Ym2612Constants.MinimumAttenuation ? (slot.Sl == Ym2612Constants.MinimumAttenuation ? EgSus : EgDec) : EgAtt);
            }
            else
            {
                slot.Volume = Ym2612Constants.MinimumAttenuation;
                slot.State = (byte)(slot.Sl == Ym2612Constants.MinimumAttenuation ? EgSus : EgDec);
            }

            if ((slot.Ssg & 0x08) == 0 || (slot.SsgN ^ (slot.Ssg & 0x04)) == 0)
            {
                slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
            }
            else
            {
                slot.VolumeFinal = ((uint)(0x200 - slot.Volume) & Ym2612Constants.MaximumAttenuation) + slot.Tl;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="slotIndex"></param>
        public void KeyOffCsm(Ym2612Channel channel, int slotIndex)
        {
            var slot = channel.Slot[slotIndex];

            if (slot.Key != 0 || slot.State <= EgRel)
            {
                return;
            }

            slot.State = EgRel;

            if ((slot.Ssg & 0x08) <= 0)
            {
                return;
            }

            if ((slot.SsgN ^ (slot.Ssg & 0x04)) > 0)
            {
                slot.Volume = 0x200 - slot.Volume;
            }

            if (slot.Volume >= 0x200)
            {
                slot.State = EgOff;
                slot.Volume = Ym2612Constants.MaximumAttenuation;
            }

            slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CsmKeyControl(Ym2612Channel channel)
        {
            KeyOnCsm(channel, Slot1);
            KeyOnCsm(channel, Slot2);
            KeyOnCsm(channel, Slot3);
            KeyOnCsm(channel, Slot4);
            _ym2612.Opn.Sl3.KeyCsm = 1;
        }

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalTimerA()
        {
            if ((_ym2612.Opn.St.Mode & 0x01) == 0x00)
            {
                return;
            }

            if ((_ym2612.Opn.St.Tac -= _ym2612.Opn.St.TimerBase) > 0)
            {
                return;
            }

            if ((_ym2612.Opn.St.Mode & 0x04) != 0x00)
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
                CsmKeyControl(_ym2612.Ch[2]);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="step"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalTimerB(int step)
        {
            if ((_ym2612.Opn.St.Mode & 0x02) == 0x00)
            {
                return;
            }

            if ((_ym2612.Opn.St.Tbc -= _ym2612.Opn.St.TimerBase * step) > 0)
            {
                return;
            }

            if ((_ym2612.Opn.St.Mode & 0x08) != 0x00)
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

        private void set_timers(int value)
        {
            if (((_ym2612.Opn.St.Mode ^ value) & 0xC0) != 0)
            {
                _ym2612.Ch[2].Slot[Slot1].Increment = -1;

                if ((value & 0xC0) != 0x80 && _ym2612.Opn.Sl3.KeyCsm != 0)
                {
                    KeyOffCsm(_ym2612.Ch[2], Slot1);
                    KeyOffCsm(_ym2612.Ch[2], Slot2);
                    KeyOffCsm(_ym2612.Ch[2], Slot3);
                    KeyOffCsm(_ym2612.Ch[2], Slot4);
                    _ym2612.Opn.Sl3.KeyCsm = 0;
                }
            }

            if ((value & 0x01) != 0 && (_ym2612.Opn.St.Mode & 0x01) == 0)
            {
                _ym2612.Opn.St.Tac = _ym2612.Opn.St.Tal;
            }

            if ((value & 2) != 0 && (_ym2612.Opn.St.Mode & 2) == 0)
            {
                _ym2612.Opn.St.Tbc = _ym2612.Opn.St.Tbl;
            }

            _ym2612.Opn.St.Status &= (byte)(~value >> 4);
            _ym2612.Opn.St.Mode = (uint)value;
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
        private void SetDetuneMultiply(Ym2612Channel channel, Ym2612Slot slot, int value)
        {
            if ((value & 0x0f) != 0)
                slot.Multiply = (uint)((value & 0x0f) * 2);
            else
                slot.Multiply = 1;
            slot.Detune = _ym2612.Opn.St.DetuneTable[(value >> 4) & 7];
            channel.Slot[Slot1].Increment = -1;
        }

        /// <summary>
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetTl(Ym2612Slot slot, int value)
        {
            slot.Tl = (uint)((value & 0x7f) << (Ym2612Constants.EnvelopeBits - 7));

            if ((slot.Ssg & 0x08) != 0 && (slot.SsgN ^ (slot.Ssg & 0x04)) != 0 && slot.State > EgRel)
            {
                slot.VolumeFinal = ((uint)(0x200 - slot.Volume) & Ym2612Constants.MaximumAttenuation) + slot.Tl;
            }
            else
            {
                slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="slot"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetArKsr(Ym2612Channel channel, Ym2612Slot slot, int value)
        {
            var previousKsr = slot.KSR;

            slot.Ar = (uint)((value & 0x1f) != 0 ? 32 + ((value & 0x1f) << 1) : 0);
            slot.KSR = (byte)(3 - (value >> 6));

            if (slot.KSR != previousKsr)
            {
                channel.Slot[Slot1].Increment = -1;
            }

            if (slot.Ar + slot.ksr < 32 + 62)
            {
                slot.eg_sh_ar = Ym2612Constants.EgRateShift[slot.Ar + slot.ksr];
                slot.eg_sel_ar = Ym2612Constants.EgRateSelect[slot.Ar + slot.ksr];
            }
            else
            {
                slot.eg_sh_ar = 0;
                slot.eg_sel_ar = 18 * Ym2612Constants.StepRate;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetDecayRate(Ym2612Slot slot, int value)
        {
            value &= 0x1F;

            if (value == 0)
            {
                slot.DecayRate = 0;
            }
            else
            {
                slot.DecayRate = (uint)(0x20 + (value << 1));
            }

            slot.eg_sh_d1r = Ym2612Constants.EgRateShift[slot.DecayRate + slot.ksr];
            slot.eg_sel_d1r = Ym2612Constants.EgRateSelect[slot.DecayRate + slot.ksr];
        }

        /// <summary>
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetSustainRate(Ym2612Slot slot, int value)
        {
            value &= 0x1F;

            if (value == 0)
            {
                slot.SustainRate = 0;
            }
            else
            {
                slot.SustainRate = (uint)(0x20 + (value << 1));
            }

            slot.eg_sh_d2r = Ym2612Constants.EgRateShift[slot.SustainRate + slot.ksr];
            slot.eg_sel_d2r = Ym2612Constants.EgRateSelect[slot.SustainRate + slot.ksr];
        }

        /// <summary>
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetReleaseRate(Ym2612Slot slot, int value)
        {
            slot.Sl = Ym2612Constants.SlTable[value >> 4];

            if (slot.State == EgDec && slot.Volume >= (int)slot.Sl)
            {
                slot.State = EgSus;
            }

            slot.ReleaseRate = (uint)(34 + ((value & 0x0F) << 2));
            slot.eg_sh_rr = Ym2612Constants.EgRateShift[slot.ReleaseRate + slot.ksr];
            slot.eg_sel_rr = Ym2612Constants.EgRateSelect[slot.ReleaseRate + slot.ksr];
        }

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdvanceLfo()
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
        private void AdvanceEgChannels()
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
                    if (slot.State == EgAtt)
                    {
                        if ((egCnt & ((1 << slot.eg_sh_ar) - 1)) == 0)
                        {
                            /* update attenuation level */
                            slot.Volume +=
                                (~slot.Volume *
                                 Ym2612Constants.EgIncrement[slot.eg_sel_ar + ((egCnt >> slot.eg_sh_ar) & 7)]) >> 4;

                            /* check phase transition*/
                            if (slot.Volume <= Ym2612Constants.MinimumAttenuation)
                            {
                                slot.Volume = Ym2612Constants.MinimumAttenuation;
                                slot.State =
                                    (byte)(slot.Sl == Ym2612Constants.MinimumAttenuation ? EgSus : EgDec); /* special case where SL=0 */
                            }

                            /* recalculate EG output */
                            if ((slot.Ssg & 0x08) != 0 && (slot.SsgN ^ (slot.Ssg & 0x04)) != 0
                            ) /* SSG-EG Output Inversion */
                                slot.VolumeFinal = ((uint)(0x200 - slot.Volume) & Ym2612Constants.MaximumAttenuation) + slot.Tl;
                            else
                                slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                        }
                    }
                    else if (slot.State == EgDec)
                    {
                        if ((egCnt & ((1 << slot.eg_sh_d1r) - 1)) == 0)
                        {
                            if ((slot.Ssg & 0x08) != 0)
                            {
                                if (slot.Volume < 0x200)
                                {
                                    slot.Volume += 4 * Ym2612Constants.EgIncrement[slot.eg_sel_d1r + ((egCnt >> slot.eg_sh_d1r) & 7)];

                                    if ((slot.SsgN ^ (slot.Ssg & 0x04)) != 0)
                                        slot.VolumeFinal = ((uint)(0x200 - slot.Volume) & Ym2612Constants.MaximumAttenuation) +
                                                       slot.Tl;
                                    else
                                        slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                                }
                            }
                            else
                            {
                                slot.Volume += Ym2612Constants.EgIncrement[slot.eg_sel_d1r + ((egCnt >> slot.eg_sh_d1r) & 7)];
                                slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                            }

                            if (slot.Volume >= (int)slot.Sl)
                            {
                                slot.State = EgSus;
                            }
                        }
                    }
                    else if (slot.State == EgSus)
                    {
                        if ((egCnt & ((1 << slot.eg_sh_d2r) - 1)) == 0)
                        {
                            if ((slot.Ssg & 0x08) != 0)
                            {
                                if (slot.Volume < 0x200)
                                {
                                    slot.Volume += 4 * Ym2612Constants.EgIncrement[slot.eg_sel_d2r + ((egCnt >> slot.eg_sh_d2r) & 7)];

                                    if ((slot.SsgN ^ (slot.Ssg & 0x04)) != 0)
                                        slot.VolumeFinal = ((uint)(0x200 - slot.Volume) & Ym2612Constants.MaximumAttenuation) + slot.Tl;
                                    else
                                        slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                                }
                            }
                            else
                            {
                                slot.Volume += Ym2612Constants.EgIncrement[slot.eg_sel_d2r + ((egCnt >> slot.eg_sh_d2r) & 7)];

                                if (slot.Volume >= Ym2612Constants.MaximumAttenuation)
                                {
                                    slot.Volume = Ym2612Constants.MaximumAttenuation;
                                }

                                slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                            }
                        }
                    }
                    else if (slot.State == EgRel)
                    {
                        if ((egCnt & ((1 << slot.eg_sh_rr) - 1)) == 0)
                        {
                            if ((slot.Ssg & 0x08) != 0)
                            {
                                if (slot.Volume < 0x200)
                                {
                                    slot.Volume += 4 * Ym2612Constants.EgIncrement[slot.eg_sel_rr + ((egCnt >> slot.eg_sh_rr) & 7)];
                                }

                                if (slot.Volume >= 0x200)
                                {
                                    slot.Volume = Ym2612Constants.MaximumAttenuation;
                                    slot.State = EgOff;
                                }
                            }
                            else
                            {
                                slot.Volume += Ym2612Constants.EgIncrement[slot.eg_sel_rr + ((egCnt >> slot.eg_sh_rr) & 7)];

                                if (slot.Volume >= Ym2612Constants.MaximumAttenuation)
                                {
                                    slot.Volume = Ym2612Constants.MaximumAttenuation;
                                    slot.State = EgOff;
                                }
                            }

                            /* recalculate EG output */
                            slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
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
            uint i = 4;
            var curSlot = Slot1;
            var slot = slots[curSlot];
            do
            {
                if ((slot.Ssg & 0x08) != 0 && slot.Volume >= 0x200 && slot.State > EgRel)
                {
                    if ((slot.Ssg & 0x01) != 0)
                    {
                        if ((slot.Ssg & 0x02) != 0)
                        {
                            slot.SsgN = 4;
                        }

                        if (slot.State != EgAtt && (slot.SsgN ^ (slot.Ssg & 0x04)) == 0)
                        {
                            slot.Volume = Ym2612Constants.MaximumAttenuation;
                        }
                    }
                    else
                    {
                        if ((slot.Ssg & 0x02) == 0)
                        {
                            slot.Phase = 0;
                        }
                        else
                        {
                            slot.SsgN ^= 4;
                        }

                        if (slot.State != EgAtt)
                        {
                            if (slot.Ar + slot.ksr < 94)
                            {
                                slot.State = (byte)(slot.Volume <= Ym2612Constants.MinimumAttenuation ? (slot.Sl == Ym2612Constants.MinimumAttenuation ? EgSus : EgDec) : EgAtt);
                            }
                            else
                            {
                                slot.Volume = Ym2612Constants.MinimumAttenuation;
                                slot.State = (byte)(slot.Sl == Ym2612Constants.MinimumAttenuation ? EgSus : EgDec);
                            }
                        }
                    }

                    if ((slot.SsgN ^ (slot.Ssg & 0x04)) != 0)
                        slot.VolumeFinal = ((uint)(0x200 - slot.Volume) & Ym2612Constants.MaximumAttenuation) + slot.Tl;
                    else
                        slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                }

                curSlot++;
                if (curSlot < slots.Count())
                    slot = slots[curSlot];
                i--;
            } while (i != 0);
        }

        private void update_phase_lfo_slot(Ym2612Slot slot, long pms, uint blockFnum)
        {
            var lfoFnTableIndexOffset = _lfoPmTable[(((blockFnum & 0x7f0) >> 4) << 8) + pms + _ym2612.Opn.LfoPm];

            if (lfoFnTableIndexOffset != 0)
            {
                blockFnum = (uint)(blockFnum * 2 + lfoFnTableIndexOffset);

                var blk = (byte)((blockFnum & 0x7000) >> 12);

                blockFnum = blockFnum & 0xFFF;

                var kc = (blk << 2) | Ym2612Constants.OpnFkTable[blockFnum >> 8];
                var fc = (int)((_ym2612.Opn.FnTable[blockFnum] >> (7 - blk)) + slot.Detune[kc]);

                if (fc < 0)
                {
                    fc += (int)_ym2612.Opn.FnMax;
                }

                slot.Phase += (uint)((fc * slot.Multiply) >> 1);
            }
            else
            {
                slot.Phase += (uint)slot.Increment;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void update_phase_lfo_channel(Ym2612Channel ch)
        {
            var blockFnum = ch.BlockFnum;
            var lfoTableIndexOffset = _lfoPmTable[(((blockFnum & 0x7f0) >> 4) << 8) + ch.Pms + _ym2612.Opn.LfoPm];

            if (lfoTableIndexOffset == 0)
            {
                ch.Slot[Slot1].Phase += (uint)ch.Slot[Slot1].Increment;
                ch.Slot[Slot2].Phase += (uint)ch.Slot[Slot2].Increment;
                ch.Slot[Slot3].Phase += (uint)ch.Slot[Slot3].Increment;
                ch.Slot[Slot4].Phase += (uint)ch.Slot[Slot4].Increment;
            }
            else
            {
                blockFnum = (uint)(blockFnum * 2 + lfoTableIndexOffset);

                var block = (byte)((blockFnum & 0x7000) >> 12);

                blockFnum = blockFnum & 0xFFF;

                var kc = (block << 2) | Ym2612Constants.OpnFkTable[blockFnum >> 8];
                var fc = (int)(_ym2612.Opn.FnTable[blockFnum] >> (7 - block));
                var finc = (int)(fc + ch.Slot[Slot1].Detune[kc]);

                if (finc < 0)
                {
                    finc += (int)_ym2612.Opn.FnMax;
                }

                ch.Slot[Slot1].Phase += (uint)((finc * ch.Slot[Slot1].Multiply) >> 1);
                finc = (int)(fc + ch.Slot[Slot2].Detune[kc]);

                if (finc < 0)
                {
                    finc += (int)_ym2612.Opn.FnMax;
                }

                ch.Slot[Slot2].Phase += (uint)((finc * ch.Slot[Slot2].Multiply) >> 1);
                finc = (int)(fc + ch.Slot[Slot3].Detune[kc]);

                if (finc < 0)
                {
                    finc += (int)_ym2612.Opn.FnMax;
                }

                ch.Slot[Slot3].Phase += (uint)((finc * ch.Slot[Slot3].Multiply) >> 1);
                finc = (int)(fc + ch.Slot[Slot4].Detune[kc]);

                if (finc < 0)
                {
                    finc += (int)_ym2612.Opn.FnMax;
                }

                ch.Slot[Slot4].Phase += (uint)((finc * ch.Slot[Slot4].Multiply) >> 1);
            }
        }

        private void refresh_fc_eg_slot(Ym2612Slot slot, int fc, int kc)
        {
            fc += (int)slot.Detune[kc];

            if (fc < 0)
            {
                fc += (int)_ym2612.Opn.FnMax;
            }

            slot.Increment = (fc * slot.Multiply) >> 1;
            kc = kc >> slot.KSR;

            if (slot.ksr == kc)
            {
                return;
            }

            slot.ksr = (byte)kc;

            if (slot.Ar + kc < 32 + 62)
            {
                slot.eg_sh_ar = Ym2612Constants.EgRateShift[slot.Ar + kc];
                slot.eg_sel_ar = Ym2612Constants.EgRateSelect[slot.Ar + kc];
            }
            else
            {
                slot.eg_sh_ar = 0;
                slot.eg_sel_ar = 18 * Ym2612Constants.StepRate;
            }

            slot.eg_sh_d1r = Ym2612Constants.EgRateShift[slot.DecayRate + kc];
            slot.eg_sel_d1r = Ym2612Constants.EgRateSelect[slot.DecayRate + kc];
            slot.eg_sh_d2r = Ym2612Constants.EgRateShift[slot.SustainRate + kc];
            slot.eg_sel_d2r = Ym2612Constants.EgRateSelect[slot.SustainRate + kc];
            slot.eg_sh_rr = Ym2612Constants.EgRateShift[slot.ReleaseRate + kc];
            slot.eg_sel_rr = Ym2612Constants.EgRateSelect[slot.ReleaseRate + kc];
        }

        private void refresh_fc_eg_chan(Ym2612Channel channel)
        {
            if (channel.Slot[Slot1].Increment != -1)
            {
                return;
            }

            var fc = (int)channel.Fc;
            var kc = (int)channel.KeyCode;

            refresh_fc_eg_slot(channel.Slot[Slot1], fc, kc);
            refresh_fc_eg_slot(channel.Slot[Slot2], fc, kc);
            refresh_fc_eg_slot(channel.Slot[Slot3], fc, kc);
            refresh_fc_eg_slot(channel.Slot[Slot4], fc, kc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint CalculateVolume(Ym2612Slot slot, uint am)
        {
            return slot.VolumeFinal + (am & slot.AMmask);
        }

        private int op_calc(uint phase, uint env, int pm)
        {
            var p = (env << 3) + _sinTab[((int)((phase & ~Ym2612Constants.FrequencyMask) + (pm << 15)) >> Ym2612Constants.FrequencyShift) & Ym2612Constants.SinMask];

            if (p >= TlTableLength)
            {
                return 0;
            }

            return _tlTab[p];
        }

        private int op_calc1(uint phase, uint env, int pm)
        {
            var p = (env << 3) + _sinTab[((int)((phase & ~Ym2612Constants.FrequencyMask) + pm) >> Ym2612Constants.FrequencyShift) & Ym2612Constants.SinMask];

            if (p >= TlTableLength)
            {
                return 0;
            }

            return _tlTab[p];
        }

        private void chan_calc(Ym2612Channel ch)
        {
            var am = _ym2612.Opn.LfoAm >> ch.Ams;
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

                    ch.Op1Output[1] = op_calc1(ch.Slot[Slot1].Phase, egOut, (int)(outVal << ch.Feedback));
                }
            }

            egOut = CalculateVolume(ch.Slot[Slot3], am);
            if (egOut < EnvQuiet) /* SLOT 3 */
                ch.Connect3.Value += op_calc(ch.Slot[Slot3].Phase, egOut, (int)_m2.Value);

            egOut = CalculateVolume(ch.Slot[Slot2], am);
            if (egOut < EnvQuiet) /* SLOT 2 */
                ch.Connect2.Value += op_calc(ch.Slot[Slot2].Phase, egOut, (int)_c1.Value);

            egOut = CalculateVolume(ch.Slot[Slot4], am);
            if (egOut < EnvQuiet) /* SLOT 4 */
                ch.Connect4.Value += op_calc(ch.Slot[Slot4].Phase, egOut, (int)_c2.Value);

            ch.MemoryValue = _mem.Value;

            if (ch.Pms != 0)
            {
                if ((_ym2612.Opn.St.Mode & 0xC0) > 0 && ch == _ym2612.Ch[2])
                {
                    update_phase_lfo_slot(ch.Slot[Slot1], ch.Pms, _ym2612.Opn.Sl3.BlockFnum[1]);
                    update_phase_lfo_slot(ch.Slot[Slot2], ch.Pms, _ym2612.Opn.Sl3.BlockFnum[2]);
                    update_phase_lfo_slot(ch.Slot[Slot3], ch.Pms, _ym2612.Opn.Sl3.BlockFnum[0]);
                    update_phase_lfo_slot(ch.Slot[Slot4], ch.Pms, ch.BlockFnum);
                }
                else
                {
                    update_phase_lfo_channel(ch);
                }
            }
            else
            {
                ch.Slot[Slot1].Phase += (uint)ch.Slot[Slot1].Increment;
                ch.Slot[Slot2].Phase += (uint)ch.Slot[Slot2].Increment;
                ch.Slot[Slot3].Phase += (uint)ch.Slot[Slot3].Increment;
                ch.Slot[Slot4].Phase += (uint)ch.Slot[Slot4].Increment;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OpnWriteMode(int r, int v)
        {
            byte c;

            switch (r)
            {
            case 0x21:

                break;

            case 0x22:
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

                    _ym2612.Opn.LfoTimerOverflow = Ym2612Constants.LfoSamplesPerStep[v & 7] << Ym2612Constants.LfoShift;
                }
                else
                {
                    _ym2612.Opn.LfoTimerOverflow = 0;
                }

                break;
            case 0x24: /* timer A High 8*/
                _ym2612.Opn.St.Ta = (_ym2612.Opn.St.Ta & 0x03) | ((long)v << 2);
                _ym2612.Opn.St.Tal = (1024 - _ym2612.Opn.St.Ta) << Ym2612Constants.TimerShift;
                break;
            case 0x25: /* timer A Low 2*/
                _ym2612.Opn.St.Ta = (_ym2612.Opn.St.Ta & 0x3fc) | ((long)v & 3);
                _ym2612.Opn.St.Tal = (1024 - _ym2612.Opn.St.Ta) << Ym2612Constants.TimerShift;
                break;
            case 0x26: /* timer B */
                _ym2612.Opn.St.Tb = v;
                _ym2612.Opn.St.Tbl = (256 - _ym2612.Opn.St.Tb) << (Ym2612Constants.TimerShift + 4);
                break;
            case 0x27: /* Mode, timer control */
                set_timers(v);
                break;
            case 0x28: /* Key on / off */
                c = (byte)(v & 0x03);
                if (c == 3)
                    break;

                if ((v & 0x04) != 0)
                    c += 3; /* CH 4-6 */
                var ch = _ym2612.Ch[c];

                if ((v & 0x10) != 0)
                    FM_KEYON(ch, Slot1);
                else
                    KeyOff(ch, Slot1);

                if ((v & 0x20) != 0)
                    FM_KEYON(ch, Slot2);
                else
                    KeyOff(ch, Slot2);

                if ((v & 0x40) != 0)
                    FM_KEYON(ch, Slot3);
                else
                    KeyOff(ch, Slot3);

                if ((v & 0x80) != 0)
                    FM_KEYON(ch, Slot4);
                else
                    KeyOff(ch, Slot4);
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

        private void OpnWriteReg(int address, int value)
        {
            var channelIndex = GetOpnChannel(address);

            if (channelIndex == 3)
            {
                return;
            }

            if (address >= 0x100)
            {
                channelIndex += 3;
            }

            var ch = _ym2612.Ch[channelIndex];
            var slot = ch.Slot[GetOpnSlot(address)];

            switch (address & 0xf0)
            {
            case 0x30:
                SetDetuneMultiply(ch, slot, value);
                break;

            case 0x40:
                SetTl(slot, value);
                break;

            case 0x50:
                SetArKsr(ch, slot, value);
                break;

            case 0x60:
                SetDecayRate(slot, value);
                slot.AMmask = (uint)((value & 0x80) != 0 ? ~0 : 0);
                break;

            case 0x70: /*     SR */
                SetSustainRate(slot, value);
                break;

            case 0x80: /* SL, RR */
                SetReleaseRate(slot, value);
                break;

            case 0x90: /* SSG-EG */
                slot.Ssg = (byte)(value & 0x0f);

                /* recalculate EG output */
                if (slot.State > EgRel)
                {
                    if ((slot.Ssg & 0x08) != 0 && (slot.SsgN ^ (slot.Ssg & 0x04)) != 0)
                        slot.VolumeFinal = ((uint)(0x200 - slot.Volume) & Ym2612Constants.MaximumAttenuation) + slot.Tl;
                    else
                        slot.VolumeFinal = (uint)slot.Volume + slot.Tl;
                }

                break;

            case 0xa0:

                switch (GetOpnSlot(address))
                {
                case 0:
                {
                    var fn = (uint)(((uint)(_ym2612.Opn.St.FnH & 7) << 8) + value);
                    var blk = (byte)(_ym2612.Opn.St.FnH >> 3);

                    ch.KeyCode = (byte)((blk << 2) | Ym2612Constants.OpnFkTable[fn >> 7]);
                    ch.Fc = _ym2612.Opn.FnTable[fn * 2] >> (7 - blk);
                    ch.BlockFnum = ((uint)blk << 11) | fn;
                    ch.Slot[Slot1].Increment = -1;
                    break;
                }
                case 1: /* 0xa4-0xa6 : FNUM2,BLK */
                    _ym2612.Opn.St.FnH = (byte)(value & 0x3f);
                    break;
                case 2: /* 0xa8-0xaa : 3CH FNUM1 */
                    if (address < 0x100)
                    {
                        var fn = (uint)(((uint)(_ym2612.Opn.Sl3.FnLatch & 7) << 8) + value);
                        var blk = (byte)(_ym2612.Opn.Sl3.FnLatch >> 3);
                        /* keyscale code */
                        _ym2612.Opn.Sl3.KeyCode[channelIndex] = (byte)((blk << 2) | Ym2612Constants.OpnFkTable[fn >> 7]);
                        /* phase increment counter */
                        _ym2612.Opn.Sl3.Fc[channelIndex] = _ym2612.Opn.FnTable[fn * 2] >> (7 - blk);
                        _ym2612.Opn.Sl3.BlockFnum[channelIndex] = ((uint)blk << 11) | fn;
                        _ym2612.Ch[2].Slot[Slot1].Increment = -1;
                    }
                    break;
                case 3: /* 0xac-0xae : 3CH FNUM2,BLK */
                    if (address < 0x100)
                        _ym2612.Opn.Sl3.FnLatch = (byte)(value & 0x3f);
                    break;
                }

                break;

            case 0xb0:
                switch (GetOpnSlot(address))
                {
                case 0: /* 0xb0-0xb2 : Feedback,Algorithm */
                {
                    var feedback = (value >> 3) & 7;
                    ch.Algorithm = (byte)(value & 7);
                    ch.Feedback = (byte)(feedback != 0 ? feedback + 6 : 0);
                    SetupConnection(ch, channelIndex);
                    break;
                }
                case 1:

                    ch.Pms = (value & 7) * 32;
                    ch.Ams = Ym2612Constants.LfoAmsDepthShift[(value >> 4) & 0x03];
                    _ym2612.Opn.Pan[channelIndex * 2] = (uint)((value & 0x80) != 0 ? ~0 : 0);
                    _ym2612.Opn.Pan[channelIndex * 2 + 1] = (uint)((value & 0x40) != 0 ? ~0 : 0);

                    break;
                }

                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitializeTimeTables(double freqbase)
        {
            for (var d = 0; d <= 3; d++)
            {
                for (var i = 0; i <= 31; i++)
                {
                    _ym2612.Opn.St.DetuneTable[d][i] = (int)(Ym2612Constants.DtTable[d * 32 + i] * freqbase * (1 << (Ym2612Constants.FrequencyShift - 10)));
                    _ym2612.Opn.St.DetuneTable[d + 4][i] = -_ym2612.Opn.St.DetuneTable[d][i];
                }
            }

            for (var i = 0; i < 4096; i++)
            {
                _ym2612.Opn.FnTable[i] = (uint)((double)i * 32 * freqbase * (1 << (Ym2612Constants.FrequencyShift - 10)));
            }

            _ym2612.Opn.FnMax = (uint)(0x20000 * freqbase * (1 << (Ym2612Constants.FrequencyShift - 10)));
        }

        private void OpnSetPres(int pres)
        {
            var freqbase = _ym2612.Opn.St.Clock / _ym2612.Opn.St.Rate / pres;

            _ym2612.Opn.EgTimerAdd = (uint)((1 << Ym2612Constants.EgShift) * freqbase);
            _ym2612.Opn.EgTimerOverflow = 3 * (1 << Ym2612Constants.EgShift);
            _ym2612.Opn.LfoTimerStep = (uint)((1 << Ym2612Constants.LfoShift) * freqbase);
            _ym2612.Opn.St.TimerBase = (int)((1 << Ym2612Constants.TimerShift) * freqbase);
            InitializeTimeTables(freqbase);
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
                    channels[channelIndex].Slot[slotIndex].Key = 0;
                    channels[channelIndex].Slot[slotIndex].SsgN = 0;
                    channels[channelIndex].Slot[slotIndex].Increment = -1;
                    channels[channelIndex].Slot[slotIndex].Phase = 0;
                    channels[channelIndex].Slot[slotIndex].State = EgOff;
                    channels[channelIndex].Slot[slotIndex].Volume = Ym2612Constants.MaximumAttenuation;
                    channels[channelIndex].Slot[slotIndex].VolumeFinal = Ym2612Constants.MaximumAttenuation;
                }
            }
        }

        /* initialize generic tables */
        private void init_tables()
        {
            int i, x;
            int n;
            double m;

            /* DAC precision */
            var mask = (uint)~((1 << (14 - DacBits)) - 1);

            /* build Linear Power Table */
            for (x = 0; x < Ym2612Constants.TlResLength; x++)
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
                    _tlTab[x * 2 + 0 + i * 2 * Ym2612Constants.TlResLength] = (int)((_tlTab[x * 2 + 0] >> i) & mask);
                    _tlTab[x * 2 + 1 + i * 2 * Ym2612Constants.TlResLength] = (int)(-_tlTab[x * 2 + 0 + i * 2 * Ym2612Constants.TlResLength] & mask);
                }
            }

            for (i = 0; i < Ym2612Constants.SinTableLength; i++)
            {
                m = Math.Sin((i * 2 + 1) * Math.PI / Ym2612Constants.SinTableLength);

                double o;

                if (m > 0.0)
                    o = 8 * Math.Log(1.0 / m) / Math.Log(2.0f);
                else
                    o = 8 * Math.Log(-1.0 / m) / Math.Log(2.0f);

                o = o / (Ym2612Constants.EnvelopeStepGx / 4);
                n = (int)(2.0 * o);

                if ((n & 1) != 0)
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

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Initialize(double clock, int rate)
        {
            init_tables();
            _ym2612.Opn.St.Clock = clock;
            _ym2612.Opn.St.Rate = (uint)rate;
            OpnSetPres(6 * 24);
        }

        /// <summary>
        /// </summary>
        public int DacBits { get; set; } = 14;

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
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
            _ym2612.Opn.St.Tbl = 256 << (Ym2612Constants.TimerShift + 4);
            _ym2612.Opn.St.Ta = 0;
            _ym2612.Opn.St.Tal = 1024 << Ym2612Constants.TimerShift;
            ResetChannels(_ym2612.Ch, 6);

            for (var i = 0xb6; i >= 0xb4; i--)
            {
                OpnWriteReg(i, 0xc0);
                OpnWriteReg(i | 0x100, 0xc0);
            }

            for (var i = 0xb2; i >= 0x30; i--)
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
            case 0:

                _ym2612.Opn.St.Address = (ushort)v;

                break;

            case 2:

                _ym2612.Opn.St.Address = (ushort)(v | 0x100);

                break;

            default:
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
                    default:

                        OpnWriteMode(addr, (int)v);

                        break;
                    }

                    break;
                default:

                    OpnWriteReg(addr, (int)v);

                    break;
                }

                break;
            }
            }
        }

        public void Update(int[] buffer, int length)
        {
            refresh_fc_eg_chan(_ym2612.Ch[0]);
            refresh_fc_eg_chan(_ym2612.Ch[1]);

            if ((_ym2612.Opn.St.Mode & 0xC0) == 0x00)
            {
                refresh_fc_eg_chan(_ym2612.Ch[2]);
            }
            else
            {
                if (_ym2612.Ch[2].Slot[Slot1].Increment == -1)
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

            var bufferPosition = 0;

            for (var i = 0; i < length; i++)
            {
                _outFm[0].Value = 0;
                _outFm[1].Value = 0;
                _outFm[2].Value = 0;
                _outFm[3].Value = 0;
                _outFm[4].Value = 0;
                _outFm[5].Value = 0;
                update_ssg_eg_channel(_ym2612.Ch[0].Slot);
                update_ssg_eg_channel(_ym2612.Ch[1].Slot);
                update_ssg_eg_channel(_ym2612.Ch[2].Slot);
                update_ssg_eg_channel(_ym2612.Ch[3].Slot);
                update_ssg_eg_channel(_ym2612.Ch[4].Slot);
                update_ssg_eg_channel(_ym2612.Ch[5].Slot);
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
                    _outFm[5].Value = _ym2612.Dacout;
                }

                AdvanceLfo();
                _ym2612.Opn.EgTimer += _ym2612.Opn.EgTimerAdd;

                while (_ym2612.Opn.EgTimer >= _ym2612.Opn.EgTimerOverflow)
                {
                    _ym2612.Opn.EgTimer -= _ym2612.Opn.EgTimerOverflow;
                    _ym2612.Opn.EgCounter++;
                    AdvanceEgChannels();
                }

                if (_outFm[0].Value > 8192)
                {
                    _outFm[0].Value = 8192;
                }
                else if (_outFm[0].Value < -8192)
                {
                    _outFm[0].Value = -8192;
                }

                if (_outFm[1].Value > 8192)
                {
                    _outFm[1].Value = 8192;
                }
                else if (_outFm[1].Value < -8192)
                {
                    _outFm[1].Value = -8192;
                }

                if (_outFm[2].Value > 8192)
                {
                    _outFm[2].Value = 8192;
                }
                else if (_outFm[2].Value < -8192)
                {
                    _outFm[2].Value = -8192;
                }

                if (_outFm[3].Value > 8192)
                {
                    _outFm[3].Value = 8192;
                }
                else if (_outFm[3].Value < -8192)
                {
                    _outFm[3].Value = -8192;
                }

                if (_outFm[4].Value > 8192)
                {
                    _outFm[4].Value = 8192;
                }
                else if (_outFm[4].Value < -8192)
                {
                    _outFm[4].Value = -8192;
                }

                if (_outFm[5].Value > 8192)
                {
                    _outFm[5].Value = 8192;
                }
                else if (_outFm[5].Value < -8192)
                {
                    _outFm[5].Value = -8192;
                }

                var mixerLeft = (int)(_outFm[0].Value & _ym2612.Opn.Pan[0]);
                var mixerRight = (int)(_outFm[0].Value & _ym2612.Opn.Pan[1]);

                mixerLeft += (int)(_outFm[1].Value & _ym2612.Opn.Pan[2]);
                mixerRight += (int)(_outFm[1].Value & _ym2612.Opn.Pan[3]);
                mixerLeft += (int)(_outFm[2].Value & _ym2612.Opn.Pan[4]);
                mixerRight += (int)(_outFm[2].Value & _ym2612.Opn.Pan[5]);
                mixerLeft += (int)(_outFm[3].Value & _ym2612.Opn.Pan[6]);
                mixerRight += (int)(_outFm[3].Value & _ym2612.Opn.Pan[7]);
                mixerLeft += (int)(_outFm[4].Value & _ym2612.Opn.Pan[8]);
                mixerRight += (int)(_outFm[4].Value & _ym2612.Opn.Pan[9]);
                mixerLeft += (int)(_outFm[5].Value & _ym2612.Opn.Pan[10]);
                mixerRight += (int)(_outFm[5].Value & _ym2612.Opn.Pan[11]);
                buffer[bufferPosition++] = mixerLeft;
                buffer[bufferPosition++] = mixerRight;
                _ym2612.Opn.Sl3.KeyCsm <<= 1;
                InternalTimerA();

                if ((_ym2612.Opn.Sl3.KeyCsm & 0x02) == 0x00)
                {
                    continue;
                }

                KeyOffCsm(_ym2612.Ch[2], Slot1);
                KeyOffCsm(_ym2612.Ch[2], Slot2);
                KeyOffCsm(_ym2612.Ch[2], Slot3);
                KeyOffCsm(_ym2612.Ch[2], Slot4);
                _ym2612.Opn.Sl3.KeyCsm = 0;
            }

            InternalTimerB(length);
        }
    }
}