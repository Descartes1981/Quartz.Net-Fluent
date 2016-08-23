using System;
using System.Threading;
using System.Threading.Tasks;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Model
{
    public class JobExecutionContext
    {
        public JobExecutionContext(Func<ISchedulerJobInfo, CancellationToken, Task> onJobTriggered, CancellationToken cancellationToken, ISchedulerJobInfo schedulerJobInfo, TriggerConfiguration triggerConfiguration)
        {
            if (onJobTriggered==null) throw new ArgumentNullException("onJobTriggered");

            if (schedulerJobInfo==null) throw new ArgumentNullException("schedulerJobInfo");

            if (triggerConfiguration==null) throw new ArgumentNullException("triggerConfiguration");

            CancellationToken = cancellationToken;
            
            OnJobTriggered = onJobTriggered;

            JobInfo = schedulerJobInfo;

            TriggerConfiguration = triggerConfiguration;
        }

        public Func<ISchedulerJobInfo, CancellationToken, Task> OnJobTriggered { get; private set; }

        public ISchedulerJobInfo JobInfo { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public TriggerConfiguration TriggerConfiguration { get; private set; }
    }
}
