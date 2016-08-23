using System;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model;

namespace Sc.Scheduler
{
    public class SchedulerFactory : ISchedulerFactory
    {
        private readonly ISchedulerContainer _schedulerContainer = new DefaultServiceProvider();
        
        public IAdvancedScheduler Create(Action<ISchedulerServiceRegister> registerServices, Action<ISchedulerConfiguration> schedulerConfiguration)
        {
            if (registerServices == null) throw new ArgumentNullException("registerServices");

            if (schedulerConfiguration == null) throw new ArgumentNullException("schedulerConfiguration");

            registerServices(_schedulerContainer);

            var schedulerConfiguration1 = new SchedulerConfiguration();

            schedulerConfiguration(schedulerConfiguration1);
            
            var schedulerRegistrations = new SchedulerRegistrations(_schedulerContainer, schedulerConfiguration1);

            schedulerRegistrations.Register();

            return _schedulerContainer.Resolve<IAdvancedScheduler>();
        }

        public IAdvancedScheduler Create(Action<ISchedulerServiceRegister> registerServices)
        {
            if (registerServices == null) throw new ArgumentNullException("registerServices");

            return Create(registerServices, configuration => { });
        }

        public IAdvancedScheduler Create(Action<ISchedulerConfiguration> schedulerConfiguration)
        {
            return Create(register => { }, schedulerConfiguration);
        }

        public IAdvancedScheduler Create()
        {
            return Create(configuration => { }, configuration => { });
        }
    }
}
