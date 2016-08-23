using System;
using System.Threading;
using System.Threading.Tasks;
using Sc.Scheduler.Contracts;
using Sc.Scheduler.Model;
using Sc.Scheduler.Utils;

namespace Sc.Scheduler
{
    internal class SchedulerJobExecuter : ISchedulerJobExecuter
    {
        private readonly ISchedulerJobCompletedStrategy _schedulerJobCompletedStrategy;
        
        private readonly ISchedulerLogger _schedulerLogger;
        
        private readonly SchedulerConfiguration _schedulerConfiguration;

        private readonly SemaphoreSlim _semaphoreSlim;

        public SchedulerJobExecuter(ISchedulerJobCompletedStrategy schedulerJobCompletedStrategy, ISchedulerLogger schedulerLogger, SchedulerConfiguration schedulerConfiguration)
        {
            if (schedulerJobCompletedStrategy == null) throw new ArgumentNullException("schedulerJobCompletedStrategy");
            
            if (schedulerLogger == null) throw new ArgumentNullException("schedulerLogger");

            if (schedulerConfiguration == null) throw new ArgumentNullException("schedulerConfiguration");
            
            _schedulerJobCompletedStrategy = schedulerJobCompletedStrategy;
            
            _schedulerLogger = schedulerLogger;
         
            _schedulerConfiguration = schedulerConfiguration;

            _semaphoreSlim = new SemaphoreSlim(_schedulerConfiguration.MaxDegreeOfParallelism < 0 ? 0 : _schedulerConfiguration.MaxDegreeOfParallelism);
        }

        public async Task Execute(JobExecutionContext jobExecutionContext)
        {
            if (jobExecutionContext == null) throw new ArgumentNullException("jobExecutionContext");

            Task completionTask;

            try
            {
                if (_schedulerConfiguration.MaxDegreeOfParallelism > 0)
                {
                    await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
                }

                completionTask = jobExecutionContext.OnJobTriggered(jobExecutionContext.JobInfo, jobExecutionContext.CancellationToken).ContinueWith(task =>
                {
                    if (_schedulerConfiguration.MaxDegreeOfParallelism > 0)
                    {
                        _semaphoreSlim.Release();
                    }

                    return task;
                }).Unwrap().AttachToParrent();
            }
            catch (Exception exception)
            {
                completionTask = SchedulerTaskHelpers.FromException(exception);
            }

            if (completionTask.Status == TaskStatus.Created)
            {
                _schedulerLogger.ErrorWrite("Task returned callback is not started. Trgger name: '{0}', Trigger Group: '{1}'", jobExecutionContext.JobInfo.TriggerName, jobExecutionContext.JobInfo.GroupName);

                completionTask = SchedulerTaskHelpers.FromException(new InvalidOperationException(string.Format("Task returned callback is not started. Trgger name: '{0}', Trigger Group: '{1}'", jobExecutionContext.JobInfo.TriggerName, jobExecutionContext.JobInfo.GroupName)));
            }

            await completionTask.ContinueWith(task => DoComplete(task, jobExecutionContext.JobInfo, jobExecutionContext.TriggerConfiguration)).Unwrap().ConfigureAwait(false);
        }

        private async Task DoComplete(Task task, ISchedulerJobInfo schedulerJobInfo, TriggerConfiguration triggerConfiguration)
        {
            if (task.Status==TaskStatus.RanToCompletion)
            {
                try
                {
                    if (triggerConfiguration.OnJobSucceded != null)
                    {
                        await triggerConfiguration.OnJobSucceded(schedulerJobInfo).ConfigureAwait(false);
                    }
                    else
                    {
                        await SchedulerTaskHelpers.ExecuteSynchronously(()=>_schedulerJobCompletedStrategy.OnComplete(schedulerJobInfo)).ConfigureAwait(false);
                    }
                }
                catch (Exception exception) 
                {
                    _schedulerLogger.ErrorWrite(exception);
                }
            }
            else if (task.Status==TaskStatus.Canceled)
            {
                try
                {
                    if (triggerConfiguration.OnJobCancelled != null)
                    {
                        await triggerConfiguration.OnJobCancelled(schedulerJobInfo).ConfigureAwait(false);
                    }
                    else
                    {
                        await SchedulerTaskHelpers.ExecuteSynchronously(()=>_schedulerJobCompletedStrategy.OnCanceled(schedulerJobInfo)).ConfigureAwait(false);
                    }
                }
                catch (Exception exception)
                {
                    _schedulerLogger.ErrorWrite(exception);
                }
            }
            else if (task.Status == TaskStatus.Faulted)
            {
                try
                {
                    if (triggerConfiguration.OnJobFailed!=null)
                    {
                        await triggerConfiguration.OnJobFailed(schedulerJobInfo, task.Exception != null ? task.Exception.GetBaseException() : new ApplicationException("UNKNOWN")).ConfigureAwait(false);
                    }
                    else
                    {
                        await SchedulerTaskHelpers.ExecuteSynchronously(() => _schedulerJobCompletedStrategy.OnError(task.Exception)).ConfigureAwait(false);
                    }
                }
                catch (Exception exception)
                {
                    _schedulerLogger.ErrorWrite(exception);
                }
            }
        }
    }
}
