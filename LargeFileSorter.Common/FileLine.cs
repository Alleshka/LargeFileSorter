using System.Text.RegularExpressions;

namespace LargeFileSorter.Common
{
    public class FileLine
    {
        public long Number { get; protected set; }
        public string String { get; protected set; }

        public FileLine(long number, string s)
        {
            Number = number;
            String = s;
        }

        public static FileLine? Parse(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            Match match = Regex.Match(input, @"(?<number>\d*)\.\s(?<string>.*)");
            if (match.Success)
            {
                long number = Convert.ToInt64(match.Groups["number"].Value);
                string str = match.Groups["string"].Value;

                return new FileLine(number, str);
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Number}. {String}";
        }
    }
}
