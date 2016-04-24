using DigitalVideoProcessingLib.Algorithms.TextDetection;
using DigitalVideoProcessingLib.IO;
using DigitalVideoProcessingLib.VideoFrameType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Mediators
{
    public delegate void FrameWasProcessed(GreyVideoFrame videoFrame);
    public class LoadDetectTextVideoMediator
    {
        /// <summary>
        /// Событие, связанное с завершением одного кадра видепотока
        /// </summary>
        public static event FrameWasProcessed FrameWasProcessedEvent;
        /// <summary>
        /// Одновременное считывание и обработка кадра видеопотока
        /// </summary>
        /// <param name="sWTVideoTextDetection">Детектор текста</param>
        /// <param name="frameLoader">Загрузчик кадра</param>
        /// <param name="keyFrameIOInformation">Информация об извлекаемом кадре</param>
        /// <param name="videoFileName">Имя видеофайла</param>
        /// <param name="threadsNumber">Число потоков для выделения текста</param>
        /// <returns></returns>
        public async static Task LoadDetectTextVideo(SWTVideoTextDetection sWTVideoTextDetection, FrameLoader frameLoader,
            List<KeyFrameIOInformation> keyFrameIOInformation, string videoFileName, int threadsNumber)
        {
            try
            {
                if (sWTVideoTextDetection == null)
                    throw new ArgumentNullException("Null sWTVideoTextDetection in LoadDetectTextVideo");
                if (frameLoader == null)
                    throw new ArgumentNullException("Null frameLoader in LoadDetectTextVideo");
                if (keyFrameIOInformation == null)
                    throw new ArgumentNullException("Null keyFrameIOInformation in LoadDetectTextVideo");
                if (videoFileName == null)
                    throw new ArgumentNullException("Null videoFileName in LoadDetectTextVideo");              

                for (int i = 0; i < keyFrameIOInformation.Count; i++)
                {
                    GreyVideoFrame frame = await frameLoader.LoadFrameAsync(videoFileName, keyFrameIOInformation[i]);
                    await sWTVideoTextDetection.DetectText(frame, threadsNumber);
                    FrameWasProcessedEvent(frame);
                }               
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
