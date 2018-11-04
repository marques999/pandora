using System;
using System.Threading;

namespace MemcardRex.Hardware
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class DexDrive : CardReader
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="comPort"></param>
        public DexDrive(string comPort) : base(comPort)
        {
            SerialPort.RtsEnable = false;
            Thread.Sleep(300);
            SerialPort.RtsEnable = true;
            Thread.Sleep(300);
            SerialPort.DtrEnable = true;

            for (var index = 0; index < 5; index++)
            {
                SerialPort.DiscardInBuffer();
                SerialPort.Write("XXXXX");
                Thread.Sleep(20);
            }

            var readData = Read();

            if (readData[0] != 0x49 || readData[1] != 0x41 || readData[2] != 0x49)
            {
                throw new Exception("DexDrive not detected on '" + comPort + "' port.");
            }

            var contents = new byte[]
            {
                0x10, 0xAA, 0xBB, 0xCC,
                0xDD, 0xEE, 0xFF, 0xAA,
                0xBB, 0xCC, 0xDD, 0xEE,
                0xFF, 0xAA, 0xBB, 0xCC, 0xDD
            };

            Write((byte)DexDriveCommands.Init, contents, 50);
            readData = Read();

            if (readData[5] != 0x50 || readData[6] != 0x53 || readData[7] != 0x58)
            {
                throw new Exception("Detected device is not a PS1 DexDrive.");
            }

            FirmwareVersion = (readData[8] >> 6) + "." + ((readData[8] >> 2) & 0xF) + (readData[8] & 0x3);

            for (var index = 0; index < 10; index++)
            {
                Write((byte)DexDriveCommands.MagicHandshake, null, 0);
            }

            Thread.Sleep(50);
            Write((byte)DexDriveCommands.Light, new byte[] { 1 }, 50);
        }

        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="contents"></param>
        /// <param name="delay"></param>
        private void Write(byte command, byte[] contents, int delay)
        {
            SerialPort.DiscardInBuffer();
            SerialPort.Write("IAI" + (char)command);

            if (contents != null)
            {
                SerialPort.Write(contents, 0, contents.Length);
            }

            if (delay > 0)
            {
                Thread.Sleep(delay);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <returns></returns>
        public override byte[] ReadMemoryCardFrame(ushort frameNumber)
        {
            var delayCounter = 0;

            Write((byte)DexDriveCommands.Read, new[]
            {
                (byte)(frameNumber & 0xFF),
                (byte)(frameNumber >> 8)
            }, 0);

            while (SerialPort.BytesToRead < 133 && delayCounter < 16)
            {
                Thread.Sleep(5);
                delayCounter++;
            }

            var receivedData = Read();
            var responseBuffer = new byte[128];
            var xorData = (byte)((byte)(frameNumber & 0xFF) ^ (byte)(frameNumber >> 8));

            Array.Copy(receivedData, 4, responseBuffer, 0, 128);

            for (var offset = 0; offset < 128; offset++)
            {
                xorData ^= responseBuffer[offset];
            }

            return xorData != receivedData[132] ? null : responseBuffer;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <param name="frameData"></param>
        /// <returns></returns>
        public override bool WriteMemoryCardFrame(ushort frameNumber, byte[] frameData)
        {
            var delayCounter = 0;
            var frameLsb = (byte)(frameNumber & 0xFF);
            var frameMsb = (byte)(frameNumber >> 8);
            var revFrameLsb = ReverseByte(frameLsb);
            var revFrameMsb = ReverseByte(frameMsb);
            var xorData = (byte)(frameMsb ^ frameLsb ^ revFrameMsb ^ revFrameLsb);

            for (var offset = 0; offset < 128; offset++)
            {
                xorData ^= frameData[offset];
            }

            Write((byte)DexDriveCommands.Write, new[]
            {
                frameMsb,
                frameLsb,
                revFrameMsb,
                revFrameLsb
            }, 0);

            SerialPort.Write(frameData, 0x00, frameData.Length);
            SerialPort.Write(new[] { xorData }, 0x00, 1);

            while (SerialPort.BytesToRead < 4 && delayCounter < 20)
            {
                Thread.Sleep(5);
                delayCounter++;
            }

            var readData = Read();

            return readData[3] == (byte)DexDriveResponses.WriteOk || readData[3] == (byte)DexDriveResponses.WriteSame;
        }

        /// <summary>
        /// </summary>
        /// <param name="inputByte"></param>
        /// <returns></returns>
        private static byte ReverseByte(byte inputByte)
        {
            var i = 0;
            var j = 7;
            var result = new byte();

            while (i < 8)
            {
                if ((inputByte & (1 << i)) > 0)
                {
                    result |= (byte)(1 << j);
                }

                i++;
                j--;
            }

            return result;
        }
    }
}