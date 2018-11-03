using System.Linq;
using System.Runtime.CompilerServices;

namespace VgmPlayer.Sn76489
{
    /// <summary>
    /// </summary>
    public sealed class Sn76489Core
    {
        /// <summary>
        /// </summary>
        private uint[] _volume = new uint[4];

        /// <summary>
        /// </summary>
        private float[] _output = new float[4];

        /// <summary>
        /// </summary>
        private readonly int[] _divider = new int[4];

        /// <summary>
        /// </summary>
        private readonly int[] _counter = new int[4];

        /// <summary>
        /// </summary>
        private uint _noiseLfsr;

        /// <summary>
        /// </summary>
        private uint _noiseTap;

        /// <summary>
        /// </summary>
        private uint _latchedChan;

        /// <summary>
        /// </summary>
        private bool _latchedVolume;

        /// <summary>
        /// </summary>
        private float _ticksPerSample;

        /// <summary>
        /// </summary>
        private float _ticksCount;

        /// <summary>
        /// </summary>
        public Sn76489Core()
        {
            SetClock(3500000);
            Reset();
        }

        /// <summary>
        /// </summary>
        public void SetClock(float ticks)
        {
            _ticksPerSample = ticks / 16 / 44100;
        }

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _volume = new[]
            {
                15u, 15u, 15u, 15u
            };

            _output = new[]
            {
                0.0f, 0.0f, 0.0f, 0.0f
            };

            _latchedChan = 0;
            _noiseLfsr = 0x8000;
            _latchedVolume = false;
            _ticksCount = _ticksPerSample;
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public uint GetDivider(uint channel)
        {
            return (uint)(channel < _divider.Length ? _divider[channel] : 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="divider"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDivider(uint channel, uint divider)
        {
            if (channel < _divider.Length)
            {
                _divider[channel] = (int)divider;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetVolume(uint channel)
        {
            return channel < _volume.Length ? _volume[channel] : 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="volume"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVolume(uint channel, uint volume)
        {
            if (channel < _volume.Length)
            {
                _volume[channel] = volume;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int val)
        {
            int chan;
            int divider;

            if ((val & 128) != 0)
            {
                chan = (val >> 5) & 3;
                divider = (int)((GetDivider((uint)chan) & 0xfff0) | ((uint)val & 15));
                _latchedChan = (uint)chan;
                _latchedVolume = (val & 16) != 0;
            }
            else
            {
                chan = (int)_latchedChan;
                divider = (int)((GetDivider((uint)chan) & 15) | (((uint)val & 63) << 4));
            }

            if (_latchedVolume)
            {
                SetVolume((uint)chan, (GetVolume((uint)chan) & 16) | ((uint)val & 15));
            }
            else
            {
                SetDivider((uint)chan, (uint)divider);

                if (chan == 3)
                {
                    _noiseTap = (uint)(((divider >> 2) & 1) != 0 ? 9 : 1);
                    _noiseLfsr = 0x8000;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Render()
        {
            while (_ticksCount > 0)
            {
                _counter[0] -= 1;

                if (_counter[0] < 0)
                {
                    if (_divider[0] > 1)
                    {
                        _volume[0] ^= 16;
                        _output[0] = Sn76489Constants.VolumeTable[_volume[0]];
                    }

                    _counter[0] = _divider[0];
                }

                _counter[1] -= 1;

                if (_counter[1] < 0)
                {
                    if (_divider[1] > 1)
                    {
                        _volume[1] ^= 16;
                        _output[1] = Sn76489Constants.VolumeTable[_volume[1]];
                    }

                    _counter[1] = _divider[1];
                }

                _counter[2] -= 1;

                if (_counter[2] < 0)
                {
                    if (_divider[2] > 1)
                    {
                        _volume[2] ^= 16;
                        _output[2] = Sn76489Constants.VolumeTable[_volume[2]];
                    }

                    _counter[2] = _divider[2];
                }

                _counter[3] -= 1;

                if (_counter[3] < 0)
                {
                    var divider = (uint)(_divider[3] & 3);

                    if (divider < 3)
                    {
                        _counter[3] = 0x10 << (int)divider;
                    }
                    else
                    {
                        _counter[3] = _divider[3] << 1;
                    }

                    uint tap;

                    if (_noiseTap == 9)
                    {
                        tap = _noiseLfsr & _noiseTap;
                        tap ^= tap >> 8;
                        tap ^= tap >> 4;
                        tap ^= tap >> 2;
                        tap ^= tap >> 1;
                        tap &= 1;
                    }
                    else
                    {
                        tap = _noiseLfsr & 0x01;
                    }

                    _noiseLfsr = (_noiseLfsr >> 1) | (tap << 15);
                    _volume[3] = (_volume[3] & 15) | ((_noiseLfsr & 1 ^ 1) << 4);
                    _output[3] = Sn76489Constants.VolumeTable[_volume[3]];
                }

                _ticksCount -= 1;
            }

            _ticksCount += _ticksPerSample;

            return _output.Sum();
        }
    }
}