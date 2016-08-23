using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Model
{
    internal class SchedulerConfiguration : ISchedulerConfiguration
    {
        public int MaxDegreeOfParallelism;

        public ISchedulerConfiguration WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism;

            return this;
        }
    }
}
