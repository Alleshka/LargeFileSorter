using LargeFileSorter.Common;

namespace LargeFileSorter.FileGenerator
{
    class SimpleRowGenerator : IRowGenerator
    {
        private static readonly string[] _stringPool = new[]
        {
            "Apple", "Something something something", "Cherry is the best", "Banana is yellow", "hi mark"
        };

        private readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        public FileLine GenerateRow()
        {
            int number = _random.Value.Next();
            string phrase = _stringPool[_random.Value.Next(_stringPool.Length)];
            return new FileLine(number, phrase);
        }
    }
}
