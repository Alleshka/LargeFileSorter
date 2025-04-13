using LargeFileSorter.Common;
using LargeFileSorter.FileSorterer.WorkerPool;
using System;
using System.Text;

namespace LargeFileSorter.FileSorterer
{
    public class SimpleFileSorterer : IFileSorterer
    {
        private SorterWorkerPool _sorterWorker;
        private BaseMergerWorkerPool _mergerWorker;


        public async Task SortFileAsync(string input, string outputDir = "", string outputFile = "sortResult.txt", long chunkSize = 0)
        {
            if (chunkSize <= 0)
            {
                chunkSize = MemoryUtils.LargeBufferSize * 2;
            }

            var reader = Task.Run(() => ReadFile(input, chunkSize));
            string tmpDir = Path.Combine(outputDir);
            string oututPath = Path.Combine(outputDir, outputFile);

            Directory.CreateDirectory(tmpDir);

            _sorterWorker = new SorterWorkerPool(Environment.ProcessorCount * 2, (chunk) => SortChunk(tmpDir, chunk));
            _mergerWorker = false
                ? new MergerWorkerPool(Environment.ProcessorCount, (file1, file2) => MergeFiles(tmpDir, file1, file2))
                : new K_MergerWorkerPool(Environment.ProcessorCount, (list) => KMergeFiles(tmpDir, list), 6);

            await reader;
            await _sorterWorker.StopAsync();
            await _mergerWorker.StopAsync();

            _mergerWorker.ResultFile.MoveTo(oututPath, true);
        }

        private void ReadFile(string inputFile, long chunkSize)
        {
            long written = 0;

            using (var reader = new StreamReader(inputFile))
            {
                List<FileLine>? lines = new List<FileLine>();
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var fileLine = FileLine.Parse(line);
                        if (fileLine != null)
                        {
                            lines.Add(fileLine);
                            written += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;

                            if (written >= chunkSize)
                            {
                                _sorterWorker.EnqueuTask(lines);
                                lines = new List<FileLine>();
                                written = 0;
                            }
                        }
                    }
                }

                if (lines.Count > 0)
                {
                    _sorterWorker.EnqueuTask(lines);
                    lines = null;
                }
            }
        }

        private void SortChunk(string tmpDir, List<FileLine> chunk)
        {
            chunk.Sort(FileLineComparer.Default);

            string fileName = $"sorted-part-{Guid.NewGuid()}";

            using (StreamWriter writer = new StreamWriter(Path.Combine(tmpDir, fileName), false, Encoding.UTF8, MemoryUtils.SmallBufferSize))
            {
                for (int i = 0; i < chunk.Count; i++)
                {
                    writer.WriteLine(chunk[i]);
                }
            }

            _mergerWorker.EnqueuTask(new FileInfo(Path.Combine(tmpDir, fileName)));
        }

        private void MergeFiles(string tmpDir, FileInfo file1, FileInfo file2)
        {
            int bufferSize = MemoryUtils.SmallBufferSize;

            string fileName = $"merged-part-{Guid.NewGuid()}";
            string outputFile = Path.Combine(tmpDir, fileName);

            using (var reader1 = new StreamReader(file1.FullName, Encoding.UTF8, true, bufferSize))
            {
                using (var reader2 = new StreamReader(file2.FullName, Encoding.UTF8, true, bufferSize))
                {
                    using (StreamWriter writer = new StreamWriter(outputFile, false, Encoding.UTF8, bufferSize))
                    {
                        string? line1 = reader1.ReadLine();
                        string? line2 = reader2.ReadLine();

                        while (line1 != null && line2 != null)
                        {
                            FileLine l1 = FileLine.Parse(line1);
                            FileLine l2 = FileLine.Parse(line2);

                            int result = FileLineComparer.Default.Compare(l1, l2);
                            if (result <= 0)
                            {
                                writer.WriteLine(l1);
                                line1 = reader1.ReadLine();
                            }
                            else
                            {
                                writer.WriteLine(l2);
                                line2 = reader2.ReadLine();
                            }
                        }

                        if (line2 == null)
                        {
                            writer.WriteLine(line1);

                            char[] buffer = new char[bufferSize];
                            int charsRead;
                            while ((charsRead = reader1.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                writer.Write(buffer, 0, charsRead);
                            }
                        }

                        if (line1 == null)
                        {
                            writer.WriteLine(line2);

                            char[] buffer = new char[bufferSize];
                            int charsRead;
                            while ((charsRead = reader2.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                writer.Write(buffer, 0, charsRead);
                            }
                        }
                    }
                }
            }

            file1.Delete();
            file2.Delete();
            _mergerWorker.EnqueuTask(new FileInfo(outputFile));
        }

        private void KMergeFiles(string tmpDir, List<FileInfo> files)
        {
            int bufferSize = MemoryUtils.SmallBufferSize;
            string fileName = $"merged-part-{Guid.NewGuid()}";
            string outputFile = Path.Combine(tmpDir, fileName);

            //Console.WriteLine($"Merge {files.Count} files into {outputFile} ({Thread.CurrentThread.ManagedThreadId})");
            //Console.WriteLine(string.Join(Environment.NewLine, files.Select(x => $"\t{x.FullName}")));

            var queue = new PriorityQueue<(FileLine line, StreamReader reader), ComparableFileLine>();

            using (StreamWriter writer = new StreamWriter(outputFile, false, Encoding.UTF8, bufferSize))
            {
                var readers = files.Select(x => new StreamReader(x.FullName, Encoding.UTF8, true, bufferSize)).ToList();
                foreach (var reader in readers)
                {
                    FileLine? line = FileLine.Parse(reader.ReadLine());
                    if (line != null)
                    {
                        queue.Enqueue((line, reader), new ComparableFileLine(line));
                    }
                }

                while (queue.Count > 1)
                {
                    (FileLine minLine, StreamReader reader) = queue.Dequeue();
                    writer.WriteLine(minLine);
                    FileLine? line = FileLine.Parse(reader.ReadLine());
                    if (line != null)
                    {
                        queue.Enqueue((line, reader), new ComparableFileLine(line));
                    }
                }

                if (queue.Count == 1)
                {
                    (FileLine minLine, StreamReader reader) = queue.Dequeue();
                    writer.WriteLine(minLine);

                    char[] buffer = new char[bufferSize];
                    int charsRead;
                    while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, charsRead);
                    }
                }

                foreach (var reader in readers)
                {
                    reader.Close();
                    reader.Dispose();
                }

                foreach (var file in files)
                {
                    file.Delete();
                }
            }

            _mergerWorker.EnqueuTask(new FileInfo(outputFile));
        }
    }
}
