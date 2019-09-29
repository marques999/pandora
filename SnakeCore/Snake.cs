using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeCore
{
    /// <summary>
    /// </summary>
    internal class Snake
    {
        /// <summary>
        /// </summary>
        private SnakeSegment _segment;

        /// <summary>
        /// </summary>
        private Direction _direction = Direction.Left;
        
        /// <summary>
        /// </summary>
        public Position2D Position => _segment.Position;

        /// <summary>
        /// </summary>
        private readonly SnakeController _snakeController;

        /// <summary>
        /// </summary>
        private readonly Random _randomGenerator = new Random();

        /// <summary>
        /// </summary>
        private readonly Queue<SnakeSegment> _body = new Queue<SnakeSegment>();

        /// <summary>
        /// </summary>
        /// <param name="snakeController"></param>
        public Snake(SnakeController snakeController)
        {
            _snakeController = snakeController;
            _segment = new SnakeSegment(_snakeController.CellAt(new Position2D(4, 5)), new Position2D(4, 5));
            _body.Enqueue(_segment);
        }

        /// <summary>
        /// </summary>
        private void CollisionHorizontal()
        {
            switch (Position.X)
            {
            case 0:
                _direction = Direction.Down;
                break;
            case SnakeConfig.RowCount - 1:
                _direction = Direction.Up;
                break;
            default:
                _direction = _randomGenerator.Next(0, 1) > 0 ? Direction.Up : Direction.Down;
                break;
            }
        }

        /// <summary>
        /// </summary>
        private void CollisionVertical()
        {
            switch (_segment.Position.Y)
            {
            case 0:
                _direction = Direction.Right;
                break;
            case SnakeConfig.ColumnCount - 1:
                _direction = Direction.Left;
                break;
            default:
                _direction = _randomGenerator.Next(0, 1) > 0 ? Direction.Left : Direction.Right;
                break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Direction direction)
        {
            if (direction != _direction && ((int)direction % 2 == 0 && (int)_direction % 2 != 0) | ((int)direction % 2 != 0 && (int)_direction % 2 == 0))
            {
                _direction = direction;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="remove"></param>
        /// <returns></returns>
        public Position2D Tick(bool remove)
        {
            switch (_direction)
            {
            case Direction.Down:

                if (Position.X == SnakeConfig.RowCount - 1)
                {
                    CollisionVertical();
                }

                break;

            case Direction.Left:

                if (Position.Y == 0)
                {
                    CollisionHorizontal();
                }

                break;

            case Direction.Right:

                if (Position.Y == SnakeConfig.ColumnCount - 1)
                {
                    CollisionHorizontal();
                }

                break;

            default:

                if (Position.X == 0)
                {
                    CollisionVertical();
                }

                break;
            }

            _segment.Step(_direction);

            if (_body.GroupBy(segment => segment.Position).Any(grouping => grouping.Count() > 1))
            {
                return null;
            }

            if (remove && _body.Count > 4)
            {
                _body.Dequeue().Reset();
            }

            _segment = new SnakeSegment(_snakeController.CellAt(Position), new Position2D(Position));
            _body.Enqueue(_segment);

            return Position;
        }
    }
}