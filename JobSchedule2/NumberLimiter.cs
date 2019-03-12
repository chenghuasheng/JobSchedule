using System;

namespace HuaQuant.JobSchedule2
{
    public class NumberLimiter
    {
        private int numberLimit=1;
        public NumberLimiter(int number)
        {
            this.numberLimit = number;
        }
        public bool Beyonded(int number)
        {
            if (number > this.numberLimit) return true;
            else return false;
        }
    }
}
