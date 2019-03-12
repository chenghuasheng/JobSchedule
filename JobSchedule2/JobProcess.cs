using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace HuaQuant.JobSchedule2
{
    internal class JobProcess
    {
        private List<IJob> jobs = new List<IJob>();
        internal List<IJob> Jobs => this.jobs;
        private ConcurrentBag<JobTask> tasks = new ConcurrentBag<JobTask>();
        private int completedCount = 0;
        internal int CompletedCount => this.completedCount;
        private int succeedCount = 0;
        internal int SucceedCount => this.succeedCount;
        private int runningCount = 0;
        internal int RunningCount => this.runningCount;

        private int maxTaskNumber = 1;
        private CancellationToken? token = null;
        internal CancellationToken? CancelToken => this.token;
        internal JobProcess(IEnumerable<IJob> jobs, int maxTaskNumber = 1, CancellationToken? token = null)
        {
            foreach (IJob job in jobs)
            {
                this.jobs.Add(job);
            }
            this.maxTaskNumber = maxTaskNumber;
            this.token = token;
        }
        internal JobProcess(IJob job, int maxTaskNumber = 1, CancellationToken? token = null)
        {
            this.jobs.Add(job);
            this.maxTaskNumber = maxTaskNumber;
            this.token = token;
        }
        internal void StartATask()
        {
            if (this.tasks.Count < this.maxTaskNumber)
            {
                JobTask newTask = new JobTask(this);
                this.tasks.Add(newTask);
                newTask.Start();
            }
            else
            {
                foreach (JobTask task in this.tasks)
                {
                    if (task.Completed)
                    {
                        task.Start();
                        break;
                    }
                }
            }
        }
        internal void OnTaskStart()
        {
            this.runningCount++;
        }
        internal void OnTaskCompleted(JobTask task)
        {
            this.completedCount++;
            this.runningCount--;
            if (task.Succeed) this.succeedCount++;
        }
        internal void Reset()
        {
            this.runningCount = 0;
            this.succeedCount = 0;
            this.completedCount = 0;
            while (this.tasks.TryTake(out JobTask task)) { };
        }
    }
}
