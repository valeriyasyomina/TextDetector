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
                    XmlWriter xmlWriter = XmlWriter.Create(fileName);
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
        private static void WriteTextBlocksInformation(List<TextRegion> textRegions, XmlWriter xmlWriter, int frameNumber, string fileName)
        {
            try
            {
                if (textRegions == null)
                    throw new ArgumentNullException("Null textRegions in WriteTextBlocksInformation");

                if (textRegions.Count != 0)
                {
                    xmlWriter.WriteStartElement("Frame");
                    xmlWriter.WriteAttributeString("ID", frameNumber.ToString());
                    int textRegionsNumber = textRegions.Count;
                    for (int i = 0; i < textRegionsNumber; i++)
                    {
                        xmlWriter.WriteStartElement("TextRegion");
                        xmlWriter.WriteAttributeString("LeftUpPointIndexI", textRegions[i].MinBorderIndexI.ToString());
                        xmlWriter.WriteAttributeString("LeftUpPointIndexJ", textRegions[i].MinBorderIndexJ.ToString());
                        xmlWriter.WriteAttributeString("RightDownPointIndexI", textRegions[i].MaxBorderIndexI.ToString());
                        xmlWriter.WriteAttributeString("RightDownPointIndexJ", textRegions[i].MaxBorderIndexJ.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
