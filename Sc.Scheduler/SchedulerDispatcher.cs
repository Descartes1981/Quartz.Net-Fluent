using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler
{
    internal class SchedulerDispatcher : ISchedulerDispatcher
    {
        private readonly ISchedulerLogger _schedulerLogger;
        
        private readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Task _dispatchTask;

        private bool _disposed;
        
        public SchedulerDispatcher(ISchedulerLogger schedulerLogger)
        {
            if (schedulerLogger==null) throw new ArgumentNullException("schedulerLogger");

            _schedulerLogger = schedulerLogger;

            var token = _cancellationTokenSource.Token;
        
            _dispatchTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (var action in _queue.GetConsumingEnumerable(token))
                    {
                        try
                        {
                            action();
                        }
                        catch (OperationCanceledException operationCanceledException)
                        {
                            _schedulerLogger.ErrorWrite(operationCanceledException, "Action was cancelled.");
                        }
                        catch (Exception exception)
                        {
                            _schedulerLogger.ErrorWrite(exception, "Action execution failed.");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _queue.Dispose();

                    _schedulerLogger.DebugWrite("Scheduler - Dispatcher was disposed.");
                }
            }, token, TaskCreationOptions.LongRunning, new ThreadPerTaskScheduler());
        }

        public void Enqueue(Action action)
        {
            if (action==null) throw new ArgumentNullException("action");

            if (!_disposed)
            {
                _queue.Add(action);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_queue != null)
                {
                    _queue.CompleteAdding();

                    ClearQueue();

                    _cancellationTokenSource.Cancel();

                    _cancellationTokenSource.Dispose();
                }

                if (_dispatchTask != null)
                {
                    _dispatchTask.SafeWait();
                }
            }
        }

        private void ClearQueue()
        {
            Action action;

            while (_queue.TryTake(out action))
            {
            }
        }
    }
}

