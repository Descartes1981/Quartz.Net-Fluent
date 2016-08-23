using System;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Utils
{
    internal class SchedulerNulllLoger : ISchedulerLogger
    {
        public void DebugWrite(string format, params object[] args)
        {
            
        }

        public void InfoWrite(string format, params object[] args)
        {
            
        }

        public void ErrorWrite(string format, params object[] args)
        {
            
        }

        public void ErrorWrite(Exception exception, string format, params object[] args)
        {
            
        }

        public void ErrorWrite(Exception exception, string message)
        {
            
        }

        public void ErrorWrite(Exception exception)
        {
            
        }
    }
}
