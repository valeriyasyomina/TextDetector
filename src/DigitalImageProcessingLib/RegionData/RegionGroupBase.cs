using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.RegionData
{
    public class RegionGroupBase: RegionBase
    {
        public List<int> RegionsNumber { get; set; }
        public double DirectionX { get; set; }
        public double DirectionY { get; set; }
        public double DistanceSqr { get; set; }
    }
}
