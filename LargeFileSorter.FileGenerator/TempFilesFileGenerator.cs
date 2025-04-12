using LargeFileSorter.Common;
using System.Text;

namespace LargeFileSorter.FileGenerator
{
    public class TempFilesFileGenerator : IFileGenerator
    {
        private const int BufferSize = 65536;
        private IRowGenerator _rowGenerator = new SimpleRowGenerator();

        public async void GenerateFile(string directory = "", string fileName = "largeFile.txt", long targetFileSizeBytes = 1024, int maxThreadsCount = 0)
        {
            if (maxThreadsCount <= 0 || maxThreadsCount >= Environment.ProcessorCount)
            {
                maxThreadsCount = Environment.ProcessorCount;
            }

            if (string.IsNullOrEmpty(directory))
            {
                directory = Environment.CurrentDirectory;
            }

            DirectoryInfo tmpDir = Directory.CreateDirectory(Path.Combine(directory, "tmp"));
            GenerateTempFiles(tmpDir, targetFileSizeBytes, maxThreadsCount);
            MergeTempFiles(tmpDir, Path.Combine(directory, fileName));
            tmpDir.Delete();
        }

        private void GenerateTempFiles(DirectoryInfo tmpDir, long targetFileSizeBytes = 1024, int maxThreadsCount = 0)
        {
            int maxFiles = Math.Min(32, maxThreadsCount * 4);
            long chunkSize = targetFileSizeBytes / maxFiles;

            Parallel.ForEach(Enumerable.Range(0, maxFiles), new ParallelOptions() { MaxDegreeOfParallelism = maxThreadsCount }, (i) =>
            {
                string tempPath = Path.Combine(tmpDir.FullName, $"part-{i}");

                using (StreamWriter writer = new StreamWriter(tempPath, false, Encoding.UTF8, BufferSize))
                {
                    long written = 0;
                    while (written < chunkSize)
                    {
                        FileLine row = _rowGenerator.GenerateRow();
                        string line = row.ToString();
                        written += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;
                        writer.WriteLine(line);
                    }
                }
            });
        }

        private void MergeTempFiles(DirectoryInfo tmpDir, string resultFilePath)
        {
            using (FileStream fileWriter = new FileStream(resultFilePath, FileMode.Create))
            {
                string[] tempFiles = Directory.GetFiles(tmpDir.FullName, $"part*");
                foreach (string part in tempFiles)
                {
                    using (FileStream reader = new FileStream(part, FileMode.Open, FileAccess.Read))
                    {
                        reader.CopyTo(fileWriter);
                    }
                    File.Delete(part);
                }
            }
        }
    }
}
