using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule2
{
    public interface ITimeLimiter
    {
        bool Unarrive(DateTime time);
        bool Beyonded(DateTime time);
    }
}
