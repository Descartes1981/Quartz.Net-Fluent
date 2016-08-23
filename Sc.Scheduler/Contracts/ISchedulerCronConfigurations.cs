using System;

namespace Sc.Scheduler.Contracts
{
    internal interface ISchedulerCronConfigurations
    {
        ISchedulerCronConfigurations OnMinute(params int[] minutes);
        
        ISchedulerCronConfigurations OnHour(params int[] hours);
        
        ISchedulerCronConfigurations OnDayOfMonth(params int[] daysOfMonth);
        
        ISchedulerCronConfigurations OnMonth(params int[] months);
        
        ISchedulerCronConfigurations OnDayOfWeek(params DayOfWeek[] daysOfWeek);
    }
}