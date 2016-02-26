using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.RegionData
{
    public class Region
    {
        public int PixelsNumber { get; set; }
        public int AverageStrokeWidth { get; set; }
        public int MaxBorderIndexI { get; set; }
        public int MaxBorderIndexJ { get; set; }        
        public int MinBorderIndexI { get; set; }
        public int MinBorderIndexJ { get; set; }
    }
}
