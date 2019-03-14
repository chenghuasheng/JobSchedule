using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace HuaQuant.JobSchedule2
{
    public class JobSchedule
    {
        private ConcurrentDictionary<IJob, ITrigger> triggerDict = new ConcurrentDictionary<IJob, ITrigger>();
        private ConcurrentDictionary<IJob, JobProcess> processDict = new ConcurrentDictionary<IJob, JobProcess>();
        private ConcurrentDictionary<ITrigger, JobProcess> triggerAndProcessDict = new ConcurrentDictionary<ITrigger, JobProcess>();

        private System.Timers.Timer timer = null;
        private int interval = 100;

        public void Start()
        {
            timer = new System.Timers.Timer(this.interval);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        public void Stop(bool block=false)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= Timer_Elapsed;
                timer = null;
            }
            
            foreach (JobProcess process in this.processDict.Values)
            {
                process.CancelTasks();
                if (block) process.WaitTasks();
                process.Clear();
            }
            this.triggerAndProcessDict.Clear();
            this.triggerDict.Clear();
            this.processDict.Clear();
        }
        private int inTimer = 0;//防止计时器事件重入
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref inTimer, 1) == 0)
            {
                foreach (KeyValuePair<ITrigger, JobProcess> kvp in this.triggerAndProcessDict)
                {
                    if (!kvp.Key.Expired) this.schedule(e.SignalTime, kvp.Key, kvp.Value);
                }
                Interlocked.Exchange(ref inTimer, 0);
            }
        }

        private void schedule(DateTime time, ITrigger trigger, JobProcess process)
        {
            if (trigger.Trigger(time, process.SucceedCount, process.RunningCount))
            {
                bool ret = true;
                foreach (IJob job in process.Jobs) ret = ret && this.canRunning(job);
                if (ret) process.StartATask();
            }
        }
        private bool canRunning(IJob job)
        {
            if (job.NeedJobs == null) return true;

            foreach (IJob needJob in job.NeedJobs)
            {
                if (this.triggerDict.TryGetValue(needJob, out ITrigger trigger))
                {
                    if (!trigger.Expired) return false;
                }
                if (this.processDict.TryGetValue(needJob, out JobProcess process))
                {
                    if (process.RunningCount > 0) return false;
                }
            }
            return true;
        }

        public void Add(IJob job, ITrigger trigger, int maxTaskNumber = 1)
        {
            JobProcess process = new JobProcess(job, maxTaskNumber);
            this.triggerAndProcessDict.TryAdd(trigger, process);
            this.triggerDict.TryAdd(job, trigger);
            this.processDict.TryAdd(job, process);
        }
        public void Add(IEnumerable<IJob> jobs, ITrigger trigger, int maxTaskNumber = 1)
        {
            JobProcess process = new JobProcess(jobs, maxTaskNumber);
            this.triggerAndProcessDict.TryAdd(trigger, process);
            foreach (IJob job in jobs)
            {
                this.triggerDict.TryAdd(job, trigger);
                this.processDict.TryAdd(job, process);
            }
        }
    }
}
