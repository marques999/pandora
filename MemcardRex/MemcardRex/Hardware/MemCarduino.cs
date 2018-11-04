using System;
using System.Text;
using System.Threading;

namespace MemcardRex.Hardware
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class MemCarduino : CardReader
    {
        /// <summary>
        /// </summary>
        private const string Signature = "MCDINO";

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="comPort"></param>
        public MemCarduino(string comPort) : base(comPort)
        {
            SerialPort.DtrEnable = true;
            SerialPort.DtrEnable = false;
            Thread.Sleep(2000);
            Write((byte)MemCarduinoCommands.GetIdentifier, 100);

            if (Encoding.ASCII.GetString(Read()).Substring(0, Signature.Length) != Signature)
            {
                throw new Exception($"MemCARDuino not detected on {comPort}.");
            }

            Write((byte)MemCarduinoCommands.GetVersion, 30);

            var versionBytes = Read();

            FirmwareVersion = $"{versionBytes[0] >> 4}.{versionBytes[0] & 0x0F}";
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <returns></returns>
        public override byte[] ReadMemoryCardFrame(ushort frameNumber)
        {
            var delayCounter = 0;

            Write((byte)MemCarduinoCommands.Mcr, 0);
            Write((byte)(frameNumber >> 8), 0);
            Write((byte)(frameNumber & 0xFF), 0);

            while (SerialPort.BytesToRead < 130 && delayCounter < 18)
            {
                Thread.Sleep(5);
                delayCounter++;
            }

            var receivedData = Read();
            var responseDataBuffer = new byte[128];
            var xorData = (byte)((byte)(frameNumber >> 8) ^ (byte)(frameNumber & 0xFF));

            Array.Copy(receivedData, 0, responseDataBuffer, 0, 128);

            for (var offset = 0; offset < 128; offset++)
            {
                xorData ^= responseDataBuffer[offset];
            }

            if (xorData != receivedData[0x80] || receivedData[0x81] != (byte)MemCarduinoResponses.Good)
            {
                return null;
            }

            return responseDataBuffer;
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
            var xorData = (byte)((byte)(frameNumber >> 8) ^ (byte)(frameNumber & 0xFF));

            for (var offset = 0; offset < 128; offset++)
            {
                xorData ^= frameData[offset];
            }

            SerialPort.DiscardInBuffer();
            Write((byte)MemCarduinoCommands.Mcw, 0);
            Write((byte)(frameNumber >> 8), 0);
            Write((byte)(frameNumber & 0xFF), 0);
            SerialPort.Write(frameData, 0x00, 128);
            Write(xorData, 0);

            while (SerialPort.BytesToRead < 1 && delayCounter < 18)
            {
                Thread.Sleep(5);
                delayCounter++;
            }

            return Read()[0] == (byte)MemCarduinoResponses.Good;
        }
    }
}