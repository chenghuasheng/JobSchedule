using System;

namespace HuaQuant.JobSchedule2
{
    public class RepeatTrigger : ITrigger
    {
        private NumberLimiter numberLimiter = null;
        private StartStopTimeLimiter startStopTimeLimiter = null;
        private SingleTimeLimiter singleTimeLimiter = null;
        private TimeSpan timeInterval;
        private DateTime? nextTriggerTime = null;
        public RepeatTrigger(TimeSpan? timeInterval,DateTime? startTime=null, DateTime? stopTime=null,int? frequency=null)
        {
            if (timeInterval != null) this.timeInterval = (TimeSpan)timeInterval;
            else this.timeInterval = new TimeSpan(0, 0, 1);

            if (startTime != null || stopTime != null) this.startStopTimeLimiter = new StartStopTimeLimiter(startTime, stopTime);
            if (frequency != null) this.numberLimiter = new NumberLimiter((int)frequency);
        }
        public RepeatTrigger(TimeSpan? timeInterval,DateTime time,int? frequency = null)
        {
            if (timeInterval != null) this.timeInterval = (TimeSpan)timeInterval;
            else this.timeInterval = new TimeSpan(0, 0, 1);
            this.singleTimeLimiter = new SingleTimeLimiter(time);
            if (frequency != null) this.numberLimiter = new NumberLimiter((int)frequency);
        }
        private bool expired = false;
        bool ITrigger.Expired => this.expired;

        bool ITrigger.Trigger(DateTime time, int succeedCount,int runningCount)
        {
            if (numberLimiter != null)
            {
                if (numberLimiter.Beyonded(succeedCount + 1))
                {
                    this.expired = true;
                    return false;
                }
                else if (numberLimiter.Beyonded(succeedCount + runningCount + 1))
                {
                    return false;
                }
            }
            if (startStopTimeLimiter != null)
            {
                if (startStopTimeLimiter.Unarrive(time)) return false;
                if (startStopTimeLimiter.Beyonded(time))
                {
                    this.expired = true;
                    return false;
                }else
                {
                    DateTime nextTime;
                    if (this.intervalBaseOnStartTime)
                    {
                        if (this.nextTriggerTime == null)
                        {
                            nextTime = this.GetNextTriggerTime(time, this.startStopTimeLimiter.StartTime, 1);
                        }else
                        {
                            nextTime = this.GetNextTriggerTime(time, (DateTime)this.nextTriggerTime, 2);
                        }
                        this.nextTriggerTime = nextTime;
                    }
                    else nextTime = time.Add(timeInterval);
                    this.startStopTimeLimiter.StartTime = nextTime;
                    return true;
                }
            }
            if (this.singleTimeLimiter != null)
            {
                if (this.singleTimeLimiter.Unarrive(time)) return false;
                else
                {
                    bool beyonded = this.singleTimeLimiter.Beyonded(time);
                    DateTime nextTime;
                    if (this.nextTriggerTime == null)
                    { 
                        nextTime =this.GetNextTriggerTime(time, this.singleTimeLimiter.LimitTime, 1);
                    }
                    else
                    {
                        nextTime = this.GetNextTriggerTime(time, (DateTime)this.nextTriggerTime, 2);
                    }
                    this.nextTriggerTime = nextTime;
                    this.singleTimeLimiter.LimitTime = nextTime;
                    if (!beyonded) return true;
                    else return false;
                }
            }
            return true;
        }

        private bool intervalBaseOnStartTime = false;
        public bool IntervalBaseOnStartTime
        {
            get => this.intervalBaseOnStartTime;
            set => this.intervalBaseOnStartTime = value;
        }
        private DateTime GetNextTriggerTime(DateTime curTime, DateTime baseTime, byte mode)
        {
            DateTime nextTime = baseTime;
            switch (mode)
            {
                case 1:
                    long k = (long)((curTime - nextTime).Ticks / timeInterval.Ticks);
                    nextTime = nextTime.Add(new TimeSpan(timeInterval.Ticks * (k + 1)));
                    break;
                case 2:
                    while (nextTime <= curTime) nextTime = nextTime.Add(timeInterval);
                    break;
            }
            return nextTime;
        }
    }
}
