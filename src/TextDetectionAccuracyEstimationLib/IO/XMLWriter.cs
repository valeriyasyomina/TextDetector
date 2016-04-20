using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TextDetectionAccuracyEstimationLib.AccuracyEstimation;
using TextDetectionAccuracyEstimationLib.Metrics;

namespace TextDetectionAccuracyEstimationLib.IO
{
    public class XMLWriter
    {
        /// <summary>
        /// Запись вычисленных метрик в xml - файл
        /// </summary>
        /// <param name="XMLFileName">Имя файла</param>
        /// <param name="metrics">Метрики</param>
        /// <returns></returns>
        public static Task WriteMetricsXML(string XMLFileName, Dictionary<int, List<Metric>> metrics)
        {
            try
            {
                if (XMLFileName == null || XMLFileName.Length == 0)
                    throw new ArgumentNullException("Null XMLFileName in WriteMetricsXML");
                return Task.Run(() =>
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = "\t";
                    System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(XMLFileName, settings);
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Metrics");

                    foreach (var pair in metrics)
                    {
                        if (pair.Key != AccuracyEstimator.VIDEO_METRICS)
                            WriteOneFrameMetrics(xmlWriter, pair.Key, pair.Value);
                        else
                            FriteVideoMetrics(xmlWriter, pair.Value);
                    }
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Запись метрик для видео в файл
        /// </summary>
        /// <param name="xmlWriter">Xml - writer</param>
        /// <param name="metricsForFrame">Метрики</param>
        private static void FriteVideoMetrics(System.Xml.XmlWriter xmlWriter, List<Metric> metricsForFrame)
        {
            try
            {
                xmlWriter.WriteStartElement("Video");            

                WriteMetricsArray(xmlWriter, metricsForFrame);

                xmlWriter.WriteEndElement();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Запись метрик в файл для одного кадра видео
        /// </summary>
        /// <param name="xmlWriter">xml - writer</param>
        /// <param name="frameNumber">Номер кадра</param>
        /// <param name="metricsForFrame">Метрики</param>
        private static void WriteOneFrameMetrics(System.Xml.XmlWriter xmlWriter, int frameNumber, List<Metric> metricsForFrame)
        {
            try
            {
                xmlWriter.WriteStartElement("Frame");
                xmlWriter.WriteAttributeString("ID", frameNumber.ToString());

                WriteMetricsArray(xmlWriter, metricsForFrame);
                
                xmlWriter.WriteEndElement();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Запись массива метрик в файл
        /// </summary>
        /// <param name="xmlWriter">xml - writer</param>
        /// <param name="metricsForFrame">Метрики</param>
        private static void WriteMetricsArray(System.Xml.XmlWriter xmlWriter, List<Metric> metricsForFrame)
        {
            try
            {
                for (int i = 0; i < metricsForFrame.Count; i++)
                {
                    if (metricsForFrame[i].GetType() == typeof(SecondTypeErrorProbability))
                    {
                        xmlWriter.WriteStartElement("SecondTypeErrorProbability");
                        if (metricsForFrame[i].Value == Metric.UNDEFINED_METRIC)
                            xmlWriter.WriteString("UNDEFINED_METRIC");
                        else
                            xmlWriter.WriteString(metricsForFrame[i].Value.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    else if (metricsForFrame[i].GetType() == typeof(FirstTypeErrorProbability))
                    {
                        xmlWriter.WriteStartElement("FirstTypeErrorProbability");
                        if (metricsForFrame[i].Value == Metric.UNDEFINED_METRIC)
                            xmlWriter.WriteString("UNDEFINED_METRIC");
                        else
                            xmlWriter.WriteString(metricsForFrame[i].Value.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    else if (metricsForFrame[i].GetType() == typeof(MissingProbability))
                    {
                        xmlWriter.WriteStartElement("MissingProbability");
                        if (metricsForFrame[i].Value == Metric.UNDEFINED_METRIC)
                            xmlWriter.WriteString("UNDEFINED_METRIC");
                        else
                            xmlWriter.WriteString(metricsForFrame[i].Value.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    else if (metricsForFrame[i].GetType() == typeof(Precision))
                    {
                        xmlWriter.WriteStartElement("Precision");
                        if (metricsForFrame[i].Value == Metric.UNDEFINED_METRIC)
                            xmlWriter.WriteString("UNDEFINED_METRIC");
                        else
                            xmlWriter.WriteString(metricsForFrame[i].Value.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    else if (metricsForFrame[i].GetType() == typeof(Recall))
                    {
                        xmlWriter.WriteStartElement("Recall");
                        if (metricsForFrame[i].Value == Metric.UNDEFINED_METRIC)
                            xmlWriter.WriteString("UNDEFINED_METRIC");
                        else
                            xmlWriter.WriteString(metricsForFrame[i].Value.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    else if (metricsForFrame[i].GetType() == typeof(F1Measure))
                    {
                        xmlWriter.WriteStartElement("F1Measure");
                        if (metricsForFrame[i].Value == Metric.UNDEFINED_METRIC)
                            xmlWriter.WriteString("UNDEFINED_METRIC");
                        else
                            xmlWriter.WriteString(metricsForFrame[i].Value.ToString());
                        xmlWriter.WriteEndElement();
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
