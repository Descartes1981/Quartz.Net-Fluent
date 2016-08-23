using System;
using System.Threading;
using System.Threading.Tasks;
using Sc.Scheduler.Model;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerTriggerContext : ICloneable
    {
        Guid TriggerId { get; }

        string TriggerName { get; }

        string TriggerGroup { get; }

        Func<ISchedulerJobInfo, CancellationToken, Task> OnJobTriggered { get; }

        TriggerConfiguration TriggerConfiguration { get; }
    }
}