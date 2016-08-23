using System;
using Quartz;
using Quartz.Spi;

namespace Sc.Scheduler.Test.UnitTest
{
    internal class OperableTriggerMock : IOperableTrigger
    {
        public JobDataMap JobDataMap { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ITrigger other)
        {
            throw new NotImplementedException();
        }

        public TriggerBuilder GetTriggerBuilder()
        {
            throw new NotImplementedException();
        }

        public IScheduleBuilder GetScheduleBuilder()
        {
            throw new NotImplementedException();
        }

        public bool GetMayFireAgain()
        {
            throw new NotImplementedException();
        }

        public DateTimeOffset? GetNextFireTimeUtc()
        {
            throw new NotImplementedException();
        }

        public DateTimeOffset? GetPreviousFireTimeUtc()
        {
            throw new NotImplementedException();
        }

        public DateTimeOffset? GetFireTimeAfter(DateTimeOffset? afterTime)
        {
            throw new NotImplementedException();
        }

        TriggerKey IMutableTrigger.Key { get; set; }
        
        JobKey IMutableTrigger.JobKey { get; set; }
        
        string IMutableTrigger.Description { get; set; }
        
        string IMutableTrigger.CalendarName { get; set; }

        JobDataMap IMutableTrigger.JobDataMap
        {
            get { return JobDataMap; }
            set { JobDataMap = value; }
        }

        TriggerKey ITrigger.Key { get { throw new NotImplementedException(); } }
        
        JobKey ITrigger.JobKey { get { throw new NotImplementedException(); } }
        
        string ITrigger.Description { get { throw new NotImplementedException(); } }

        string ITrigger.CalendarName { get { throw new NotImplementedException(); } }

        JobDataMap ITrigger.JobDataMap { get { return JobDataMap; } }
        
        public DateTimeOffset? FinalFireTimeUtc { get; private set; }
        
        int IMutableTrigger.MisfireInstruction { get; set; }
        
        int ITrigger.MisfireInstruction { get { throw new NotImplementedException(); } }
        
        DateTimeOffset? IMutableTrigger.EndTimeUtc { get; set; }
        
        DateTimeOffset? ITrigger.EndTimeUtc { get { throw new NotImplementedException(); } }
        
        DateTimeOffset IMutableTrigger.StartTimeUtc { get; set; }
        
        DateTimeOffset ITrigger.StartTimeUtc { get { throw new NotImplementedException(); } }
        
        int IMutableTrigger.Priority { get; set; }
        
        int ITrigger.Priority { get; set; }
        
        public bool HasMillisecondPrecision { get; private set; }
        
        public void Triggered(ICalendar calendar)
        {
            throw new NotImplementedException();
        }

        public DateTimeOffset? ComputeFirstFireTimeUtc(ICalendar calendar)
        {
            throw new NotImplementedException();
        }

        public SchedulerInstruction ExecutionComplete(IJobExecutionContext context, JobExecutionException result)
        {
            throw new NotImplementedException();
        }

        public void UpdateAfterMisfire(ICalendar cal)
        {
            throw new NotImplementedException();
        }

        public void UpdateWithNewCalendar(ICalendar cal, TimeSpan misfireThreshold)
        {
            throw new NotImplementedException();
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }

        public void SetNextFireTimeUtc(DateTimeOffset? value)
        {
            throw new NotImplementedException();
        }

        public void SetPreviousFireTimeUtc(DateTimeOffset? value)
        {
            throw new NotImplementedException();
        }

        public string FireInstanceId { get; set; }
    }
}
