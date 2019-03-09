using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HuaQuant.JobSchedule
{
    public abstract class Job:IJob
    {
        private string name="Job";
        public string Name  => this.name;
        bool showDetail = false;
        public bool ShowDetail {
            get => this.showDetail;
            set => this.showDetail=value;
        }
        private IEnumerable<IJob> needJobs = null;
        public IEnumerable<IJob> NeedJobs  => this.needJobs;
        private int frequencies = 0;
        public int Frequencies => this.frequencies;

        public Job(string name, IEnumerable<IJob> needJobs = null, bool showDetail = false)
        {
            this.name = name;
            this.needJobs = needJobs;
            this.showDetail = showDetail;
        }

        public virtual bool Execute()
        {
            return true;
        }
        public void IncrementFrequency()
        {
            Interlocked.Increment(ref frequencies);
        }
    }
}
