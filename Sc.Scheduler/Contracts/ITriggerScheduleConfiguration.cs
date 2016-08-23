using System;
using Quartz;

namespace Sc.Scheduler.Contracts
{
    public interface ITriggerScheduleConfiguration<out TBuilder> where TBuilder : ITriggerScheduleConfiguration<TBuilder>
    {
        TBuilder WithStartDelay(TimeSpan delay);
        
        TBuilder WithInterval(TimeSpan period);
        
        TBuilder WithMaximumIteration(int maxIterations);
        
        TBuilder WithCronExpression(string cronExpression);
    }

    public interface ITriggerScheduleConfiguration : ITriggerScheduleConfiguration<ITriggerScheduleConfiguration>
    {
        
    }
}