using System;

namespace Sc.Scheduler.Model.Events
{
    class JobDisposedEvent
    {
        private readonly Guid _triggerId;
        
        private readonly bool _cancellAllJobs;

        public JobDisposedEvent(Guid triggerId, bool cancellAllJobs)
        {
            _triggerId = triggerId;
            
            _cancellAllJobs = cancellAllJobs;
        }

        public Guid TriggerId
        {
            get { return _triggerId; }
        }

        public bool CancellAllJobs
        {
            get { return _cancellAllJobs; }
        }
    }
}
