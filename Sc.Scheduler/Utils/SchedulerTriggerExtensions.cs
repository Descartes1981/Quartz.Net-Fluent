using System.Collections.Generic;
using Quartz;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model;

namespace Sc.Scheduler.Utils
{
    internal static class SchedulerTriggerExtensions
    {
        public static void SetTriggerContext(this ITrigger trigger, ISchedulerTriggerContext schedulerTriggerContext)
        {
            trigger.JobDataMap[SchedulerKeysConstants.TriggerInfo] = schedulerTriggerContext;
        }

        public static ISchedulerTriggerContext GetTriggerContext(this ITrigger trigger)
        {
            return trigger.JobDataMap[SchedulerKeysConstants.TriggerInfo] as ISchedulerTriggerContext;
        }

        public static void SetTriggerContext(this TriggerBuilder triggerBuilder, ISchedulerTriggerContext schedulerTriggerContext)
        {
            triggerBuilder.UsingJobData(new JobDataMap { new KeyValuePair<string, object>(SchedulerKeysConstants.TriggerInfo, schedulerTriggerContext)});
        }
    }
}
