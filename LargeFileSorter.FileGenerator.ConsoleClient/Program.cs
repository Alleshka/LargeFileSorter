using CommandLine;
using System.Diagnostics;

namespace LargeFileSorter.FileGenerator.ConsoleClient
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
            IFileGenerator fileGenerator = new TempFilesFileGenerator();
            fileGenerator.GenerateFileAsync(options.OutputDirectory, options.OutputFile, options.TargetSizeBytes);
        }
    }
}
