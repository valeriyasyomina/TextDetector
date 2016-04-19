using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDetectionAccuracyEstimationLib.Metrics
{
    public abstract class Metric
    {
        public static double UNDEFINED_METRIC = -1.0;
        
        public double Value {get; protected set;}         
        public abstract double Calculate();
        public double Calculate(double numerator, double denominator)
        {
            if (denominator == 0)
                this.Value = UNDEFINED_METRIC;
            else
                this.Value = numerator / denominator;    
            return this.Value;
        }
    }
}
