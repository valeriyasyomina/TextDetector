using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.SWTData
{
    public class StrokeWidthData
    {
        public static double UNDEFINED_WIDTH = -1;
        public StrokeWidthData()
        {
            this.Width = UNDEFINED_WIDTH;
            this.WasProcessed = false;
        }
        public double Width { get; set; }
        public bool WasProcessed { get; set; }
    }
}
