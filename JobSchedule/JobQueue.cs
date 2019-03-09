using System;
using System.Collections.Generic;
using System.Threading;

namespace HuaQuant.JobSchedule
{
    public class JobQueue : IJob
    {
        private string name = "JobQueue";
        public string Name => this.name;

        bool showDetail = false;
        public bool ShowDetail
        {
            get => this.showDetail;
            set => this.showDetail = value;
        }
        private IEnumerable<IJob> needJobs = null;
        public IEnumerable<IJob> NeedJobs => this.needJobs;
        private int frequencies = 0;
        public int Frequencies => this.frequencies;

        private List<IJob> jobs = new List<IJob>();
        public JobQueue(string name, IEnumerable<IJob> needJobs = null, bool showDetail = false)
        {
            this.name = name;
            this.needJobs = needJobs;
            this.showDetail = showDetail;
        }
        public bool Execute()
        {
            bool ret = true;
            foreach (IJob job in this.jobs)
            {
                ret = ret && job.Execute();
            }
            return ret;
        }

        public void IncrementFrequency()
        {
            Interlocked.Increment(ref frequencies);
        }

        public void AddJob(IJob job)
        {
            lock (this.jobs)
            {
                this.jobs.Add(job);
            }
        }
    }
}
