namespace LargeFileSorter.FileSorterer.WorkerPool
{
    internal abstract class BaseWorkerPool
    {
        protected readonly AutoResetEvent _evt = new AutoResetEvent(false);
        protected readonly CancellationTokenSource _cts;

        protected readonly int _maxThreads;
        protected int _activeThreads;
        protected readonly object _locker = new object();

        protected LinkedList<Task> _workers = new LinkedList<Task>();

        public BaseWorkerPool(int maxThreads, CancellationTokenSource? cts = null)
        {
            _maxThreads = maxThreads;
            _cts = cts ?? new CancellationTokenSource();
        }

        public void Stop()
        {
            StopAsync();
        }

        public async virtual Task StopAsync()
        {
            _cts.Cancel();
            _evt.Set();

            Task[] runningTasks;

            lock (_workers)
            {
                runningTasks = _workers.ToArray();
            }

            await Task.WhenAll(runningTasks);
        }

        protected void StartWorker()
        {
            Interlocked.Increment(ref _activeThreads);
            var task = Task.Run(() => WorkerLoop());

            lock (_workers)
            {
                _workers.AddLast(task);
            }
        }

        protected abstract void WorkerLoop();
    }
}
