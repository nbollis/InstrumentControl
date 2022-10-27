using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Util
{
    public class MassTargetListItem
    {
        public double Mass { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }

        public MassTargetListItem(double mass, double startTime, double endTime)
        {
            Mass = mass;
            StartTime = startTime;
            EndTime = endTime;
        }

        public static List<MassTargetListItem> CreateListItemsInBulk(List<double> masses,
            List<double> retentionTimes, double bufferTimeInms)
        {
            List<MassTargetListItem> items = new List<MassTargetListItem>();
            for (var i = 0; i < masses.Count; i++)
            {
                var mass = masses[i];
                var retentionTime = retentionTimes[i];
                MassTargetListItem item = new(mass, retentionTime - bufferTimeInms, retentionTime + bufferTimeInms);
                items.Add(item);
            }

            return items;
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
