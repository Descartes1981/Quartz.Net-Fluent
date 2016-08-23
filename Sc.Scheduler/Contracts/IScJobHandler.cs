using System;
using Quartz;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerJobContoller : IDisposable
    {
        bool Pause();

        bool Resume();

        void Reschedule(Action<ITriggerScheduleConfiguration<ITriggerConfiguration>> triggerSchedule, bool cancelAllTasks = false);

        TriggerState State { get; }
        void Dispose(bool cancelAllTasks);
    }
}
