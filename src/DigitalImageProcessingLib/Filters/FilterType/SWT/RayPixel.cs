using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.SWT
{
    public class RayPixel
    {
        public static double UNDEFINED_VALUE = -1.0;
        public RayPixel()
        {
            this.StrokeWidth = UNDEFINED_VALUE;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public double StrokeWidth { get; set; }
    }
}
