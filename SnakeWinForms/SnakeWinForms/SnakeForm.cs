using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeWinForms
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal partial class SnakeForm : Form
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public SnakeForm()
        {
            InitializeComponent();
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.RowCount = SnakeConfig.RowCount;
            tableLayoutPanel1.ColumnCount = SnakeConfig.ColumnCount;
            ClientSize = new Size(SnakeConfig.ColumnCount * SnakeConfig.CellSize, SnakeConfig.CellSize + SnakeConfig.RowCount * SnakeConfig.CellSize);

            var gameBoard = new SnakeController(tableLayoutPanel1);

            timer1.Tick += (sender, e) =>
            {
                if (gameBoard.GameOver)
                {
                    timer1.Stop();
                    MessageBox.Show("GAME OVER!");
                }
                else
                {
                    label2.Text = $@"{gameBoard.Tick():D4}";
                }
            };

            KeyDown += (sender, eventArgs) => gameBoard.Move(eventArgs.KeyCode);
            timer1.Start();
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