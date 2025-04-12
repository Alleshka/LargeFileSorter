using CommandLine;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LargeFileSorter.FileGenerator.ConsoleClient
{
    class Options
    {
        [Option('d', "dir", HelpText = "Output file dir", Required = false, Default = "")]
        public string OutputDirectory { get; set; }

        [Option('f', "file", HelpText = "Output file name", Required = false, Default = "largeFile.txt")]
        public string OutputFile { get; set; }

        [Option('s', "size", Default = "1Mb", Required = false, HelpText = "Target file size (1Mb, 2Gb, e.t.c)")]
        public string TargetSize { get; set; }
        public long TargetSizeBytes => ParseSize(TargetSize);

        private long ParseSize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 1024;
            }

            Match match = Regex.Match(input.Trim(), @"^(?<size>\d*)(?<unit>\w*)$");

            if (!match.Success) return 1024;

            double value = double.Parse(match.Groups["size"].Value, CultureInfo.InvariantCulture);
            string unit = match.Groups["unit"].Value.ToLowerInvariant();

            return unit switch
            {
                "b" or "" => (long)value,
                "kb" => (long)(value * 1024),
                "mb" => (long)(value * 1024 * 1024),
                "gb" => (long)(value * 1024 * 1024 * 1024),
                _ => (long)value
            };
        }
    }
}
