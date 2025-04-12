
namespace LargeFileSorter.Common
{
    /// <summary>
    /// First, sort by the string part (alphabetically)
    /// If two lines have the same string, sort by the number (ascending)
    /// </summary>
    public class StringNumberAscFileLineComparer : IComparer<FileLine>
    {
        public int Compare(FileLine? x, FileLine? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int result = x.String.CompareTo(y.String);
            if (result != 0) return result;

            return x.Number.CompareTo(y.Number);
        }
    }
}
