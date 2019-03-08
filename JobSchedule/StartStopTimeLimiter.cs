using System;

namespace HuaQuant.JobSchedule
{
    public class StartStopTimeLimiter
    {
        private DateTime startTime=DateTime.MinValue;
        private DateTime stopTime = DateTime.MaxValue;
        public StartStopTimeLimiter(DateTime? startTime,DateTime? stopTime)
        {
            if (startTime!=null) this.startTime = (DateTime)startTime;
            if (stopTime!=null)  this.stopTime = (DateTime)stopTime;
        }
        public bool Arrived(DateTime time)
        {
            if (time >= this.startTime) return true;
            else return false;
        }
        public bool Beyonded(DateTime time)
        {
            if (time > this.stopTime) return true;
            else return false;
        }
    }
}
