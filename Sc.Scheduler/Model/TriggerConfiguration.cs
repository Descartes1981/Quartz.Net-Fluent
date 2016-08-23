using System;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Exceptions;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler.Model
{
    public class TriggerConfiguration : TriggerScheduleConfiguration<ITriggerConfiguration>, ITriggerConfiguration
    {
        private readonly IScheduler _scheduler;

        public TriggerConfiguration(IScheduler scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            
            _scheduler = scheduler;
        }

        private string _description;
        
        private string _name;
        
        private string _group;
        
        private Func<ISchedulerJobInfo, Task> _onJobSucceded;
        
        private Func<ISchedulerJobInfo, Exception, Task> _onJobFailed;
        
        private Func<ISchedulerJobInfo, Task> _onJobCancelled;

        public string Description
        {
            get { return _description; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Group
        {
            get { return _group; }
        }

        public Func<ISchedulerJobInfo, Task> OnJobSucceded
        {
            get { return _onJobSucceded; }
        }

        public Func<ISchedulerJobInfo, Exception, Task> OnJobFailed
        {
            get { return _onJobFailed; }
        }

        public Func<ISchedulerJobInfo, Task> OnJobCancelled
        {
            get { return _onJobCancelled; }
        }

        public ITriggerConfiguration WithIdentity(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            
            _name = name;

            return This;
        }

        public ITriggerConfiguration WithIdentity(string name, string group)
        {
            if (name == null) throw new ArgumentNullException("name");

            if (group == null) throw new ArgumentNullException("group");

            _name = name;

            _group = name;

            return This;
        }
        
        public ITriggerConfiguration WithDescription(string description)
        {
            if (description == null) throw new ArgumentNullException("description");
            
            _description = description;

            return This;
        }

        public ITriggerConfiguration WithOnJobSucceded(Action<ISchedulerJobInfo> onJobSucceded)
        {
            if (onJobSucceded==null) throw new ArgumentNullException("onJobSucceded");

            _onJobSucceded = jobInfo => SchedulerTaskHelpers.ExecuteSynchronously(() => onJobSucceded(jobInfo));
           
            return this;
        }

        public ITriggerConfiguration WithOnJobFailed(Action<ISchedulerJobInfo, Exception> onJobFailed)
        {
            if (onJobFailed == null) throw new ArgumentNullException("onJobFailed");

            _onJobFailed = (jobInfo, exception) => SchedulerTaskHelpers.ExecuteSynchronously(() => onJobFailed(jobInfo, exception));

            return this;
        }

        public ITriggerConfiguration WithOnJobCancelled(Action<ISchedulerJobInfo> onJobCancelled)
        {
            if (onJobCancelled == null) throw new ArgumentNullException("onJobCancelled");

            _onJobCancelled = jobInfo => SchedulerTaskHelpers.ExecuteSynchronously(() => onJobCancelled(jobInfo));

            return this;
        }

        public ITriggerConfiguration WithOnJobSucceded(Func<ISchedulerJobInfo, Task> onJobSucceded)
        {
            if (onJobSucceded == null) throw new ArgumentNullException("onJobSucceded");

            _onJobSucceded = onJobSucceded;

            return this;
        }

        public ITriggerConfiguration WithOnJobFailed(Func<ISchedulerJobInfo, Exception, Task> onJobFailed)
        {
            if (onJobFailed == null) throw new ArgumentNullException("onJobFailed");

            _onJobFailed = onJobFailed;

            return this;
        }

        public ITriggerConfiguration WithOnJobCancelled(Func<ISchedulerJobInfo, Task> onJobCancelled)
        {
            if (onJobCancelled == null) throw new ArgumentNullException("onJobCancelled");

            _onJobCancelled = onJobCancelled;

            return this;
        }


        protected override TriggerBuilder Create(TriggerScheduleConfiguration<ITriggerConfiguration> triggerScheduleConfiguration)
        {
            var triggerBuilder = base.Create(triggerScheduleConfiguration);

            if (!string.IsNullOrEmpty(_description))
            {
                
                triggerBuilder.WithDescription(_description);
            }

            if (!string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_group))
            {
                var trggerKey = new TriggerKey(_name, _group);

                ValidateIfTriggerWasAlreadyScheduled(trggerKey);

                triggerBuilder.WithIdentity(trggerKey);
            }
            else if (!string.IsNullOrEmpty(_name))
            {
                var trggerKey = new TriggerKey(_name);

                ValidateIfTriggerWasAlreadyScheduled(trggerKey);

                triggerBuilder.WithIdentity(_name);
            }
            else
            {
                triggerBuilder.WithIdentity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }

            return triggerBuilder;
        }

        private void ValidateIfTriggerWasAlreadyScheduled(TriggerKey triggerKey)
        {
            var allTriggerKeys = _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());

            var triggerAlreadyScheduled = (allTriggerKeys.Any(triggerKey1 => string.Equals(triggerKey1.Name, triggerKey.Name) && string.Equals(triggerKey1.Group, triggerKey.Group)));

            if (triggerAlreadyScheduled)
            {
                throw new SchedulerTriggerWasAlreadyScheduledException(triggerKey);
            }
        }
    }
}
