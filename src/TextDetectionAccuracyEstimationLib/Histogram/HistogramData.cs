using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDetectionAccuracyEstimationLib.Histogram
{
    public enum METRIC_TYPE { SECOND_TYPE_ERROR, FIRST_TYPE_ERROR, MISSING_ERROR, PRECISION, RECALL, F1_MEASURE}
    public class HistogramData
    {
        private static int DEFAULT_METRICS_NUMBER = 6;
        public HistogramData()
        {
            this.SecondTypeError = new List<HistogramDataElement>();
            this.FirstTypeError = new List<HistogramDataElement>();
            this.MissingTypeError = new List<HistogramDataElement>();
            this.Precision = new List<HistogramDataElement>();
            this.Recall = new List<HistogramDataElement>();
            this.FirstTypeError = new List<HistogramDataElement>();
            this.F1Measure = new List<HistogramDataElement>();
            this.MetricsNumber = DEFAULT_METRICS_NUMBER;
        }
        public void IncrementMetricAmount(double value, METRIC_TYPE metricType)
        {
            switch (metricType)
            {
                case METRIC_TYPE.SECOND_TYPE_ERROR:
                    IncrementMetricAmount(this.SecondTypeError, value);
                    break;
                case METRIC_TYPE.FIRST_TYPE_ERROR:
                    IncrementMetricAmount(this.FirstTypeError, value);
                    break;
                case METRIC_TYPE.MISSING_ERROR:
                    IncrementMetricAmount(this.MissingTypeError, value);
                    break;
                case METRIC_TYPE.PRECISION:
                    IncrementMetricAmount(this.Precision, value);
                    break;
                case METRIC_TYPE.RECALL:
                    IncrementMetricAmount(this.Recall, value);
                    break;
                case METRIC_TYPE.F1_MEASURE:
                    IncrementMetricAmount(this.F1Measure, value);
                    break;
                default:
                    break;
            }           
        }
        public bool Contains(double value, METRIC_TYPE metricType)
        {
            bool result = false;
            switch (metricType)
            {
                case METRIC_TYPE.SECOND_TYPE_ERROR:
                    result = Contains(this.SecondTypeError, value);
                    break;
                case METRIC_TYPE.FIRST_TYPE_ERROR:
                    result = Contains(this.FirstTypeError, value);
                    break;
                case METRIC_TYPE.MISSING_ERROR:
                    result = Contains(this.MissingTypeError, value);
                    break;
                case METRIC_TYPE.PRECISION:
                    result = Contains(this.Precision, value);
                    break;
                case METRIC_TYPE.RECALL:
                    result = Contains(this.Recall, value);
                    break;
                case METRIC_TYPE.F1_MEASURE:
                    result = Contains(this.F1Measure, value);
                    break;
                default:
                    break;          
            }           
            return result;
        }

        private void IncrementMetricAmount(List<HistogramDataElement> histogramList, double value)
        {
            bool wasIncremented = false;
            for (int i = 0; i < histogramList.Count && !wasIncremented; i++)
            {
                if (histogramList[i].Value == value)
                {
                    histogramList[i].Amount++;
                    wasIncremented = true;
                }
            }
        }
        private bool Contains(List<HistogramDataElement> histogramList, double value)
        {
            bool result = false;
            for (int i = 0; i < histogramList.Count && !result; i++)
            {
                if (histogramList[i].Value == value)
                    result = true;
            }
            return result;
        }
       
        public void Sort()
        {
            this.SecondTypeError = this.SecondTypeError.OrderBy(data => data.Value).ToList();
            this.FirstTypeError = this.FirstTypeError.OrderBy(data => data.Value).ToList();
            this.MissingTypeError = this.MissingTypeError.OrderBy(data => data.Value).ToList();
            this.Precision = this.Precision.OrderBy(data => data.Value).ToList();
            this.Recall = this.Recall.OrderBy(data => data.Value).ToList();
            this.F1Measure = this.F1Measure.OrderBy(data => data.Value).ToList();
        }
        public int MetricsNumber { get; set; }
        public List<HistogramDataElement> SecondTypeError { get; set; }
        public List<HistogramDataElement> FirstTypeError { get; set; }
        public List<HistogramDataElement> MissingTypeError { get; set; }
        public List<HistogramDataElement> Precision { get; set; }
        public List<HistogramDataElement> Recall { get; set; }
        public List<HistogramDataElement> F1Measure { get; set; }
    }
}
