using System;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerJobInfo
    {
        string TriggerName { get; }
        string GroupName { get; }
        DateTime PublishedTimeUtc { get; }
        object AdditionalArguments { get; }
        DateTimeOffset ExpectedStartTime { get; }
        int RefireCount { get; }
        int RescheduledCount { get; }
        DateTimeOffset? PreviousFireTimeUtc { get; set; }
        DateTimeOffset? NextFireTimeUtc { get; set; }
        bool IsLastExectution { get; set; }
    }
}