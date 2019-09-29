using System.Windows.Forms;

namespace MemcardRex
{
    /// <summary>
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        public static DialogResult Warning(string caption, string message)
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static DialogResult Prompt(string caption, string message)
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        }
    }
}