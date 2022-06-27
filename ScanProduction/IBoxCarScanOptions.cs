using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterfaces;

namespace ScanProdution
{
    public interface IBoxCarScanOptions : ITaskOptions<IBoxCarScanOptions>
    {
        [Option]
        // with on either side of the selected mass to grab
        public double DaTolerance { get; set; }
    }

    public class BoxCarScanOptions : IBoxCarScanOptions
    {
        public double DaTolerance { get; set; }
        public BoxCarScanOptions()
        {

        }

    }
}
