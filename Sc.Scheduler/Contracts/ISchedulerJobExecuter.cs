using System.Threading.Tasks;
using Sc.Scheduler.Model;

namespace Sc.Scheduler.Contracts
{
    public interface ISchedulerJobExecuter
    {
        Task Execute(JobExecutionContext jobExecutionContext);
    }
}