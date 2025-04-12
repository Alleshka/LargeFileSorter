using LargeFileSorter.Common;

namespace LargeFileSorter.FileSorterer.WorkerPool
{
    class SorterWorkerPool : BaseWorkerPool
    {
        protected readonly Queue<List<FileLine>> _tasks = new Queue<List<FileLine>>();
        private Action<List<FileLine>> _handler;

        public SorterWorkerPool(int maxThreads, Action<List<FileLine>> handler, CancellationTokenSource? cts = null) : base(maxThreads, cts)
        {
            _handler = handler;
        }

        public void EnqueuTask(List<FileLine> task)
        {
            lock (_locker)
            {
                _tasks.Enqueue(task);

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
            int count = 1;

            try
            {
                while (true)
                {
                    List<FileLine>? task = null;
                    

                    lock (_locker)
                    {
                        if (_tasks.Count > 0)
                        {
                            task = _tasks.Dequeue();
                            count = _tasks.Count;
                        }
                    }

                    if (task != null)
                    {
                        if (count > 0)
                        {
                            _evt.Set();
                        }

                        _handler(task);
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
