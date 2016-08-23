using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Test.Utils;

namespace Sc.Scheduler.Test.UnitTest
{
    [TestClass]
    public class SchedulerDispatcherTest
    {
        private readonly Mock<ISchedulerLogger> _schedulerLoggerMock = new Mock<ISchedulerLogger>();

        private readonly Mock<ITestInvokeCallback<object>> _invokeForActionTestMock = new Mock<ITestInvokeCallback<object>>();

        [TestMethod]
        public void TestDisposeWaitsForJobs()
        {
            var sleepTime = TimeSpan.FromSeconds(2);

            var disposeStopWatch = new Stopwatch();

            using (var schedulerDispatcher = new SchedulerDispatcher(_schedulerLoggerMock.Object))
            {
                var actionStartedTaskCompletionSource = new TaskCompletionSource<object>();

                _invokeForActionTestMock.Setup(callback => callback.Invoke(It.IsAny<object>())).Callback(() =>
                {
                    actionStartedTaskCompletionSource.SetResult(null);

                    Task.Delay(sleepTime).Wait();
                });

                var invokeForActionTest = _invokeForActionTestMock.Object;

                schedulerDispatcher.Enqueue(() => invokeForActionTest.Invoke(new object()));

                actionStartedTaskCompletionSource.Task.Wait();

                disposeStopWatch.Start();
            }

            disposeStopWatch.Stop();

            Assert.AreEqual(sleepTime, disposeStopWatch.Elapsed.Round(0));
        }

        [TestMethod]
        public void TestEnqueue()
        {
            var obj = new object();

            using (var schedulerDispatcher = new SchedulerDispatcher(_schedulerLoggerMock.Object))
            {
                var actionStartedTaskCompletionSource = new TaskCompletionSource<object>();

                var invokeForActionTest = _invokeForActionTestMock.Object;

                schedulerDispatcher.Enqueue(() =>
                {
                    actionStartedTaskCompletionSource.SetResult(null);

                    invokeForActionTest.Invoke(obj);
                });

                actionStartedTaskCompletionSource.Task.Wait();
            }

            _invokeForActionTestMock.Verify(callback => callback.Invoke(obj), Times.Once);
        }

        [TestMethod]
        public void TestActionException()
        {
            using (var schedulerDispatcher = new SchedulerDispatcher(_schedulerLoggerMock.Object))
            {
                _invokeForActionTestMock.Setup(callback => callback.Invoke(It.IsAny<object>())).Throws<TestException>();

                var invokeForActionTest = _invokeForActionTestMock.Object;

                schedulerDispatcher.Enqueue(() => invokeForActionTest.Invoke(new object()));
            }

            _schedulerLoggerMock.Verify(logger => logger.ErrorWrite(It.IsAny<TestException>(), It.Is<string>(s => s.IndexOf("failed", StringComparison.InvariantCultureIgnoreCase)>-1)));
        }

        [TestMethod]
        public void TestCancellation()
        {
            using (var schedulerDispatcher = new SchedulerDispatcher(_schedulerLoggerMock.Object))
            {
                var actionStartedTaskCompletionSource = new TaskCompletionSource<object>();

                _invokeForActionTestMock.Setup(callback => callback.Invoke(It.IsAny<object>())).Callback(() =>
                {
                    actionStartedTaskCompletionSource.TrySetResult(null);

                    throw new OperationCanceledException();
                });

                var invokeForActionTest = _invokeForActionTestMock.Object;

                schedulerDispatcher.Enqueue(() => invokeForActionTest.Invoke(new object()));

                actionStartedTaskCompletionSource.Task.Wait();
            }

            _schedulerLoggerMock.Verify(logger => logger.ErrorWrite(It.IsAny<OperationCanceledException>(), It.Is<string>(s => s.IndexOf("cancelled", StringComparison.InvariantCultureIgnoreCase)>-1)));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorArguments()
        {
            //new SchedulerDispatcher(null);
            try
            {
                Activator.CreateInstance(typeof(SchedulerDispatcher), new object[] { null });
            }
            catch (TargetInvocationException targetInvocationException)
            {
                throw targetInvocationException.InnerException;
            }
            
            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEnqueueArguments()
        {
            var schedulerDispatcher = new SchedulerDispatcher(_schedulerLoggerMock.Object);

            schedulerDispatcher.Enqueue(null);
        }
    }
}
