using CommandLine;
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

            Stopwatch sw = Stopwatch.StartNew();
            var sorterer = new SimpleFileSorterer();
            sorterer.SortFileAsync(options.Input, options.OutputDir, options.OutputFile).GetAwaiter().GetResult();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
