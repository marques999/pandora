using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MemcardRex.GUI
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class IconWindow : Form
    {
        /// <summary>
        /// </summary>
        private readonly PictureBox _colorRender;

        /// <summary>
        /// </summary>
        private readonly PictureBox _colorRender2;

        /// <summary>
        /// </summary>
        private readonly ComboBox _frameCombo;

        /// <summary>
        /// </summary>
        private readonly PictureBox _iconRender;

        /// <summary>
        /// </summary>
        private readonly PictureBox _paletteRender;

        /// <summary>
        /// </summary>
        private readonly int[] _selectedColor = new int[2];

        /// <summary>
        /// </summary>
        private readonly Label _xLabel;

        /// <summary>
        /// </summary>
        private readonly Label _yLabel;

        /// <summary>
        /// </summary>
        private Bitmap[] _iconBitmap = new Bitmap[3];

        /// <summary>
        /// </summary>
        private Color[] _iconPalette = new Color[16];

        /// <summary>
        /// </summary>
        private int _selectedIcon;

        /// <summary>
        /// </summary>
        public byte[] IconData;

        /// <summary>
        /// </summary>
        public bool OkPressed;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="dialogTitle"></param>
        /// <param name="iconFrames"></param>
        /// <param name="iconBytes"></param>
        public IconWindow(string dialogTitle, int iconFrames, byte[] iconBytes)
        {
            IconData = iconBytes;
            ((ISupportInitialize)_iconRender).BeginInit();
            ((ISupportInitialize)_paletteRender).BeginInit();
            ((ISupportInitialize)_colorRender).BeginInit();
            ((ISupportInitialize)_colorRender2).BeginInit();
            SuspendLayout();

            var okButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(108, 260),
                Name = "okButton",
                Size = new Size(76, 24),
                TabIndex = 8,
                Text = "OK",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            var cancelButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(188, 260),
                Name = "cancelButton",
                Size = new Size(76, 24),
                TabIndex = 0,
                Text = "Cancel",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            _frameCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(4, 4),
                Name = "frameCombo",
                Size = new Size(259, 21),
                TabIndex = 1
            };

            var exportButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(188, 56),
                Name = "exportButton",
                Size = new Size(76, 24),
                TabIndex = 3,
                Text = "Export...",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            var horizontalFlipButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(188, 84),
                Name = "horizontalFlipButton",
                Size = new Size(76, 24),
                TabIndex = 4,
                Text = "Horizontal Flip",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            var verticalFlipButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(188, 112),
                Name = "verticalFlipButton",
                Size = new Size(76, 24),
                TabIndex = 5,
                Text = "Vertical Flip",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            var leftButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(188, 140),
                Name = "leftButton",
                Size = new Size(76, 24),
                TabIndex = 6,
                Text = "Rotate Left",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            var rightButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(188, 168),
                Name = "rightButton",
                Size = new Size(76, 24),
                TabIndex = 7,
                Text = "Rotate right",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            _iconRender = new PictureBox
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(4, 28),
                Name = "iconRender",
                Size = new Size(181, 181),
                TabIndex = 9,
                TabStop = false
            };

            _paletteRender = new PictureBox
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(60, 213),
                Name = "paletteRender",
                Size = new Size(125, 35),
                TabIndex = 10,
                TabStop = false
            };

            _colorRender = new PictureBox
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(4, 213),
                Name = "colorRender",
                Size = new Size(26, 35),
                TabIndex = 11,
                TabStop = false
            };

            _xLabel = new Label
            {
                AutoSize = true,
                Location = new Point(188, 232),
                Name = "xLabel",
                Size = new Size(17, 13),
                TabIndex = 12,
                Text = "X:"
            };

            _yLabel = new Label
            {
                AutoSize = true,
                Location = new Point(220, 232),
                Name = "yLabel",
                Size = new Size(17, 13),
                TabIndex = 13,
                Text = "Y:"
            };

            _colorRender2 = new PictureBox
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(32, 213),
                Name = "colorRender2",
                Size = new Size(26, 35),
                TabIndex = 14,
                TabStop = false
            };

            var spacerLabel = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(4, 254),
                Name = "spacerLabel",
                Size = new Size(260, 2),
                TabIndex = 15
            };

            var importButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(188, 28),
                Name = "importButton",
                Size = new Size(76, 24),
                TabIndex = 2,
                Text = "Import icon...",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            leftButton.Click += LeftRotate;
            okButton.Click += OkButton_Click;
            importButton.Click += ImportIcon;
            rightButton.Click += RightRotate;
            exportButton.Click += ExportIcon;
            verticalFlipButton.Click += VerticalFlip;
            horizontalFlipButton.Click += HorizontalFlip;
            _iconRender.MouseLeave += IconRender_MouseLeave;
            _iconRender.MouseDown += IconRender_MouseDownMove;
            _iconRender.MouseMove += IconRender_MouseDownMove;
            _paletteRender.MouseDown += paletteRender_MouseDown;
            cancelButton.Click += (sender, arguments) => Close();
            _paletteRender.DoubleClick += PaletteRender_DoubleClick;
            _frameCombo.SelectedIndexChanged += FrameCombo_SelectedIndexChanged;
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(267, 288);
            Controls.Add(importButton);
            Controls.Add(spacerLabel);
            Controls.Add(_colorRender2);
            Controls.Add(_yLabel);
            Controls.Add(_xLabel);
            Controls.Add(_colorRender);
            Controls.Add(_paletteRender);
            Controls.Add(_iconRender);
            Controls.Add(rightButton);
            Controls.Add(leftButton);
            Controls.Add(verticalFlipButton);
            Controls.Add(horizontalFlipButton);
            Controls.Add(exportButton);
            Controls.Add(_frameCombo);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "IconWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = dialogTitle;
            ((ISupportInitialize)_iconRender).EndInit();
            ((ISupportInitialize)_paletteRender).EndInit();
            ((ISupportInitialize)_colorRender).EndInit();
            ((ISupportInitialize)_colorRender2).EndInit();
            ResumeLayout(false);
            PerformLayout();

            switch (iconFrames)
            {
            case 2:
                _frameCombo.Items.Add("1st frame");
                _frameCombo.Items.Add("2nd frame");
                break;
            case 3:
                _frameCombo.Items.Add("1st frame");
                _frameCombo.Items.Add("2nd frame");
                _frameCombo.Items.Add("3rd frame");
                break;
            default:
                _frameCombo.Items.Add("1st frame");
                _frameCombo.Enabled = false;
                break;
            }

            SetUpDisplay();
            _frameCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// </summary>
        private void SetUpDisplay()
        {
            DrawPalette();
            SetSelectedColor(0, 0);
            SetSelectedColor(1, 1);
            DrawIcon();
        }

        /// <summary>
        /// </summary>
        private void LoadPalette()
        {
            var colorIndex = 0;

            _iconPalette = new Color[16];

            for (var byteIndex = 0; byteIndex < 32; byteIndex += 2)
            {
                var redChannel = (IconData[byteIndex] & 0x1F) << 3;
                var greenChannel = ((IconData[byteIndex + 1] & 0x3) << 6) | ((IconData[byteIndex] & 0xE0) >> 2);
                var blueChannel = (IconData[byteIndex + 1] & 0x7C) << 1;

                _iconPalette[colorIndex] = Color.FromArgb(redChannel, greenChannel, blueChannel);
                colorIndex++;
            }
        }

        /// <summary>
        /// </summary>
        private void LoadIcons()
        {
            _iconBitmap = new Bitmap[3];

            for (var frameIndex = 0; frameIndex < _iconBitmap.Length; frameIndex++)
            {
                _iconBitmap[frameIndex] = new Bitmap(16, 16);

                var byteIndex = 32 + 128 * frameIndex;

                for (var yPixel = 0; yPixel < 16; yPixel++)
                {
                    for (var xPixel = 0; xPixel < 16; xPixel += 2)
                    {
                        _iconBitmap[frameIndex].SetPixel(xPixel, yPixel, _iconPalette[IconData[byteIndex] & 0xF]);
                        _iconBitmap[frameIndex].SetPixel(xPixel + 1, yPixel, _iconPalette[IconData[byteIndex] >> 4]);
                        byteIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        private void DrawIcon()
        {
            var blackPen = new Pen(Color.Black);
            var drawBitmap = new Bitmap(177, 177);
            var drawGraphics = Graphics.FromImage(drawBitmap);

            LoadIcons();
            drawGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            drawGraphics.PixelOffsetMode = PixelOffsetMode.Default;
            drawGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            drawGraphics.DrawImage(_iconBitmap[_selectedIcon], 0, 0, 177, 177);

            for (var yPixel = 0; yPixel < 17; yPixel++)
            {
                drawGraphics.DrawLine(blackPen, 0, yPixel * 11, 177, yPixel * 11);
            }

            for (var xPixel = 0; xPixel < 17; xPixel++)
            {
                drawGraphics.DrawLine(blackPen, xPixel * 11, 0, xPixel * 11, 177);
            }

            drawGraphics.Dispose();
            _iconRender.Image = drawBitmap;
        }

        /// <summary>
        /// </summary>
        private void DrawPalette()
        {
            var colorIndex = 0;
            var blackPen = new Pen(Color.Black);
            var paletteBitmap = new Bitmap(8, 2);
            var drawBitmap = new Bitmap(121, 31);
            var drawGraphics = Graphics.FromImage(drawBitmap);

            LoadPalette();
            drawGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            drawGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            for (var yPixel = 0; yPixel < 2; yPixel++)
            {
                for (var xPixel = 0; xPixel < 8; xPixel++)
                {
                    paletteBitmap.SetPixel(xPixel, yPixel, _iconPalette[colorIndex]);
                    colorIndex++;
                }
            }

            drawGraphics.DrawImage(paletteBitmap, 0, 0, 120, 30);
            drawGraphics.PixelOffsetMode = PixelOffsetMode.Default;

            for (var yPixel = 0; yPixel < 3; yPixel++)
            {
                drawGraphics.DrawLine(blackPen, 0, yPixel * 15, 121, yPixel * 15);
            }

            for (var xPixel = 0; xPixel < 9; xPixel++)
            {
                drawGraphics.DrawLine(blackPen, xPixel * 15, 0, xPixel * 15, 31);
            }

            drawGraphics.Dispose();
            _paletteRender.Image = drawBitmap;
        }

        /// <summary>
        /// </summary>
        /// <param name="selectedColor"></param>
        /// <param name="selectedColorIndex"></param>
        private void SetSelectedColor(int selectedColor, int selectedColorIndex)
        {
            if (selectedColorIndex == 0)
            {
                _selectedColor[0] = selectedColor;
                _colorRender.BackColor = _iconPalette[_selectedColor[0]];
            }
            else
            {
                _selectedColor[1] = selectedColor;
                _colorRender2.BackColor = _iconPalette[_selectedColor[1]];
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="xPixel"></param>
        /// <param name="yPixel"></param>
        /// <param name="selectedColorIndex"></param>
        private void PutPixel(int xPixel, int yPixel, int selectedColorIndex)
        {
            var destinationByte = (xPixel + yPixel * 16) / 2;

            if ((xPixel + yPixel * 16) % 2 == 0)
            {
                IconData[32 + destinationByte + _selectedIcon * 128] &= 0xF0;
                IconData[32 + destinationByte + _selectedIcon * 128] |= (byte)_selectedColor[selectedColorIndex];
            }
            else
            {
                IconData[32 + destinationByte + _selectedIcon * 128] &= 0x0F;
                IconData[32 + destinationByte + _selectedIcon * 128] |= (byte)(_selectedColor[selectedColorIndex] << 4);
            }

            DrawIcon();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void ImportIcon(object sender, EventArgs arguments)
        {
            var foundColors = new List<Color>();
            var returnData = new byte[16, 16];

            var openFileDialog = new OpenFileDialog
            {
                Title = "Open icon",
                Filter =
                    "All supported|*.bmp;*.gif;*.jpeg;*.jpg;*.png|Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpeg;*.jpg)|*.jpeg;*.jpg|PNG (*.png)|*.png"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Bitmap openedBitmap = null;

            try
            {
                openedBitmap = new Bitmap(openFileDialog.FileName);
            }
            catch (Exception exception)
            {
                Messages.Warning("Error", exception.Message);
                openedBitmap?.Dispose();
                return;
            }

            if (openedBitmap.Width != 16 || openedBitmap.Height != 16)
            {
                Messages.Warning("Warning", "Selected image is not a 16x16 pixel image.");
                openedBitmap.Dispose();
                return;
            }

            for (var y = 0; y < 16; y++)
                for (var x = 0; x < 16; x++)
                    if (!foundColors.Contains(openedBitmap.GetPixel(x, y)))
                        foundColors.Add(openedBitmap.GetPixel(x, y));

            if (foundColors.Count > 16)
            {
                Messages.Warning("Warning", "Selected image contains more then 16 colors.");
                openedBitmap.Dispose();
                return;
            }

            for (var colorIndex = foundColors.Count; colorIndex < 16; colorIndex++)
            {
                foundColors.Add(Color.Black);
            }

            for (var colorIndex = 0; colorIndex < 16; colorIndex++)
            {
                var greenChannel = foundColors[colorIndex].G >> 3;

                IconData[colorIndex * 2] = (byte)((foundColors[colorIndex].R >> 3) | ((greenChannel & 0x07) << 5));
                IconData[colorIndex * 2 + 1] =
                    (byte)(((foundColors[colorIndex].B >> 3) << 2) | ((greenChannel & 0x18) >> 3));
            }

            for (var y = 0; y < 16; y++)
                for (var x = 0; x < 16; x++)
                {
                    byte tempIndex = 0;

                    var color = openedBitmap.GetPixel(x, y);

                    for (byte pIndex = 0; pIndex < 16; pIndex++)
                    {
                        if (foundColors[pIndex] != color) continue;

                        tempIndex = pIndex;

                        break;
                    }

                    returnData[x, y] = tempIndex;
                }

            SetDataGrid(returnData);
            openedBitmap.Dispose();
            SetUpDisplay();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void ExportIcon(object sender, EventArgs arguments)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save icon",
                Filter = "Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpeg;*.jpg)|*.jpeg;*.jpg|PNG (*.png)|*.png"
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            switch (saveFileDialog.FilterIndex)
            {
            case 2:
                _iconBitmap[_selectedIcon].Save(saveFileDialog.FileName, ImageFormat.Gif);
                break;
            case 3:
                _iconBitmap[_selectedIcon].Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                break;
            case 4:
                _iconBitmap[_selectedIcon].Save(saveFileDialog.FileName, ImageFormat.Png);
                break;
            default:
                _iconBitmap[_selectedIcon].Save(saveFileDialog.FileName, ImageFormat.Bmp);
                break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void HorizontalFlip(object sender, EventArgs arguments)
        {
            var iconData = GetDataGrid();
            var processedData = new byte[16, 16];

            for (var yPixel = 0; yPixel < 16; yPixel++)
            {
                for (var xPixel = 0; xPixel < 16; xPixel++)
                {
                    processedData[xPixel, yPixel] = iconData[15 - xPixel, yPixel];
                }
            }

            SetDataGrid(processedData);
            DrawIcon();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void VerticalFlip(object sender, EventArgs arguments)
        {
            var iconData = GetDataGrid();
            var processedData = new byte[16, 16];

            for (var y = 0; y < 16; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    processedData[x, y] = iconData[x, 15 - y];
                }
            }

            SetDataGrid(processedData);
            DrawIcon();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void LeftRotate(object sender, EventArgs arguments)
        {
            var iconData = GetDataGrid();
            var processedData = new byte[16, 16];

            for (var y = 0; y < 16; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    processedData[x, y] = iconData[y, x];
                }
            }

            SetDataGrid(processedData);
            VerticalFlip(sender, arguments);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void RightRotate(object sender, EventArgs arguments)
        {
            var iconData = GetDataGrid();
            var processedData = new byte[16, 16];

            for (var y = 0; y < 16; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    processedData[x, y] = iconData[y, x];
                }
            }

            SetDataGrid(processedData);
            HorizontalFlip(sender, arguments);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private byte[,] GetDataGrid()
        {
            var returnData = new byte[16, 16];
            var byteIndex = 32 + _selectedIcon * 128;

            for (var y = 0; y < 16; y++)
            {
                for (var x = 0; x < 16; x += 2)
                {
                    returnData[x, y] = (byte)(IconData[byteIndex] & 0x0F);
                    returnData[x + 1, y] = (byte)((IconData[byteIndex++] & 0xF0) >> 4);
                }
            }

            return returnData;
        }

        /// <summary>
        /// </summary>
        /// <param name="gridData"></param>
        private void SetDataGrid(byte[,] gridData)
        {
            var byteIndex = 32 + _selectedIcon * 128;

            for (var y = 0; y < 16; y++)
            {
                for (var x = 0; x < 16; x += 2)
                {
                    IconData[byteIndex++] = (byte)(gridData[x, y] | (gridData[x + 1, y] << 4));
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paletteRender_MouseDown(object sender, MouseEventArgs e)
        {
            var xPosition = e.X / 15;
            var yPosition = e.Y / 15;

            if (xPosition > 7) xPosition = 7;
            if (yPosition > 1) yPosition = 1;

            if (e.Button == MouseButtons.Left)
                SetSelectedColor(xPosition + yPosition * 8, 0);
            else
                SetSelectedColor(xPosition + yPosition * 8, 1);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void PaletteRender_DoubleClick(object sender, EventArgs arguments)
        {
            var selectedColorIndex = 1;

            if (((MouseEventArgs)arguments).Button == MouseButtons.Left)
            {
                selectedColorIndex = 0;
            }

            var colorDialog = new ColorDialog
            {
                Color = _iconPalette[_selectedColor[0]],
                FullOpen = true
            };

            if (colorDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var redChannel = colorDialog.Color.R >> 3;
            var greenChannel = colorDialog.Color.G >> 3;
            var blueChannel = colorDialog.Color.B >> 3;

            IconData[_selectedColor[selectedColorIndex] * 2] = (byte)(redChannel | ((greenChannel & 0x07) << 5));
            IconData[_selectedColor[selectedColorIndex] * 2 + 1] = (byte)((blueChannel << 2) | ((greenChannel & 0x18) >> 3));
            DrawPalette();
            SetSelectedColor(_selectedColor[0], 0);
            SetSelectedColor(_selectedColor[1], 1);
            DrawIcon();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void FrameCombo_SelectedIndexChanged(object sender, EventArgs arguments)
        {
            _selectedIcon = _frameCombo.SelectedIndex;
            DrawIcon();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void IconRender_MouseDownMove(object sender, MouseEventArgs arguments)
        {
            var xPositionOriginal = arguments.X / 11;
            var yposOriginal = arguments.Y / 11;
            var xPosition = arguments.X / 11;
            var yPosition = arguments.Y / 11;

            if (xPosition > 15)
            {
                xPosition = 15;
            }

            if (yPosition > 15)
            {
                yPosition = 15;
            }

            if (xPosition < 0)
            {
                xPosition = 0;
            }

            if (yPosition < 0)
            {
                yPosition = 0;
            }

            _xLabel.Text = "X: " + xPosition;
            _yLabel.Text = "Y: " + yPosition;

            if (xPositionOriginal < 0 || xPositionOriginal > 15 || yposOriginal < 0 || yposOriginal > 15)
            {
                return;
            }

            if (arguments.Button == MouseButtons.Left)
            {
                PutPixel(xPosition, yPosition, 0);
            }

            if (arguments.Button == MouseButtons.Right)
            {
                PutPixel(xPosition, yPosition, 1);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void IconRender_MouseLeave(object sender, EventArgs arguments)
        {
            _xLabel.Text = "X:";
            _yLabel.Text = "Y:";
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void OkButton_Click(object sender, EventArgs arguments)
        {
            OkPressed = true;
            Close();
        }
    }
}