using CommandLine;
using LargeFileSorter.Common;
using System.Diagnostics;

namespace LargeFileSorter.FileSorterer.ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => Run(options))
                .WithNotParsed(erros => Console.WriteLine(string.Join(Environment.NewLine, erros)));
        }

        private static void Run(Options options)
        {
            if (string.IsNullOrEmpty(options.OutputDir))
            {
                options.OutputDir = Environment.CurrentDirectory;
            }

            var fileInfo = new FileInfo(options.Input);
            if (string.IsNullOrWhiteSpace(options.OutputFile))
            {
                options.OutputFile = "Sorted_" + fileInfo.Name;
            }

            long availableSpace = MemoryUtils.GetAvailableDiskSpace(options.OutputDir);
            long requestendSpace = fileInfo.Length * 2; // We need 2x file space to merge files

            if (availableSpace < requestendSpace)
            {
                Console.WriteLine($"I can't sort the file. " +
                    $"This file requires approximately {StringUtils.GetHumanReadableSize(requestendSpace)} to sort. " +
                    $"Currently available: {StringUtils.GetHumanReadableSize(availableSpace)}");
                return;
            }
            else
            {
                Console.WriteLine($"Sorting has been started ...");
                Stopwatch sw = Stopwatch.StartNew();
                var sorterer = new SimpleFileSorterer();
                sorterer.SortFileAsync(options.Input, options.OutputDir, options.OutputFile).GetAwaiter().GetResult();
                Console.WriteLine($"Sorting has been completed in {sw.ElapsedMilliseconds} ms");
            }
        }
    }
}
