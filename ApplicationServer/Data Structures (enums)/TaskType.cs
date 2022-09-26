﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public enum TaskType
    {
        Normalization,
        Standardization,
        SpectrumAveraging,
        ChargeStateEnvelope,
        BoxcarFragger
    }
}
