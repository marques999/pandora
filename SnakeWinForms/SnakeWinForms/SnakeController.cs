using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SnakeWinForms
{
    /// <summary>
    /// </summary>
    internal class SnakeController
    {
        /// <summary>
        /// </summary>
        private readonly List<Button> _matrix = new List<Button>();

        /// <summary>
        /// </summary>
        private readonly Random _randomGenerator = new Random();

        /// <summary>
        /// </summary>
        private readonly Snake _snake;

        /// <summary>
        /// </summary>
        private Button _activeBonus;

        /// <summary>
        /// </summary>
        private Position2D _bonusPosition;

        /// <summary>
        /// </summary>
        private int _score;

        /// <summary>
        /// </summary>
        public SnakeController(TableLayoutPanel drawingPanel)
        {
            for (var i = 0; i < SnakeConfig.RowCount; i++)
            {
                drawingPanel.RowStyles.Add(new RowStyle(SizeType.Percent, SnakeConfig.RowPercent));

                for (var j = 0; j < SnakeConfig.ColumnCount; j++)
                {
                    drawingPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, SnakeConfig.ColumnPercent));

                    var buttonX = new Button
                    {
                        Enabled = false,
                        BackColor = SnakeConfig.BlankColor,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(1),
                        Name = $"button{i + 1}"
                    };

                    _matrix.Add(buttonX);
                    drawingPanel.Controls.Add(buttonX, j, i);
                }
            }

            _snake = new Snake(this);
        }

        /// <summary>
        /// </summary>
        public bool GameOver { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="position2D"></param>
        /// <returns></returns>
        public Button CellAt(Position2D position2D)
        {
            return _matrix[position2D.X * SnakeConfig.ColumnCount + position2D.Y];
        }

        /// <summary>
        /// </summary>
        public int Tick()
        {
            if (_activeBonus == null)
            {
                var position = new Position2D(
                    _randomGenerator.Next(0, SnakeConfig.RowCount - 1),
                    _randomGenerator.Next(0, SnakeConfig.ColumnCount - 1)
                );

                while (CellAt(position).BackColor != SnakeConfig.BlankColor)
                {
                    position = new Position2D(
                        _randomGenerator.Next(0, SnakeConfig.RowCount - 1),
                        _randomGenerator.Next(0, SnakeConfig.ColumnCount - 1)
                    );
                }

                _bonusPosition = position;
                _activeBonus = CellAt(_bonusPosition);
                _activeBonus.BackColor = SnakeConfig.AppleColor;
            }

            if (_snake.Position.Equals(_bonusPosition))
            {
                if (_snake.Tick(false) == null)
                {
                    GameOver = true;
                }
                else
                {
                    _activeBonus.BackColor = SnakeConfig.SnakeNormalColor;
                    _activeBonus = null;
                    _score += 100;
                }
            }
            else if (_snake.Tick(true) == null)
            {
                GameOver = true;
            }
            else
            {
                _score++;
            }

            return _score;
        }

        /// <summary>
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Keys direction)
        {
            switch (direction)
            {
            case Keys.Up:
                _snake.Move(DirectionEnum.Up);
                break;
            case Keys.Down:
                _snake.Move(DirectionEnum.Down);
                break;
            case Keys.Left:
                _snake.Move(DirectionEnum.Left);
                break;
            case Keys.Right:
                _snake.Move(DirectionEnum.Right);
                break;
            }
        }
    }
}