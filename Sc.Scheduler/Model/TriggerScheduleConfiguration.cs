using System;
using Quartz;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler.Model
{
    public class TriggerScheduleConfiguration<TBuilder> : ITriggerScheduleConfiguration<TBuilder> where TBuilder : class, ITriggerScheduleConfiguration<TBuilder>
    {
        private bool? _startNow;

        private TimeSpan? _startWithDelay;

        private TimeSpan _interval;

        private int? _maxIterations;

        private string _cronExpression;
        

        public TimeSpan? StartWithDelay
        {
            get { return _startWithDelay; }
        }

        public TimeSpan? Interval
        {
            get { return _interval; }
        }

        public int? MaxIterations
        {
            get { return _maxIterations; }
        }

        public string CronExpression
        {
            get { return _cronExpression; }
        }

        
        public TBuilder WithStartDelay(TimeSpan delay)
        {
            _startWithDelay = delay;

            return This;
        }

        public TBuilder WithInterval(TimeSpan period)
        {
            _interval = period;

            return This;
        }

        public TBuilder WithMaximumIteration(int maxIterations)
        {
            _maxIterations = maxIterations;

            return This;
        }

        public TBuilder WithCronExpression(string cronExpression)
        {
            _cronExpression = cronExpression;

            return This;
        }

        protected TBuilder This
        {
            get { return this as TBuilder; }
        }

        public ITrigger Build(ISchedulerTriggerContext schedulerTriggerContext)
        {
            var triggerBuilder = Create(this);

            triggerBuilder.SetTriggerContext(schedulerTriggerContext);

            return triggerBuilder.Build();
        }

        protected virtual TriggerBuilder Create(TriggerScheduleConfiguration<TBuilder> triggerScheduleConfiguration)
        {
            var triggerBuilder = TriggerBuilder.Create();

            if (!String.IsNullOrEmpty(triggerScheduleConfiguration.CronExpression))
            {
                triggerBuilder = triggerBuilder.WithCronSchedule(triggerScheduleConfiguration.CronExpression);
            }
            else
            {
                triggerBuilder = triggerBuilder.WithSimpleSchedule(builder =>
                {
                    if (triggerScheduleConfiguration.Interval.HasValue)
                    {
                        builder.WithInterval(triggerScheduleConfiguration.Interval.Value);
                    }

                    if (triggerScheduleConfiguration.MaxIterations.HasValue)
                    {
                        builder.WithRepeatCount(triggerScheduleConfiguration.MaxIterations.Value);
                    }
                    else
                    {
                        builder.RepeatForever();
                    }

                    builder.WithMisfireHandlingInstructionIgnoreMisfires();
                });

                if (triggerScheduleConfiguration.StartWithDelay.HasValue)
                {
                    triggerBuilder = triggerBuilder.StartAt(DateTime.Now + triggerScheduleConfiguration.StartWithDelay.Value);
                }
                else
                {
                    triggerBuilder.StartNow();
                }
            }

            return triggerBuilder;
        }
    }

    internal class TriggerScheduleConfiguration : TriggerScheduleConfiguration<ITriggerScheduleConfiguration>, ITriggerScheduleConfiguration
    {
           
    }
}