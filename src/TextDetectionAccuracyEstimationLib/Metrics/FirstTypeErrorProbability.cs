using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDetectionAccuracyEstimationLib.Metrics
{
    public class FirstTypeErrorProbability: Metric
    {
        public FirstTypeErrorProbability() { }
        public FirstTypeErrorProbability(double value)
        {
            this.Value = value;
        }
        public override double Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
