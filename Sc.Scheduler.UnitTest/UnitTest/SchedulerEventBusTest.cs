using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Sc.Scheduler.Test.UnitTest
{
    [TestClass]
    public class SchedulerEventBusTest
    {
        [TestMethod]
        public void TestPublisSubscribe()
        {
            var invokerMock = new Mock<IMockInvoker<TestEvent>>();

            var invoker = invokerMock.Object;

            var eventBus = new SchedulerEventBus();

            eventBus.Subscribe<TestEvent>(invoker.Invoke);

            for (int i = 0; i < 10; i++)
            {
                eventBus.Publish(new TestEvent());
            }

            invokerMock.Verify(mockInvoker => mockInvoker.Invoke(It.IsAny<TestEvent>()), Times.Exactly(10));
        }

        [TestMethod]
        public void TestPublishInParallel()
        {
            var invokerMock = new Mock<IMockInvoker<TestEvent>>();

            var invoker = invokerMock.Object;

            var eventBus = new SchedulerEventBus();

            eventBus.Subscribe<TestEvent>(invoker.Invoke);

            Parallel.ForEach(Enumerable.Range(0, 100),new ParallelOptions
            {
                MaxDegreeOfParallelism = 100,
            },  current => eventBus.Publish(new TestEvent()));
            
            invokerMock.Verify(mockInvoker => mockInvoker.Invoke(It.IsAny<TestEvent>()), Times.Exactly(100));
        }

        [TestMethod]
        public void TestCancelSubscription()
        {
            var invokerMock = new Mock<IMockInvoker<TestEvent>>();

            var invoker = invokerMock.Object;

            var eventBus = new SchedulerEventBus();

            var subscription = eventBus.Subscribe<TestEvent>(invoker.Invoke);

            Parallel.ForEach(Enumerable.Range(0, 100), new ParallelOptions
            {
                MaxDegreeOfParallelism = 100,
            }, current => eventBus.Publish(new TestEvent()));

            invokerMock.Verify(mockInvoker => mockInvoker.Invoke(It.IsAny<TestEvent>()), Times.Exactly(100));

            subscription();

            invokerMock.ResetCalls();

            Parallel.ForEach(Enumerable.Range(0, 100), new ParallelOptions
            {
                MaxDegreeOfParallelism = 100,
            }, current => eventBus.Publish(new TestEvent()));

            invokerMock.Verify(mockInvoker => mockInvoker.Invoke(It.IsAny<TestEvent>()), Times.Never);
        }

        public interface IMockInvoker<in TEvent>
        {
            void Invoke(TEvent @event);
        }

        public class TestEvent
        {
        }
    }
}
