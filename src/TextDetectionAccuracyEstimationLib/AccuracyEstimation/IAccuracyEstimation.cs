using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDetectionAccuracyEstimationLib.IO;
using TextDetectionAccuracyEstimationLib.Metrics;

namespace TextDetectionAccuracyEstimationLib.AccuracyEstimation
{
    public interface IAccuracyEstimation
    {
        Task<Dictionary<int, List<Metric>>> CalculateMetrics(Dictionary<int, List<TextRegion>> patternTextBlocks,
            Dictionary<int, List<TextRegion>> generatedTextBlocks);
    }
}
