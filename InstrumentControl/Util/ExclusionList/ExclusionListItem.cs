using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class ExclusionListItem
    {
        public double Mass { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }

        public ExclusionListItem(double mass, double startTime, double endTime)
        {
            Mass = mass;
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool WithinTimeSpan(double retentionTime)
        {
            if (retentionTime >= StartTime && retentionTime <= EndTime)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return $"{Mass} : {StartTime}-{EndTime}";
        }
    }
}
