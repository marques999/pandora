using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CriPakTools
{
    /// <summary>
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="arguments"></param>
        private static void Main(string[] arguments)
        {
            switch (arguments.Length)
            {
            case 0:
                Console.WriteLine("CriPakTools.exe <filename> - Displays all contained chunks.");
                Console.WriteLine("CriPakTools.exe <filename> EXTRACT_ME - Extracts a file.");
                Console.WriteLine("CriPakTools.exe <filename> ALL - Extracts all files.");
                Console.ReadKey();
                break;
            case 2:
                ExtractFiles(arguments);
                break;
            default:
                DisplayAllChunks(arguments);
                break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="arguments"></param>
        private static void ExtractFiles(IList<string> arguments)
        {
            var cpkFile = new Cpk(arguments[0]);
            var oldFile = new BinaryReader(File.OpenRead(arguments[0]));

            List<FileEntry> entries;

            if (arguments[1].ToUpper() == "ALL")
            {
                entries = cpkFile.FileTable.Where(fileEntry => fileEntry.FileType == "FILE").ToList();
            }
            else
            {
                entries = cpkFile.FileTable.Where(x => (x.DirectoryName != null ? x.DirectoryName + "/" : "") + x.FileName.ToString().ToLower() == arguments[1].ToLower()).ToList();
            }

            if (entries.Count == 0)
            {
                throw new FileNotFoundException(arguments[1]);
            }

            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty((string)entry.DirectoryName) == false)
                {
                    Directory.CreateDirectory(entry.DirectoryName.ToString());
                }

                oldFile.BaseStream.Seek((long)entry.FileOffset, SeekOrigin.Begin);

                var isCompressed = Encoding.ASCII.GetString(oldFile.ReadBytes(8));

                oldFile.BaseStream.Seek((long)entry.FileOffset, SeekOrigin.Begin);

                var chunk = oldFile.ReadBytes(int.Parse(entry.FileSize.ToString()));

                if (isCompressed == "CRILAYLA")
                {
                    chunk = cpkFile.DecompressCrilayla(chunk, int.Parse((entry.ExtractSize ?? entry.FileSize).ToString()));
                }

                var directoryName = string.Empty;

                if (entry.DirectoryName != null)
                {
                    directoryName = entry.DirectoryName + "/";
                }

                Console.WriteLine($"Extracting: {directoryName}{entry.FileName}");
                File.WriteAllBytes($"{directoryName}{entry.FileName}", chunk);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="arguments"></param>
        private static void DisplayAllChunks(IList<string> arguments)
        {
            foreach (var entry in new Cpk(arguments[0]).FileTable.OrderBy(fileEntry => fileEntry.FileOffset).ToList())
            {
                var directoryName = string.Empty;

                if (entry.DirectoryName != null)
                {
                    directoryName = entry.DirectoryName + "/";
                }

                Console.WriteLine(directoryName + entry.FileName);
            }
        }
    }
}