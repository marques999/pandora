using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SexyTerminal
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed partial class ColorButton2 : UserControl
    {
        /// <summary>
        /// </summary>
        private static readonly Color Foreground = Color.FromArgb(127, 255, 255, 255);

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public ColorButton2() : this("N/A", Color.White)
        {
        }

        public ColorButton2(string tag) : this(tag, Color.White)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="color"></param>
        public ColorButton2(string tag, Color color)
        {
            InitializeComponent();
            label1.Text = tag;
            UpdateColor(color);
            WireAllControls(this);
        }

        private void WireAllControls(Control cont)
        {
            foreach (Control ctl in cont.Controls)
            {
                ctl.Click += ctl_Click;
                if (ctl.HasChildren)
                {
                    WireAllControls(ctl);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ctl_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, EventArgs.Empty);
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static string GenerateHexString(Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        /// <summary>
        /// </summary>
        /// <param name="schemeBackground"></param>
        public void UpdateColor(Color schemeBackground)
        {
            label2.Text = GenerateHexString(schemeBackground);
            ForeColor = Foreground;
            BackColor = schemeBackground;
            label2.BackColor = Color.FromArgb(48, 0, 0, 0);
        }
    }
}