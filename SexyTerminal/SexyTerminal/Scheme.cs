using System;
using System.Drawing;
using System.IO;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    [Serializable]
    internal class Scheme
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public Scheme(string name)
        {
            Name = name;
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        public Scheme(BinaryReader reader)
        {
            Name = reader.ReadString();
            Background = Color.FromArgb(reader.ReadInt32());
            Foreground = Color.FromArgb(reader.ReadInt32());

            for (var index = 0; index < Colors.Length; index++)
            {
                Colors[index] = Color.FromArgb(reader.ReadInt32());
            }
        }

        /// <summary>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// </summary>
        public Color Foreground { get; set; }

        /// <summary>
        /// </summary>
        public Color[] Colors { get; set; } = new Color[16];

        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Background.ToArgb());
            writer.Write(Foreground.ToArgb());

            foreach (var color in Colors)
            {
                writer.Write(color.ToArgb());
            }
        }
    }
}