using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MemcardRex.GUI
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class CompareWindow : Form
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public CompareWindow(IWin32Window mainWindow, string applicationName, IReadOnlyList<byte> save1Data, string save1Title, IReadOnlyList<byte> save2Data, string save2Title)
        {
            var listView = new ListView
            {
                FullRowSelect = true,
                Location = new Point(4, 44),
                MultiSelect = false,
                Name = "listView",
                Size = new Size(368, 216),
                TabIndex = 0,
                UseCompatibleStateImageBehavior = false,
                View = View.Details
            };

            var save1Label = new Label
            {
                AutoSize = true,
                Location = new Point(4, 4),
                Name = "save1Label",
                Size = new Size(62, 13),
                TabIndex = 1,
                Text = "Save 1: " + save1Title
            };

            var save2Label = new Label
            {
                AutoSize = true,
                Location = new Point(4, 24),
                Name = "save2Label",
                Size = new Size(64, 13),
                TabIndex = 2,
                Text = "Save 2: " + save2Title
            };

            var spacerLabel = new Label
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(4, 265),
                Name = "spacerLabel",
                Size = new Size(368, 2),
                TabIndex = 11
            };

            var okButton = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(296, 269),
                Name = "okButton",
                Size = new Size(76, 24),
                TabIndex = 10,
                Text = "OK",
                UseVisualStyleBackColor = true
            };

            listView.Columns.AddRange(new[]
            {
                new ColumnHeader
                {
                    Text = "Offset (hex, int)",
                    Width = 115
                },
                new ColumnHeader
                {
                    Text = "Save1 (hex, int)",
                    Width = 115
                },
                new ColumnHeader
                {
                    Text = "Save2 (hex, int)",
                    Width = 115
                }
            });

            SuspendLayout();
            okButton.Click += (sender, arguments) => Close();
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(376, 297);
            Controls.Add(spacerLabel);
            Controls.Add(okButton);
            Controls.Add(save2Label);
            Controls.Add(save1Label);
            Controls.Add(listView);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CompareWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Comparison";
            ResumeLayout(false);
            PerformLayout();

            for (var offset = 0; offset < save1Data.Count; offset++)
            {
                if (save1Data[offset] == save2Data[offset])
                {
                    continue;
                }

                listView.Items.Add($"0x{offset:X4} ({offset})");
                listView.Items[listView.Items.Count - 1].SubItems.Add($"0x{save1Data[offset]:X2} ({save1Data[offset]})");
                listView.Items[listView.Items.Count - 1].SubItems.Add($"0x{save2Data[offset]:X2} ({save2Data[offset]})");
            }

            if (listView.Items.Count > 0)
            {
                ShowDialog(mainWindow);
            }
        }
    }
}