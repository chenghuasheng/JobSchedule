using System;

namespace HuaQuant.JobSchedule2
{
    internal class SingleTimeLimiter:ITimeLimiter
    {
        private DateTime limitTime;
        public DateTime LimitTime
        {
            get { return this.limitTime; }
            set { this.limitTime = value; }
        }
        private SingleTimeLimiterPrecission precission;
        public SingleTimeLimiter(DateTime limitTime,SingleTimeLimiterPrecission precission=SingleTimeLimiterPrecission.Second)
        {
            this.limitTime = limitTime;
            this.precission = precission;
        }
        public bool Unarrive(DateTime time)
        {
            if (time <this.limitTime) return true;
            else return false;
        }

        public bool Beyonded(DateTime time)
        {
            TimeSpan span = time - this.limitTime;
            switch (this.precission)
            {
                case SingleTimeLimiterPrecission.Second:
                    if (Convert.ToInt32(span.TotalSeconds) > 0) return true;
                    break;
                case SingleTimeLimiterPrecission.Minute:
                    if (Convert.ToInt32(span.TotalMinutes) > 0) return true;
                    break;
                case SingleTimeLimiterPrecission.Hour:
                    if (Convert.ToInt32(span.TotalHours) > 0) return true;
                    break;
                case SingleTimeLimiterPrecission.Day:
                    if (Convert.ToInt32(span.TotalDays) > 0) return true;
                    break;
            }
            return false;
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
