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
                    throw new ArgumentNullException("Null data in LoadFrameAsync");
                IOData ioData = (IOData)data;
                string frameFileName = ioData.FileName;
                if (frameFileName == null || frameFileName.Length == 0)
                    throw new ArgumentNullException("Null frameFileName in LoadFrameAsync");
                if (ioData.FrameHeight <= 0)
                    throw new ArgumentException("Error frameHeight value in LoadFrameAsync");
                if (ioData.FrameWidth <= 0)
                    throw new ArgumentException("Error frameWidth value in LoadFrameAsync");

                return Task.Run(() =>
                {
                    Size size = new Size(ioData.FrameWidth, ioData.FrameHeight);
                    Bitmap bitmapFrame = new Bitmap(frameFileName);                    
                    
                  //  bitmapFrame.SetResolution(ioData.FrameWidth, ioData.FrameHeight);
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
                  /*  string videoPath = System.IO.Path.GetDirectoryName(videoFileName);
                    string framesDirName = System.IO.Path.Combine(videoPath, "VideoFrames");
                    if (!Directory.Exists(framesDirName))
                        Directory.CreateDirectory(framesDirName);*/

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
                       // string frameFileName = Path.Combine(framesDirName, keyFrameIOInformation.Number.ToString() + ".jpg");
                        frame = frame.Resize(keyFrameIOInformation.Width, keyFrameIOInformation.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                      //  frame.Save(frameFileName);
                        videoFrame = CreateVideoFrame(frame, keyFrameIOInformation);                        
                    }
                    capture.Dispose();
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
        /// <param name="frame">Кадр</param>
        /// <param name="keyFrameIOInformation">Информация о кадре</param>      
        /// <returns>Кадр</returns>
        private GreyVideoFrame CreateVideoFrame(Image<Gray, byte> frame, KeyFrameIOInformation keyFrameIOInformation)
        {
            try
            {
              //  Bitmap bitmapFrame = new Bitmap(frameFileName);

                ImageConvertor ImageConvertor = new DigitalVideoProcessingLib.IO.ImageConvertor();
                

                GreyVideoFrame keyFrame = new GreyVideoFrame();
                keyFrame.FrameNumber = keyFrameIOInformation.Number;
                BitmapConvertor bitmapConvertor = new BitmapConvertor();

                keyFrame.Frame = ImageConvertor.ConvertColor(frame);//bitmapConvertor.ToGreyImage(bitmapFrame);

                keyFrame.NeedProcess = keyFrameIOInformation.NeedProcess;
                keyFrame.AspectRatio = keyFrameIOInformation.AspectRatio;
                keyFrame.BbPixelsNumberMaxRatio = keyFrameIOInformation.BbPixelsNumberMaxRatio;
                keyFrame.BbPixelsNumberMinRatio = keyFrameIOInformation.BbPixelsNumberMinRatio;
                keyFrame.DiamiterSWRatio = keyFrameIOInformation.DiamiterSWRatio;
                keyFrame.ImageRegionHeightRationMin = keyFrameIOInformation.ImageRegionHeightRationMin;
                keyFrame.ImageRegionWidthRatioMin = keyFrameIOInformation.ImageRegionWidthRatioMin;
                keyFrame.MergeByDirectionAndChainEnds = keyFrameIOInformation.MergeByDirectionAndChainEnds;
                keyFrame.MinLettersNumberInTextRegion = keyFrameIOInformation.MinLettersNumberInTextRegion;
                keyFrame.PairsHeightRatio = keyFrameIOInformation.PairsHeightRatio;
                keyFrame.PairsIntensityRatio = keyFrameIOInformation.PairsIntensityRatio;
                keyFrame.PairsOccupationRatio = keyFrameIOInformation.PairsOccupationRatio;
                keyFrame.PairsSWRatio = keyFrameIOInformation.PairsSWRatio;
                keyFrame.PairsWidthDistanceSqrRatio = keyFrameIOInformation.PairsWidthDistanceSqrRatio;
                keyFrame.UseAdaptiveSmoothing = keyFrameIOInformation.UseAdaptiveSmoothing;
                keyFrame.VarienceAverageSWRation = keyFrameIOInformation.VarienceAverageSWRation;
                keyFrame.GaussFilterSize = keyFrameIOInformation.GaussFilterSize;
                keyFrame.GaussFilterSigma = keyFrameIOInformation.GaussFilterSigma;
                keyFrame.CannyLowTreshold = keyFrameIOInformation.CannyLowTreshold;
                keyFrame.CannyHighTreshold = keyFrameIOInformation.CannyHighTreshold;
                return keyFrame;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
