using System;
using System.Drawing;
using System.Windows.Forms;

using NAudio.Wave;

namespace VgmPlayer
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class VgmPlayer : Form
    {
        /// <summary>
        /// </summary>
        private const string ApplicationTitle = "VgmPlayer";

        /// <summary>
        /// </summary>
        private readonly WaveOut _waveout = new WaveOut();

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public VgmPlayer()
        {
            SuspendLayout();
            Opacity = 0.75;
            MaximizeBox = false;
            Name = ApplicationTitle;
            Text = ApplicationTitle;
            ClientSize = new Size(640, 0);
            FormClosing += VgmForm_FormClosing;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            ResumeLayout(false);
            _waveout.Init(new VgmFile("29.vgm", 24000));
            _waveout.Play();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void VgmForm_FormClosing(object sender, FormClosingEventArgs arguments)
        {
            _waveout.Stop();
            _waveout.Dispose();
        }

        /// <summary>
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VgmPlayer());
        }
    }
}