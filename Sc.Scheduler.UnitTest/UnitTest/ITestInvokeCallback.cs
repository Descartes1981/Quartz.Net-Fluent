namespace Sc.Scheduler.Test.UnitTest
{
    public interface ITestInvokeCallback<in T>
    {
        void Invoke(T value);
    }
}
