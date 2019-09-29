using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeCore
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal sealed class SnakeForm : Form
    {
        /// <summary>
        /// </summary>
        private readonly Timer _timer = new Timer
        {
            Interval = 150
        };

        /// <summary>
        /// </summary>
        private readonly SnakeController _snakeController;

        /// <summary>
        /// </summary>
        private readonly Label _label2 = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Right,
            Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point, 0),
            ForeColor = Color.LawnGreen,
            Margin = new Padding(0),
            Padding = new Padding(4, 6, 4, 4),
            Size = new Size(53, 29),
            Text = "0000",
            TextAlign = ContentAlignment.MiddleRight
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public SnakeForm()
        {
            var topLayout = new Panel
            {
                Dock = DockStyle.Top,
                Margin = new Padding(4),
                Size = new Size(284, 32)
            };

            var bottomLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = SnakeConfig.RowCount,
                ColumnCount = SnakeConfig.ColumnCount
            };

            SuspendLayout();
            topLayout.SuspendLayout();
            topLayout.Controls.Add(_label2);

            topLayout.Controls.Add(new Label
            {
                AutoSize = true,
                Dock = DockStyle.Left,
                Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.White,
                Location = new Point(0, 0),
                Margin = new Padding(0),
                Padding = new Padding(4, 6, 4, 4),
                Size = new Size(62, 29),
                Text = "SCORE",
                TextAlign = ContentAlignment.MiddleLeft
            });

            BackColor = Color.Black;
            ClientSize = new Size(284, 261);
            Controls.Add(bottomLayout);
            Controls.Add(topLayout);
            Text = "SNAAAAAKE";
            _snakeController = new SnakeController(bottomLayout);
            ClientSize = new Size(SnakeConfig.ColumnCount * SnakeConfig.CellSize, SnakeConfig.CellSize + SnakeConfig.RowCount * SnakeConfig.CellSize);
            KeyDown += (sender, eventArgs) => _snakeController.Move(eventArgs.KeyCode);
            _timer.Tick += Tick;
            _timer.Start();
            topLayout.ResumeLayout(false);
            topLayout.PerformLayout();
            ResumeLayout(false);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private void Tick(object sender, EventArgs arguments)
        {
            if (_snakeController.GameOver)
            {
                _timer.Stop();
                MessageBox.Show("GAME OVER!");
            }
            else
            {
                _label2.Text = $@"{_snakeController.Tick():D4}";
            }
        }

        /// <summary>
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SnakeForm());
        }
    }
}