using System;

namespace HuaQuant.JobSchedule2
{
    internal class StartStopTimeLimiter:ITimeLimiter
    {
        private DateTime startTime=DateTime.MinValue;
        public DateTime StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }
        private DateTime stopTime = DateTime.MaxValue;
        public StartStopTimeLimiter(DateTime? startTime,DateTime? stopTime)
        {
            if (startTime!=null) this.startTime = (DateTime)startTime;
            if (stopTime!=null)  this.stopTime = (DateTime)stopTime;
        }
        public bool Unarrive(DateTime time)
        {
            if (time < this.startTime) return true;
            else return false;
        }
        public bool Beyonded(DateTime time)
        {
            if (time > this.stopTime) return true;
            else return false;
        }
    }
}
