namespace LargeFileSorter.FileSorterer.WorkerPool
{
    abstract class BaseMergerWorkerPool(int maxThreads, CancellationTokenSource? cts = null) : BaseWorkerPool(maxThreads, cts)
    {
        protected readonly PriorityQueue<FileInfo, long> _tasks = new PriorityQueue<FileInfo, long>();

        public FileInfo ResultFile { get; protected set; }

        protected int _minFilesToMerge = 2;
        protected int _maxFilesToMerge = 2;

        public virtual void EnqueuTask(FileInfo path)
        {
            lock (_locker)
            {
                _tasks.Enqueue(path, path.Length);

                if (_tasks.Count >= _minFilesToMerge)
                {
                    if (_activeThreads == 0)
                    {
                        StartWorker();
                    }
                    else
                    {
                        _evt.Set();
                    }
                }
            }

            if (_activeThreads < _maxThreads && _tasks.Count > _minFilesToMerge)
            {
                lock (_locker)
                {
                    if (_activeThreads < _maxThreads && _tasks.Count > _minFilesToMerge)
                    {
                        StartWorker();
                    }
                }
            }
        }

        public override async Task StopAsync()
        {
            await base.StopAsync();
            ResultFile = _tasks.Dequeue();
        }
    }
}
