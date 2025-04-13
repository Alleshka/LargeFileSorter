using LargeFileSorter.Common;

namespace LargeFileSorter.FileSorterer
{
    internal class ComparableFileLine : IComparable<ComparableFileLine>
    {
        public FileLine Line { get; }
        private static readonly IComparer<FileLine> _comparer = FileLineComparer.Default;

        public ComparableFileLine(FileLine line)
        {
            Line = line;
        }

        public int CompareTo(ComparableFileLine? other)
        {
            if (other == null) return 1;
            return _comparer.Compare(this.Line, other.Line);
        }
    }
}
