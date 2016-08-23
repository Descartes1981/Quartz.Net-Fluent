using System;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Utils
{
    class SchedulerJobCompletedNullStrategy : ISchedulerJobCompletedStrategy
    {
        public void OnComplete(ISchedulerJobInfo schedulerJobInfo)
        {
            
        }

        public void OnCanceled(ISchedulerJobInfo schedulerJobInfo)
        {
            
        }

        public void OnError(Exception exception)
        {
            
        }
    }
}
