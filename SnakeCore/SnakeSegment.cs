using System.Windows.Forms;

namespace SnakeCore
{
    /// <summary>
    /// </summary>
    internal class SnakeSegment
    {
        /// <summary>
        /// </summary>
        /// <param name="button"></param>
        /// <param name="position"></param>
        public SnakeSegment(Control button, Position2D position)
        {
            Button = button;
            Position = position;
            button.BackColor = SnakeConfig.SnakeHeadColor;
        }

        /// <summary>
        /// </summary>
        private Control Button { get; }

        /// <summary>
        /// </summary>
        public Position2D Position { get; }

        /// <summary>
        /// </summary>
        public void Reset()
        {
            Button.BackColor = SnakeConfig.BlankColor;
        }

        /// <summary>
        /// </summary>
        /// <param name="direction"></param>
        public void Step(Direction direction)
        {
            Position.Step(direction);
            Button.BackColor = SnakeConfig.SnakeNormalColor;
        }
    }
}