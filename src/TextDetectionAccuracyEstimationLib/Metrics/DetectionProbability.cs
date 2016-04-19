using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDetectionAccuracyEstimationLib.Metrics
{
    public class DetectionProbability: Metric
    {
        public DetectionProbability() { }
        public DetectionProbability(double value)
        {
            this.Value = value;
        }
        public override double Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
