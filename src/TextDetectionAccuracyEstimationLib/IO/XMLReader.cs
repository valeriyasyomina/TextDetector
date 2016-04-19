using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
