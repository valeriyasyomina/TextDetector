using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TextDetectionAccuracyEstimationLib.Fabric;
using TextDetectionAccuracyEstimationLib.Metrics;

namespace TextDetectionAccuracyEstimationLib.IO
{
    public class XMLReader
    {
        /// <summary>
        ///  Считывание информации о тексчтовых блоках из xml - файла
        /// </summary>
        /// <param name="XMLFileName">Имя Xml - файла</param>
        /// <returns></returns>
        public static Task<Dictionary<int, List<TextRegion>>> ReadTextBlocksInformation(string XMLFileName)
        {
            try
            {
                if (XMLFileName == null || XMLFileName.Length == 0)
                    throw new ArgumentNullException("Null XMLFileName in ReadTextBlocksInformation");

                return Task.Run(() =>
                {
                    Dictionary<int, List<TextRegion>> framesTextBlocksInformation = new Dictionary<int, List<TextRegion>>();
                    System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(XMLFileName);                   
                    while (xmlReader.Read())
                    {
                         if (xmlReader.Name == "Frame")
                         {
                             List<TextRegion> textRegions = null;
                             int frameNumber = 0;
                             ReadFrameTextBlocksInformation(xmlReader, out textRegions, out frameNumber);
                             framesTextBlocksInformation.Add(frameNumber, textRegions);
                         }
                    }
                    return framesTextBlocksInformation;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Считывание метрик из файла
        /// </summary>
        /// <param name="XMLFileName">Файл</param>
        /// <returns>Метрики</returns>
        public static Task<Dictionary<int, List<Metric>>> ReadMetricsXML(string XMLFileName)
        {
            if (XMLFileName == null || XMLFileName.Length == 0)
                throw new ArgumentNullException("Null XMLFileName in ReadMetricsXML");
            return Task.Run(() =>
            {
                Dictionary<int, List<Metric>> metric = new Dictionary<int, List<Metric>>();

                XmlDocument xml = new XmlDocument();
                xml.Load(XMLFileName);
                XmlNodeList xmlNodeList = xml.SelectNodes("/Metrics/Frame");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    int frameNumber = 0;
                    List<Metric> metricList = null;
                    ReadXmlNode(xmlNode, out metricList, out frameNumber);
                    metric.Add(frameNumber, metricList);                 
                }             
                return metric;
            });
        }
        /// <summary>
        /// Обработка метрик одного узла файла
        /// </summary>
        /// <param name="xmlNode">Узел</param>
        /// <param name="metricsList">Метрики</param>
        /// <param name="frameNumber">Номер кадра</param>
        private static void ReadXmlNode(XmlNode xmlNode, out List<Metric> metricsList, out int frameNumber)
        {
            try
            {
                metricsList = new List<Metric>();
                frameNumber = 0;
                AbstractMetricFactory firstTypeErrorProbabilityMF = new FirstTypeErrorProbabilityMetricFactory();
                AbstractMetricFactory secondTypeErrorProbabilityMF = new SecondTypeErrorProbabilityMetricFactory();
                AbstractMetricFactory missingTypeErrorProbabilityMF = new MissingProbabilityMetricFactory();
                AbstractMetricFactory precisionMetricFactory = new PrecisionMetricFactory();
                AbstractMetricFactory recallMetricFactory = new RecallMetricFactory();
                AbstractMetricFactory f1MeasureMetricFactory = new F1MeasureMetricFactory();

                if (xmlNode.Attributes != null)
                {
                    string attributeValue = xmlNode.Attributes["ID"].Value;
                    if (attributeValue != null)
                        frameNumber = Convert.ToInt32(attributeValue);
                }
                string secondTypeString = xmlNode["SecondTypeErrorProbability"].InnerText;
                if (secondTypeString == "UNDEFINED_METRIC")
                    metricsList.Add(secondTypeErrorProbabilityMF.GetMetric(Metric.UNDEFINED_METRIC));
                else
                    metricsList.Add(secondTypeErrorProbabilityMF.GetMetric(Convert.ToDouble(secondTypeString)));

                string firstTypeString = xmlNode["FirstTypeErrorProbability"].InnerText;
                if (firstTypeString == "UNDEFINED_METRIC")
                    metricsList.Add(firstTypeErrorProbabilityMF.GetMetric(Metric.UNDEFINED_METRIC));
                else
                    metricsList.Add(firstTypeErrorProbabilityMF.GetMetric(Convert.ToDouble(firstTypeString)));

                string missingTypeString = xmlNode["MissingProbability"].InnerText;
                if (missingTypeString == "UNDEFINED_METRIC")
                    metricsList.Add(missingTypeErrorProbabilityMF.GetMetric(Metric.UNDEFINED_METRIC));
                else
                    metricsList.Add(missingTypeErrorProbabilityMF.GetMetric(Convert.ToDouble(missingTypeString)));

                string precisionString = xmlNode["Precision"].InnerText;
                if (precisionString == "UNDEFINED_METRIC")
                    metricsList.Add(precisionMetricFactory.GetMetric(Metric.UNDEFINED_METRIC));
                else
                    metricsList.Add(precisionMetricFactory.GetMetric(Convert.ToDouble(precisionString)));

                string recallString = xmlNode["Recall"].InnerText;
                if (recallString == "UNDEFINED_METRIC")
                    metricsList.Add(recallMetricFactory.GetMetric(Metric.UNDEFINED_METRIC));
                else
                    metricsList.Add(recallMetricFactory.GetMetric(Convert.ToDouble(recallString)));

                string f1MeasureString = xmlNode["F1Measure"].InnerText;
                if (f1MeasureString == "UNDEFINED_METRIC")
                    metricsList.Add(f1MeasureMetricFactory.GetMetric(Metric.UNDEFINED_METRIC));
                else
                    metricsList.Add(f1MeasureMetricFactory.GetMetric(Convert.ToDouble(f1MeasureString)));
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Считывание текстовых областей для одного кадра
        /// </summary>
        /// <param name="xmlReader">xml - reader</param>
        /// <param name="textRegions">Текстовые области</param>
        /// <param name="frameNumber">Номер кадра</param>
        private static void ReadFrameTextBlocksInformation(System.Xml.XmlReader xmlReader, out List<TextRegion> textRegions,
            out int frameNumber)
        {
            try
            {
                textRegions = new List<TextRegion>();          
                xmlReader.MoveToFirstAttribute();
                frameNumber = Convert.ToInt32(xmlReader.Value);
                xmlReader.MoveToNextAttribute();
                int textRegionsNumber = Convert.ToInt32(xmlReader.Value);
                if (textRegionsNumber != 0)
                {
                    bool readingFrame = true;
                    while (xmlReader.Read() && readingFrame)
                    {
                        if (xmlReader.Name == "Frame")
                            readingFrame = false;
                        else if (xmlReader.Name == "TextRegion")
                        {
                            TextRegion textRegion = new TextRegion();
                            while (xmlReader.MoveToNextAttribute())
                            {
                                switch (xmlReader.Name)
                                {
                                    case "LeftUpPointIndexI":
                                        textRegion.MinBorderIndexI = Convert.ToInt32(xmlReader.Value);
                                        break;
                                    case "LeftUpPointIndexJ":
                                        textRegion.MinBorderIndexJ = Convert.ToInt32(xmlReader.Value);
                                        break;
                                    case "RightDownPointIndexI":
                                        textRegion.MaxBorderIndexI = Convert.ToInt32(xmlReader.Value);
                                        break;
                                    case "RightDownPointIndexJ":
                                        textRegion.MaxBorderIndexJ = Convert.ToInt32(xmlReader.Value);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            textRegions.Add(textRegion);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
