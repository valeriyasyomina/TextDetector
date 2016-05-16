using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDetectionAccuracyEstimationLib.Histogram
{
    public class HistogramDataElement
    {
        public HistogramDataElement(double value, int amount)
        {
            this.Amount = amount;
            this.Value = value;
        }
        public int Amount { get; set; }
        public double Value { get; set; } 
    }
}
