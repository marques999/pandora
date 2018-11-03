using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeWinForms
{
    /// <summary>
    /// </summary>
    internal class Snake
    {
        /// <summary>
        /// </summary>
        private readonly Queue<SnakeSegment> _body = new Queue<SnakeSegment>();

        /// <summary>
        /// </summary>
        private readonly Random _randomGenerator = new Random();

        /// <summary>
        /// </summary>
        private readonly SnakeController _snakeController;

        /// <summary>
        /// </summary>
        private SnakeSegment _current;

        /// <summary>
        /// </summary>
        private DirectionEnum _direction = DirectionEnum.Left;

        /// <summary>
        /// </summary>
        /// <param name="snakeController"></param>
        public Snake(SnakeController snakeController)
        {
            _snakeController = snakeController;
            _current = new SnakeSegment(_snakeController.CellAt(new Position2D(4, 5)), new Position2D(4, 5));
            _body.Enqueue(_current);
        }

        /// <summary>
        /// </summary>
        public Position2D Position => _current.Position;

        /// <summary>
        /// </summary>
        private void CollisionHorizontal()
        {
            switch (Position.X)
            {
            case 0:
                _direction = DirectionEnum.Down;
                break;
            case SnakeConfig.RowCount - 1:
                _direction = DirectionEnum.Up;
                break;
            default:
                _direction = _randomGenerator.Next(0, 1) > 0 ? DirectionEnum.Up : DirectionEnum.Down;
                break;
            }
        }

        /// <summary>
        /// </summary>
        private void CollisionVertical()
        {
            switch (_current.Position.Y)
            {
            case 0:
                _direction = DirectionEnum.Right;
                break;
            case SnakeConfig.ColumnCount - 1:
                _direction = DirectionEnum.Left;
                break;
            default:
                _direction = _randomGenerator.Next(0, 1) > 0 ? DirectionEnum.Left : DirectionEnum.Right;
                break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="direction"></param>
        public void Move(DirectionEnum direction)
        {
            if (direction != _direction && ((int)direction % 2 == 0 && (int)_direction % 2 != 0) | ((int)direction % 2 != 0 && (int)_direction % 2 == 0))
            {
                _direction = direction;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="removeSegment"></param>
        /// <returns></returns>
        public Position2D Tick(bool removeSegment)
        {
            if (_direction == DirectionEnum.Down)
            {
                if (Position.X == SnakeConfig.RowCount - 1)
                {
                    CollisionVertical();
                }
            }
            else if (_direction == DirectionEnum.Left)
            {
                if (Position.Y == 0)
                {
                    CollisionHorizontal();
                }
            }
            else if (_direction == DirectionEnum.Right)
            {
                if (Position.Y == SnakeConfig.ColumnCount - 1)
                {
                    CollisionHorizontal();
                }
            }
            else if (Position.X == 0)
            {
                CollisionVertical();
            }

            _current.Step(_direction);

            if (_body.GroupBy(x => x.Position).Any(g => g.Count() > 1))
            {
                return null;
            }

            if (removeSegment && _body.Count > 4)
            {
                _body.Dequeue().Reset();
            }

            _current = new SnakeSegment(_snakeController.CellAt(Position), new Position2D(Position));
            _body.Enqueue(_current);

            return Position;
        }
    }
}