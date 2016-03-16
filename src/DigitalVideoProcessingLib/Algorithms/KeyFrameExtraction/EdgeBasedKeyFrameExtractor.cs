using DigitalVideoProcessingLib.Interface;
using DigitalVideoProcessingLib.IO;
using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Algorithms.KeyFrameExtraction
{
    public delegate void KeyFrameExtracted(int frameNumber);
    public class EdgeBasedKeyFrameExtractor: IKeyFrameExtraction
    {
        public static event KeyFrameExtracted keyFrameExtracted;

        /// <summary>
        /// Извлечение ключевых кадров из набора кадров видео
        /// </summary>
        /// <param name="data">Набор кадров</param>
        /// <returns></returns>
        public Task<List<GreyVideoFrame>> ExtractKeyFrames(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in ExtractKeyFrames");
                List<Image<Bgr, Byte>> coloredFrames = (List<Image<Bgr, Byte>>)data;
                List<Image<Gray, Byte>> frames = ColoredFramesToGrayscale(coloredFrames);

                return Task.Run(() =>
                {
                    List<int> framesDifferences = GetFramesDifferences(frames, new Gray(149), new Gray(149));
                    double matExp = CountMatExp(framesDifferences);
                    double sigma = CountSigma(framesDifferences, matExp);
                    double treshold = CountTreshold(matExp, sigma, 2);

                    return GetKeyFrames(frames, coloredFrames, framesDifferences, treshold);
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Переводит цветные кадры в серые
        /// </summary>
        /// <param name="coloredFrames">Цветные кадры</param>
        /// <returns>Серые кадры</returns>
        private List<Image<Gray, Byte>> ColoredFramesToGrayscale(List<Image<Bgr, Byte>> coloredFrames)
        {
            try
            {
                List<Image<Gray, Byte>> grayscaleFrames = new List<Image<Gray, byte>>();
                for (int i = 0; i < coloredFrames.Count; i++)
                    grayscaleFrames.Add(coloredFrames[i].Convert<Gray, Byte>());
                return grayscaleFrames;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Поиск ключевых кадров видео
        /// </summary>
        /// <param name="frames">Все серые кадры</param>
        /// <param name="coloredFrames">Все цветные кадры</param>
        /// <param name="framesDifferences">Разницы кадров</param>
        /// <param name="treshold">Порог</param>
        /// <returns>Ключевые кадры</returns>
        private List<GreyVideoFrame> GetKeyFrames(List<Image<Gray, Byte>> frames, List<Image<Bgr, Byte>> coloredFrames,
            List<int> framesDifferences, double treshold)
        {
            try
            {
                ImageConvertor imageConvertor = new ImageConvertor();

                List<GreyVideoFrame> keyFrames = new List<GreyVideoFrame>();
                GreyVideoFrame firstKeyFrame = new GreyVideoFrame();
                firstKeyFrame.FrameNumber = 0;
                firstKeyFrame.Frame = imageConvertor.ConvertColor(frames[0]);
                keyFrames.Add(firstKeyFrame);           

                int framesDifferencesNumber = framesDifferences.Count;
                int previousFrameNumber = 0;
                for (int i = 0; i < framesDifferencesNumber; i++)
                {
                    if (framesDifferences[i] > treshold && i + 1 != previousFrameNumber + 1)
                    {
                        GreyVideoFrame keyFrame = new GreyVideoFrame();
                        keyFrame.FrameNumber = i + 1;
                        previousFrameNumber = i + 1;
                        keyFrame.OriginalFrame = coloredFrames[i + 1];
                        keyFrame.Frame = imageConvertor.ConvertColor(frames[i + 1]);
                        keyFrames.Add(keyFrame);
                    }
                }
                return keyFrames;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисление порога
        /// </summary>
        /// <param name="matExp">Мат ожидание</param>
        /// <param name="sigma">СКО</param>
        /// <param name="parameter">Параметр</param>
        /// <returns>Порог</returns>
        private double CountTreshold(double matExp, double sigma, double parameter)
        {
            return matExp + parameter * sigma;
        }

        /// <summary>
        /// Подсчет математического ожидания для разниц в кадрах
        /// </summary>
        /// <param name="framesDifferences"></param>
        /// <returns>Мат. ожидание</returns>
        private double CountMatExp(List<int> framesDifferences)
        {
            try
            {               
                int differencesNumber = framesDifferences.Count;
                double matExp = 0.0;
                
                for (int i = 0; i < differencesNumber; i++)
                    matExp += framesDifferences[i];

                return matExp / (double) differencesNumber;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисление СКО для разниц в кадрах
        /// </summary>
        /// <param name="framesDifferences">Разницы в кадрах</param>
        /// <param name="matExp">Мат. ожидание</param>
        /// <returns>СКО</returns>
        private double CountSigma(List<int> framesDifferences, double matExp)
        {
            try
            {               
                int differencesNumber = framesDifferences.Count;
                double dispersia = 0.0;

                for (int i = 0; i < differencesNumber; i++)
                    dispersia += Math.Pow(framesDifferences[i] - matExp, 2);

                dispersia = dispersia / (double) differencesNumber;

                return Math.Sqrt(dispersia);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисление разлиц в границах между кадрами 
        /// </summary>
        /// <param name="frames">Кадры</param>
        /// <param name="cannyThreshold">Порог для Кенни</param>
        /// <param name="cannyThresholdLinking">Порог слияния границ для Кении</param>
        /// <returns></returns>
        private List<int> GetFramesDifferences(List<Image<Gray, Byte>> frames, Gray cannyThreshold, Gray cannyThresholdLinking)
        {
            try
            {
                if (frames == null)
                    throw new ArgumentNullException("Null frames in GetFramesDifferences");

                List<int> framesDifferences = new List<int>();

                int framesNumber = frames.Count - 1;
                for (int i = 0; i < framesNumber; i++)
                {
                    Image<Gray, Byte> currentCannyFrame = frames[i].Canny(cannyThreshold, cannyThresholdLinking);
                    Image<Gray, Byte> nextCannyFrame = frames[i + 1].Canny(cannyThreshold, cannyThresholdLinking);
                    int framesDifference = CountFramesDifference(currentCannyFrame, nextCannyFrame);
                    framesDifferences.Add(framesDifference);
                }
                return framesDifferences;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }       

        /// <summary>
        /// Вычисления разницы в границах между двумя кадрами
        /// </summary>
        /// <param name="firstFrame">Первый кадр</param>
        /// <param name="secondFrame">Второй кадр</param>
        /// <returns></returns>
        private int CountFramesDifference(Image<Gray, Byte> firstFrame, Image<Gray, Byte> secondFrame)
        {
            try
            {
                int frameHeigth = firstFrame.Height;
                int frameWidth = firstFrame.Width;

                int framesDifference = 0;
                for (int i = 0; i < frameHeigth; i++)
                    for (int j = 0; j < frameWidth; j++)
                        framesDifference += firstFrame.Data[i, j, 0] - secondFrame.Data[i, j, 0];
                    
                return framesDifference;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
