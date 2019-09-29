using System.Drawing;
using System.Windows.Forms;

namespace MemcardRex.GUI
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class HeaderWindow : Form
    {
        /// <summary>
        /// </summary>
        private readonly ushort _saveRegion;

        /// <summary>
        /// </summary>
        public bool OkPressed;

        /// <summary>
        /// </summary>
        public string ProductCode;

        /// <summary>
        /// </summary>
        public string SaveIdentifier;

        /// <summary>
        /// </summary>
        public ushort SaveRegion;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public HeaderWindow(string applicationName, string windowTitle, string productCode, string identifier, ushort saveRegion)
        {
            var productLabel = new Label
            {
                AutoSize = true,
                Location = new Point(4, 4),
                Name = "productLabel",
                Size = new Size(71, 13),
                TabIndex = 0,
                Text = "Product Code"
            };

            var productCodeTextbox = new TextBox
            {
                Location = new Point(4, 20),
                MaxLength = 10,
                Name = "productCodeTextbox",
                Size = new Size(104, 20),
                TabIndex = 0,
                Text = productCode
            };

            var identifierLabel = new Label
            {
                AutoSize = true,
                Location = new Point(112, 4),
                Name = "identifierLabel",
                Size = new Size(47, 13),
                TabIndex = 2,
                Text = "Identifier"
            };

            var identifierTextbox = new TextBox
            {
                Location = new Point(112, 20),
                MaxLength = 8,
                Name = "identifierTextbox",
                Size = new Size(104, 20),
                TabIndex = 1,
                Text = identifier
            };

            var regionCombobox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(220, 20),
                Name = "regionCombobox",
                Size = new Size(111, 21),
                TabIndex = 2
            };

            var regionLabel = new Label
            {
                AutoSize = true,
                Location = new Point(220, 4),
                Name = "regionLabel",
                Size = new Size(41, 13),
                TabIndex = 5,
                Text = "Region"
            };

            var okButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(176, 54),
                Name = "okButton",
                Size = new Size(76, 23),
                TabIndex = 3,
                Text = "OK",
                UseVisualStyleBackColor = true
            };

            var cancelButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(256, 54),
                Name = "cancelButton",
                Size = new Size(76, 23),
                TabIndex = 4,
                Text = "Cancel",
                UseVisualStyleBackColor = true
            };

            var spacerLabel = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(4, 48),
                Name = "spacerLabel",
                Size = new Size(328, 2),
                TabIndex = 12
            };

            regionCombobox.Items.AddRange(new object[]
            {
                "America",
                "Europe",
                "Japan"
            });

            okButton.Click += (sender, arguments) =>
            {
                if (productCodeTextbox.Text.Length < 10 && identifierTextbox.Text.Length != 0)
                {
                    Messages.Warning(applicationName, "Product code must be exactly 10 characters long.");
                }
                else
                {
                    ProductCode = productCodeTextbox.Text;
                    SaveIdentifier = identifierTextbox.Text;

                    switch (regionCombobox.SelectedIndex)
                    {
                    case 0:
                        SaveRegion = 0x4142;
                        break;
                    case 1:
                        SaveRegion = 0x4542;
                        break;
                    case 2:
                        SaveRegion = 0x4942;
                        break;
                    default:
                        SaveRegion = _saveRegion;
                        break;
                    }

                    OkPressed = true;
                    Close();
                }
            };

            SuspendLayout();
            cancelButton.Click += (sender, arguments) => Close();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(336, 81);
            Controls.Add(spacerLabel);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(regionLabel);
            Controls.Add(regionCombobox);
            Controls.Add(identifierTextbox);
            Controls.Add(identifierLabel);
            Controls.Add(productCodeTextbox);
            Controls.Add(productLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "HeaderWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = windowTitle;
            ResumeLayout(false);
            PerformLayout();

            switch (saveRegion)
            {
            case 0x4142:
                regionCombobox.SelectedIndex = 0;
                break;
            case 0x4542:
                regionCombobox.SelectedIndex = 1;
                break;
            case 0x4942:
                regionCombobox.SelectedIndex = 2;
                break;
            default:
                _saveRegion = saveRegion;
                regionCombobox.Items.Add("0x" + saveRegion.ToString("X4"));
                regionCombobox.SelectedIndex = 3;
                break;
            }

            productCodeTextbox.Select(productCodeTextbox.Text.Length, 0);
            identifierTextbox.Select(identifierTextbox.Text.Length, 0);
        }
    }
}