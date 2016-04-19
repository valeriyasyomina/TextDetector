using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDetectionAccuracyEstimationLib.Metrics
{
    public class SecondTypeErrorProbability: Metric
    {
        public SecondTypeErrorProbability() { }
        public SecondTypeErrorProbability(double value)
        {
            this.Value = value;
        }
        public override double Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
