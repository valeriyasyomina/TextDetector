using DigitalImageProcessingLib.IO;
using DigitalVideoProcessingLib.Interface;
using DigitalVideoProcessingLib.VideoFrameType;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class FrameLoader : IFrameLoader
    {
        /// <summary>
        /// Загрузка одного кадра (с изображения)
        /// </summary>
        /// <param name="data">Информация о кадре</param>
        /// <returns>Кадр</returns>
        public Task<GreyVideoFrame> LoadFrameAsync(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in LoadFrames");
                IOData ioData = (IOData)data;
                string frameFileName = ioData.FileName;
                if (frameFileName == null || frameFileName.Length == 0)
                    throw new ArgumentNullException("Null frameFileName in LoadFrames");                

                return Task.Run(() =>
                {
                    Bitmap bitmapFrame = new Bitmap(frameFileName);
                    BitmapConvertor bitmapConvertor = new BitmapConvertor();
                    GreyVideoFrame greyVideoFrame = new GreyVideoFrame();

                    greyVideoFrame.Frame = bitmapConvertor.ToGreyImage(bitmapFrame);
                    return greyVideoFrame;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Загрузка кадра по номеру (с видео)
        /// </summary>
        /// <param name="videoFileName">Имя видеофайла</param>
        /// <param name="keyFrameIOInformation">Информация о кадре</param>
        /// <returns>Кард</returns>
        public Task<GreyVideoFrame> LoadFrameAsync(string videoFileName, KeyFrameIOInformation keyFrameIOInformation)
        {
            try
            {
                if (videoFileName == null || videoFileName.Length == 0)
                    throw new ArgumentNullException("Null videoFileName in LoadFrameAsync");
                if (keyFrameIOInformation == null)
                    throw new ArgumentNullException("Null keyFrameIOInformation in LoadFrameAsync");
                if (keyFrameIOInformation.Number < 0)
                    throw new ArgumentException("Error frameNumber in LoadFrameAsync");
                if (keyFrameIOInformation.Width <= 0)
                    throw new ArgumentException("Error Width in LoadFrameAsync");
                if (keyFrameIOInformation.Height <= 0)
                    throw new ArgumentException("Error Height in LoadFrameAsync");

                return Task.Run(() =>
                {
                    string videoPath = System.IO.Path.GetDirectoryName(videoFileName);
                    string framesDirName = System.IO.Path.Combine(videoPath, "VideoFrames");
                    if (!Directory.Exists(framesDirName))
                        Directory.CreateDirectory(framesDirName);

                    GreyVideoFrame videoFrame = null;

                    int currentFrameNumnber = -1;
                    Capture capture = new Capture(videoFileName);
                    Image<Gray, byte> frame = null;
                    while (currentFrameNumnber != keyFrameIOInformation.Number)
                    {
                        frame = capture.QueryGrayFrame();
                        currentFrameNumnber++;
                    }
                    if (frame != null)
                    {
                        string frameFileName = Path.Combine(framesDirName, keyFrameIOInformation.Number.ToString() + ".jpg");
                        frame = frame.Resize(keyFrameIOInformation.Width, keyFrameIOInformation.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                        frame.Save(frameFileName);
                        videoFrame = CreateVideoFrame(frameFileName, keyFrameIOInformation.Number, keyFrameIOInformation.NeedProcess);                        
                    }
                    return videoFrame;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Создание кадра видео 
        /// </summary>
        /// <param name="frameFileName">Имя файла</param>
        /// <param name="frameNumber">Номер кадра</param>
        /// <param name="needProcess">Нуждается ли кадр в обработке</param>
        /// <returns>Кадр</returns>
        private GreyVideoFrame CreateVideoFrame(string frameFileName, int frameNumber, bool needProcess)
        {
            try
            {
                Bitmap bitmapFrame = new Bitmap(frameFileName);
                GreyVideoFrame keyFrame = new GreyVideoFrame();
                keyFrame.FrameNumber = frameNumber;
                BitmapConvertor bitmapConvertor = new BitmapConvertor();
                keyFrame.Frame = bitmapConvertor.ToGreyImage(bitmapFrame);
                keyFrame.NeedProcess = needProcess;

                return keyFrame;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
