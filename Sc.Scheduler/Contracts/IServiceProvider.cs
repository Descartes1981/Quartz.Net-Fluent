namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerServiceProvider
    {
      
        TService Resolve<TService>() where TService : class;
    }
}