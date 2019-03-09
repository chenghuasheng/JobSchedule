using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule
{
    public interface IJob
    {
        string Name { get; }
        bool ShowDetail { get; set; }
        IEnumerable<IJob> NeedJobs { get; }
        int Frequencies { get; }
        bool Execute();
        void IncrementFrequency();
    }
}
