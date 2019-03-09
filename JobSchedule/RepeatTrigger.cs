using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule
{
    public class RepeatTrigger : ITrigger
    {
        private FrequencyLimiter freLimiter = null;
        private StartStopTimeLimiter startStopTimeLimiter = null;
        private SingleTimeLimiter singleTimeLimiter = null;
        private TimeSpan timeInterval;
        private DateTime? nextTriggerTime = null;
        public RepeatTrigger(TimeSpan timeInterval,DateTime? startTime=null, DateTime? stopTime=null,int? frequencyLimit=null)
        {
            this.timeInterval = timeInterval;
            if (startTime != null || stopTime != null) this.startStopTimeLimiter = new StartStopTimeLimiter(startTime, stopTime);
            if (frequencyLimit != null) this.freLimiter = new FrequencyLimiter((int)frequencyLimit);
        }
        public RepeatTrigger(TimeSpan timeInterval,DateTime time,int? frequencyLimit = null)
        {
            this.timeInterval = timeInterval;
            this.singleTimeLimiter = new SingleTimeLimiter(time);
            if (frequencyLimit != null) this.freLimiter = new FrequencyLimiter((int)frequencyLimit);
        }
        private bool expired = false;
        bool ITrigger.Expired => this.expired;

        bool ITrigger.Trigger(DateTime time, IJob job)
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
            if (freLimiter != null)
            {
                if (freLimiter.Beyonded(job.Frequencies+1))
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
                    nextTriggerTime = this.singleTimeLimiter.LimitTime.Add(timeInterval);
                    this.singleTimeLimiter.LimitTime = (DateTime)nextTriggerTime;
                    if (!beyonded) return true;
                    else return false;
                }
            }
            else
            {
                if (nextTriggerTime == null)
                {
                    nextTriggerTime = time.Add(timeInterval);
                    return true;
                }
                if (time >= nextTriggerTime)
                {
                    nextTriggerTime = time.Add(timeInterval);
                    return true;
                }
                else return false;
            }
        }
    }
}
