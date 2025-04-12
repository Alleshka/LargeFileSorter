namespace LargeFileSorter.Common
{
    public static class FileLineComparer
    {
        public static IComparer<FileLine> Default => StringNumberAscComparer;
        public static StringNumberAscFileLineComparer StringNumberAscComparer { get; } = new StringNumberAscFileLineComparer();
    }
}
