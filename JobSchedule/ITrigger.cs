using System;

namespace HuaQuant.JobSchedule
{
    public interface ITrigger
    {
        bool Trigger(DateTime time, IJob job,int runnings);
        bool Expired { get; }
    }
}
