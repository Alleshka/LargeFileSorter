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
    }
}
