using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule
{
    public class SingleTrigger : ITrigger
    {
        private SingleTimeLimiter singleTimeLimiter = null;
        private RepeatTrigger subTrigger = null;
        private Job subJob = null;
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

        bool ITrigger.Expired => throw new NotImplementedException();

        bool ITrigger.Trigger(DateTime time, Job job)
        {
            throw new NotImplementedException();
        }
    }
}
