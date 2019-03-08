using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule
{
    public class EmptyJob:Job
    {
        public EmptyJob() : base("EmptyJob", null, false) { } 
    }
}
