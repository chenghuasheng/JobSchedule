﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule
{
    public interface ITrigger
    {
        bool Trigger(DateTime time, Job job);
        bool Expired { get; }
    }
}
