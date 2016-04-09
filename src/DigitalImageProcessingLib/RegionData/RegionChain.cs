using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.RegionData
{
    public class RegionChain: RegionGroupBase
    {
        public static double DIRECTION_UNDEFINED = 1000.0;
        public static double DISTANCE_UNDEFINED = -1.0;
        private static int UNDEFINED_REGION_NUMBER = -1;
        public RegionChain()
        {
            this.DirectionX = DIRECTION_UNDEFINED;
            this.DirectionY = DIRECTION_UNDEFINED;
            this.DistanceSqr = DISTANCE_UNDEFINED;
            this.FirstRegionNumber = UNDEFINED_REGION_NUMBER;
            this.LastRegionNumber = UNDEFINED_REGION_NUMBER;
            this.RegionsNumber = new List<int>();
            this.WasMerged = false;
        }        
        public int FirstRegionNumber { get; set; }
        public int LastRegionNumber { get; set; }
        public bool WasMerged { get; set; }
    }
}
