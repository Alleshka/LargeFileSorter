using CommandLine;
using LargeFileSorter.Common;

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
            if (string.IsNullOrEmpty(options.OutputDirectory))
            {
                options.OutputDirectory = Environment.CurrentDirectory;
            }

            long availableSpace = MemoryUtils.GetAvailableDiskSpace(options.OutputDirectory);
            long requestendSpace = (long)(options.TargetSizeBytes * 1.5);
            if (availableSpace < requestendSpace)
            {
                Console.WriteLine($"I can't generate the file. " +
                    $"This file requires approximately {StringUtils.GetHumanReadableSize(requestendSpace)} to generate. " +
                    $"Currently available: {StringUtils.GetHumanReadableSize(availableSpace)}");
                return;
            }
            else
            {
                IFileGenerator fileGenerator = new TempFilesFileGenerator();
                fileGenerator.GenerateFile(options.OutputDirectory, options.OutputFile, options.TargetSizeBytes);
            }
        }
    }
}
