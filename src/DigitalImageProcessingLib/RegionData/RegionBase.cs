using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.RegionData
{
    public class RegionBase
    {
        public int MaxBorderIndexI { get; set; }
        public int MaxBorderIndexJ { get; set; }
        public int MinBorderIndexI { get; set; }
        public int MinBorderIndexJ { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
