using Quartz;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Model.Events
{
    internal class TriggerDeletedEvent
    {
        private readonly ISchedulerTriggerContext _triggerContext;
        
        private readonly bool _cancelAllTasks;

        public TriggerDeletedEvent(ISchedulerTriggerContext triggerContext, bool cancelAllTasks)
        {
            _triggerContext = triggerContext;
            
            _cancelAllTasks = cancelAllTasks;
        }

        public ISchedulerTriggerContext TriggerContext
        {
            get { return _triggerContext; }
        }

        public bool CancelAllTasks
        {
            get { return _cancelAllTasks; }
        }
    }
}