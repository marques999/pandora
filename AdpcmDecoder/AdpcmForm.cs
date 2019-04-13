using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AdpcmDecoder
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal sealed class AdpcmForm : Form
    {
        /// <summary>
        /// </summary>
        private const string ApplicationTitle = "AdpcmDecoder";

        /// <summary>
        /// </summary>
        private readonly Button _buttonDecode;

        /// <summary>
        /// </summary>
        private readonly AdpcmDecoder _decoder = new AdpcmDecoder();

        /// <summary>
        /// </summary>
        private readonly Label _labelFilename;

        /// <summary>
        /// </summary>
        private readonly Label _labelLength;

        /// <summary>
        /// </summary>
        private readonly RadioButton _radioAdpcmA;

        /// <summary>
        /// </summary>
        private readonly TextBox _textFrequency;

        /// <summary>
        /// </summary>
        private byte[] _adpcmData;

        /// <summary>
        /// </summary>
        private string _fileName;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public AdpcmForm()
        {
            var buttonBrowse = new Button
            {
                Location = new Point(12, 12),
                Name = "buttonBrowse",
                Size = new Size(136, 26),
                TabIndex = 0,
                Text = "Browse...",
                UseVisualStyleBackColor = true
            };

            var labelFrequency = new Label
            {
                AutoSize = true,
                Location = new Point(160, 44),
                Name = "labelFrequency",
                Size = new Size(20, 13),
                TabIndex = 11,
                Text = "Hz"
            };

            _buttonDecode = new Button
            {
                Enabled = false,
                Location = new Point(308, 82),
                Name = "buttonDecode",
                Size = new Size(123, 33),
                TabIndex = 20,
                Text = "Decode",
                UseVisualStyleBackColor = true
            };

            _labelFilename = new Label
            {
                AutoSize = true,
                Location = new Point(154, 12),
                Name = "labelFilename",
                Size = new Size(26, 13),
                TabIndex = 2,
                Text = "File:"
            };

            _labelLength = new Label
            {
                AutoSize = true,
                Location = new Point(154, 25),
                Name = "labelLength",
                Size = new Size(30, 13),
                TabIndex = 3,
                Text = "Length:"
            };

            _textFrequency = new TextBox
            {
                Enabled = false,
                Location = new Point(85, 41),
                Name = "textFrequency",
                Size = new Size(69, 20),
                TabIndex = 10
            };

            _radioAdpcmA = new RadioButton
            {
                AutoSize = true,
                Checked = true,
                Location = new Point(6, 19),
                Name = "radioAdpcmA",
                Size = new Size(128, 17),
                TabIndex = 8,
                TabStop = true,
                Text = "ADPCM-A (18500 Hz)",
                UseVisualStyleBackColor = true
            };

            var radioAdpcmB = new RadioButton
            {
                AutoSize = true,
                Location = new Point(6, 42),
                Name = "radioAdpcmB",
                Size = new Size(79, 17),
                TabIndex = 9,
                Text = "ADPCM-B",
                UseVisualStyleBackColor = true
            };

            var groupFormat = new GroupBox
            {
                Location = new Point(12, 44),
                Name = "groupFormat",
                Size = new Size(198, 71),
                TabIndex = 7,
                TabStop = false,
                Text = "Format"
            };

            groupFormat.SuspendLayout();
            groupFormat.Controls.Add(labelFrequency);
            groupFormat.Controls.Add(_textFrequency);
            groupFormat.Controls.Add(radioAdpcmB);
            groupFormat.Controls.Add(_radioAdpcmA);
            _buttonDecode.Click += ButtonDecodeClick;
            buttonBrowse.Click += ButtonOpenFile_Click;
            radioAdpcmB.CheckedChanged += (sender, arguments) => _textFrequency.Enabled = radioAdpcmB.Checked;
            SuspendLayout();
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(443, 127);
            Controls.Add(groupFormat);
            Controls.Add(_labelLength);
            Controls.Add(_labelFilename);
            Controls.Add(_buttonDecode);
            Controls.Add(buttonBrowse);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = ApplicationTitle;
            Text = ApplicationTitle;
            groupFormat.ResumeLayout(false);
            groupFormat.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void ButtonOpenFile_Click(object sender, EventArgs arguments)
        {
            var openFile = new OpenFileDialog();

            try
            {
                openFile.ShowDialog();
                _fileName = openFile.FileName;
                _adpcmData = File.ReadAllBytes(_fileName);
                _buttonDecode.Enabled = true;
                _labelFilename.Text = string.Concat("File: ", _fileName);
                _labelLength.Text = string.Concat("Length: ", _adpcmData.Length.ToString(), " bytes");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="frequency"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        private bool ToWav(IReadOnlyCollection<short> buffer, int frequency = 18500, int shift = 4)
        {
            try
            {
                using (var stream = new BinaryWriter(File.Open($"{_fileName}.wav", FileMode.OpenOrCreate)))
                {
                    stream.Write(Encoding.ASCII.GetBytes("RIFF"));
                    stream.Write(0x24 + buffer.Count * 2);
                    stream.Write(Encoding.ASCII.GetBytes("WAVE"));
                    stream.Write(Encoding.ASCII.GetBytes("fmt "));
                    stream.Write(0x10);
                    stream.Write((short)0x01);
                    stream.Write((short)0x01);
                    stream.Write(frequency);
                    stream.Write(frequency << 1);
                    stream.Write((short)0x02);
                    stream.Write((short)0x10);
                    stream.Write(Encoding.ASCII.GetBytes("data"));
                    stream.Write(buffer.Count << 1);

                    foreach (var value in buffer)
                    {
                        stream.Write((short)(value << (shift & 0x1F)));
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void ButtonDecodeClick(object sender, EventArgs arguments)
        {
            _buttonDecode.Enabled = false;

            if (_radioAdpcmA.Checked)
            {
                DisplayFinished(ToWav(_decoder.ConvertAdpcmA(_adpcmData)));
            }
            else
            {
                var adpcmFrequency = int.Parse(_textFrequency.Text);
                var buffer = _decoder.ConvertAdpcmB(_adpcmData, adpcmFrequency);

                if (buffer == null)
                {
                    MessageBox.Show("Please enter a valid frequency (1800 ~ 55500 Hz)", ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    _buttonDecode.Enabled = true;
                    _textFrequency.Focus();
                }
                else
                {
                    DisplayFinished(ToWav(buffer, adpcmFrequency, 0));
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="result"></param>
        private void DisplayFinished(bool result)
        {
            if (result)
            {
                MessageBox.Show($"Finished!\n{_fileName}.wav", ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            _buttonDecode.Enabled = true;
        }

        /// <summary>
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AdpcmForm());
        }
    }
}