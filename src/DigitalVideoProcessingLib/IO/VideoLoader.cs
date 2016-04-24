using DigitalVideoProcessingLib.Interface;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class VideoLoader: IVideoLoader
    {
        public delegate void FrameLoaded(int frameNumber, bool isLastFrame);
        public static event FrameLoaded frameLoadedEvent;
        /// <summary>
        /// Подсчет количества кадров видео
        /// </summary>
        /// <param name="data">Информация о видео</param>
        /// <returns>Количество кадров</returns>
        public Task<int> CountFramesNumberAsync(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in LoadFrames");
                IOData ioData = (IOData)data;
                string videoFileName = ioData.FileName;
                if (videoFileName == null || videoFileName.Length == 0)
                    throw new ArgumentNullException("Null videoFileName in LoadFrames");               

                return Task.Run(() =>
                {
                    List<Image<Bgr, Byte>> frames = new List<Image<Bgr, byte>>();

                    Capture capture = new Capture(videoFileName);
                    Image<Bgr, Byte> frame = null;
                    int frameNumber = 0;
                    do
                    {
                        frame = capture.QueryFrame();
                        if (frame != null)
                            ++frameNumber;                      
                    }
                    while (frame != null);

                    return frameNumber;
                });               
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Загрузка кадров
        /// </summary>
        /// <param name="data">Информация о видео</param>
        /// <returns>Кадры</returns>
        public Task<List<Image<Bgr, Byte>>> LoadFramesAsync(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in LoadFrames");
                IOData ioData = (IOData)data;
                string videoFileName = ioData.FileName;
                if (videoFileName == null || videoFileName.Length == 0)
                    throw new ArgumentNullException("Null videoFileName in LoadFrames");
                int frameWidth = ioData.FrameWidth;
                if (frameWidth <= 0)
                    throw new ArgumentException("Error frameWidth in LoadFrames");
                int frameHeight = ioData.FrameHeight;
                if (frameHeight <= 0)
                    throw new ArgumentException("Error frameHeight in LoadFrames");

                return Task.Run(() =>
                {
                    List<Image<Bgr, Byte>> frames = new List<Image<Bgr, byte>>();

                    Capture capture = new Capture(videoFileName);
                    Image<Bgr, Byte> frame = null;
                    int frameNumber = 0;
                    do
                    {
                        frame = capture.QueryFrame();
                        ++frameNumber;                        
                        if (frame != null)
                        {
                            Image<Bgr, Byte> resizedFrame = frame.Resize(frameWidth, frameHeight, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                            frames.Add(resizedFrame);
                            frameLoadedEvent(frameNumber, false);
                        }
                        else
                            frameLoadedEvent(frameNumber, true);
                    }
                    while (frame != null);

                    return frames;
                });               
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
