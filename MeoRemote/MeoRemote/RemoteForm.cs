using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MeoRemote
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public partial class RemoteForm : Form
    {
        /// <summary>
        /// </summary>
        private readonly NetworkStream _socket;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="hostName"></param>
        public RemoteForm(string hostName)
        {
            try
            {
                InitializeComponent();
                _socket = new TcpClient(hostName, 8082).GetStream();

                if (Receive() == "hello")
                {
                    return;
                }

                MessageBox.Show("Response Error!");
                Close();
            }
            catch (SocketException)
            {
                MessageBox.Show("Connection Error!");
                Close();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool Send(string request)
        {
            try
            {
                var bytes = Encoding.ASCII.GetBytes(request);

                _socket.Write(bytes, 0, bytes.Length);

                if (Receive() != "ok")
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private string Receive()
        {
            var responseBuffer = new byte[64];

            try
            {
                return Encoding.ASCII.GetString(responseBuffer, 0, _socket.Read(responseBuffer, 0, responseBuffer.Length)).Trim();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4_Click(object sender, EventArgs e)
        {
            if (Send("key=8\n") == false)
            {
                MessageBox.Show("Send Error!");
            }
        }
    }
}