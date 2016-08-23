using System;
using System.Threading;
using Quartz ;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler
{
    internal class SchedulerJob : ISchedulerJob
    {
        private readonly ISchedulerDispatcherFactory _schedulerDispatcherFactory;
        
        private readonly ISchedulerJobExecuter _schedulerJobExecuter;

        private readonly ISchedulerLogger _schedulerLogger;
        
        private readonly int _rescheduledCount;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        private bool _disposed;

        public SchedulerJob(ISchedulerDispatcherFactory schedulerDispatcherFactory, ISchedulerJobExecuter schedulerJobExecuter, ISchedulerLogger schedulerLogger, int rescheduledCount)
        {
            if (schedulerDispatcherFactory==null) throw new ArgumentNullException("schedulerDispatcherFactory");

            if (schedulerJobExecuter == null) throw new ArgumentNullException("schedulerJobExecuter");

            if (schedulerLogger == null) throw new ArgumentNullException("schedulerLogger");

            if (rescheduledCount < 0)
            {
                throw new ArgumentException("Reschedule count can't be less than zero (0).", "rescheduledCount");
            }

            _schedulerDispatcherFactory = schedulerDispatcherFactory;
            
            _schedulerJobExecuter = schedulerJobExecuter;

            _schedulerLogger = schedulerLogger;
            
            _rescheduledCount = rescheduledCount;
        }

        public void Execute(IJobExecutionContext context)
        {
            if (_disposed) return;

            if (IsMissedFire(context, 500))
            {
                return;
            }
            
            var triggerContext = context.Trigger.GetTriggerContext();

            _schedulerLogger.DebugWrite("Firing trigger with name: {0}, group: {1}, description: {2}.", context.Trigger.Key.Name, !string.IsNullOrEmpty(context.Trigger.Key.Group) ? context.Trigger.Key.Group : "NULL", !string.IsNullOrEmpty(context.Trigger.Description) ? context.Trigger.Description : "NULL");

            var dispatcher = _schedulerDispatcherFactory.CreateDispatcher(triggerContext.TriggerId);

            var jobContext = new JobExecutionContext(triggerContext.OnJobTriggered, _cancellationTokenSource.Token, new SchedulerJobInfo
            {
                TriggerName = triggerContext.TriggerName,

                GroupName = triggerContext.TriggerGroup,

                PublishedTimeUtc = DateTime.Now.ToUniversalTime(),

                ExpectedStartTime = context.Trigger.StartTimeUtc,

                PreviousFireTimeUtc = context.PreviousFireTimeUtc,

                NextFireTimeUtc = context.NextFireTimeUtc,

                IsLastExectution = !context.Trigger.GetMayFireAgain(),

                RefireCount = context.RefireCount,

                RescheduledCount = _rescheduledCount,

                RemainingCount = triggerContext.TriggerConfiguration.MaxIterations.HasValue ? triggerContext.TriggerConfiguration.MaxIterations - context.RefireCount : null
            }, triggerContext.TriggerConfiguration);

            dispatcher.Value.Enqueue(() => _schedulerJobExecuter.Execute(jobContext));

            _schedulerLogger.DebugWrite("Action enqued for trigger with name: {0}, group: {1}, description: {2}.", context.Trigger.Key.Name, !string.IsNullOrEmpty(context.Trigger.Key.Group) ? context.Trigger.Key.Group : "NULL", !string.IsNullOrEmpty(context.Trigger.Description) ? context.Trigger.Description : "NULL");
        }

        public void Dispose()
        {
            Dispose(false);
        }

        public void Dispose(bool cancellAllTasks)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (cancellAllTasks)
                {
                    _cancellationTokenSource.Cancel();
                }

                _cancellationTokenSource.Dispose();
            }
        }

        private static bool IsMissedFire(IJobExecutionContext context, int offsetMilliseconds)
        {
            if (!context.ScheduledFireTimeUtc.HasValue)
                return false;
            if (!context.FireTimeUtc.HasValue)
                return false;

            var scheduledFireTimeUtc = context.ScheduledFireTimeUtc.Value;
            
            var fireTimeUtc = context.FireTimeUtc.Value;

            return fireTimeUtc.Subtract(scheduledFireTimeUtc).TotalMilliseconds > offsetMilliseconds;
        }
    }
}
