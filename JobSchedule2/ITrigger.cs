using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule2
{
    public interface ITrigger
    {
        bool Trigger(DateTime time, int succeedCount, int runningCount);
        bool Expired { get; }
    }
}
