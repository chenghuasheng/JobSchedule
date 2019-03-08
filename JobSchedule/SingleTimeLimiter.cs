using System;

namespace HuaQuant.JobSchedule
{
    public class SingleTimeLimiter
    {
        private DateTime limitTime;
        private SingleTimeLimiterPrecission precission;
        public SingleTimeLimiter(DateTime limitTime,SingleTimeLimiterPrecission precission=SingleTimeLimiterPrecission.Second)
        {
            this.limitTime = limitTime;
            this.precission = precission;
        }
        public bool Arrived(DateTime time)
        {
            if (time >= this.limitTime) return true;
            else return false;
        }

        public bool Beyonded(DateTime time)
        {
            TimeSpan span = time - this.limitTime;
            switch (this.precission)
            {
                case SingleTimeLimiterPrecission.Second:
                    if (span.TotalSeconds > 0) return true;
                    break;
                case SingleTimeLimiterPrecission.Minute:
                    if (span.TotalMinutes > 0) return true;
                    break;
                case SingleTimeLimiterPrecission.Hour:
                    if (span.TotalHours > 0) return true;
                    break;
                case SingleTimeLimiterPrecission.Day:
                    if (span.TotalDays > 0) return true;
                    break; 
            }
            return false;
        }

        public DateTime LimitTime
        {
            get { return this.limitTime; }
            set { this.limitTime = value; }
        }
    }
    public enum SingleTimeLimiterPrecission
    {
        Day=0,
        Hour,
        Minute,
        Second,
    }
}
