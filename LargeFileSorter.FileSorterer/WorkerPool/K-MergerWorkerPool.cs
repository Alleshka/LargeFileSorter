namespace LargeFileSorter.FileSorterer.WorkerPool
{
    class K_MergerWorkerPool : BaseMergerWorkerPool
    {
        private Action<List<FileInfo>> _handler;

        public K_MergerWorkerPool(int maxThreads, Action<List<FileInfo>> handler, int minFiles = 2, int maxFiles = 8, CancellationTokenSource? cts = null) : base(maxThreads, cts)
        {
            _handler = handler;
            _minFilesToMerge = Math.Max(2, minFiles);
            _maxFilesToMerge = maxFiles;
        }

        protected override void WorkerLoop()
        {
            int count = 0;
            List<FileInfo> list = new List<FileInfo>();

            try
            {
                while (true)
                {
                    lock (_locker)
                    {
                        if (_tasks.Count >= _minFilesToMerge || 
                            (_cts.IsCancellationRequested && _activeThreads < 2 && _tasks.Count >= 2))
                        {
                            while (_tasks.Count > 0 && list.Count < _maxFilesToMerge)
                            {
                                if (_tasks.TryDequeue(out FileInfo? file, out long priority))
                                {
                                    list.Add(file);
                                }
                            }

                            count = _tasks.Count;
                        }
                    }

                    if (list.Count >= 2)
                    {
                        if (count >= _minFilesToMerge)
                        {
                            _evt.Set();
                        }

                        _handler(list);
                        list.Clear();
                    }
                    else if (_cts.IsCancellationRequested && _tasks.Count == 1)
                    {
                        break;
                    }
                    else
                    {
                        _evt.WaitOne();
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                Interlocked.Decrement(ref _activeThreads);
                _evt.Set();
            }
        }
    }
}
