using System;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerServiceRegister
    {
        ISchedulerServiceRegister Register<TService>(Func<ISchedulerServiceProvider, TService> serviceCreator) where TService : class;

        ISchedulerServiceRegister Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
    }
}