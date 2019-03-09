using System;

namespace HuaQuant.JobSchedule
{
    public class SingleTrigger : ITrigger
    {
        private SingleTimeLimiter singleTimeLimiter = null;
        private StartStopTimeLimiter startStopTimeLimiter = null;
        private RepeatTrigger subTrigger = null;
        private Job subJob = null;
        
        public SingleTrigger(DateTime? starTime = null,DateTime? stopTime=null, int tryTimes = 0, TimeSpan? timeInterval = null)
        {
            if (tryTimes > 0)
            {
                this.subJob = new EmptyJob();
                this.subTrigger = new RepeatTrigger((TimeSpan)timeInterval,starTime,stopTime,tryTimes);
            }
            else
            {
                this.startStopTimeLimiter = new StartStopTimeLimiter(starTime, stopTime);
            }
        }
        public SingleTrigger(DateTime time,int tryTimes=0,TimeSpan? timeInterval=null)
        {
            if (tryTimes > 0)
            {
                this.subJob = new EmptyJob();
                this.subTrigger = new RepeatTrigger((TimeSpan)timeInterval, time, tryTimes);
            }else
            {
                this.singleTimeLimiter = new SingleTimeLimiter(time);
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
                if (startStopTimeLimiter != null)
                {
                    if (!startStopTimeLimiter.Arrived(time)) return false;
                    if (startStopTimeLimiter.Beyonded(time))
                    {
                        this.expired = true;
                        return false;
                    }
                }
                if (this.singleTimeLimiter != null)
                {
                    if (!this.singleTimeLimiter.Arrived(time)) return false;
                    else
                    {
                        bool beyonded = this.singleTimeLimiter.Beyonded(time);
                        if (!beyonded) return true;
                        else
                        {
                            this.expired = true;
                            return false;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
