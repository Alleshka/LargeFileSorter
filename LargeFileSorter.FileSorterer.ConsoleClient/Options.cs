using CommandLine;

namespace LargeFileSorter.FileSorterer.ConsoleClient
{
    class Options
    {
        [Value(0, HelpText = "Input file path", Required = true)]
        public string Input { get; set; }

        [Option('d', "dir", HelpText = "Output dir")]
        public string OutputDir { get; set; }

        [Option('f', "file", HelpText = "Output file", Default = "largeFileSorted.txt")]
        public string OutputFile { get; set; }
    }
}
