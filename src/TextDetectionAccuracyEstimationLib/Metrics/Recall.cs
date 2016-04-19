using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDetectionAccuracyEstimationLib.Metrics
{
    public class Recall: Metric
    {
        public Recall() { }
        public Recall(double value)
        {
            this.Value = value;
        }
        public override double Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
