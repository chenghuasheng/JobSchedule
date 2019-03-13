using System;
using System.Collections.Generic;
using System.Threading;

namespace HuaQuant.JobSchedule2
{
    public interface IJob
    {
        string Name { get; set; }
        bool ShowDetail { get; set; }
        IEnumerable<IJob> NeedJobs { get; }
        bool Execute(CancellationToken token);
    }
}
