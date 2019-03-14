using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace HuaQuant.JobSchedule2
{
    internal class JobProcess
    {
        private List<IJob> jobs = new List<IJob>();
        internal List<IJob> Jobs => this.jobs;
        private ConcurrentBag<JobTask> jTasks = new ConcurrentBag<JobTask>();
        private int completedCount = 0;
        internal int CompletedCount => this.completedCount;
        private int succeedCount = 0;
        internal int SucceedCount => this.succeedCount;
        private int runningCount = 0;
        internal int RunningCount => this.runningCount;

        private int maxTaskNumber = 1;
        private CancellationTokenSource tokenSource =new CancellationTokenSource();
        internal CancellationToken CancelToken => this.tokenSource.Token;
        internal JobProcess(IEnumerable<IJob> jobs, int maxTaskNumber = 1 )
        {
            foreach (IJob job in jobs)
            {
                this.jobs.Add(job);
            }
            this.maxTaskNumber = maxTaskNumber;
        }
        internal JobProcess(IJob job, int maxTaskNumber = 1)
        {
            this.jobs.Add(job);
            this.maxTaskNumber = maxTaskNumber;
        }
        internal void StartATask()
        {
            if (this.jTasks.Count < this.maxTaskNumber)
            {
                JobTask newTask = new JobTask(this);
                this.jTasks.Add(newTask);
                newTask.Start();
            }
            else
            {
                foreach (JobTask task in this.jTasks)
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
        internal void Clear()
        {
            while (this.jTasks.TryTake(out JobTask task)) {  };
            this.jobs.Clear();
        }
        internal void CancelTasks()
        {
            this.tokenSource.Cancel(false);
        }
        internal void WaitTasks()
        {
            foreach (JobTask jTask in this.jTasks)
            {
                jTask.InnerTask.Wait();
            }
        }
    }
}
