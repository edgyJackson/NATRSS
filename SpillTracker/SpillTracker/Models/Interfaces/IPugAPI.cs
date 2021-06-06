﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Models.Interfaces
{
    public interface IPugAPI
    {
        ExtraChemData GitCIDAndMolWeight(string url);
        ExtraChemData GetDensVapPresFromPUGView(ExtraChemData data);
    }
}
