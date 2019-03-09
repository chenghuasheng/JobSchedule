using System;

namespace HuaQuant.JobSchedule
{
    public class EmptyJob:Job
    {
        public EmptyJob() : base("EmptyJob", null, false) { } 
    }
}
