using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule2
{
    internal class JobTask
    {
        private JobProcess process;
        private Task<bool> task = null;
        private Dictionary<IJob, bool> succeedDict = new Dictionary<IJob, bool>();
        internal Dictionary<IJob, bool> SucceedDict => this.succeedDict;
        private bool completed = true;
        internal bool Completed => this.completed;
        private bool succeed = true;
        internal bool Succeed => this.succeed;
        internal JobTask(JobProcess process)
        {
            this.process = process;
            foreach (IJob job in this.process.Jobs)
            {
                this.succeedDict.Add(job, true);
            }
        }
       
        internal void Start()
        {
            this.completed = false;
            this.succeed = false;
            this.task = new Task<bool>(run);
            this.task.ContinueWith(task => {
                this.completed = task.IsCompleted;
                this.succeed = task.Result;
                this.process.OnTaskCompleted(this);
            });
            this.process.OnTaskStart();
            this.task.Start();
        }
        private bool run()
        {
            bool ret = true;
            foreach(IJob job in this.process.Jobs)
            {
                bool succeed = job.Execute(this.process.CancelToken);
                ret = ret && succeed;
                this.succeedDict[job] = succeed;
            }
            return ret;
        }
    }
}
