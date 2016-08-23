using System;
using Sc.Scheduler.Model;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerFactory
    {
        IAdvancedScheduler Create(Action<ISchedulerServiceRegister> registerServices, Action<ISchedulerConfiguration> schedulerConfiguration);
        IAdvancedScheduler Create(Action<ISchedulerServiceRegister> registerServices);
        IAdvancedScheduler Create(Action<ISchedulerConfiguration> schedulerConfiguration);
        IAdvancedScheduler Create();
    }
}