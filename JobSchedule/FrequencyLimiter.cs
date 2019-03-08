using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule
{
    public class FrequencyLimiter
    {
        private int numLimit=1;
        public FrequencyLimiter(int num)
        {
            this.numLimit = num;
        }
        public bool Beyonded(int frequencies)
        {
            if (frequencies > this.numLimit) return true;
            else return false;
        }
    }
}
