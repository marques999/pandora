using System;
using System.IO;

using NAudio.Wave;

namespace VgmPlayer
{
    internal class VgmFile : IWaveProvider
    {
        private int[] _intBuffer;
        private readonly Ym2612.Ym2612 _ym2612 = new Ym2612.Ym2612();
        private readonly Sn76489.Sn76489 _sn76489 = new Sn76489.Sn76489();

        private bool _iSaidStop;
        private readonly bool _looped = true;

        public bool GameFroze
        {
            get;
            set;
        }

        public VgmFile(string fileName, int sampleRate)
        {
            WaveFormat = new WaveFormat(sampleRate, 16, 2);
            OpenVgmFile(fileName);
            _ym2612.Initialize((int)_vgmHeader.ClockYm2612, sampleRate);
            _sn76489.Initialize(_vgmHeader.Sn76489Clock);
        }

        public void Stop()
        {
            _vgmReader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        private const uint FccVgm = 0x206D6756;
        private VgmHeader _vgmHeader;
        private BinaryReader _vgmReader;
        private byte[] _dacData;
        private byte[] _vgmData;
        private int _dacOffset;
        private int _dataOffset;
        private byte _lastCommand;

        bool OpenVgmFile(string fileName)
        {
            var vgmFile = File.Open(fileName, FileMode.Open);
            var fileSize = (uint)vgmFile.Length;

            _vgmReader = new BinaryReader(vgmFile);

            if (_vgmReader.ReadUInt32() != FccVgm)
            {
                return false;
            }

            _vgmHeader = new VgmHeader(_vgmReader);
            _vgmReader.BaseStream.Seek(0, SeekOrigin.Begin);

            var dataOffset = _vgmHeader.DataOffset;

            if (dataOffset == 0x00 || dataOffset == 0x0C)
            {
                dataOffset = 0x40;
            }

            _dataOffset = dataOffset;
            _vgmReader.ReadBytes(dataOffset);
            _vgmData = _vgmReader.ReadBytes((int)(fileSize - dataOffset));
            _vgmReader = new BinaryReader(new MemoryStream(_vgmData));

            if ((byte)_vgmReader.PeekChar() == 0x67)
            {
                _vgmReader.ReadByte();

                if ((byte)_vgmReader.PeekChar() == 0x66)
                {
                    _vgmReader.ReadByte();
                    _vgmReader.ReadByte();
                    _dacData = _vgmReader.ReadBytes((int)_vgmReader.ReadUInt32());
                }
            }

            vgmFile.Close();

            return true;
        }

        private int _wait;
        private int _waitInc;

        public int Read(byte[] buffer, int offset, int count)
        {
            if (_iSaidStop)
            {
                return 0;
            }

            if (_lastCommand == 0x66 && _looped == false)
            {
                _lastCommand = 0;
                _iSaidStop = true;
                Stop();
                return 0;
            }

            var songEnded = false;
            var samplesWritten = 0;
            var bufferData = new int[2];
            var samplesToWrite = count / 2;

            _intBuffer = new int[count];

            while (samplesWritten != samplesToWrite)
            {
                bool writeSample;

                if (_wait != 0 || GameFroze)
                {
                    writeSample = true;

                    if (_wait > 0)
                    {
                        _waitInc += 1;

                        while (_wait > 0 && _waitInc >= 1)
                        {
                            _waitInc -= 1;
                            _wait--;
                        }
                    }
                }
                else
                {
                    var command = _vgmReader.ReadByte();

                    writeSample = false;
                    _lastCommand = command;
                    NewMethod(ref writeSample, ref songEnded, command);

                    if (command >= 0x70 && command <= 0x7F)
                    {
                        _wait = (command & 15) + 1;

                        if (_wait != 0)
                        {
                            writeSample = true;
                        }
                    }
                    else if (command >= 0x80 && command <= 0x8F)
                    {
                        _wait = command & 0x0F;
                        _ym2612.WritePort0(0x2A, _dacData[_dacOffset]);
                        _dacOffset++;

                        if (_wait != 0)
                        {
                            writeSample = true;
                        }
                    }

                    if (_wait != 0)
                    {
                        _wait -= 1;
                    }
                }

                if (songEnded)
                {
                    break;
                }

                if (writeSample)
                {
                    _ym2612.Update(bufferData, 1);

                    var aLeft = (short)bufferData[0];
                    var aRight = (short)bufferData[1];

                    _sn76489.Update(bufferData, 1);

                    var bLeft = (short)bufferData[0];
                    var bRight = (short)bufferData[1];

                    _intBuffer[samplesWritten * 2] = Math.Min(Math.Max((aLeft + bLeft) * 2, short.MinValue), short.MaxValue);
                    _intBuffer[samplesWritten * 2 + 1] = Math.Min(Math.Max((aRight + bRight) * 2, short.MinValue), short.MaxValue);
                    samplesWritten += 1;

                    if (samplesWritten == samplesToWrite)
                    {
                        break;
                    }
                }
            }

            for (var sampleIndex = 0; sampleIndex < samplesToWrite; sampleIndex++)
            {
                var sValue = (short)_intBuffer[sampleIndex];
                buffer[sampleIndex * 2] = (byte)(sValue & 0xFF);
                buffer[sampleIndex * 2 + 1] = (byte)(sValue >> 8);
            }

            samplesWritten *= 2;

            if (samplesWritten / 4.0f - (int)(samplesWritten / 4.0f) > 0)
            {
                samplesWritten -= 2;
            }

            return samplesWritten;
        }

        private void NewMethod(ref bool writeSample, ref bool songEnded, byte command)
        {
            switch (command)
            {
            case 0x4F:
                _sn76489.Write(_vgmReader.ReadByte());
                break;
            case 0x50:
                _sn76489.Write(_vgmReader.ReadByte());
                break;
            case 0x52:
                _ym2612.WritePort0(_vgmReader.ReadByte(), _vgmReader.ReadByte());
                break;

            case 0x53:

                _ym2612.WritePort1(_vgmReader.ReadByte(), _vgmReader.ReadByte());

                break;

            case 0x61:
            {
                _wait = _vgmReader.ReadUInt16();

                if (_wait != 0)
                {
                    writeSample = true;
                }
            }
            break;

            case 0x62:

                _wait = 735;
                writeSample = true;

                break;

            case 0x63:

                _wait = 882;
                writeSample = true;

                break;

            case 0xE0:

                _dacOffset = (int)_vgmReader.ReadUInt32();

                break;

            case 0x67:

                _vgmReader.ReadByte();
                _vgmReader.ReadByte();
                _vgmReader.BaseStream.Position += _vgmReader.ReadUInt32();

                break;

            case 0x66:

                if (_looped == false)
                {
                    _vgmReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    songEnded = true;
                }
                else if (_vgmHeader.LoopOffset != 0 && _dataOffset < _vgmHeader.LoopOffset)
                {
                    _vgmReader.BaseStream.Seek(_vgmHeader.LoopOffset - _dataOffset, SeekOrigin.Begin);
                }
                else
                {
                    _vgmReader.BaseStream.Seek(0, SeekOrigin.Begin);
                }

                break;
            }
        }

        public WaveFormat WaveFormat { get; }
    }
}