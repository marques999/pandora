using System;
using System.Text;
using System.Threading;

namespace MemcardRex.Hardware
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class Ps1CardLink : CardReader
    {
        /// <summary>
        /// </summary>
        private const string Signature = "PS1CLNK";

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="comPort"></param>
        public Ps1CardLink(string comPort) : base(comPort)
        {
            Write((byte)CardLinkCommands.GetIdentifier, 0x64);

            if (Encoding.ASCII.GetString(Read()).Substring(0, Signature.Length) != Signature)
            {
                throw new Exception($"CardLink not detected on {comPort}.");
            }

            Write((byte)CardLinkCommands.GetVersion, 0x1E);

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
            var xorData = (byte)((byte)(frameNumber >> 8) ^ (byte)(frameNumber & 0xFF));

            Write((byte)CardLinkCommands.Mcr, 0);
            Write((byte)(frameNumber >> 8), 0);
            Write((byte)(frameNumber & 0xFF), 0);

            while (SerialPort.BytesToRead < 0x82 && delayCounter < 18)
            {
                Thread.Sleep(5);
                delayCounter++;
            }

            var receivedData = Read();
            var responseDataBuffer = new byte[128];

            Array.Copy(receivedData, 0, responseDataBuffer, 0, 128);

            for (var offset = 0; offset < 128; offset++)
            {
                xorData ^= responseDataBuffer[offset];
            }

            if (receivedData[0x80] == xorData && receivedData[0x81] == (byte)CardLinkResponses.Good)
            {
                return responseDataBuffer;
            }

            return null;
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
            Write((byte)CardLinkCommands.Mcw, 0);
            Write((byte)(frameNumber >> 8), 0);
            Write((byte)(frameNumber & 0xFF), 0);
            SerialPort.Write(frameData, 0x00, 0x80);
            Write(xorData, 0);

            while (SerialPort.BytesToRead < 1 && delayCounter < 18)
            {
                Thread.Sleep(5);
                delayCounter++;
            }

            return Read()[0] == (byte)CardLinkResponses.Good;
        }
    }
}