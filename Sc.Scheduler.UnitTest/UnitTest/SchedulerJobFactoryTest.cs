using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Quartz;
using Quartz.Spi;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model;
using Sc.Scheduler.Model.Events;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler.Test.UnitTest
{
    [TestClass]
    public class SchedulerJobFactoryTest
    {
        private readonly Mock<ISchedulerDispatcherFactory> _schedulerDispatcherFatoryMock = new Mock<ISchedulerDispatcherFactory>();

        private readonly Mock<ISchedulerJobExecuter> _schedulerJobExecuterMock = new Mock<ISchedulerJobExecuter>();

        private readonly Mock<ISchedulerEventBus> _schedulerEventBusMock = new Mock<ISchedulerEventBus>();

        private readonly Mock<ISchedulerLogger> _schedulerLoggerMock = new Mock<ISchedulerLogger>();

        private readonly IOperableTrigger _operableTriggerMock = new OperableTriggerMock();

        private readonly Mock<IScheduler> _schedulerMock = new Mock<IScheduler>();

        private readonly Mock<ISchedulerTriggerContext> _schedulerTriggerContextMock = new Mock<ISchedulerTriggerContext>();
        
        private readonly SchedulerJobFactory _schedulerJobFactory;

        public SchedulerJobFactoryTest()
        {
            _schedulerJobFactory = new SchedulerJobFactory(_schedulerDispatcherFatoryMock.Object, _schedulerJobExecuterMock.Object, _schedulerEventBusMock.Object, _schedulerLoggerMock.Object);
        }
            
        [TestInitialize]
        public void Setup()
        {
            _schedulerDispatcherFatoryMock.Reset();

            _schedulerJobExecuterMock.Reset();

            _schedulerEventBusMock.Reset();

            _schedulerLoggerMock.Reset();

            _schedulerTriggerContextMock.Reset();

            _schedulerMock.Reset();

            IDictionary dictionary = new Dictionary<string, object>();

            dictionary.Add(SchedulerKeysConstants.TriggerInfo, _schedulerTriggerContextMock.Object);
            
            var jobDataMap = new JobDataMap(dictionary);

            _operableTriggerMock.JobDataMap = jobDataMap;
        }

        [TestMethod]
        public void TestInitializtion()
        {
            using (new SchedulerJobFactory(_schedulerDispatcherFatoryMock.Object, _schedulerJobExecuterMock.Object, _schedulerEventBusMock.Object, _schedulerLoggerMock.Object))
            
            _schedulerEventBusMock.Verify(bus => bus.Subscribe(It.IsAny<Action<TriggerCreatedEvent>>()), Times.Once);

            _schedulerEventBusMock.Verify(bus => bus.Subscribe(It.IsAny<Action<TriggerDeletedEvent>>()), Times.Once);
        }


        [TestMethod]
        public void TestNewJobNoEventFired()
        {
            var triggerFiredBundle = new TriggerFiredBundle(
                new Mock<IJobDetail>().Object,
                _operableTriggerMock,
                new Mock<ICalendar>().Object,
                false, null, null, null, null);

            var job = _schedulerJobFactory.NewJob(triggerFiredBundle, _schedulerMock.Object);

            Assert.IsInstanceOfType(job, typeof(DummyJob));
        }


        [TestMethod]
        public void TestNewJobTriggerCreatedEventFired()
        {
            var eventBus = new SchedulerEventBus();

            var schedulerJobFactory = new SchedulerJobFactory(_schedulerDispatcherFatoryMock.Object, _schedulerJobExecuterMock.Object, eventBus, _schedulerLoggerMock.Object);

            var triggerFiredBundle = new TriggerFiredBundle(
                new Mock<IJobDetail>().Object,
                _operableTriggerMock,
                new Mock<ICalendar>().Object,
                false, null, null, null, null);

            eventBus.Publish(new TriggerCreatedEvent(_schedulerTriggerContextMock.Object));

            var job = schedulerJobFactory.NewJob(triggerFiredBundle, _schedulerMock.Object);

            Assert.IsInstanceOfType(job, typeof(SchedulerJob));
        }


        [TestMethod]
        public void TestNewJobTriggerDeletedEventFired()
        {
            var eventBus = new SchedulerEventBus();

            var schedulerJobFactory = new SchedulerJobFactory(_schedulerDispatcherFatoryMock.Object, _schedulerJobExecuterMock.Object, eventBus, _schedulerLoggerMock.Object);

            var triggerFiredBundle = new TriggerFiredBundle(
                new Mock<IJobDetail>().Object,
                _operableTriggerMock,
                new Mock<ICalendar>().Object,
                false, null, null, null, null);

            eventBus.Publish(new TriggerCreatedEvent(_schedulerTriggerContextMock.Object));

            var job = schedulerJobFactory.NewJob(triggerFiredBundle, _schedulerMock.Object);

            Assert.IsInstanceOfType(job, typeof(SchedulerJob));

            eventBus.Publish(new TriggerDeletedEvent(_schedulerTriggerContextMock.Object, false));

            job = schedulerJobFactory.NewJob(triggerFiredBundle, _schedulerMock.Object);

            Assert.IsInstanceOfType(job, typeof(DummyJob));
        }
    }
}
