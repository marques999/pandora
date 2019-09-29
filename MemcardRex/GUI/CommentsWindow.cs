using System.Drawing;
using System.Windows.Forms;

namespace MemcardRex.GUI
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class CommentsWindow : Form
    {
        /// <summary>
        /// </summary>
        public string Comment { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public CommentsWindow(string windowTitle, string saveComment)
        {
            var commentsTextBox = new TextBox
            {
                Location = new Point(4, 4),
                MaxLength = 255,
                Multiline = true,
                Name = "commentsTextBox",
                Size = new Size(284, 100),
                TabIndex = 0,
                Text = saveComment
            };

            var warningLabel = new Label
            {
                AutoSize = true,
                Location = new Point(4, 108),
                Name = "warningLabel",
                Size = new Size(263, 13),
                TabIndex = 10,
                Text = "Comments are only supported by DexDrive (.gme) files."
            };

            var cancelButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(212, 134),
                Name = "cancelButton",
                Size = new Size(76, 23),
                TabIndex = 2,
                Text = "Cancel",
                UseVisualStyleBackColor = true
            };

            var okButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(132, 134),
                Name = "okButton",
                Size = new Size(76, 23),
                TabIndex = 1,
                Text = "OK",
                UseVisualStyleBackColor = true
            };

            var spacerLabel = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(4, 128),
                Name = "spacerLabel",
                Size = new Size(284, 2),
                TabIndex = 11
            };

            okButton.Click += (sender, arguments) =>
            {
                Comment = commentsTextBox.Text;
                DialogResult = DialogResult.OK;
                Close();
            };

            cancelButton.Click += (sender, arguments) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            SuspendLayout();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(292, 161);
            Controls.Add(spacerLabel);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(warningLabel);
            Controls.Add(commentsTextBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CommentsWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = windowTitle;
            ResumeLayout(false);
            PerformLayout();
            commentsTextBox.Select(commentsTextBox.Text.Length, 0);
        }
    }
}