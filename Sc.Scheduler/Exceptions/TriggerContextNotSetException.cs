using System;

namespace Sc.Scheduler.Exceptions
{
    public class TriggerContextNotSetException : InvalidOperationException
    {
        private const string ErrorMessage = "Trigger context not found for trigger {0}, trigger group: {1}.";

        public TriggerContextNotSetException(string triggerName, string triggerGroup = null) : base(string.Format(ErrorMessage, triggerName, triggerGroup ?? "no group"))
        {
            if (triggerName==null) throw new ArgumentNullException("triggerName");
        }
    }
}
