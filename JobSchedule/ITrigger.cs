using System;

namespace HuaQuant.JobSchedule
{
    public interface ITrigger
    {
        bool Trigger(DateTime time, IJob job);
        bool Expired { get; }
    }
}
