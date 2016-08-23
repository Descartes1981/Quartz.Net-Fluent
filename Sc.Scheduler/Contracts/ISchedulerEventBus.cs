using System;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerEventBus : IDisposable
    {
        Action Subscribe<TEvent>(Action<TEvent> eventHandler);
        
        void Publish<TEvent>(TEvent @event);
    }
}