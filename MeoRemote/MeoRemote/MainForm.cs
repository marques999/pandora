using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeoRemote
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// </summary>
        private const int BaseHost = 64;

        /// <summary>
        /// </summary>
        private const int NumberHosts = 16;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            listBox1.Enabled = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScanForm_Load(object sender, EventArgs e)
        {
            progressBar1.Maximum = NumberHosts;
        }

        /// <summary>
        /// </summary>
        /// <param name="host"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Ping(string host, int timeout)
        {
            try
            {
                var pingReply = new Ping().Send(host, timeout);

                if (pingReply != null && pingReply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ScanForm_Shown(object sender, EventArgs e)
        {
            for (var index = 0; index < NumberHosts; index++)
            {
                var hostIp = $"192.168.1.{BaseHost + index}";

                if (await Task.Run(() => Ping(hostIp, 50)))
                {
                    try
                    {
                        label1.Text = "Identifying device... (" + hostIp + ")";
                        new TcpClient(hostIp, 8082);
                        listBox1.Items.Add(hostIp);
                    }
                    catch
                    {
                    }
                }

                progressBar1.Value++;
                Text = $"Scanning MEOBox(s)... [{index}/{NumberHosts}]";
                label1.Text = hostIp;
            }

            listBox1.Enabled = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void ListBox1_DoubleClick(object sender, EventArgs arguments)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }

            var remoteForm = new RemoteForm(listBox1.SelectedItem as string);

            if (remoteForm.IsDisposed == false)
            {
                remoteForm.ShowDialog();
            }
        }

        /// <summary>
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}