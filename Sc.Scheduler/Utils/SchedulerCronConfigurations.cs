using System;
using System.Collections.Generic;
using Sc.Scheduler.Contracts;

namespace Sc.Scheduler.Utils
{
    internal class SchedulerCronConfigurations : ISchedulerCronConfigurations
    {
        private readonly IList<int> _minutes = new List<int>();
        
        private readonly IList<int> _hours = new List<int>();
        
        private readonly IList<int> _dayOfMonths = new List<int>();
        
        private readonly IList<int> _months = new List<int>();

        private readonly IList<int> _daysOfWeek = new List<int>();

        
        public ISchedulerCronConfigurations OnMinute(params int[] minutes)
        {
            foreach (var minute in minutes)
            {
                if (minute < 0 || minute > 59)
                {
                    throw new ArgumentException("Invalid minute value {0}, allowed value range 0-59.");
                }

                if (!_minutes.Contains(minute))
                {
                    _minutes.Add(minute);
                }    
            }
            
            return this;
        }

        public ISchedulerCronConfigurations OnHour(params int[] hours)
        {
            foreach (var hour in hours)
            {
                if (hour < 0 || hour > 23)
                {
                    throw new ArgumentException("Invalid hour value {0}, allowed value range 0-23.");
                }

                if (!_hours.Contains(hour))
                {
                    _hours.Add(hour);
                }    
            }

            return this;
        }


        public ISchedulerCronConfigurations OnDayOfMonth(params int[] daysOfMonth)
        {
            foreach (var dayOfMonth in daysOfMonth)
            {
                if (dayOfMonth < 1 || dayOfMonth > 31)
                {
                    throw new ArgumentException("Invalid dayOfMonth value {0}, allowed value range 1-31.");
                }

                if (!_dayOfMonths.Contains(dayOfMonth))
                {
                    _dayOfMonths.Add(dayOfMonth);    
                }
            }

            return this;
        }


        public ISchedulerCronConfigurations OnMonth(params int[] months)
        {
            foreach (var month in months)
            {
                if (month < 1 || month > 12)
                {
                    throw new ArgumentException("Invalid month value {0}, allowed value range 1-12.");
                }

                if (!_months.Contains(month))
                {
                    _months.Add(month);
                }
            }
            
            return this;
        }

        public ISchedulerCronConfigurations OnDayOfWeek(params DayOfWeek[] daysOfWeek)
        {
            foreach (var dayOfWeek in daysOfWeek)
            {
                if (((int)(dayOfWeek)) < 0 || ((int)(dayOfWeek)) > 6)
                {
                    throw new ArgumentException("Invalid dayOfWeek value {0}, allowed value range 0-6.");
                }

                if (!_daysOfWeek.Contains((int) dayOfWeek))
                {
                    _daysOfWeek.Add((int) dayOfWeek);
                }
            }
            
            return this;
        }

        
    }
}
