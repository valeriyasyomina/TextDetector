using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.RegionData
{
    public class Region: RegionBase
    {
        public Region()
        {
            this.PixelsStrokeWidthList = new List<int>();
        }
        public int PixelsNumber { get; set; }
        public int AverageStrokeWidth { get; set; }
        public int SummaryStrokeWidth { get; set; }        
        public int TruePixelsNumber { get; set; }
        public int Square { get; set; }
        public int MaxStrokeWidth { get; set; }
        public int MinStrokeWidth { get; set; }        
        public int CenterPointIndexI { get; set; }
        public int CenterPointIndexJ { get; set; }
        public int Number { get; set; }
        public int Diameter { get; set; }
        public int BoundingBoxPixelsNumber { get; set; }
        public int SummaryIntensity { get; set; }
        public int AverageIntensity { get; set; }
        public List<int> PixelsStrokeWidthList { get; set; }
    }
}
