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

        public FileLine(string input)
        {
            Match match = Regex.Match(input, @"(?<number>\d*)\.\s(?<string>.*)");
            if (match.Success)
            {
                Number = Convert.ToInt64(match.Groups["number"].Value);
                String = match.Groups["string"].Value;
            }
        }

        public override string ToString()
        {
            return $"{Number}. {String}";
        }
    }
}
