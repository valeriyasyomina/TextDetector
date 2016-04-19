using DigitalImageProcessingLib.RegionData;
using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DigitalVideoProcessingLib.IO
{
    public class XMLWriter
    {
        /// <summary>
        /// Запись инфориации о текстовых блоках видеопотока в XML - файл 
        /// </summary>
        /// <param name="video">Видео</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns>true</returns>
        public static Task<bool> WriteTextBlocksInformation(GreyVideo video, string fileName)
        {
            if (video == null || video.Frames == null)
                throw new ArgumentNullException("Null video in WriteTextBlocksInformation");
            if (fileName == null || fileName.Length == 0)
                throw new ArgumentNullException("Null fileName in WriteTextBlocksInformation");
            try
            {
                return Task.Run(() =>
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = "\t";
                    System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(fileName, settings);
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Video");

                    for (int i = 0; i < video.Frames.Count; i++)
                        XMLWriter.WriteTextBlocksInformation(video.Frames[i].Frame.TextRegions, xmlWriter, video.Frames[i].FrameNumber, fileName);
                    
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                    return true;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Запись инфориации о текстовых блоках кадра видеопотока в XML - файл 
        /// </summary>
        /// <param name="videoFrame">Фрейм видео</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns>true</returns>
        public static Task<bool> WriteTextBlocksInformation(GreyVideoFrame videoFrame, string fileName)
        {
            if (videoFrame == null)
                throw new ArgumentNullException("Null videoFrame in WriteTextBlocksInformation");
            if (fileName == null || fileName.Length == 0)
                throw new ArgumentNullException("Null fileName in WriteTextBlocksInformation");
            try
            {
                return Task.Run(() =>
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = "\t";
                    XmlWriter xmlWriter = XmlWriter.Create(fileName, settings);
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Video");                    
                    XMLWriter.WriteTextBlocksInformation(videoFrame.Frame.TextRegions, xmlWriter, videoFrame.FrameNumber, fileName);                    
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                    return true;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Запись информации о текстовых блоках одного кадра в XML - файл (частичная запись)
        /// </summary>
        /// <param name="textRegions">Текстовые регионы</param>
        /// <param name="xmlWriter">xml - writer</param>
        /// <param name="frameNumber">Номер кадра</param>
        /// <param name="fileName">Имя xml - файла</param>
        private static void WriteTextBlocksInformation(List<TextRegion> textRegions, System.Xml.XmlWriter xmlWriter, int frameNumber, string fileName)
        {
            try
            {
                if (textRegions == null)
                    throw new ArgumentNullException("Null textRegions in WriteTextBlocksInformation");

                xmlWriter.WriteStartElement("Frame");
                xmlWriter.WriteAttributeString("ID", frameNumber.ToString());
                int textRegionsNumber = textRegions.Count;
                xmlWriter.WriteAttributeString("TextRegionsNumber", textRegionsNumber.ToString());
                if (textRegions.Count != 0)
                {                    
                    for (int i = 0; i < textRegionsNumber; i++)
                    {
                        xmlWriter.WriteStartElement("TextRegion");
                        xmlWriter.WriteAttributeString("LeftUpPointIndexI", textRegions[i].MinBorderIndexI.ToString());
                        xmlWriter.WriteAttributeString("LeftUpPointIndexJ", textRegions[i].MinBorderIndexJ.ToString());
                        xmlWriter.WriteAttributeString("RightDownPointIndexI", textRegions[i].MaxBorderIndexI.ToString());
                        xmlWriter.WriteAttributeString("RightDownPointIndexJ", textRegions[i].MaxBorderIndexJ.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
