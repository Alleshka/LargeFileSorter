namespace LargeFileSorter.FileGenerator
{
    public interface IFileGenerator
    {
        FileInfo GenerateFile(string directory = "", string fileName = "largeFile.txt", long targetFileSizeBytes = 1024, int maxThreadsCount = 0);
    }
}
