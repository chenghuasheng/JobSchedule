using System;
using System.Collections.Generic;


namespace HuaQuant.JobSchedule
{
    public interface IJob
    {
        string Name { get; set; }
        bool ShowDetail { get; set; }
        IEnumerable<IJob> NeedJobs { get; }
        int Frequencies { get; }
        bool Execute();
        void IncrementFrequency();
    }
}
