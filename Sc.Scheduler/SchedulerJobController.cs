using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Exceptions;
using Sc.Scheduler.Model;
using Sc.Scheduler.Model.Events;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler
{
    public class SchedulerJobController : ISchedulerJobContoller
    {
        private readonly IScheduler _scheduler;
        
        private readonly TriggerKey _triggerKey;
        
        private readonly ISchedulerEventBus _schedulerEventBus;
        
        private bool _disposed;
        
        private readonly Action _triggerDeletedSubscription;

        private int _rescheduledCount;

        internal SchedulerJobController(IScheduler scheduler, TriggerKey triggerKey, ISchedulerEventBus schedulerEventBus)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            if (triggerKey == null) throw new ArgumentNullException("triggerKey");

            if (schedulerEventBus == null) throw new ArgumentNullException("schedulerEventBus");
            
            _scheduler = scheduler;
           
            _triggerKey = triggerKey;

            _schedulerEventBus = schedulerEventBus;

            _triggerDeletedSubscription = _schedulerEventBus.Subscribe<TriggerDeletedEvent>(OnTriggerDeleted);
        }

        public bool Pause()
        {
            ThrowIfDisposed();

            var trigger = _scheduler.GetTrigger(_triggerKey);

            if (trigger != null)
            {
                _scheduler.PauseTrigger(trigger.Key);

                while (_scheduler.GetTriggerState(trigger.Key) != TriggerState.Paused)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                }

                return true;
            }

            return false;
        }

        
        public bool Resume()
        {
            ThrowIfDisposed();

            var trigger = _scheduler.GetTrigger(_triggerKey);

            if (trigger != null)
            {
                if (_scheduler.GetTriggerState(_triggerKey) == TriggerState.Paused)
                {
                    _scheduler.ResumeTrigger(_triggerKey);

                    return true;
                }
            }

            return false;
        }


        public void Reschedule(Action<ITriggerScheduleConfiguration<ITriggerConfiguration>> triggerSchedule, bool cancelAllTasks = false)
        {
            ThrowIfDisposed();

            if (triggerSchedule == null) throw new ArgumentNullException("triggerSchedule");

            var trigger = _scheduler.GetTrigger(_triggerKey);

            if (trigger != null)
            {
                var schedulerTriggerContext = trigger.GetTriggerContext();

                var triggerConfiguration = (TriggerConfiguration)schedulerTriggerContext.TriggerConfiguration;

                triggerSchedule(triggerConfiguration);
                
                var newTrigger = triggerConfiguration.Build(schedulerTriggerContext);

                _schedulerEventBus.Publish(new TriggerDeletedEvent(schedulerTriggerContext, cancelAllTasks));

                Interlocked.Increment(ref _rescheduledCount);

                _schedulerEventBus.Publish(new TriggerCreatedEvent(schedulerTriggerContext, _rescheduledCount));
                
                _scheduler.RescheduleJob(_triggerKey, newTrigger);

                return;
            }
           
            throw new TriggerNotFoundException(_triggerKey);
        }

        public TriggerState State
        {
            get
            {
                if (!_disposed)
                {
                    var state = _scheduler.GetTriggerState(_triggerKey);

                    return state;
                }

                return TriggerState.Complete;
            }
        }

        public void Dispose()
        {
            Dispose(false);
        }

        public void Dispose(bool cancelAllTasks)
        {
            if (!_disposed)
            {
               _disposed = true;

               var trigger = _scheduler.GetTrigger(_triggerKey);

                var triggerContext = trigger.GetTriggerContext();
                
               _scheduler.UnscheduleJob(_triggerKey);

                var triggerDeletedSubscription = _triggerDeletedSubscription;

                if (triggerDeletedSubscription != null)
                {
                    triggerDeletedSubscription();
                }

                _schedulerEventBus.Publish(new TriggerDeletedEvent(triggerContext, cancelAllTasks));
            }
        }

        private void OnTriggerDeleted(TriggerDeletedEvent triggerDeletedEvent)
        {
            if (!_disposed)
            {
                _disposed = true;

                var triggerDeletedSubscription = _triggerDeletedSubscription;

                if (triggerDeletedSubscription != null)
                {
                    triggerDeletedSubscription();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}