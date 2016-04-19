﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDetectionAccuracyEstimationLib.Metrics;

namespace TextDetectionAccuracyEstimationLib.Fabric
{
    public class RecallMetricFactory: AbstractMetricFactory
    {
        public override Metric GetMetric(double numerator, double denominator)
        {
            Metric metric = new Recall();
            metric.Calculate(numerator, denominator);
            return metric;
        }
        public override Metric GetMetric(double value)
        {
            return new Recall(value);
        }
    }
}
