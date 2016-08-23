using System;
using Quartz;

namespace Sc.Scheduler.Exceptions
{
    public class TriggerNotFoundException : InvalidOperationException
    {
        private const string ErrorMessageNameGroup = "Trigger not found by key, name: {0}, group {1}.";

        private const string ErrorMessageName = "Trigger not found by key, name: {0}.";

        public TriggerNotFoundException(TriggerKey triggerKey) : base(FormatMessage(triggerKey))
        {
            
        }

        public TriggerNotFoundException(string name, string group) : this(new TriggerKey(name, group))
        {

        }

        public TriggerNotFoundException(string name) : this(name, null)
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
