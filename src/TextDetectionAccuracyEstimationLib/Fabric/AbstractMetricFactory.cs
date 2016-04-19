using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDetectionAccuracyEstimationLib.Metrics;

namespace TextDetectionAccuracyEstimationLib.Fabric
{
    public abstract class AbstractMetricFactory
    {
        public abstract Metric GetMetric(double numerator, double denominator);
        public abstract Metric GetMetric(double value);
    }
}
