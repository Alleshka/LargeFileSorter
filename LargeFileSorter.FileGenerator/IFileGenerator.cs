namespace LargeFileSorter.FileGenerator
{
    public interface IFileGenerator
    {
        void GenerateFile(string directory = "", string fileName = "largeFile.txt", long targetFileSizeBytes = 1024, int maxThreadsCount = 0);
    }
}
