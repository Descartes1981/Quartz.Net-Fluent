using System;
using System.Collections.Concurrent;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler
{
    internal class SchedulerEventBus : ISchedulerEventBus
    {
        private readonly object _lock = new object();

        private readonly ConcurrentDictionary<object, ConcurrentDictionary<Action<object>, object>> _handlers = new ConcurrentDictionary<object, ConcurrentDictionary<Action<object>, object>>();
        
        private bool _disposed;

        public Action Subscribe<TEvent>(Action<TEvent> eventHandler)
        {
            ThrowIfDisposed();

            if (eventHandler==null) throw new ArgumentNullException("eventHandler");

            var handlers = _handlers.GetOrAdd(typeof (TEvent), new ConcurrentDictionary<Action<object>, object>());

            Action<object> executeHandler = o => eventHandler((TEvent) o);

            handlers.TryAdd(executeHandler, null);

            return () =>
            {
                lock (_lock)
                {
                    object foo;
                    handlers.TryRemove(executeHandler, out foo);

                    if (handlers.Count == 0)
                    {
                        ConcurrentDictionary<Action<object>, object> foo1;

                        _handlers.TryRemove(typeof (TEvent), out foo1);
                    }
                }
            };
        }

        public void Publish<TEvent>(TEvent @event)
        {
            ThrowIfDisposed();

            if (!typeof(TEvent).IsValueType)
            {
                if (Equals(@event, default(TEvent))) throw new ArgumentNullException("event");
            }

            ConcurrentDictionary<Action<object>, object> handlers;

            if (_handlers.TryGetValue(typeof (TEvent), out handlers))
            {
                foreach (var handler in handlers.Keys)
                {
                    handler(@event);
                }   
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _handlers.Clear();
            }
        }
    }
}
