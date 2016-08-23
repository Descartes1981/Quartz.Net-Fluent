using System;
using Quartz;

namespace Sc.Scheduler.Exceptions
{
    public class SchedulerTriggerWasAlreadyScheduledException : InvalidOperationException
    {
        private const string ErrorMessageNameGroup = "Trigger with name: {0}, group: {1} was already scheduled.";

        private const string ErrorMessageName = "Trigger with name: {0} was already scheduled.";

        public SchedulerTriggerWasAlreadyScheduledException(TriggerKey triggerKey) : base(FormatMessage(triggerKey))
        {
            
        }

        public SchedulerTriggerWasAlreadyScheduledException(string name, string group) : this(new TriggerKey(name, group))
        {
        }

        public SchedulerTriggerWasAlreadyScheduledException(string name) : this(name, null)
        {
        }


        private static string FormatMessage(TriggerKey triggerKey)
        {
            if (!string.IsNullOrEmpty(triggerKey.Group))
            {
                return string.Format(ErrorMessageName, triggerKey.Name);
            }

            return string.Format(ErrorMessageNameGroup, triggerKey.Name, triggerKey.Group);
        }

    }
}
