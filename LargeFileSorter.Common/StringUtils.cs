using System.Globalization;
using System.Text.RegularExpressions;

namespace LargeFileSorter.Common
{
    public static class StringUtils
    {
        public static long ParseSize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 1024;
            }

            Match match = Regex.Match(input.Trim(), @"^(?<size>\d*\.*\d*)(?<unit>\w*)$");

            if (!match.Success) return 1024;

            double value = double.Parse(match.Groups["size"].Value, CultureInfo.InvariantCulture);
            string unit = match.Groups["unit"].Value.ToLowerInvariant();

            return unit switch
            {
                "b" or "" => (long)value,
                "kb" => (long)(value * 1024),
                "mb" => (long)(value * 1024 * 1024),
                "gb" => (long)(value * 1024 * 1024 * 1024),
                "tb" => (long)(value * 1024 * 1024 * 1024 * 1024),
                _ => (long)value
            };
        }


        public static string GetHumanReadableSize(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", size, sizes[order]);
            return result;
        }
    }
}