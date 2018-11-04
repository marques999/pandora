using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace MemcardRex.GUI
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class InformationWindow : Form
    {
        /// <summary>
        /// </summary>
        private readonly int _backgroundColor;

        /// <summary>
        /// </summary>
        private readonly IContainer _components;

        /// <summary>
        /// </summary>
        private readonly Bitmap[] _iconData;

        /// <summary>
        /// </summary>
        private readonly int _iconInterpolationMode;

        /// <summary>
        /// </summary>
        private readonly PictureBox _iconRender;

        /// <summary>
        /// </summary>
        private readonly int _iconSize;

        /// <summary>
        /// </summary>
        private int _iconIndex;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public InformationWindow(string saveTitle, string saveProdCode, string saveIdentifier, ushort saveRegion,
            int saveSize, int iconFrames, int interpolationMode, int iconPropertiesSize, Bitmap[] saveIcons,
            IEnumerable<int> slotNumbers, int backgroundColor)
        {
            _iconData = saveIcons;
            _components = new Container();
            _iconSize = iconPropertiesSize;
            _backgroundColor = backgroundColor;
            _iconInterpolationMode = interpolationMode;
            ((ISupportInitialize)_iconRender).BeginInit();

            var okButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(296, 120),
                Name = "okButton",
                Size = new Size(76, 24),
                TabIndex = 0,
                Text = "OK",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            var slotLabel = new Label
            {
                Location = new Point(136, 88),
                Name = "slotLabel",
                Size = new Size(236, 13),
                TabIndex = 12,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            var slotLabelInfo = new Label
            {
                Location = new Point(8, 88),
                Name = "slotLabelInfo",
                Size = new Size(128, 13),
                TabIndex = 5,
                Text = "Slot:",
                TextAlign = ContentAlignment.MiddleRight
            };

            var iconFramesLabel = new Label
            {
                Location = new Point(136, 128),
                Name = "iconFramesLabel",
                Size = new Size(156, 13),
                TabIndex = 14,
                Text = iconFrames.ToString(),
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            var sizeLabel = new Label
            {
                Location = new Point(136, 108),
                Name = "sizeLabel",
                Size = new Size(156, 13),
                TabIndex = 13,
                Text = $"{saveSize} KB",
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            var regionLabel = new Label
            {
                Location = new Point(136, 68),
                Name = "regionLabel",
                Size = new Size(236, 13),
                TabIndex = 11,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            var identifierLabel = new Label
            {
                Location = new Point(136, 48),
                Name = "identifierLabel",
                Size = new Size(236, 13),
                TabIndex = 10,
                Text = saveIdentifier,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            var productCodeLabel = new Label
            {
                Location = new Point(136, 28),
                Name = "productCodeLabel",
                Size = new Size(236, 13),
                TabIndex = 9,
                Text = saveProdCode,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            var saveTitleLabel = new Label
            {
                Location = new Point(136, 8),
                Name = "saveTitleLabel",
                Size = new Size(236, 13),
                TabIndex = 8,
                Text = saveTitle,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            var iconFramesLabelInfo = new Label
            {
                Location = new Point(8, 128),
                Name = "iconFramesLabelInfo",
                Size = new Size(128, 13),
                TabIndex = 7,
                Text = "Icon frames:",
                TextAlign = ContentAlignment.MiddleRight
            };

            var sizeLabelInfo = new Label
            {
                Location = new Point(8, 108),
                Name = "sizeLabelInfo",
                Size = new Size(128, 13),
                TabIndex = 6,
                Text = "Size:",
                TextAlign = ContentAlignment.MiddleRight
            };

            var regionLabelInfo = new Label
            {
                Location = new Point(8, 68),
                Name = "regionLabelInfo",
                Size = new Size(128, 13),
                TabIndex = 4,
                Text = "Region:",
                TextAlign = ContentAlignment.MiddleRight
            };

            var identifierLabelInfo = new Label
            {
                Location = new Point(60, 48),
                Name = "identifierLabelInfo",
                Size = new Size(76, 13),
                TabIndex = 3,
                Text = "Identifier:",
                TextAlign = ContentAlignment.MiddleRight
            };

            var productCodeLabelInfo = new Label
            {
                Location = new Point(60, 28),
                Name = "productCodeLabelInfo",
                Size = new Size(76, 13),
                TabIndex = 2,
                Text = "Product code:",
                TextAlign = ContentAlignment.MiddleRight
            };

            var saveTitleLabelInfo = new Label
            {
                Location = new Point(60, 8),
                Name = "saveTitleLabelInfo",
                Size = new Size(76, 13),
                TabIndex = 1,
                Text = "Title:",
                TextAlign = ContentAlignment.MiddleRight
            };

            _iconRender = new PictureBox
            {
                BackColor = Color.Transparent,
                BackgroundImageLayout = ImageLayout.None,
                Location = new Point(8, 8),
                Name = "IconRender",
                Size = new Size(48, 48),
                TabIndex = 17,
                TabStop = false
            };

            var iconPaintTimer = new Timer(_components)
            {
                Enabled = iconFrames > 1,
                Interval = 180
            };

            iconPaintTimer.Tick += (sender, arguments) =>
            {
                if (_iconIndex < iconFrames - 1)
                {
                    _iconIndex++;
                }
                else
                {
                    _iconIndex = 0;
                }

                DrawIcons(_iconIndex);
            };

            SuspendLayout();
            okButton.Click += (sender, arguments) => Close();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(376, 148);
            Controls.Add(slotLabel);
            Controls.Add(slotLabelInfo);
            Controls.Add(iconFramesLabel);
            Controls.Add(sizeLabel);
            Controls.Add(regionLabel);
            Controls.Add(identifierLabel);
            Controls.Add(productCodeLabel);
            Controls.Add(saveTitleLabel);
            Controls.Add(iconFramesLabelInfo);
            Controls.Add(sizeLabelInfo);
            Controls.Add(regionLabelInfo);
            Controls.Add(identifierLabelInfo);
            Controls.Add(productCodeLabelInfo);
            Controls.Add(saveTitleLabelInfo);
            Controls.Add(_iconRender);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InformationWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Information";
            FormClosing += (sender, arguments) => iconPaintTimer.Enabled = false;
            ((ISupportInitialize)_iconRender).EndInit();
            ResumeLayout(false);

            switch (saveRegion)
            {
            default:
                regionLabel.Text = $"0x{saveRegion:X4}";
                break;
            case 0x4142:
                regionLabel.Text = "America";
                break;
            case 0x4542:
                regionLabel.Text = "Europe";
                break;
            case 0x4942:
                regionLabel.Text = "Japan";
                break;
            }

            var slots = slotNumbers.Aggregate<int, string>(null, (current, description) => $"{current}{description + 1}, ");

            slotLabel.Text = slots.Remove(slots.Length - 2);
            DrawIcons(_iconIndex);
        }

        /// <summary>
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void DrawIcons(int selectedIndex)
        {
            var tempBitmap = new Bitmap(48, 48);
            var iconGraphics = Graphics.FromImage(tempBitmap);

            if (_iconInterpolationMode == 0)
            {
                iconGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
            else
            {
                iconGraphics.InterpolationMode = InterpolationMode.Bilinear;
            }

            iconGraphics.PixelOffsetMode = PixelOffsetMode.Half;

            switch (_backgroundColor)
            {
            case 1:
                iconGraphics.FillRegion(new SolidBrush(Color.Black), new Region(new Rectangle(0, 0, 48, 48)));
                break;
            case 2:
                iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x30, 0x30, 0x30)), new Region(new Rectangle(0, 0, 48, 48)));
                break;
            case 3:
                iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x98)), new Region(new Rectangle(0, 0, 48, 48)));
                break;
            }

            iconGraphics.DrawImage(_iconData[selectedIndex], 0, 0, 32 + _iconSize * 16, 32 + _iconSize * 16);
            _iconRender.Image = tempBitmap;
            iconGraphics.Dispose();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _components?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}