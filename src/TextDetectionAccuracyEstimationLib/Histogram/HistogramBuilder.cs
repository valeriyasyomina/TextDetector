using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDetectionAccuracyEstimationLib.Metrics;
using Excel = Microsoft.Office.Interop.Excel; 

namespace TextDetectionAccuracyEstimationLib.Histogram
{
    public class HistogramBuilder
    {
        public int HistogramWidth {get; set;}
        public int HistogramHeidht { get; set; }
        public int HistogramTopPadding { get; set; }
        public int HistogramLeftPadding { get; set; }
        /// <summary>
        /// Построение гистограммы для метрик
        /// </summary>
        /// <param name="metrics">Метрики</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns></returns>
        public static Task BuildMetricsHistogram(Dictionary<int, List<Metric>> metrics, string fileName)
        {
            if (fileName == null || fileName.Length == 0)
                throw new ArgumentNullException("Empty fileName in BuildMetricsHistogram");
            return Task.Run(() =>
            {
                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                HistogramData histogramData = CreateHistogramData(metrics);
                histogramData.Sort();
                WriteChartData(histogramData, xlWorkSheet); 
           
           /*     xlWorkSheet.Cells[1, 1] = "Вероятность ошибки 2 рода";                
                xlWorkSheet.Cells[1, 2] = "Вероятность ошибки 1 рода";          
                xlWorkSheet.Cells[1, 3] = "Вероятность пропуска";         
                xlWorkSheet.Cells[1, 4] = "Точность";        
                xlWorkSheet.Cells[1, 5] = "Полнота";
                xlWorkSheet.Cells[1, 6] = "F1 мера";
                int rowNumber = 2;
                foreach (var pair in metrics)
                {
                    if (AllMetricsExists(pair.Value))
                    {                       
                        WriteChartData(pair.Value, xlWorkSheet, rowNumber);
                        rowNumber++;
                    }
                }
                string highBorderIndexStr = (rowNumber - 1).ToString();
                BuildMetricHistogram(xlWorkSheet, misValue, "A1", "A" + highBorderIndexStr, 10, 80, 300, 250);
                BuildMetricHistogram(xlWorkSheet, misValue, "B1", "B" + highBorderIndexStr, 310, 80, 300, 250);
                BuildMetricHistogram(xlWorkSheet, misValue, "C1", "C" + highBorderIndexStr, 610, 80, 300, 250);
                BuildMetricHistogram(xlWorkSheet, misValue, "D1", "D" + highBorderIndexStr, 910, 80, 300, 250);
                BuildMetricHistogram(xlWorkSheet, misValue, "E1", "E" + highBorderIndexStr, 1210, 80, 300, 250);
                BuildMetricHistogram(xlWorkSheet, misValue, "F1", "F" + highBorderIndexStr, 1510, 80, 300, 250);

               */ xlWorkBook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                ReleaseObject(xlWorkSheet);
                ReleaseObject(xlWorkBook);
                ReleaseObject(xlApp);
            });
        }
        private static void BuildMetricHistogram(Excel.Worksheet xlWorkSheet, object misValue, string lowBorder, string highBorder, double left, 
            double top, double width, double height)
        {
            try
            {
                Excel.Range chartRange;
                Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                Excel.ChartObject chart = (Excel.ChartObject)xlCharts.Add(left, top, width, height);
                Excel.Chart chartPage = chart.Chart;
                chartRange = xlWorkSheet.get_Range(lowBorder, highBorder);
                chartPage.SetSourceData(chartRange, misValue);
                chartPage.ChartType = Excel.XlChartType.xlColumnClustered; 
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        private static bool AllMetricsExists(List<Metric> metricsList)
        {
            bool result = true;
            for (int i = 0; i < metricsList.Count && result; i++)
                if (metricsList[i].Value == Metric.UNDEFINED_METRIC)
                    result = false;           
            return result;
        }

        private static void WriteChartData(List<Metric> metricsList, Excel.Worksheet xlWorkSheet, int rowNumber)
        {
            try
            {
                for (int i = 0; i < metricsList.Count; i++)
                {
                    if (metricsList[i].GetType() == typeof(SecondTypeErrorProbability))                    
                        xlWorkSheet.Cells[rowNumber, 1] = metricsList[i].Value;
                    else if (metricsList[i].GetType() == typeof(FirstTypeErrorProbability))
                        xlWorkSheet.Cells[rowNumber, 2] = metricsList[i].Value;
                    else if (metricsList[i].GetType() == typeof(MissingProbability))
                        xlWorkSheet.Cells[rowNumber, 3] = metricsList[i].Value;
                    else if (metricsList[i].GetType() == typeof(Precision))
                        xlWorkSheet.Cells[rowNumber, 4] = metricsList[i].Value;
                    else if (metricsList[i].GetType() == typeof(Recall))
                        xlWorkSheet.Cells[rowNumber, 5] = metricsList[i].Value;
                    else if (metricsList[i].GetType() == typeof(F1Measure))
                        xlWorkSheet.Cells[rowNumber, 6] = metricsList[i].Value;                    
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static void CreateHistogramDataFrame(HistogramData histogramData, List<Metric> metricsList)
        {
            try
            {
                for (int i = 0; i < metricsList.Count; i++)
                {
                    if (metricsList[i].GetType() == typeof(SecondTypeErrorProbability))
                    {
                        if (histogramData.Contains(metricsList[i].Value, METRIC_TYPE.SECOND_TYPE_ERROR))                 
                            histogramData.IncrementMetricAmount(metricsList[i].Value, METRIC_TYPE.SECOND_TYPE_ERROR);
                        else
                            histogramData.SecondTypeError.Add(new HistogramDataElement(metricsList[i].Value, 1));
                    }
                    else if (metricsList[i].GetType() == typeof(FirstTypeErrorProbability))
                    {
                        if (histogramData.Contains(metricsList[i].Value, METRIC_TYPE.FIRST_TYPE_ERROR))                 
                            histogramData.IncrementMetricAmount(metricsList[i].Value, METRIC_TYPE.FIRST_TYPE_ERROR);
                        else
                            histogramData.FirstTypeError.Add(new HistogramDataElement(metricsList[i].Value, 1));
                    }
                    else if (metricsList[i].GetType() == typeof(MissingProbability))
                    {
                        if (histogramData.Contains(metricsList[i].Value, METRIC_TYPE.MISSING_ERROR))                            
                            histogramData.IncrementMetricAmount(metricsList[i].Value, METRIC_TYPE.MISSING_ERROR);
                        else
                            histogramData.MissingTypeError.Add(new HistogramDataElement(metricsList[i].Value, 1));                        
                    }
                    else if (metricsList[i].GetType() == typeof(Precision))
                    {
                        if (histogramData.Contains(metricsList[i].Value, METRIC_TYPE.PRECISION))
                            histogramData.IncrementMetricAmount(metricsList[i].Value, METRIC_TYPE.PRECISION);
                        else
                            histogramData.Precision.Add(new HistogramDataElement(metricsList[i].Value, 1));                     
                    }
                    else if (metricsList[i].GetType() == typeof(Recall))
                    {
                        if (histogramData.Contains(metricsList[i].Value, METRIC_TYPE.RECALL))
                            histogramData.IncrementMetricAmount(metricsList[i].Value, METRIC_TYPE.RECALL);
                            
                        else
                            histogramData.Recall.Add(new HistogramDataElement(metricsList[i].Value, 1));                                                    
                    }
                    else if (metricsList[i].GetType() == typeof(F1Measure))
                    {
                        if (histogramData.Contains(metricsList[i].Value, METRIC_TYPE.F1_MEASURE))
                            histogramData.IncrementMetricAmount(metricsList[i].Value, METRIC_TYPE.F1_MEASURE);
                        else
                            histogramData.F1Measure.Add(new HistogramDataElement(metricsList[i].Value, 1));                    
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static HistogramData CreateHistogramData(Dictionary<int, List<Metric>> metrics)
        {
            try
            {
                HistogramData histogramData = new HistogramData();
                foreach (var pair in metrics)                
                    CreateHistogramDataFrame(histogramData, pair.Value);                
                return histogramData;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static void WriteChartData(HistogramData histogramData, Excel.Worksheet xlWorkSheet)
        {
            try
            {
                WriteChartDataMetric(histogramData.SecondTypeError, xlWorkSheet, 1, 2);
                WriteChartDataMetric(histogramData.FirstTypeError, xlWorkSheet, 3, 4);
                WriteChartDataMetric(histogramData.MissingTypeError, xlWorkSheet, 5, 6);
                WriteChartDataMetric(histogramData.Precision, xlWorkSheet, 7, 8);
                WriteChartDataMetric(histogramData.Recall, xlWorkSheet, 9, 10);
                WriteChartDataMetric(histogramData.F1Measure, xlWorkSheet, 11, 12);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static void WriteChartDataMetric(List<HistogramDataElement> histogram, Excel.Worksheet xlWorkSheet, int columnNumberAmount, int columnNumberValue)
        {
            try
            {
                xlWorkSheet.Cells[1, columnNumberAmount] = "Количество";
                xlWorkSheet.Cells[1, columnNumberValue] = "Pyfxtybt";
                for (int i = 0, j = 2; i < histogram.Count; i++, j++)
                {
                    xlWorkSheet.Cells[j, columnNumberAmount] = histogram[i].Amount;
                    xlWorkSheet.Cells[j, columnNumberValue] = histogram[i].Value;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception exception)
            {
                obj = null;
                throw exception;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
