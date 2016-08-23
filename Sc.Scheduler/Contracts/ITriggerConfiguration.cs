using System;
using System.Threading.Tasks;

namespace Sc.Scheduler.Contracts
{
    public interface ITriggerConfiguration : ITriggerScheduleConfiguration<ITriggerConfiguration>
    {
        ITriggerConfiguration WithIdentity(string name);
        
        ITriggerConfiguration WithIdentity(string name, string group);
        
        ITriggerConfiguration WithDescription(string description);


        ITriggerConfiguration WithOnJobSucceded(Action<ISchedulerJobInfo> onJobSucceded);

        ITriggerConfiguration WithOnJobFailed(Action<ISchedulerJobInfo, Exception> onJobFailed);

        ITriggerConfiguration WithOnJobCancelled(Action<ISchedulerJobInfo> onJobCancelled);


        ITriggerConfiguration WithOnJobSucceded(Func<ISchedulerJobInfo, Task> onJobSucceded);

        ITriggerConfiguration WithOnJobFailed(Func<ISchedulerJobInfo, Exception, Task> onJobFailed);

        ITriggerConfiguration WithOnJobCancelled(Func<ISchedulerJobInfo, Task> onJobCancelled);
    }
}