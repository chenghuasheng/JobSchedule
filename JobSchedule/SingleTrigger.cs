using System;

namespace HuaQuant.JobSchedule
{
    public class SingleTrigger : ITrigger
    {
        private ITimeLimiter timeLimiter = null;
        private RepeatTrigger subTrigger = null;
        private Job subJob = null;
        
        public SingleTrigger(DateTime? starTime = null,DateTime? stopTime=null, int tryTimes = 0, TimeSpan? timeInterval = null)
        {
            if (tryTimes > 0)
            {
                this.subJob = new EmptyJob();
                this.subTrigger = new RepeatTrigger(timeInterval,starTime,stopTime,tryTimes);
            }
            else
            {
                this.timeLimiter = new StartStopTimeLimiter(starTime, stopTime);
            }
        }
        public SingleTrigger(DateTime time,int tryTimes=0,TimeSpan? timeInterval=null)
        {
            if (tryTimes > 0)
            {
                this.subJob = new EmptyJob();
                this.subTrigger = new RepeatTrigger(timeInterval, time, tryTimes);
            }else
            {
                this.timeLimiter = new SingleTimeLimiter(time);
            }
        }

        private bool expired = false;
        bool ITrigger.Expired => this.expired;

        bool ITrigger.Trigger(DateTime time, IJob job, int runnings)
        {
            if (job.Frequencies + 1 > 1)
            {
                this.expired = true;
                return false;
            }
            else if (job.Frequencies + runnings + 1 > 1)
            {
                return false;
            }
            if (this.subTrigger != null)
            {
                ITrigger trigger = (ITrigger)this.subTrigger;
                if (trigger.Expired)
                {
                    this.expired = true;
                    return false;
                }
                bool ret = trigger.Trigger(time, this.subJob, 0);
                if (ret) this.subJob.IncrementFrequency();
                return ret;
            }
            else
            {
                if (timeLimiter != null)
                {
                    if (!timeLimiter.Arrived(time)) return false;
                    if (timeLimiter.Beyonded(time))
                    {
                        this.expired = true;
                        return false;
                    }
                    else return true;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
