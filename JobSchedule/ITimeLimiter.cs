using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule
{
    public interface ITimeLimiter
    {
        bool Arrived(DateTime time);
        bool Beyonded(DateTime time);
    }
}
