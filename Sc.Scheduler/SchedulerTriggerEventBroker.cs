using System;
using Quartz;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model.Events;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler
{
    internal class SchedulerTriggerEventBroker : ISchedulerTriggerEventBroker
    {
        private readonly ISchedulerEventBus _schedulerEventBus;

        private readonly string _guid = Guid.NewGuid().ToString();

        public SchedulerTriggerEventBroker(ISchedulerEventBus schedulerEventBus)
        {
            if (schedulerEventBus==null) throw new ArgumentNullException("schedulerEventBus");

            _schedulerEventBus = schedulerEventBus;
        }

        public void TriggerFired(ITrigger trigger, IJobExecutionContext context)
        {
            _schedulerEventBus.Publish(new TriggerFiredEvent(trigger));
        }

        public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context)
        {
            return false;
        }

        public void TriggerMisfired(ITrigger trigger)
        {
            
        }

        public void TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode)
        {
            switch (triggerInstructionCode)
            {
                case SchedulerInstruction.DeleteTrigger:
                    _schedulerEventBus.Publish(new TriggerDeletedEvent(trigger.GetTriggerContext(), false));
                    break;
            }
        }

        public string Name
        {
            get { return _guid; }
        }
    }
}
