using System.IO;
using System.Text;

using SmoothDownloader.Smooth;

namespace SmoothDownloader.Mkv
{
    /// <summary>
    /// </summary>
    public class MuxStateWriter
    {
        /// <summary>
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        public MuxStateWriter(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// </summary>
        public void Close()
        {
            _stream.Close();
        }

        /// <summary>
        /// </summary>
        public void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteUlong(char key, ulong value)
        {
            Write(Encoding.ASCII.GetBytes($"{key}{value}\n"));
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="buffer"></param>
        public void WriteBytes(char key, byte[] buffer)
        {
            Write(Encoding.ASCII.GetBytes($"{key}:{Utils.HexEncodeString(buffer)}\n"));
        }

        /// <summary>
        /// </summary>
        /// <param name="output"></param>
        private void Write(byte[] output)
        {
            _stream.Write(output, 0, output.Length);
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public void WriteRaw(byte[] bytes, int start, int finish)
        {
            _stream.Write(bytes, start, finish - start);
        }
    }
}