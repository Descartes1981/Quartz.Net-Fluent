using System;
using Quartz;

namespace Sc.Scheduler.Contracts
{
    internal interface ISchedulerJob : IJob, IDisposable
    {
        void Dispose(bool cancellAllTasks);
    }
}