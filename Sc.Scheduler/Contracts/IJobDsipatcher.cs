using System;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerDispatcher : IDisposable
    {
        void Enqueue(Action action);
    }
}
