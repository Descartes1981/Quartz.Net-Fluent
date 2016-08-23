using Quartz;

namespace Sc.Scheduler.Model.Events
{
    internal class TriggerFiredEvent
    {
        private readonly ITrigger _trigger;

        public TriggerFiredEvent(ITrigger trigger)
        {
            _trigger = trigger;
        }
    }
}
