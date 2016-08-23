using System;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerLogger
    {
        void DebugWrite(string format, params object[] args);
        
        void InfoWrite(string format, params object[] args);
        
        void ErrorWrite(string format, params object[] args);

        void ErrorWrite(Exception exception, string format, params object[] args);

        void ErrorWrite(Exception exception, string message);

        void ErrorWrite(Exception exception);

    }
}
