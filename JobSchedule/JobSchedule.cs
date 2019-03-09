using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Collections.Concurrent;


namespace HuaQuant.JobSchedule
{
    public class JobSchedule
    {
        private ConcurrentDictionary<IJob, ITrigger> triggerDict = new ConcurrentDictionary<IJob, ITrigger>();
        private ConcurrentDictionary<IJob, ConcurrentBag<JobProcess>> processDict = new ConcurrentDictionary<IJob, ConcurrentBag<JobProcess>>();
        private System.Timers.Timer timer = null;
        private int interval = 100;
        private int processNumPerJob = 1;//每个作业的进程数
        public int ProcessNumPerJob
        {
            get { return this.processNumPerJob; }
            set { this.processNumPerJob = value; }
        }
        public JobSchedule() { }

        public void Start()
        {
            timer = new System.Timers.Timer(this.interval);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= Timer_Elapsed;
                timer = null;
            }
            foreach (ConcurrentBag<JobProcess> processBag in processDict.Values)
            {
                foreach(JobProcess process in processBag) process.Stop(true);
            }
            processDict.Clear();
        }
        private int inTimer = 0;//防止计时器事件重入
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref inTimer, 1) == 0)
            {
                foreach (KeyValuePair<IJob, ITrigger> kvp in triggerDict)
                {
                    if (!kvp.Value.Expired) schedule(e.SignalTime, kvp.Value, kvp.Key);
                }
                Interlocked.Exchange(ref inTimer, 0);
            }
        }
        private bool passedNeedJobs(IJob job)
        {
            bool can = true;
            if (job.NeedJobs!=null&&job.NeedJobs.Count() > 0)
            {                
                foreach (IJob needJob in job.NeedJobs)
                {
                    ITrigger tgr = null;
                    if (this.triggerDict.TryGetValue(needJob, out tgr))
                    {
                        if (!tgr.Expired)
                        {
                            can = false;
                            break;
                        }
                    }
                    ConcurrentBag<JobProcess> processBag;
                    if (this.processDict.TryGetValue(needJob, out processBag))
                    {
                        foreach (JobProcess process in processBag)
                        {
                            if (!process.IsFinished)
                            {
                                can = false;
                                break;
                            }
                        }
                    }
                }
            }
            return can;
        }
        private void schedule(DateTime time, ITrigger trigger, IJob job)
        {
            int runnings = 0;
            ConcurrentBag<JobProcess> processBag = null;
            if (this.processDict.TryGetValue(job, out processBag))
            {
                foreach (JobProcess process in processBag) if (process.IsRunning) runnings++;
            }
            if (trigger.Trigger(time, job, runnings))
            {
                if (!passedNeedJobs(job))
                {
                    Console.WriteLine("作业{0}的先行作业没有成功地完全结束，本作业暂时无法调度。", job.Name);
                    return;
                }
                if (processBag == null)
                {
                    processBag = new ConcurrentBag<JobProcess>();
                    this.processDict.TryAdd(job, processBag);
                }

                if (processBag.Count < this.processNumPerJob)
                {
                    JobProcess process = new JobProcess(job);
                    processBag.Add(process);
                    process.Start();
                }
                else
                {
                    foreach (JobProcess process in processBag)
                    {
                        if (!process.IsRunning)
                        {
                            process.Start();
                            break;
                        }
                    }
                }
            }
        }
        public void Add(IJob job, ITrigger trigger)
        {
            this.triggerDict.TryAdd(job, trigger);
        }

        public void ClearExpiredJobs()
        {
            List<IJob> expiredJobs = new List<IJob>();
            foreach (KeyValuePair<IJob, ITrigger> jobTriggerPair in this.triggerDict)
            {
                if (jobTriggerPair.Value.Expired) expiredJobs.Add(jobTriggerPair.Key);
            }

            foreach (IJob job in expiredJobs)
            {
                this.triggerDict.TryRemove(job, out ITrigger trigger);
                if (this.processDict.TryRemove(job, out ConcurrentBag<JobProcess> processBag))
                {
                    while (processBag.TryTake(out JobProcess process))
                    {
                        process.Stop();
                    }
                }
            }
        }
    }
}
