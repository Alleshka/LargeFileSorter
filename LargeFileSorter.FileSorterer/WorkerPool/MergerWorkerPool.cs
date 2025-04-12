
using LargeFileSorter.Common;

namespace LargeFileSorter.FileSorterer.WorkerPool
{
    class MergerWorkerPool : BaseWorkerPool
    {
        protected readonly PriorityQueue<FileInfo, long> _tasks = new PriorityQueue<FileInfo, long>();

        public FileInfo ResultFile { get; protected set; }

        private Action<FileInfo, FileInfo> _handler;

        public MergerWorkerPool(int maxThreads, Action<FileInfo, FileInfo> handler, CancellationTokenSource? cts = null) : base(maxThreads, cts)
        {
            _handler = handler;
        }

        public void EnqueuTask(FileInfo path)
        {
            lock (_locker)
            {
                _tasks.Enqueue(path, path.Length);

                if (_activeThreads == 0)
                {
                    StartWorker();
                }
            }

            _evt.Set();

            if (_activeThreads < _maxThreads && _tasks.Count > 0)
            {
                lock (_locker)
                {
                    if (_activeThreads < _maxThreads && _tasks.Count > 0)
                    {
                        StartWorker();
                    }
                }
            }
        }

        protected override void WorkerLoop()
        {
            int count = 0;

            try
            {
                while (true)
                {
                    FileInfo? file1 = null;
                    FileInfo? file2 = null;

                    lock (_locker)
                    {
                        if (_tasks.Count >= 2)
                        {
                            file1 = _tasks.Dequeue();
                            file2 = _tasks.Dequeue();
                            count = _tasks.Count;
                        } 
                    }

                    if (file1 != null && file2 != null)
                    {
                        if (count >= 2)
                        {
                            _evt.Set();
                        }

                        _handler(file1, file2);
                    }
                    else if (!_cts.IsCancellationRequested)
                    {
                        _evt.WaitOne();
                    }
                    else
                    {
                        _evt.Set();
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                Interlocked.Decrement(ref _activeThreads);
            }
        }

        public override async Task StopAsync()
        {
            await base.StopAsync();
            ResultFile = _tasks.Dequeue();
        }
    }
}
