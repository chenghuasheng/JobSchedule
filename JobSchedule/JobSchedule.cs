using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace HuaQuant.JobSchedule
{
    public class JobSchedule
    {
        private Dictionary<Job, ITrigger> triggerDict = new Dictionary<Job, ITrigger>();
        private Dictionary<Job, List<JobProcess>> processDict = new Dictionary<Job, List<JobProcess>>();
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
            processDict.Clear();
        }
        private int inTimer = 0;//防止计时器事件重入
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref inTimer, 1) == 0)
            {
                lock (triggerDict)
                {
                    foreach (KeyValuePair<Job, ITrigger> kvp in triggerDict)
                    {
                        if (!kvp.Value.Expired) schedule(e.SignalTime, kvp.Value, kvp.Key);
                    }
                }
                Interlocked.Exchange(ref inTimer, 0);
            }
        }
        private void schedule(DateTime time, ITrigger trigger, Job job)
        {
            if (trigger.Trigger(time, job))
            {
                List<JobProcess> processList;
                if (!this.processDict.TryGetValue(job,out processList))
                {
                    processList = new List<JobProcess>();
                    this.processDict.Add(job, processList);
                }

                if (processList.Count < this.processNumPerJob)
                {
                    JobProcess process = new JobProcess(job);
                    processList.Add(process);
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
        public void Add(Job job, ITrigger trigger)
        {
            lock (this.triggerDict)
            {
                this.triggerDict.Add(job, trigger);
            }
        }
    }
}
