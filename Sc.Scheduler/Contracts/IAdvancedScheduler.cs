using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sc.Scheduler.Contracts
{
    public interface IAdvancedScheduler : IDisposable
    {
        ISchedulerJobContoller Schedule(Action<ISchedulerJobInfo, CancellationToken> action, Action<ITriggerConfiguration> triggerConfiguration);

        ISchedulerJobContoller Schedule(Func<ISchedulerJobInfo, CancellationToken, Task> action, Action<ITriggerConfiguration> triggerConfiguration);
    }
}