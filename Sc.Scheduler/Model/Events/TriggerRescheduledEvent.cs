using System;

namespace Sc.Scheduler.Model.Events
{
    internal class TriggerRescheduledEvent
    {
        private readonly Guid _triggerId;
        
        private readonly bool _cancelAllTasks;

        public TriggerRescheduledEvent(Guid triggerId, bool cancelAllTasks)
        {
            _triggerId = triggerId;
            
            _cancelAllTasks = cancelAllTasks;
        }


        public Guid TriggerId
        {
            get { return _triggerId; }
        }

        public bool CancelAllTasks
        {
            get { return _cancelAllTasks; }
        }
    }
}
