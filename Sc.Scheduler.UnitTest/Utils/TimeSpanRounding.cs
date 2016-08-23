using System;

namespace Sc.Scheduler.Test.Utils
{
    static class TimeSpanRounding
    {
        const int TimespanSize = 7;

        public static TimeSpan Round(this TimeSpan timeSpan, int precision)
        {
            var factor = (int) Math.Pow(10, (TimespanSize - precision));

            return new TimeSpan(((long) Math.Round((1.0*timeSpan.Ticks/factor))*factor));
        }
    }
}
