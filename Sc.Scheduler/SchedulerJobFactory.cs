using System;
using System.Collections.Concurrent;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Exceptions;
using Sc.Scheduler.Model.Events;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler
{
    internal class SchedulerJobFactory : SimpleJobFactory, IDisposable
    {
        private readonly ISchedulerDispatcherFactory _schedulerDispatcherFactory;
        
        private readonly ISchedulerJobExecuter _schedulerJobExecuter;

        private readonly ISchedulerLogger _schedulerLogger;

        private readonly ConcurrentDictionary<Guid, ISchedulerJob> _jobsStore = new ConcurrentDictionary<Guid, ISchedulerJob>();

        private readonly Action _onTriggerCreatedSubscription;
        
        private readonly Action _onTriggerDeletedSubscription;

        private bool _disposed;
        
        public SchedulerJobFactory(ISchedulerDispatcherFactory schedulerDispatcherFactory, ISchedulerJobExecuter schedulerJobExecuter, ISchedulerEventBus schedulerEventBus, ISchedulerLogger schedulerLogger)
        {
            if (schedulerDispatcherFactory == null) throw new ArgumentNullException("schedulerDispatcherFactory");

            if (schedulerJobExecuter == null) throw new ArgumentNullException("schedulerJobExecuter");

            if (schedulerLogger == null) throw new ArgumentNullException("schedulerLogger");

            _schedulerDispatcherFactory = schedulerDispatcherFactory;
            
            _schedulerJobExecuter = schedulerJobExecuter;

            _schedulerLogger = schedulerLogger;

            _onTriggerCreatedSubscription = schedulerEventBus.Subscribe<TriggerCreatedEvent>(OnTriggerCreated);

            _onTriggerDeletedSubscription = schedulerEventBus.Subscribe<TriggerDeletedEvent>(OnTriggerDeleted);
        }

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var triggerContext = bundle.Trigger.GetTriggerContext();

            if (triggerContext == null)
            {
                throw new TriggerContextNotSetException(bundle.Trigger.Key.Name, bundle.Trigger.Key.Group);
            }

            ISchedulerJob schedulerJob;

            if (_jobsStore.TryGetValue(triggerContext.TriggerId, out schedulerJob))
            {
                if (schedulerJob != null)
                {
                    return schedulerJob;
                }
            }

            return new DummyJob();
        }

        public override void ReturnJob(IJob job)
        {
        }

        
        private void OnTriggerCreated(TriggerCreatedEvent triggerCreatedEvent)
        {
            var triggerId = triggerCreatedEvent.TriggerContext.TriggerId;

            _jobsStore.TryAdd(triggerId, new SchedulerJob(_schedulerDispatcherFactory, _schedulerJobExecuter, _schedulerLogger, triggerCreatedEvent.RescheduledCount));
        }

        private void OnTriggerDeleted(TriggerDeletedEvent triggerDeletedEvent)
        {
            var triggerId = triggerDeletedEvent.TriggerContext.TriggerId;

            ISchedulerJob schedulerJob;

            if (_jobsStore.TryRemove(triggerId, out schedulerJob))
            {
                if (schedulerJob != null)
                {
                    schedulerJob.Dispose(triggerDeletedEvent.CancelAllTasks);
                }
            }
        }
        
        public void Dispose()
        {
            if (_disposed)
            {
                _disposed = true;

                var onTriggerCreatedSubscription = _onTriggerCreatedSubscription;

                if (onTriggerCreatedSubscription != null)
                {
                    onTriggerCreatedSubscription();
                }

                var onTriggerDeletedSubscription = _onTriggerDeletedSubscription;

                if (onTriggerDeletedSubscription != null)
                {
                    onTriggerDeletedSubscription();
                }

                if (_jobsStore != null)
                {
                    _jobsStore.Clear();
                }
            }
        }
    }
}
