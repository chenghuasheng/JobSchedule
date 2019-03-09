using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace HuaQuant.JobSchedule
{
    public class JobSchedule
    {
        private Dictionary<IJob, ITrigger> triggerDict = new Dictionary<IJob, ITrigger>();
        private Dictionary<IJob, List<JobProcess>> processDict = new Dictionary<IJob, List<JobProcess>>();
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
            foreach (List<JobProcess> processList in processDict.Values)
            {
                foreach(JobProcess process in processList) process.Stop(true);
            }
            lock (this.processDict)
            {
                processDict.Clear();
            }
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
                    List<JobProcess> processList;
                    if (this.processDict.TryGetValue(needJob, out processList))
                    {
                        foreach (JobProcess process in processList)
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
            if (trigger.Trigger(time, job))
            {
                if (!passedNeedJobs(job))
                {
                    Console.WriteLine("作业{0}的先行作业没有成功地完全结束，本作业暂时无法调度。",job.Name);
                    return;
                }
                List<JobProcess> processList;
                if (!this.processDict.TryGetValue(job,out processList))
                {
                    processList = new List<JobProcess>();
                    lock (this.processDict)
                    {
                        this.processDict.Add(job, processList);
                    }
                }

                if (processList.Count < this.processNumPerJob)
                {
                    JobProcess process = new JobProcess(job);
                    lock (processList)
                    {
                        processList.Add(process);
                    }
                    process.Start();
                }else
                {
                    foreach(JobProcess process in processList)
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
            lock (this.triggerDict)
            {
                this.triggerDict.Add(job, trigger);
            }
        }
        public void ClearExpiredJobs()
        {
            List<IJob> expiredJobs = new List<IJob>();
            foreach (KeyValuePair<IJob,ITrigger> jobTriggerPair in this.triggerDict)
            {
                if (jobTriggerPair.Value.Expired) expiredJobs.Add(jobTriggerPair.Key);
            }
            lock (this.triggerDict)
            {
                foreach (IJob job in expiredJobs) this.triggerDict.Remove(job);
            }
            foreach (IJob job in expiredJobs)
            {
                List<JobProcess> processList;
                if (this.processDict.TryGetValue(job,out processList)){
                    List<JobProcess> stoppedProcesses = new List<JobProcess>();
                    foreach (JobProcess process in processList)
                    {
                        if (!process.IsRunning) stoppedProcesses.Add(process);
                    }
                    lock (processList)
                    {
                        foreach (JobProcess process in stoppedProcesses) processList.Remove(process);
                    }
                    if (processList.Count == 0)
                    {
                        lock (this.processDict)
                        {
                            this.processDict.Remove(job);
                        }
                    }
                }
            }
        }
    }
}
