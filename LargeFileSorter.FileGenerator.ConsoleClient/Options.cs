using CommandLine;
using LargeFileSorter.Common;

namespace LargeFileSorter.FileGenerator.ConsoleClient
{
    class Options
    {
        [Option('d', "dir", HelpText = "Output file dir", Required = false, Default = "")]
        public string OutputDirectory { get; set; }

        [Option('f', "file", HelpText = "Output file name", Required = false, Default = "largeFile.txt")]
        public string OutputFile { get; set; }

        [Option('s', "size", Default = "0.5Gb", Required = false, HelpText = "Target file size (1b, 2Kb, 3Mb, 4Gb, 5Tb)")]
        public string TargetSize { get; set; }
        public long TargetSizeBytes => StringUtils.ParseSize(TargetSize);
    }
}
