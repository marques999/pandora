using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using SmoothDownloader.Download;
using SmoothDownloader.Smooth;

namespace SmoothDownloader
{
    /// <summary>
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var isDeterministic = false;
            var assemblyName = Assembly.GetEntryAssembly().GetName();

            Console.WriteLine(string.Concat(assemblyName.Name, " v", assemblyName.Version));
            Console.WriteLine();

            int position;

            for (position = 0; position < args.Length; ++position)
            {
                if (args[position] == "--")
                {
                    ++position;
                    break;
                }

                if (args[position].Length == 0 || args[position] == "-" || args[position][0] != '-')
                {
                    break;
                }

                if (args[position] == "--det")
                {
                    isDeterministic = true;
                }
            }

            if (args.Length < position + 2)
            {
                Console.WriteLine("Microsoft IIS Smooth Streaming Downloader and muxer to MKV.");
                Console.WriteLine();
                Console.WriteLine("Supported media stream formats:");
                Console.WriteLine("- Audio: AAC, WMA");
                Console.WriteLine("- Video: H264, VC-1");
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine(assemblyName.Name + " [<flag> ...] <source> [...] <output-directory>");
                Console.WriteLine("<source> is an .ism (or /manifest) file or URL.");
                Console.WriteLine("<output-directory> can be just . , a properly named file will be created.");
                Console.WriteLine("Many temporary files and subdirs may be created and left in <output-directory>.");
                Console.WriteLine("--det  Enable deterministic MKV output (no random, no current time).");
                Environment.Exit(1);
            }

            var lastIndex = args.Length - 1;
            var downloadDirectory = args[lastIndex].Trim(Path.GetInvalidFileNameChars()).Trim(Path.GetInvalidPathChars());

            Console.WriteLine("Download directory: " + downloadDirectory);

            var urls = new string[lastIndex - position];

            for (var index = position; index < lastIndex; ++index)
            {
                urls[index - position] = args[index];
            }

            var partialUrls = ProcessUrls(urls);

            Console.WriteLine("Parts to download:");

            foreach (var partialUrl in partialUrls)
            {
                Console.WriteLine("  Part URL: " + partialUrl);
            }

            Console.WriteLine();

            foreach (var partialUrl in partialUrls)
            {
                RecordAndMux(partialUrl, downloadDirectory, isDeterministic);
            }

            Console.WriteLine("All downloading and muxing done.");
        }

        /// <summary>
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        private static IList<string> ProcessUrls(IList<string> urls)
        {
            var urlProcessors = (
                from type in typeof(IUrlProcessor).Assembly.GetTypes()
                where type.IsClass && typeof(IUrlProcessor).IsAssignableFrom(type)
                select (IUrlProcessor)type.GetConstructor(Type.EmptyTypes)?.Invoke(null)
            ).OrderBy(urlProcessor => urlProcessor.GetOrder()).ToList();

            foreach (var urlProcessor in urlProcessors)
            {
                var nextUrls = new List<string>();
                urlProcessor.Process(urls, nextUrls);
                urls = nextUrls;
            }

            return urls;
        }

        /// <summary>
        /// </summary>
        /// <param name="ismFileName"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="isDeterministic"></param>
        private static void RecordAndMux(string ismFileName, string outputDirectory, bool isDeterministic)
        {
            var mkvPath = outputDirectory + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(ismFileName) + ".mkv";

            if (File.Exists(mkvPath) && File.Exists(Path.ChangeExtension(mkvPath, "muxstate")) == false)
            {
                Console.WriteLine($"Already downloaded: {mkvPath}{Environment.NewLine}");
            }
            else
            {
                Console.WriteLine("Muxing MKV: " + mkvPath);

                Uri manifestUri = null;
                string manifestPath = null;
                var manifestUrl = ismFileName + "/manifest";

                if (manifestUrl.StartsWith("http://") || manifestUrl.StartsWith("https://"))
                {
                    manifestUri = new Uri(manifestUrl);
                }
                else if (manifestUrl.StartsWith("file://"))
                {
                    manifestPath = new Uri(manifestUrl).LocalPath;
                }
                else
                {
                    manifestPath = manifestUrl;
                }

                var muxingInteractiveState = new MuxingInteractiveState();

                Downloader.DownloadAndMux(
                    manifestUri,
                    manifestPath,
                    mkvPath,
                    isDeterministic,
                    new TimeSpan(10, 0, 0),
                    muxingInteractiveState.SetupStop,
                    muxingInteractiveState.DisplayDuration);

                muxingInteractiveState.Abort();
            }
        }
    }
}