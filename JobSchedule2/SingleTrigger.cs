using System;

namespace HuaQuant.JobSchedule2
{
    public class SingleTrigger : ITrigger
    {
        private ITimeLimiter timeLimiter = null;
        private RepeatTrigger tryTrigger = null;
        private int hasTried = 0;
        public SingleTrigger(DateTime? starTime = null,DateTime? stopTime=null, int tryTimes = 0, TimeSpan? timeInterval = null)
        {
            if (tryTimes > 0)
            {
                this.tryTrigger = new RepeatTrigger(timeInterval,starTime,stopTime,tryTimes);
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
                this.tryTrigger = new RepeatTrigger(timeInterval, time, tryTimes);
            }else
            {
                this.timeLimiter = new SingleTimeLimiter(time);
            }
        }

        private bool expired = false;
        bool ITrigger.Expired => this.expired;

        bool ITrigger.Trigger(DateTime time, int succeedCount, int runningCount)
        {
            if (succeedCount + 1 > 1)
            {
                this.expired = true;
                return false;
            }
            else if (succeedCount + runningCount + 1 > 1)
            {
                return false;
            }
            if (this.tryTrigger != null)
            {
                ITrigger trigger = (ITrigger)this.tryTrigger;
                if (trigger.Expired)
                {
                    this.expired = true;
                    return false;
                }
                bool ret = trigger.Trigger(time, this.hasTried, 0);
                if (ret) this.hasTried++;
                return ret;
            }
            else
            {
                if (timeLimiter != null)
                {
                    if (!timeLimiter.Unarrive(time)) return false;
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
