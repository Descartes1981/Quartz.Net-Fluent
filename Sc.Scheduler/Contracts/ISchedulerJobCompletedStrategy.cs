using System;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerJobCompletedStrategy
    {
        void OnComplete(ISchedulerJobInfo schedulerJobInfo);

        void OnCanceled(ISchedulerJobInfo schedulerJobInfo);

        void OnError(Exception exception);
    }
}
