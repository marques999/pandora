using System;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}