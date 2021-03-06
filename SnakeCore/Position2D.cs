﻿using System;

namespace SnakeCore
{
    /// <summary>
    /// </summary>
    [Serializable]
    internal class Position2D
    {
        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Position2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        public Position2D(Position2D other)
        {
            X = other.X;
            Y = other.Y;
        }

        /// <summary>
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            return other is Position2D point && point.X == X && point.Y == Y;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return X ^ Y;
        }

        /// <summary>
        /// </summary>
        /// <param name="direction"></param>
        public void Step(Direction direction)
        {
            switch (direction)
            {
            case Direction.Down:
                X++;
                break;
            case Direction.Left:
                Y--;
                break;
            case Direction.Right:
                Y++;
                break;
            default:
                X--;
                break;
            }
        }
    }
}