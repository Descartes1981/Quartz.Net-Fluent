using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model;
using Sc.Scheduler.Model.Events;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler
{
    public class Scheduler : IAdvancedScheduler
    {
        private readonly ISchedulerDispatcherFactory _dispatcherFactory;
        
        private readonly ISchedulerEventBus _schedulerEventBus;
        
        private readonly ISchedulerTriggerEventBroker _schedulerTriggerEventBroker;

        private readonly IScheduler _scheduler;

        private bool _disposed;
        
        public Scheduler(IScheduler scheduler, IJobFactory jobFactory, ISchedulerDispatcherFactory dispatcherFactory, ISchedulerEventBus schedulerEventBus, ISchedulerTriggerEventBroker schedulerTriggerEventBroker, ISchedulerLogger schedulerLogger)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            if (jobFactory == null) throw new ArgumentNullException("jobFactory");

            if (dispatcherFactory==null) throw new ArgumentNullException("dispatcherFactory");

            if (schedulerEventBus == null) throw new ArgumentNullException("schedulerEventBus");

            if (schedulerTriggerEventBroker == null) throw new ArgumentNullException("schedulerTriggerEventBroker");

            if (schedulerEventBus == null) throw new ArgumentNullException("schedulerEventBus");

            if (schedulerLogger == null) throw new ArgumentNullException("schedulerLogger");
            
            _dispatcherFactory = dispatcherFactory;
            
            _schedulerEventBus = schedulerEventBus;
            
            _schedulerTriggerEventBroker = schedulerTriggerEventBroker;

            _scheduler = scheduler;

            _scheduler.JobFactory = jobFactory;
        }

        public ISchedulerJobContoller Schedule(Action<ISchedulerJobInfo, CancellationToken> action, Action<ITriggerConfiguration> triggerConfiguration)
        {
            if (action == null) throw new ArgumentNullException("action");

            if (triggerConfiguration == null) throw new ArgumentNullException("triggerConfiguration");

            ThrowIfDisposed();

            return ScheduleInternal((info, token) => SchedulerTaskHelpers.ExecuteSynchronously(() => action(info, token)), triggerConfiguration);
        }

        public ISchedulerJobContoller Schedule(Func<ISchedulerJobInfo, CancellationToken, Task> action, Action<ITriggerConfiguration> triggerConfiguration)
        {
            if (action==null) throw new ArgumentNullException("action");

            if (triggerConfiguration == null) throw new ArgumentNullException("triggerConfiguration");

            ThrowIfDisposed();

            return ScheduleInternal(action, triggerConfiguration);
        }

        private ISchedulerJobContoller ScheduleInternal(Func<ISchedulerJobInfo, CancellationToken, Task> action, Action<ITriggerConfiguration> triggerConfiguration)
        {
            Initialize();

            var triggerConfiguration1 = new TriggerConfiguration(_scheduler);

            triggerConfiguration(triggerConfiguration1);

            var schedulerTriggerContext = new SchedulerTriggerContext
            {
                TriggerName = triggerConfiguration1.Name,
                TriggerGroup = triggerConfiguration1.Group,
                TriggerConfiguration = triggerConfiguration1,
                OnJobTriggered = action,
            };

            var trigger = triggerConfiguration1.Build(schedulerTriggerContext);

            var emptyjob = JobBuilder.Create().Build();

            _scheduler.ListenerManager.AddTriggerListener(_schedulerTriggerEventBroker, KeyMatcher<TriggerKey>.KeyEquals(trigger.Key));

            _schedulerEventBus.Publish(new TriggerCreatedEvent(schedulerTriggerContext));
            
            var dt = _scheduler.ScheduleJob(emptyjob, trigger);

            return new SchedulerJobController(_scheduler, trigger.Key, _schedulerEventBus);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                
                if (_scheduler != null)
                {
                    _scheduler.Shutdown(false);
                }

                if (_dispatcherFactory != null)
                {
                    _dispatcherFactory.Dispose();
                }
                if (_schedulerEventBus != null)
                {
                    _schedulerEventBus.Dispose();
                }
            }
        }

        private void Initialize()
        {
            lock (typeof(Scheduler))
            {
                if (!_scheduler.IsStarted)
                {
                    _scheduler.Start();
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
