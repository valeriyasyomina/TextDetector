using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.RegionData
{
    public class RegionChain: RegionGroupBase
    {
        public static double ANGLE_DIRECTION_UNDEFINED = 1000.0;
        private static int UNDEFINED_REGION_NUMBER = -1;
        public RegionChain()
        {
            this.AngleDirection = ANGLE_DIRECTION_UNDEFINED;
            this.FirstRegionNumber = UNDEFINED_REGION_NUMBER;
            this.LastRegionNumber = UNDEFINED_REGION_NUMBER;
        }
        public List<int> RegionsNumber { get; set; }
        public int FirstRegionNumber { get; set; }
        public int LastRegionNumber { get; set; }
    }
}
