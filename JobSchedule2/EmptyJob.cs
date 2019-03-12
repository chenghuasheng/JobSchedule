using System;

namespace HuaQuant.JobSchedule2
{
    public class EmptyJob : Job
    {
        public EmptyJob() : base("EmptyJob", null, false) { }
    }
}
