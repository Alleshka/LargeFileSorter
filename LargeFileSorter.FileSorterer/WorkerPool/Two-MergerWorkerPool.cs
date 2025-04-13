namespace LargeFileSorter.FileSorterer.WorkerPool
{
    class MergerWorkerPool : BaseMergerWorkerPool
    {
        private Action<FileInfo, FileInfo> _handler;

        public MergerWorkerPool(int maxThreads, Action<FileInfo, FileInfo> handler, CancellationTokenSource? cts = null) : base(maxThreads, cts)
        {
            _handler = handler;
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
                        if (_tasks.Count >= _minFilesToMerge)
                        {
                            file1 = _tasks.Dequeue();
                            file2 = _tasks.Dequeue();
                            count = _tasks.Count;
                        } 
                    }

                    if (file1 != null && file2 != null)
                    {
                        if (count >= _minFilesToMerge)
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
    }
}
