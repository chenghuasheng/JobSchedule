using System;
using System.Collections.Generic;
using System.Threading;
namespace HuaQuant.JobSchedule2
{
    public abstract class Job : IJob
    {
        private string name = "Job";
        public string Name
        {
            get => this.name;
            set => this.name = value;
        }
        bool showDetail = false;
        public bool ShowDetail
        {
            get => this.showDetail;
            set => this.showDetail = value;
        }
        private IEnumerable<IJob> needJobs = null;
        public IEnumerable<IJob> NeedJobs => this.needJobs;
        

        public Job(string name, IEnumerable<IJob> needJobs = null, bool showDetail = false)
        {
            this.name = name;
            this.needJobs = needJobs;
            this.showDetail = showDetail;
        }

        public virtual bool Execute(CancellationToken token)
        {
            return true;
        }
    }
}
