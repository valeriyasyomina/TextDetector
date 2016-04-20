using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDetectionAccuracyEstimationLib.Fabric;
using TextDetectionAccuracyEstimationLib.IO;
using TextDetectionAccuracyEstimationLib.Metrics;

namespace TextDetectionAccuracyEstimationLib.AccuracyEstimation
{
    public class AccuracyEstimator: IAccuracyEstimation
    {
        public static int VIDEO_METRICS = -1;
        /// <summary>
        /// Коэффициент для расчета f1 меры
        /// </summary>
        public double Alhpa {get; set;}
        /// <summary>
        /// Порог площади перекрытия для двух текстовых блоков
        /// </summary>
        public double IntersectionSquareThreshold { get; set; }
        public AccuracyEstimator(double alpha = 0.5, double intersectionSquareThreshold = 0.8)
        {
            if (alpha < 0 || alpha > 1)
                throw new ArgumentException("Error alpha in AccuracyEstimator, alpha [0; 1]");
            if (intersectionSquareThreshold < 0 || intersectionSquareThreshold > 1)
                throw new ArgumentException("Error intersectionSquareThreshold in AccuracyEstimator, intersectionSquareThreshold [0; 1]");
            this.Alhpa = alpha;
            this.IntersectionSquareThreshold = intersectionSquareThreshold;
        }
        /// <summary>
        /// Вычисление матрик для двух видео
        /// </summary>
        /// <param name="patternTextBlocks">Информация о текстовых блоках эталонного видео</param>
        /// <param name="generatedTextBlocks">Информация о текстовых блоках, сгенерированная программой</param>
        /// <returns><Рассчитанные метрики/returns>
        public Task<Dictionary<int, List<Metric>>> CalculateMetrics(Dictionary<int, List<TextRegion>> patternTextBlocks, Dictionary<int, List<TextRegion>> generatedTextBlocks)
        {
            try
            {
                if (patternTextBlocks == null)
                    throw new ArgumentNullException("Null patternTextBlocks in CalculateMetrics");
                if (generatedTextBlocks == null)
                    throw new ArgumentNullException("Null generatedTextBlocks in CalculateMetrics");
                Dictionary<int, List<Metric>> metricsList = new Dictionary<int, List<Metric>>();
             
                return Task.Run(() =>
                {
                    CalculateMetricsForFrames(metricsList, patternTextBlocks, generatedTextBlocks);
                    CalculateMetricsForVideo(metricsList);
                    return metricsList;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Вычисленение метрик для видео
        /// </summary>
        /// <param name="metricsList">Метрики</param>
        private void CalculateMetricsForVideo(Dictionary<int, List<Metric>> metricsList)
        {
            try
            {
                AbstractMetricFactory firstTypeErrorProbabilityMF = new FirstTypeErrorProbabilityMetricFactory();
                AbstractMetricFactory secondTypeErrorProbabilityMF = new SecondTypeErrorProbabilityMetricFactory();
                AbstractMetricFactory missingTypeErrorProbabilityMF = new MissingProbabilityMetricFactory();
                AbstractMetricFactory precisionMetricFactory = new PrecisionMetricFactory();
                AbstractMetricFactory recallMetricFactory = new RecallMetricFactory();
                AbstractMetricFactory f1MeasureMetricFactory = new F1MeasureMetricFactory();

                double secondTypeErrorMetric = 0, firstTypeErrorMetric = 0, precisionMetric = 0,
                    recallMetric = 0, f1Metric = 0, missingTypeErrorMetric = 0;
                int secondTypeErrorMetricNumber = 0, firstTypeErrorMetricNumber = 0, precisionMetricNumber = 0,
                    recallMetricNumber = 0, f1MetricNumber = 0, missingTypeErrorMetricNumber = 0;
                foreach (var pair in metricsList)
                {
                    List<Metric> metrisFrame = pair.Value;
                    for (int i = 0; i < metrisFrame.Count; i++)
                    {
                        if (metrisFrame[i].GetType() == typeof(SecondTypeErrorProbability) && metrisFrame[i].Value != Metric.UNDEFINED_METRIC)
                        {
                            secondTypeErrorMetric += metrisFrame[i].Value;
                            secondTypeErrorMetricNumber++;
                        }
                        else if (metrisFrame[i].GetType() == typeof(FirstTypeErrorProbability) && metrisFrame[i].Value != Metric.UNDEFINED_METRIC)
                        {
                            firstTypeErrorMetric += metrisFrame[i].Value;
                            firstTypeErrorMetricNumber++;
                        }
                        else if (metrisFrame[i].GetType() == typeof(MissingProbability) && metrisFrame[i].Value != Metric.UNDEFINED_METRIC)
                        {
                            missingTypeErrorMetric += metrisFrame[i].Value;
                            missingTypeErrorMetricNumber++;
                        }
                        else if (metrisFrame[i].GetType() == typeof(Precision) && metrisFrame[i].Value != Metric.UNDEFINED_METRIC)
                        {
                            precisionMetric += metrisFrame[i].Value;
                            precisionMetricNumber++;
                        }
                        else if (metrisFrame[i].GetType() == typeof(Recall) && metrisFrame[i].Value != Metric.UNDEFINED_METRIC)
                        {
                            recallMetric += metrisFrame[i].Value;
                            recallMetricNumber++;
                        }
                        else if (metrisFrame[i].GetType() == typeof(F1Measure) && metrisFrame[i].Value != Metric.UNDEFINED_METRIC)
                        {
                            f1Metric += metrisFrame[i].Value;
                            f1MetricNumber++;
                        }
                    }
                }
                List<Metric> videoMetricList = new List<Metric>();
                videoMetricList.Add(secondTypeErrorProbabilityMF.GetMetric(secondTypeErrorMetric, secondTypeErrorMetricNumber));
                videoMetricList.Add(firstTypeErrorProbabilityMF.GetMetric(firstTypeErrorMetric, firstTypeErrorMetricNumber));
                videoMetricList.Add(missingTypeErrorProbabilityMF.GetMetric(missingTypeErrorMetric, missingTypeErrorMetricNumber));
                videoMetricList.Add(precisionMetricFactory.GetMetric(precisionMetric, precisionMetricNumber));
                videoMetricList.Add(recallMetricFactory.GetMetric(recallMetric, recallMetricNumber));
                videoMetricList.Add(f1MeasureMetricFactory.GetMetric(f1Metric, f1MetricNumber));

                metricsList.Add(VIDEO_METRICS, videoMetricList);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Вычисление матрик
        /// </summary>
        /// <param name="metricsList">Результат</param>
        /// <param name="patternTextBlocks">Информация о текстовых блоках эталонного видео</param>
        /// <param name="generatedTextBlocks">Информация о текстовых блоках, сгенерированная программой</param>
        private void CalculateMetricsForFrames(Dictionary<int, List<Metric>> metricsList, Dictionary<int, List<TextRegion>> patternTextBlocks, Dictionary<int, List<TextRegion>> generatedTextBlocks)
        {
            try
            {                
                AbstractMetricFactory firstTypeErrorProbabilityMF = new FirstTypeErrorProbabilityMetricFactory();
                AbstractMetricFactory secondTypeErrorProbabilityMF = new SecondTypeErrorProbabilityMetricFactory();
                AbstractMetricFactory missingTypeErrorProbabilityMF = new MissingProbabilityMetricFactory();
                AbstractMetricFactory precisionMetricFactory = new PrecisionMetricFactory();
                AbstractMetricFactory recallMetricFactory = new RecallMetricFactory();
                AbstractMetricFactory f1MeasureMetricFactory = new F1MeasureMetricFactory();

                foreach (var pair in generatedTextBlocks)
                {
                    int frameNumber = pair.Key;
                    if (patternTextBlocks.ContainsKey(frameNumber))
                    {
                        int textBlocksWithMissedDataNumber = 0;
                        int falseTextBlockNumber = 0;
                        int notDetectedTextBlocksNumber = 0;
                        double precision = 0.0;
                        CalculateTextBlocksTypesNumber(patternTextBlocks[frameNumber], generatedTextBlocks[frameNumber], out textBlocksWithMissedDataNumber,
                            out falseTextBlockNumber, out notDetectedTextBlocksNumber, out precision);

                        List<Metric> metrics = new List<Metric>();
                        metrics.Add(secondTypeErrorProbabilityMF.GetMetric(falseTextBlockNumber, generatedTextBlocks[frameNumber].Count));
                        metrics.Add(firstTypeErrorProbabilityMF.GetMetric(notDetectedTextBlocksNumber, patternTextBlocks[frameNumber].Count));
                        metrics.Add(missingTypeErrorProbabilityMF.GetMetric(textBlocksWithMissedDataNumber, generatedTextBlocks[frameNumber].Count - falseTextBlockNumber));
                        metrics.Add(precisionMetricFactory.GetMetric(precision));

                        double recall = 0.0;
                        CalculateTextBlocksTypesNumber(generatedTextBlocks[frameNumber], patternTextBlocks[frameNumber], out textBlocksWithMissedDataNumber,
                            out falseTextBlockNumber, out notDetectedTextBlocksNumber, out recall);

                        metrics.Add(recallMetricFactory.GetMetric(recall));
                        if (precision == Metric.UNDEFINED_METRIC || recall == Metric.UNDEFINED_METRIC)
                            metrics.Add(f1MeasureMetricFactory.GetMetric(precision, recall));
                        else
                        {
                            double denominator = 1.0 / ((this.Alhpa / precision) + ((1 - this.Alhpa) / recall));
                            metrics.Add(f1MeasureMetricFactory.GetMetric(1.0, denominator));
                        }
                        metricsList.Add(frameNumber, metrics);                    
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Вычисление количества текстовых блоков разного типа
        /// </summary>
        /// <param name="patternTextBlocks">Эталонные текстовые блоки</param>
        /// <param name="generatedTextBlocks">Вычисленные текстовые блоки</param>
        /// <param name="textBlocksWithMissedDataNumber">Кол-во текстовых блоков с потерей данных (выходной)</param>
        /// <param name="falseTextBlockNumber">Кол-во ложно выделенных текстовых блоков (выходное)</param>
        /// <param name="notDetectedTextBlocksNumber">Кол-во невыделенных текстовых блоков (выходное)</param>
        /// <param name="precision">Точность локализации</param>
        private void CalculateTextBlocksTypesNumber(List<TextRegion> patternTextBlocks, List<TextRegion> generatedTextBlocks,
            out int textBlocksWithMissedDataNumber, out int falseTextBlockNumber, out int notDetectedTextBlocksNumber,
            out double precision)
        {
            try
            {
                falseTextBlockNumber = 0;
                textBlocksWithMissedDataNumber = 0;
                notDetectedTextBlocksNumber = 0;

                int detectedTextBlocksNumber = 0;
                int generatedTextBlocksNumber = generatedTextBlocks.Count;
                int patternTextBlocksNumber = patternTextBlocks.Count;

                double squareSum = 0;
                for (int i = 0; i < generatedTextBlocksNumber; i++)
                {
                    bool wasMatch = false;
                    double bestMatch = 0;                    
                    for (int j = 0; j < patternTextBlocksNumber; j++)
                    {
                        double match = CalculateMatch(generatedTextBlocks[i], patternTextBlocks[j]);                  
                        if (match != 0.0)
                        {
                            wasMatch = true;
                            if (match > bestMatch)
                                bestMatch = match;                
                        }
                    }
                    if (wasMatch)
                    {
                        detectedTextBlocksNumber++;
                        squareSum += bestMatch;
                        if (bestMatch < this.IntersectionSquareThreshold)
                            textBlocksWithMissedDataNumber++;
                    }
                    else
                        falseTextBlockNumber++;
                }
                if (generatedTextBlocksNumber != 0)
                    precision = squareSum / (double)generatedTextBlocksNumber;
                else
                    precision = Metric.UNDEFINED_METRIC;
                notDetectedTextBlocksNumber = patternTextBlocks.Count - detectedTextBlocksNumber;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Вычисление минимальной площади прямоугольника, обрамляющего два области
        /// </summary>
        /// <param name="firstRegion">Первая область</param>
        /// <param name="secondRegion">Вторая область</param>
        /// <returns></returns>
        private int CalculateAllTextBoxSquare(TextRegion firstRegion, TextRegion secondRegion)
        {
            try
            {
                int minAllBBI = Math.Min(firstRegion.MinBorderIndexI, secondRegion.MinBorderIndexI);
                int minAllBBJ = Math.Min(firstRegion.MinBorderIndexJ, secondRegion.MinBorderIndexJ);
                int maxAllBBI = Math.Max(firstRegion.MaxBorderIndexI, secondRegion.MaxBorderIndexI);
                int maxAllBBJ = Math.Max(firstRegion.MaxBorderIndexJ, secondRegion.MaxBorderIndexJ);

                return (maxAllBBI - minAllBBI + 1) * (maxAllBBJ - minAllBBJ + 1);
            }
            catch (Exception excption)
            {
                throw excption;
            }
        }
        /// <summary>
        /// Принадлежит ли точка с заданными координатами области
        /// </summary>
        /// <param name="textRegion">Область</param>
        /// <param name="pointIndexI">Координата по строке</param>
        /// <param name="pointIndexJ">Координата по столбцу</param>
        /// <returns>1 - принадлежит, 0 - иначе</returns>
        private bool IsBelongToRegion(TextRegion textRegion, int pointIndexI, int pointIndexJ)
        {
            try
            {
                bool result = false;
                if (pointIndexI >= textRegion.MinBorderIndexI && pointIndexJ >= textRegion.MinBorderIndexJ &&
                    pointIndexJ <= textRegion.MaxBorderIndexJ && pointIndexI <= textRegion.MaxBorderIndexI)
                    result = true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Вычисление площади пересечения двух текстовых областей
        /// </summary>
        /// <param name="firstRegion">Первая область</param>
        /// <param name="secondRegion">Вторая область</param>
        /// <returns></returns>
        private int CalculateIntersectionSquare(TextRegion firstRegion, TextRegion secondRegion)
        {
            try
            {
                int minAllBBI = Math.Min(firstRegion.MinBorderIndexI, secondRegion.MinBorderIndexI);
                int minAllBBJ = Math.Min(firstRegion.MinBorderIndexJ, secondRegion.MinBorderIndexJ);
                int maxAllBBI = Math.Max(firstRegion.MaxBorderIndexI, secondRegion.MaxBorderIndexI);
                int maxAllBBJ = Math.Max(firstRegion.MaxBorderIndexJ, secondRegion.MaxBorderIndexJ);

                int square = 0;
                for (int i = minAllBBI; i <= maxAllBBI; i++)
                    for (int j = minAllBBJ; j <= maxAllBBJ; j++)
                        if (IsBelongToRegion(firstRegion, i, j) && IsBelongToRegion(secondRegion, i, j))
                            ++square;
                return square;             
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Вычисление вероятности совпадения двух текстовых областей
        /// </summary>
        /// <param name="firstRegion">Первая текстовая область</param>
        /// <param name="secondRegion">Вторая текстовая область</param>
        /// <returns></returns>
        private double CalculateMatch(TextRegion firstRegion, TextRegion secondRegion)
        {
            try
            {
                int allBBSquare = CalculateAllTextBoxSquare(firstRegion, secondRegion);
                int intersectionSquare = CalculateIntersectionSquare(firstRegion, secondRegion);
                return (double) intersectionSquare / (double) allBBSquare;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
