using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.RegionData
{
    public class Region: RegionBase
    {
        public int PixelsNumber { get; set; }
        public double AverageStrokeWidth { get; set; }
        public double SummaryStrokeWidth { get; set; }        
        public int TruePixelsNumber { get; set; }
        public int Square { get; set; }
        public double MaxStrokeWidth { get; set; }
        public double MinStrokeWidth { get; set; }        
        public int CenterPointIndexI { get; set; }
        public int CenterPointIndexJ { get; set; }
        public int Number { get; set; }
        public int Diameter { get; set; }
        public int BoundingBoxPixelsNumber { get; set; }
        public int SummaryIntensity { get; set; }
        public int AverageIntensity { get; set; }
    }
}
