using System;
using System.Collections.Generic;


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
