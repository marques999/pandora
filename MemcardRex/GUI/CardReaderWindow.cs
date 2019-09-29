using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using MemcardRex.Hardware;

namespace MemcardRex.GUI
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class CardReaderWindow : Form
    {
        /// <summary>
        /// </summary>
        private CardReader _device;

        /// <summary>
        /// </summary>
        private int _maximumFrames;

        /// <summary>
        /// </summary>
        private byte[] _completeMemoryCard = new byte[131072];

        /// <summary>
        /// </summary>
        private bool _successfulRead;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public CardReaderWindow()
        {
            var abortButton = new Button
            {
                Location = new Point(228, 52),
                Name = "abortButton",
                Size = new Size(76, 24),
                TabIndex = 0,
                Text = "Abort",
                UseVisualStyleBackColor = true
            };

            _mainProgressBar = new ProgressBar
            {
                Location = new Point(4, 24),
                Maximum = 1024,
                Name = "mainProgressBar",
                Size = new Size(300, 16),
                TabIndex = 5
            };

            _backgroundReader = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _infoLabel = new Label
            {
                AutoSize = true,
                Location = new Point(4, 5),
                Name = "infoLabel",
                Size = new Size(106, 13),
                TabIndex = 6,
                Text = "infoLabelPlaceholder",
                TextAlign = ContentAlignment.MiddleLeft
            };

            var spacerLabel = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(4, 46),
                Name = "spacerLabel",
                Size = new Size(300, 2),
                TabIndex = 9
            };

            _backgroundWriter = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            abortButton.Click += OKbutton_Click;
            _backgroundReader.DoWork += BackgroundReader_DoWork;
            _backgroundWriter.DoWork += BackgroundWriter_DoWork;
            _backgroundReader.RunWorkerCompleted += (sender, arguments) => Close();
            _backgroundWriter.RunWorkerCompleted += (sender, arguments) => Close();
            _backgroundReader.ProgressChanged += (sender, arguments) => _mainProgressBar.Value = arguments.ProgressPercentage;
            _backgroundWriter.ProgressChanged += (sender, arguments) => _mainProgressBar.Value = arguments.ProgressPercentage;

            SuspendLayout();
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(308, 81);
            ControlBox = false;
            Controls.Add(spacerLabel);
            Controls.Add(_infoLabel);
            Controls.Add(_mainProgressBar);
            Controls.Add(abortButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CardReaderWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Card Reader";
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// </summary>
        private readonly ProgressBar _mainProgressBar;

        /// <summary>
        /// </summary>
        private readonly BackgroundWorker _backgroundReader;

        /// <summary>
        /// </summary>
        private readonly Label _infoLabel;

        /// <summary>
        /// </summary>
        private readonly BackgroundWorker _backgroundWriter;

        /// <summary>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="applicationName"></param>
        /// <param name="comPort"></param>
        /// <returns></returns>
        public byte[] ReadMemoryCardDexDrive(Form parent, string applicationName, string comPort)
        {
            try
            {
                _device = new DexDrive(comPort);
                _mainProgressBar.Maximum = 1024;
                Text = "DexDrive Communication";
                _infoLabel.Text = $"Reading data from DexDrive (ver. {_device.FirmwareVersion})...";
                _backgroundReader.RunWorkerAsync();
                ShowDialog(parent);
                _device.Stop();

                if (_successfulRead)
                {
                    return _completeMemoryCard;
                }
            }
            catch (Exception exception)
            {
                Messages.Warning(applicationName, exception.Message);
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="applicationName"></param>
        /// <param name="comPort"></param>
        /// <returns></returns>
        public byte[] ReadMemoryCardCarDuino(Form parent, string applicationName, string comPort)
        {
            try
            {
                _device = new MemCarduino(comPort);
                _mainProgressBar.Maximum = 1024;
                Text = "MemCARDuino Communication";
                _infoLabel.Text = $"Reading data from MemCARDuino (ver. {_device.FirmwareVersion})...";
                _backgroundReader.RunWorkerAsync();
                ShowDialog(parent);
                _device.Stop();

                if (_successfulRead)
                {
                    return _completeMemoryCard;
                }
            }
            catch (Exception exception)
            {
                Messages.Warning(applicationName, exception.Message);
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="applicationName"></param>
        /// <param name="comPort"></param>
        /// <returns></returns>
        public byte[] ReadMemoryCardPs1CLnk(Form parent, string applicationName, string comPort)
        {
            try
            {
                _device = new Ps1CardLink(comPort);
                _mainProgressBar.Maximum = 1024;
                Text = "PS1CardLink Communication";
                _infoLabel.Text = $"Reading data from PS1CardLink (ver. {_device.FirmwareVersion})...";
                _backgroundReader.RunWorkerAsync();
                ShowDialog(parent);
                _device.Stop();

                if (_successfulRead)
                {
                    return _completeMemoryCard;
                }
            }
            catch (Exception exception)
            {
                Messages.Warning(applicationName, exception.Message);
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="applicationName"></param>
        /// <param name="comPort"></param>
        /// <param name="memoryCardData"></param>
        /// <param name="frameNumber"></param>
        public void WriteMemoryCardDexDrive(Form parent, string applicationName, string comPort, byte[] memoryCardData, int frameNumber)
        {
            try
            {
                _device = new DexDrive(comPort);
                _maximumFrames = frameNumber;
                _mainProgressBar.Maximum = frameNumber;
                Text = "DexDrive Communication";
                _infoLabel.Text = $"Writing data to DexDrive (ver. {_device.FirmwareVersion})...";
                _completeMemoryCard = memoryCardData;
                _backgroundWriter.RunWorkerAsync();
                ShowDialog(parent);
            }
            catch (Exception exception)
            {
                Messages.Warning(applicationName, exception.Message);
            }
            finally
            {
                _device?.Stop();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="applicationName"></param>
        /// <param name="comPort"></param>
        /// <param name="memoryCardData"></param>
        /// <param name="frameNumber"></param>
        public void WriteMemoryCardCarDuino(Form parent, string applicationName, string comPort, byte[] memoryCardData, int frameNumber)
        {
            try
            {
                _device = new MemCarduino(comPort);
                _maximumFrames = frameNumber;
                _mainProgressBar.Maximum = frameNumber;
                Text = "MemCARDuino Communication";
                _infoLabel.Text = $"Writing data to MemCARDuino (ver. {_device.FirmwareVersion})...";
                _completeMemoryCard = memoryCardData;
                _backgroundWriter.RunWorkerAsync();
                ShowDialog(parent);
            }
            catch (Exception exception)
            {
                Messages.Warning(applicationName, exception.Message);
            }
            finally
            {
                _device?.Stop();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="applicationName"></param>
        /// <param name="comPort"></param>
        /// <param name="memoryCardData"></param>
        /// <param name="frameNumber"></param>
        public void WriteMemoryCardPs1CLnk(Form parent, string applicationName, string comPort, byte[] memoryCardData, int frameNumber)
        {
            try
            {
                _device = new Ps1CardLink(comPort);
                _maximumFrames = frameNumber;
                _mainProgressBar.Maximum = frameNumber;
                Text = "PS1CardLink Communication";
                _infoLabel.Text = $"Writing data to PS1CardLink (ver. {_device.FirmwareVersion})...";
                _completeMemoryCard = memoryCardData;
                _backgroundWriter.RunWorkerAsync();
                ShowDialog(parent);
            }
            catch (Exception exception)
            {
                Messages.Warning(applicationName, exception.Message);
            }
            finally
            {
                _device?.Stop();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void OKbutton_Click(object sender, EventArgs arguments)
        {
            if (_backgroundReader.IsBusy)
            {
                _backgroundReader.CancelAsync();
            }

            if (_backgroundWriter.IsBusy)
            {
                _backgroundWriter.CancelAsync();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void BackgroundReader_DoWork(object sender, DoWorkEventArgs arguments)
        {
            ushort offset = 0;

            while (offset < 1024)
            {
                if (_backgroundReader.CancellationPending || _device == null)
                {
                    return;
                }

                var cardFrame = _device.ReadMemoryCardFrame(offset);

                if (cardFrame == null)
                {
                    continue;
                }

                Array.Copy(cardFrame, 0, _completeMemoryCard, offset * 128, 128);
                _backgroundReader.ReportProgress(offset);
                offset++;
            }

            _successfulRead = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void BackgroundWriter_DoWork(object sender, DoWorkEventArgs arguments)
        {
            var buffer = new byte[128];
            ushort current = 0;

            while (current < _maximumFrames)
            {
                if (_backgroundWriter.CancellationPending || _device == null)
                {
                    return;
                }

                Array.Copy(_completeMemoryCard, current * 128, buffer, 0, 128);

                if (_device.WriteMemoryCardFrame(current, buffer))
                {
                    _backgroundWriter.ReportProgress(current++);
                }
            }

            _successfulRead = true;
        }
    }
}