using System;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Model
{
    internal class SchedulerJobInfo : ISchedulerJobInfo
    {
        public string TriggerName { get; internal set; }
        
        public string GroupName { get; internal set; }
        
        public DateTime PublishedTimeUtc { get; internal set; }

        public object AdditionalArguments { get; internal set; }
        
        public DateTimeOffset ExpectedStartTime { get; internal set; }
        
        public int RefireCount { get; internal set; }
        
        public int RescheduledCount { get; internal set; }
        
        public DateTimeOffset? PreviousFireTimeUtc { get; set; }
        
        public DateTimeOffset? NextFireTimeUtc { get; set; }
        
        public bool IsLastExectution { get; set; }
        
        public int? RemainingCount { get; set; }
    }
}
