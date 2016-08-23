namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerConfiguration
    {
        ISchedulerConfiguration WithMaxDegreeOfParallelism(int maxDegreeOfParallelism);
    }
}