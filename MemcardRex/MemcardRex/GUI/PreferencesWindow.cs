using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace MemcardRex.GUI
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class PreferencesWindow : Form
    {
        /// <summary>
        /// </summary>
        private readonly ComboBox _backgroundCombo;

        /// <summary>
        /// </summary>
        private readonly CheckBox _backupCheckbox;

        /// <summary>
        /// </summary>
        private readonly CheckBox _backupWarningCheckBox;

        /// <summary>
        /// </summary>
        private readonly ComboBox _encodingCombo;

        /// <summary>
        /// </summary>
        private readonly ComboBox _fontCombo;

        /// <summary>
        /// </summary>
        private readonly ComboBox _formatCombo;

        /// <summary>
        /// </summary>
        private readonly CheckBox _gridCheckbox;

        /// <summary>
        /// </summary>
        private readonly ComboBox _iconSizeCombo;

        /// <summary>
        /// </summary>
        private readonly ComboBox _interpolationCombo;

        /// <summary>
        /// </summary>
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// </summary>
        private readonly CheckBox _restorePositionCheckbox;

        /// <summary>
        /// </summary>
        private string _savedComPort;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public PreferencesWindow(MainWindow mainWindow, MainWindow.ProgramSettings settings)
        {
            var okButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(224, 184),
                Name = "okButton",
                Size = new Size(76, 24),
                TabIndex = 99,
                Text = "OK",
                UseVisualStyleBackColor = true
            };

            var cancelButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(304, 184),
                Name = "cancelButton",
                Size = new Size(76, 24),
                TabIndex = 0,
                Text = "Cancel",
                UseVisualStyleBackColor = true
            };

            var applyButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(384, 184),
                Name = "applyButton",
                Size = new Size(76, 24),
                TabIndex = 1,
                Text = "Apply",
                UseVisualStyleBackColor = true
            };

            _encodingCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(4, 20),
                Name = "encodingCombo",
                Size = new Size(116, 21),
                TabIndex = 2
            };

            var titleEncoding = new Label
            {
                AutoSize = true,
                Location = new Point(4, 4),
                Name = "titleEncoding",
                Size = new Size(101, 13),
                TabIndex = 5,
                Text = "Save title encoding:"
            };

            _fontCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(124, 20),
                Name = "fontCombo",
                Size = new Size(116, 21),
                TabIndex = 3
            };

            var fontLabel = new Label
            {
                AutoSize = true,
                Location = new Point(124, 4),
                Name = "fontLabel",
                Size = new Size(75, 13),
                TabIndex = 3,
                Text = "Save title font:"
            };

            _gridCheckbox = new CheckBox
            {
                AutoSize = true,
                Checked = settings.ShowListGrid == 1,
                Location = new Point(248, 12),
                Name = "gridCheckbox",
                Size = new Size(125, 17),
                TabIndex = 9,
                Text = "Show grid on slot list.",
                UseVisualStyleBackColor = true
            };

            var iconSizeLabel = new Label
            {
                AutoSize = true,
                Location = new Point(124, 48),
                Name = "iconSizeLabel",
                Size = new Size(52, 13),
                TabIndex = 5,
                Text = "Icon size:"
            };

            _iconSizeCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(124, 64),
                Name = "iconSizeCombo",
                Size = new Size(116, 21),
                TabIndex = 5
            };

            _interpolationCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(4, 64),
                Name = "interpolationCombo",
                Size = new Size(116, 21),
                TabIndex = 4
            };

            var interpolationLabel = new Label
            {
                AutoSize = true,
                Location = new Point(4, 48),
                Name = "interpolationLabel",
                Size = new Size(53, 13),
                TabIndex = 0,
                Text = "Icon filter:"
            };

            _backupWarningCheckBox = new CheckBox
            {
                AutoSize = true,
                Checked = settings.WarningMessage == 1,
                Location = new Point(248, 60),
                Name = "backupWarningCheckBox",
                Size = new Size(212, 17),
                TabIndex = 11,
                Text = "Show warning messages (save editing).",
                UseVisualStyleBackColor = true
            };

            _backupCheckbox = new CheckBox
            {
                AutoSize = true,
                Checked = settings.BackupMemcards == 1,
                Location = new Point(248, 36),
                Name = "backupCheckbox",
                Size = new Size(204, 17),
                TabIndex = 10,
                Text = "Backup Memory Cards upon opening.",
                UseVisualStyleBackColor = true
            };

            var dexDriveCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(4, 152),
                Name = "dexDriveCombo",
                Size = new Size(116, 21),
                TabIndex = 7
            };

            var hardwarePortLabel = new Label
            {
                AutoSize = true,
                Location = new Point(4, 136),
                Name = "hardwarePortLabel",
                Size = new Size(103, 13),
                TabIndex = 6,
                Text = "Communication port:"
            };

            var spacerLabel = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(4, 179),
                Name = "spacerLabel",
                Size = new Size(456, 2),
                TabIndex = 8
            };

            _restorePositionCheckbox = new CheckBox
            {
                AutoSize = true,
                Checked = settings.RestoreWindowPosition == 1,
                Location = new Point(248, 108),
                Name = "restorePositionCheckbox",
                Size = new Size(191, 17),
                TabIndex = 13,
                Text = "Restore window position on startup",
                UseVisualStyleBackColor = true
            };

            _formatCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(124, 152),
                Name = "formatCombo",
                Size = new Size(116, 21),
                TabIndex = 8
            };

            var formatLabel = new Label
            {
                AutoSize = true,
                Location = new Point(124, 136),
                Name = "formatLabel",
                Size = new Size(111, 13),
                TabIndex = 101,
                Text = "Hardware format type:"
            };

            _backgroundCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(4, 108),
                Name = "backgroundCombo",
                Size = new Size(236, 21),
                TabIndex = 6
            };

            var backgroundLabel = new Label
            {
                AutoSize = true,
                Location = new Point(4, 92),
                Name = "backgroundLabel",
                Size = new Size(117, 13),
                TabIndex = 102,
                Text = "Icon background color:"
            };

            _formatCombo.Items.AddRange(new object[]
            {
                "Quick format",
                "Full format"
            });

            _encodingCombo.Items.AddRange(new object[]
            {
                "ASCII",
                "UTF-16"
            });

            _iconSizeCombo.Items.AddRange(new object[]
            {
                "32x32",
                "48x48"
            });

            _backgroundCombo.Items.AddRange(new object[]
            {
                "Transparent",
                "Black (Slim PS1 models)",
                "Gray (Older european PS1 models)",
                "Blue (Standard BIOS color)"
            });

            _interpolationCombo.Items.AddRange(new object[]
            {
                "Nearest Neighbor",
                "Bilinear"
            });

            okButton.Click += (sender, arguments) =>
            {
                ApplySettings(sender, arguments);
                Close();
            };

            _mainWindow = mainWindow;
            applyButton.Click += ApplySettings;
            cancelButton.Click += (sender, arguments) => Close();
            dexDriveCombo.SelectedIndexChanged += (sender, arguments) => _savedComPort = dexDriveCombo.Text;
            SuspendLayout();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(463, 212);
            Controls.Add(_backgroundCombo);
            Controls.Add(backgroundLabel);
            Controls.Add(formatLabel);
            Controls.Add(_formatCombo);
            Controls.Add(_restorePositionCheckbox);
            Controls.Add(spacerLabel);
            Controls.Add(_gridCheckbox);
            Controls.Add(_backupWarningCheckBox);
            Controls.Add(iconSizeLabel);
            Controls.Add(_backupCheckbox);
            Controls.Add(_iconSizeCombo);
            Controls.Add(fontLabel);
            Controls.Add(_interpolationCombo);
            Controls.Add(dexDriveCombo);
            Controls.Add(interpolationLabel);
            Controls.Add(_fontCombo);
            Controls.Add(hardwarePortLabel);
            Controls.Add(_encodingCombo);
            Controls.Add(titleEncoding);
            Controls.Add(applyButton);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PreferencesWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Preferences";
            ResumeLayout(false);
            PerformLayout();
            _savedComPort = settings.CommunicationPort;
            _formatCombo.SelectedIndex = settings.FormatType;
            _encodingCombo.SelectedIndex = settings.TitleEncoding;
            _iconSizeCombo.SelectedIndex = settings.IconPropertiesSize;
            _backgroundCombo.SelectedIndex = settings.IconBackgroundColor;
            _interpolationCombo.SelectedIndex = settings.IconInterpolationMode;

            foreach (var port in SerialPort.GetPortNames())
            {
                dexDriveCombo.Items.Add(port);
            }

            if (dexDriveCombo.Items.Count < 1)
            {
                dexDriveCombo.Enabled = false;
            }

            dexDriveCombo.SelectedItem = settings.CommunicationPort;

            foreach (var font in FontFamily.Families)
            {
                _fontCombo.Items.Add(font.Name);
            }

            _fontCombo.SelectedItem = settings.ListFont;
        }

        /// <summary>
        /// </summary>
        private void ApplySettings(object sender, EventArgs arguments)
        {
            var settings = new MainWindow.ProgramSettings
            {
                TitleEncoding = _encodingCombo.SelectedIndex,
                IconInterpolationMode = _interpolationCombo.SelectedIndex,
                IconPropertiesSize = _iconSizeCombo.SelectedIndex,
                IconBackgroundColor = _backgroundCombo.SelectedIndex,
                FormatType = _formatCombo.SelectedIndex,
                CommunicationPort = _savedComPort,
                ShowListGrid = _gridCheckbox.Checked ? 1 : 0,
                BackupMemcards = _backupCheckbox.Checked ? 1 : 0,
                WarningMessage = _backupWarningCheckBox.Checked ? 1 : 0,
                RestoreWindowPosition = _restorePositionCheckbox.Checked ? 1 : 0
            };

            if (_fontCombo.SelectedIndex != -1)
            {
                settings.ListFont = _fontCombo.SelectedItem.ToString();
            }

            _mainWindow.ApplyProgramSettings(settings);
        }
    }
}