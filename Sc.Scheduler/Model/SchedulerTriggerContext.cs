using System;
using System.Threading;
using System.Threading.Tasks;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Model
{
    internal class SchedulerTriggerContext : ISchedulerTriggerContext
    {
        public SchedulerTriggerContext()
        {
            TriggerId = Guid.NewGuid();
        }

        public Guid TriggerId { get; private set; }
        
        public string TriggerName { get; internal set; }
        
        public string TriggerGroup { get; internal set; }
        
        public Func<ISchedulerJobInfo, CancellationToken, Task> OnJobTriggered { get; internal set; }
        
        public TriggerConfiguration TriggerConfiguration { get; internal set; }
        
        public object Clone()
        {
            return new SchedulerTriggerContext
            {
                OnJobTriggered = OnJobTriggered,

                TriggerConfiguration = TriggerConfiguration,

                TriggerId = TriggerId,

                TriggerName = TriggerName,

                TriggerGroup = TriggerGroup,
            };
        }
    }
}
