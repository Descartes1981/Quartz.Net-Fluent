using System;
using System.Threading.Tasks;

namespace Sc.Scheduler.Utils
{
    internal static class SchedulerTaskHelpers
    {
        public static Task FromException(Exception ex)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(ex);
            return tcs.Task;
        }

        public static Task ExecuteSynchronously(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            try
            {
                action();
                tcs.SetResult(null);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
            return tcs.Task;
        }



        public static Task AttachToParrent(this Task task)
        {
            var tsc = new TaskCompletionSource<Task>(TaskCreationOptions.AttachedToParent);

            task.ContinueWith(_ => tsc.TrySetResult(task), TaskContinuationOptions.OnlyOnRanToCompletion);

            task.ContinueWith(t => tsc.TrySetException(task.Exception ?? new AggregateException(new ApplicationException("Unknown"))), TaskContinuationOptions.OnlyOnFaulted);

            task.ContinueWith(t => tsc.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);

            return tsc.Task.Unwrap();
        }

        public static void SafeWait(this Task task)
        {
            try
            {
                task.Wait();
            }
            catch
            {
                //Ignore
            }
        }
    }
}
