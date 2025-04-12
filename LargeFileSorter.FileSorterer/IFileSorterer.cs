namespace LargeFileSorter.FileSorterer
{
    public interface IFileSorterer
    {
        public Task SortFileAsync(string input, string outputDir = "", string outputFile = "sortResult.txt", long chunkSize = 0);
    }
}
