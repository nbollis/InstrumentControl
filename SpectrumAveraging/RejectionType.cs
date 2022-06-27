using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumAveraging
{
    public enum RejectionType
    {
        NoRejection,
        MinMaxClipping,
        PercentileClipping,
        SigmaClipping,
        WinsorizedSigmaClipping,
        AveragedSigmaClipping,
        BelowThresholdRejection
    }


}
