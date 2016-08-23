using System;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model;
using Sc.Scheduler.Utils;


namespace Sc.Scheduler
{
    internal class SchedulerRegistrations
    {
        private readonly ISchedulerContainer _schedulerContainer;
        
        readonly SchedulerConfiguration _schedulerConfiguration1;

        public SchedulerRegistrations(ISchedulerContainer schedulerContainer, SchedulerConfiguration schedulerConfiguration1 = null)
        {
            if (schedulerContainer==null) throw new ArgumentNullException("schedulerContainer");

            _schedulerContainer = schedulerContainer;
            
            _schedulerConfiguration1 = schedulerConfiguration1;
        }

        public void Register()
        {
            _schedulerContainer.Register<ISchedulerDispatcherFactory, SchedulerDispatcherFactory>()
                .Register<ISchedulerEventBus, SchedulerEventBus>()
                .Register<ISchedulerLogger, SchedulerNulllLoger>()
                .Register<ISchedulerJobExecuter, SchedulerJobExecuter>()
                .Register<ISchedulerJobCompletedStrategy, SchedulerJobCompletedNullStrategy>()
                .Register<IJobFactory, SchedulerJobFactory>()
                .Register<IScheduler>(provider => StdSchedulerFactory.GetDefaultScheduler())
                .Register<ISchedulerTriggerEventBroker, SchedulerTriggerEventBroker>()
                .Register<IAdvancedScheduler, Scheduler>();

            if (_schedulerConfiguration1 != null)
            {
                _schedulerContainer.Register(provider => _schedulerConfiguration1);
            }
        }
    }
}
