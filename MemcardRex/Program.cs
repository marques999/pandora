using System;
using System.Text;
using System.Windows.Forms;

using MemcardRex.GUI;

namespace MemcardRex
{
    /// <summary>
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}