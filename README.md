# Quartz.Net-Fluent

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Test
{
    [TestClass]
    public class SchedulerFunctionalTest
    {
        [TestMethod]
        public void TestDisposeWithCancellation()
        {
            var factory = new SchedulerFactory();

            var schedulerJobCompletedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var scheduler = factory.Create(register => register.Register(provider => schedulerJobCompletedStrategyMock.Object));

            int iterations = 0;

            var scheduled = scheduler.Schedule(async (info, token) =>
            {
                iterations++;

                await Task.Delay(TimeSpan.FromSeconds(30), token);

            }, configuration => configuration.WithInterval(TimeSpan.FromSeconds(1)));

            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            scheduled.Dispose(true);

            Task.Delay(TimeSpan.FromSeconds(2));

            schedulerJobCompletedStrategyMock.Verify(strategy => strategy.OnCanceled(It.IsAny<ISchedulerJobInfo>()), Times.Exactly(iterations));
        }

        [TestMethod]
        public void TestDisposeSingleJobWithCancellation()
        {
            var factory = new SchedulerFactory();

            var schedulerJobCompletedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var scheduler = factory.Create(register => register.Register(provider => schedulerJobCompletedStrategyMock.Object));

            int iterations = 0;

            var scheduled = scheduler.Schedule(async (info, token) =>
            {
                iterations++;

                await Task.Delay(TimeSpan.FromSeconds(30), token);

            }, configuration => configuration.WithInterval(TimeSpan.FromSeconds(50)));

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            scheduled.Dispose(true);

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();

            schedulerJobCompletedStrategyMock.Verify(strategy => strategy.OnCanceled(It.IsAny<ISchedulerJobInfo>()), Times.Exactly(iterations));
        }

        [TestMethod]

        public void TestDisposeSingleJobWithDefinedCancellationHandler()
        {
            var factory = new SchedulerFactory();

            var schedulerJobCompletedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var schedulerJobCompleteddefinedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var scheduler = factory.Create(register => register.Register(provider => schedulerJobCompletedStrategyMock.Object));

            var scheduled = scheduler.Schedule(async (info, token) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30), token);

            }, configuration => configuration.WithInterval(TimeSpan.FromSeconds(50)).WithOnJobCancelled(info => schedulerJobCompleteddefinedStrategyMock.Object.OnCanceled(info)));

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            scheduled.Dispose(true);

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();

            schedulerJobCompletedStrategyMock.Verify(strategy => strategy.OnCanceled(It.IsAny<ISchedulerJobInfo>()), Times.Never);
        }

        [TestMethod]
        public void TestDisposeWithoutCancellation()
        {
            var factory = new SchedulerFactory();

            var schedulerJobCompletedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var scheduler = factory.Create(register => register.Register(provider => schedulerJobCompletedStrategyMock.Object));

            int iterations = 0;

            var scheduled = scheduler.Schedule(async (info, token) =>
            {
                iterations++;

                await Task.Delay(TimeSpan.FromSeconds(5), token);

            }, configuration => configuration.WithInterval(TimeSpan.FromSeconds(1)));

            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            scheduled.Dispose();

            Task.Delay(TimeSpan.FromSeconds(30)).Wait();

            schedulerJobCompletedStrategyMock.Verify(strategy => strategy.OnCanceled(It.IsAny<ISchedulerJobInfo>()), Times.Never);

            schedulerJobCompletedStrategyMock.Verify(strategy => strategy.OnComplete(It.IsAny<ISchedulerJobInfo>()), Times.Exactly(iterations));
        }

        [TestMethod]
        public void TestReschedule()
        {
            var factory = new SchedulerFactory();

            var schedulerJobCompletedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var scheduler = factory.Create(register => register.Register(provider => schedulerJobCompletedStrategyMock.Object));

            var timeStamps = new List<DateTime>();

            var rescheduleCount = new List<int>();

            var scheduled = scheduler.Schedule((info, token) =>
            {
                timeStamps.Add(DateTime.Now);

                rescheduleCount.Add(info.RescheduledCount);
            }
                , configuration => configuration.WithInterval(TimeSpan.FromSeconds(1)));

            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            scheduled.Reschedule(configuration => configuration.WithStartDelay(TimeSpan.FromSeconds(2)).WithInterval(TimeSpan.FromSeconds(2)), false);
            
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            scheduled.Dispose();

            Assert.AreEqual(16, timeStamps.Count);
        }


        [TestMethod]
        public void TestPause()
        {
            var factory = new SchedulerFactory();

            var schedulerJobCompletedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var scheduler = factory.Create(register => register.Register(provider => schedulerJobCompletedStrategyMock.Object));

            var timeStamps = new List<DateTime>();

            var scheduled = scheduler.Schedule((info, token) =>
            {
                timeStamps.Add(DateTime.Now);
            }
            ,configuration => configuration.WithInterval(TimeSpan.FromSeconds(1)));

            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            scheduled.Pause();

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            Assert.AreEqual(11, timeStamps.Count);

            scheduled.Resume();

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            Assert.AreEqual(17, timeStamps.Count);
            
            scheduled.Dispose();
        }

        
        [TestMethod]
        public void TestMaxDegreeOfParallelism()
        {
            var factory = new SchedulerFactory();

            var schedulerJobCompletedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var scheduler = factory.Create(register => register.Register(provider => schedulerJobCompletedStrategyMock.Object), configuration => configuration.WithMaxDegreeOfParallelism(2));

            var scheduled = scheduler.Schedule(async (info, token) =>
            {
                Console.WriteLine("Test");

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            ,configuration => configuration.WithInterval(TimeSpan.FromMilliseconds(500)));

            Task.Delay(TimeSpan.FromSeconds(30)).Wait();

            scheduled.Dispose();
        }


        [TestMethod]
        public void TestWaitForAllTasksToComplete()
        {
            var factory = new SchedulerFactory();

            var schedulerJobCompletedStrategyMock = new Mock<ISchedulerJobCompletedStrategy>();

            var scheduler = factory.Create(register => register.Register(provider => schedulerJobCompletedStrategyMock.Object));

            var scheduled = scheduler.Schedule((info, token) =>
            {
                Console.WriteLine("Test");

                Task.Delay(TimeSpan.FromSeconds(20)).Wait();
            }
            ,configuration => configuration.WithInterval(TimeSpan.FromMilliseconds(500)));

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            scheduled.Dispose();
        }
    }
}


Quartz.Net Fluent
