using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Model.Events
{
    class TriggerCreatedEvent
    {
        private readonly ISchedulerTriggerContext _schedulerTriggerContext;
        
        private readonly int _rescheduledCount;

        public TriggerCreatedEvent(ISchedulerTriggerContext schedulerTriggerContext, int rescheduledCount = 0)
        {
            _schedulerTriggerContext = schedulerTriggerContext;
           
            _rescheduledCount = rescheduledCount;
        }

        public ISchedulerTriggerContext TriggerContext
        {
            get { return _schedulerTriggerContext; }
        }

        public int RescheduledCount
        {
            get { return _rescheduledCount; }
        }
    }
}
