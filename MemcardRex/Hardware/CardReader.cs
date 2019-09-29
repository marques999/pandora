using System.IO.Ports;
using System.Threading;

namespace MemcardRex.Hardware
{
    /// <summary>
    /// </summary>
    internal abstract class CardReader
    {
        /// <summary>
        /// </summary>
        protected SerialPort SerialPort;

        /// <summary>
        /// </summary>
        public string FirmwareVersion { get; protected set; } = "0.0";

        /// <summary>
        /// </summary>
        public void Stop()
        {
            if (SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay"></param>
        protected void Write(byte command, int delay)
        {
            SerialPort.DiscardInBuffer();
            SerialPort.Write(new[] { command }, 0, 1);

            if (delay > 0)
            {
                Thread.Sleep(delay);
            }
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected byte[] Read()
        {
            var buffer = new byte[256];

            if (SerialPort.BytesToRead != 0)
            {
                SerialPort.Read(buffer, 0, 256);
            }

            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <returns></returns>
        public abstract byte[] ReadMemoryCardFrame(ushort frameNumber);

        /// <summary>
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <param name="frameData"></param>
        /// <returns></returns>
        public abstract bool WriteMemoryCardFrame(ushort frameNumber, byte[] frameData);

        /// <summary>
        /// </summary>
        /// <param name="comPort"></param>
        /// <returns></returns>
        protected CardReader(string comPort)
        {
            SerialPort = new SerialPort(comPort, 38400, Parity.None, 8, StopBits.One)
            {
                ReadBufferSize = 256
            };

            SerialPort.Open();
        }
    }
}