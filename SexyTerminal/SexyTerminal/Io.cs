using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    internal class Io
    {
        public static Scheme ReadPreset(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                return new Scheme(reader);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Scheme[] ReadPresets(string fileName)
        {
            var presets = new Scheme[16];

            using (var stream = File.OpenRead(fileName))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                for (var index = 0; index < presets.Length; index++)
                {
                    presets[index] = new Scheme(reader);
                }
            }

            return presets;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scheme"></param>
        public static void WritePreset(string fileName, Scheme scheme)
        {
            using (var stream = File.OpenWrite(fileName))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                scheme.Write(writer);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="presets"></param>
        public static void WritePresets(string fileName, IReadOnlyList<Scheme> presets)
        {
            using (var stream = File.OpenWrite(fileName))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                foreach (var preset in presets)
                {
                    preset.Write(writer);
                }
            }
        }
    }
}