using System;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerDispatcherFactory : IDisposable
    {
        Lazy<ISchedulerDispatcher> CreateDispatcher(Guid triggerId);
    }
}