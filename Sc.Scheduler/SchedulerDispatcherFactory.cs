using System;
using System.Collections.Concurrent;
using System.Threading;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model.Events;

namespace Sc.Scheduler
{
    internal class SchedulerDispatcherFactory : ISchedulerDispatcherFactory
    {
        private readonly ISchedulerLogger _schedulerLogger;

        private readonly object _lock = new object();

        private readonly ConcurrentDictionary<Guid, ISchedulerDispatcher> _dispatcherTracker = new ConcurrentDictionary<Guid, ISchedulerDispatcher>();
        
        private bool _disposed;
        
        private readonly Action _consumerDisposedSubscription;

        public SchedulerDispatcherFactory(ISchedulerEventBus schedulerEventBus, ISchedulerLogger schedulerLogger)
        {
            if (schedulerEventBus==null) throw new ArgumentNullException("schedulerEventBus");

            if (schedulerLogger == null) throw new ArgumentNullException("schedulerLogger");

            _schedulerLogger = schedulerLogger;
            
            _consumerDisposedSubscription = schedulerEventBus.Subscribe<TriggerDeletedEvent>(OnTriggerDeleted);
        }

        private void OnTriggerDeleted(TriggerDeletedEvent triggerDeletedEvent)
        {
            ISchedulerDispatcher dispatcher;

            if (_dispatcherTracker.TryRemove(triggerDeletedEvent.TriggerContext.TriggerId, out dispatcher))
            {
                dispatcher.Dispose();
            }
        }
        
        public Lazy<ISchedulerDispatcher> CreateDispatcher(Guid guid)
        {
            ThrowIfDisposed();

            if (guid==Guid.Empty) throw new ArgumentException("Guid can't be empty.", "guid");

            lock (_lock)
            {
                var schedulerDispatcherLazy = new Lazy<ISchedulerDispatcher>(() =>
                {
                    var schedulerDispatcher = new SchedulerDispatcher(_schedulerLogger);

                    _dispatcherTracker.TryAdd(guid, schedulerDispatcher);
                    
                    return schedulerDispatcher;
                }, LazyThreadSafetyMode.ExecutionAndPublication);
                
                return schedulerDispatcherLazy;
            }
        }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                var consumerDisposedSubscription = _consumerDisposedSubscription;

                if (consumerDisposedSubscription != null)
                {
                    consumerDisposedSubscription();
                }
               
        
                foreach (var schedulerDispatcher in _dispatcherTracker.Values)
                {
                    schedulerDispatcher.Dispose();
                }

                _dispatcherTracker.Clear();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
