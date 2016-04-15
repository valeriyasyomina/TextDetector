using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.IO;
using DigitalVideoProcessingLib.Interface;
using DigitalVideoProcessingLib.IO;
using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Algorithms.KeyFrameExtraction
{
    public delegate void KeyFrameExtracted(int firstFrameNumber, int secondFrameNumber, bool isLastFrame);
    public delegate void FramesDifference(int firstFrameNumber, int secondFrameNumber, bool isLastFrame);

    public class EdgeBasedKeyFrameExtractor: IKeyFrameExtraction
    {
        public static event KeyFrameExtracted keyFrameExtractedEvent;
        public static event FramesDifference framesDifferenceEvent;

        /// <summary>
        /// Извлечение ключевых кадров из набора кадров видео
        /// </summary>
        /// <param name="data">Набор кадров</param>
        /// <returns></returns>
        public Task<List<GreyVideoFrame>> ExtractKeyFramesAsync(object data)
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
        /// Извлечение ключевых кадров из видео за два прохода
        /// </summary>
        /// <param name="data">Имя видеофайла, ширина и высота кадра</param>
        /// <returns></returns>
        public Task<List<GreyVideoFrame>> ExtractKeyFramesTwoPassAsync(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in ExtractKeyFramesTwoPass");
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
                    List<int> framesDifferences = GetFramesDifferences(videoFileName, frameWidth, frameHeight, new Gray(149), new Gray(149));
                    double matExp = CountMatExp(framesDifferences);
                    double sigma = CountSigma(framesDifferences, matExp);
                    double treshold = CountTreshold(matExp, sigma, 2);
                    return GetKeyFrames(videoFileName, frameWidth, frameHeight, framesDifferences, treshold);
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
                    if (i == framesDifferencesNumber - 1)
                        keyFrameExtractedEvent(i, i + 1, true);
                    else
                        keyFrameExtractedEvent(i, i + 1, false);
                }
                return keyFrames;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Поиск ключевых кадров (второй проход алгоритма)
        /// </summary>
        /// <param name="videoFileName">Имя видеофайла</param>
        /// <param name="frameWidth">Ширина кадра</param>
        /// <param name="frameHeight">Высотка кадра</param>
        /// <param name="framesDifferences">Разница кадров</param>
        /// <param name="treshold">Порог</param>
        /// <returns></returns>
        private List<GreyVideoFrame> GetKeyFrames(string videoFileName, int frameWidth, int frameHeight,
            List<int> framesDifferences, double treshold)
        {
            try
            {
                List<GreyVideoFrame> keyFrames = new List<GreyVideoFrame>();
                ImageConvertor imageConvertor = new ImageConvertor();

                string videoPath = System.IO.Path.GetDirectoryName(videoFileName);
                string framesDirName = Path.Combine(videoPath, "VideoFrames");
                if (!Directory.Exists(framesDirName))
                    Directory.CreateDirectory(framesDirName);

                Capture capture = new Capture(videoFileName);
                Image<Gray, Byte> frame = capture.QueryGrayFrame().Resize(frameWidth, frameHeight, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);                
                AddKeyFrameFunction(keyFrames, frame, Path.Combine(framesDirName, "0.jpg"), 0, true);       

                int framesDifferencesNumber = framesDifferences.Count;
                int previousFrameNumber = 0;
             //   BitmapConvertor bitmapConvertor = new BitmapConvertor();
                for (int i = 0; i < framesDifferencesNumber; i++)
                {
                    frame = capture.QueryGrayFrame();
                    if (framesDifferences[i] > treshold && i + 1 != previousFrameNumber + 1)
                    { 
                        int frameNumber = i + 1;
                        previousFrameNumber = i + 1; 
                        frame = capture.QueryGrayFrame().Resize(frameWidth, frameHeight, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);                       
                        AddKeyFrameFunction(keyFrames, frame, Path.Combine(framesDirName, frameNumber.ToString() + ".jpg"), frameNumber, true);                   
                    }
                    if (i == framesDifferencesNumber - 1)
                        keyFrameExtractedEvent(i, i + 1, true);
                    else
                        keyFrameExtractedEvent(i, i + 1, false);
                }
                return keyFrames;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Добавление ключевого кадра в список ключевых кадров
        /// </summary>
        /// <param name="keyFrames">Список клбючевых кадров</param>
        /// <param name="frame">Ключевой кадр</param>
        /// <param name="frameFileName">Имя файла для сохранения ключевого кадра</param>
        /// <param name="frameNumber">Номер ключевого кадра</param>
        /// <param name="needProcess">Нуждается ли кадр в обработке</param>
        private void AddKeyFrameFunction(List<GreyVideoFrame> keyFrames, Image<Gray, Byte> frame, string frameFileName, int frameNumber,
            bool needProcess)
        {
            try
            {
                frame.Save(frameFileName);
                Bitmap bitmapFrame = new Bitmap(frameFileName);                         

                GreyVideoFrame keyFrame = new GreyVideoFrame();
                keyFrame.FrameNumber = frameNumber;
                BitmapConvertor bitmapConvertor = new BitmapConvertor();   
                keyFrame.Frame = bitmapConvertor.ToGreyImage(bitmapFrame);
                keyFrame.NeedProcess = needProcess;
                keyFrames.Add(keyFrame);
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
                    framesDifferenceEvent(i, i + 1, false);
                }
                return framesDifferences;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисление разницы кадров (первый проход алгоритма)
        /// </summary>
        /// <param name="videoFileName">Имя видеофайла</param>        
        /// <param name="cannyThreshold">Порог для Кенни</param>
        /// <param name="cannyThresholdLinking">Порог слияния границ для Кении</param>
        /// <returns></returns>
        private List<int> GetFramesDifferences(string videoFileName, int frameWidth, int frameHeight, Gray cannyThreshold, Gray cannyThresholdLinking)
        {
            try
            {
                List<int> framesDifferences = new List<int>();

                Capture capture = new Capture(videoFileName);
                Image<Gray, Byte> currentFrame = capture.QueryGrayFrame().Resize(frameWidth, frameHeight, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR); 
                Image<Gray, Byte> nextFrame = null;
                int frameNumber = 0;
                do
                {
                    nextFrame = capture.QueryGrayFrame();//.Resize(frameWidth, frameHeight, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);                    
                    ++frameNumber;
                    if (nextFrame != null)
                    {
                        nextFrame = nextFrame.Resize(frameWidth, frameHeight, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                        Image<Gray, Byte> currentCannyFrame = currentFrame.Canny(cannyThreshold, cannyThresholdLinking);
                        Image<Gray, Byte> nextCannyFrame = nextFrame.Canny(cannyThreshold, cannyThresholdLinking);
                        int framesDifference = CountFramesDifference(currentCannyFrame, nextCannyFrame);
                        framesDifferences.Add(framesDifference);
                        currentFrame = nextFrame;
                        framesDifferenceEvent(frameNumber - 1, frameNumber, false);
                    }
                    else
                        framesDifferenceEvent(frameNumber - 1, frameNumber, true);
                }
                while (nextFrame != null);

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
        /// <summary>
        /// Проверка, есть ли кадр в списке ключевых и добавление его в список ключевых
        /// </summary>
        /// <param name="keyFrames">Ключевые кадры</param>
        /// <param name="keyFramesInformation">Информация, какие кадры нужны</param>
        /// <param name="frame">Текущий кадр</param>
        /// <param name="framesDirName">Путь к директории</param>
        /// <param name="frameNumber">Номер текущего кадра по порядку</param>
        /// <param name="frameWidth">Ширина кадра</param>
        /// <param name="frameHeight">Высота кадра</param>
        private void CheckKeyFrameAndAddIfInList(List<GreyVideoFrame> keyFrames, List<KeyFrameIOInformation> keyFramesInformation,
            Image<Gray, Byte> frame, string framesDirName, int frameNumber, int frameWidth, int frameHeight)
        {
            try
            {
                if (frame != null)
                {
                    bool needProcess = false;
                    if (IsPresentInKeyFrameInformationList(keyFramesInformation, frameNumber, ref needProcess))
                    {                    
                        frame = frame.Resize(frameWidth, frameHeight, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                        AddKeyFrameFunction(keyFrames, frame, Path.Combine(framesDirName, frameNumber.ToString() + ".jpg"), frameNumber, needProcess);                        
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Проверка, присутствует ли номер кадра в списке кадров, необходимых для извлечения
        /// </summary>
        /// <param name="keyFramesInformation">Информация о кадрах, котрые необходимо извлечь</param>
        /// <param name="frameNumber">Номер кадра, котрый ищем в списке</param>
        /// <param name="needProcess">Нуждается ли кадр в обработке или нет</param>
        /// <returns>1 - кадр найден, 0 - иначе</returns>
        private bool IsPresentInKeyFrameInformationList(List<KeyFrameIOInformation> keyFramesInformation, int frameNumber, ref bool needProcess)
        {
            try
            {
                bool result = false;
                for (int i = 0; i < keyFramesInformation.Count && !result; i++)
                {
                    if (keyFramesInformation[i].Number == frameNumber)
                    {
                        result = true;
                        needProcess = keyFramesInformation[i].NeedProcess;
                    }
                }            
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Извлечение ключевых кадров из списка кадров
        /// </summary>
        /// <param name="videoFileName">Имя видео файла</param>
        /// <param name="frameWidth">Ширина кадра</param>
        /// <param name="frameHeight">Высота кадра</param>
        /// <param name="keyFramesInformation">Список нужных кадров</param>
        /// <returns>Ключевые кадры</returns>
        private List<GreyVideoFrame> GetKeyFrames(string videoFileName, int frameWidth, int frameHeight, List<KeyFrameIOInformation> keyFramesInformation)
        {
            try
            {
                List<GreyVideoFrame> keyFrames = new List<GreyVideoFrame>();
                ImageConvertor imageConvertor = new ImageConvertor();

                string videoPath = System.IO.Path.GetDirectoryName(videoFileName);
                string framesDirName = Path.Combine(videoPath, "VideoFrames");
                if (!Directory.Exists(framesDirName))
                    Directory.CreateDirectory(framesDirName);      

                Capture capture = new Capture(videoFileName);
                Image<Gray, Byte> frame = capture.QueryGrayFrame();

                int frameNumber = 0;
                CheckKeyFrameAndAddIfInList(keyFrames, keyFramesInformation, frame, framesDirName, frameNumber, frameWidth, frameHeight);
                if (frame != null)
                {
                    keyFrameExtractedEvent(frameNumber, frameNumber + 1, false);
                    do
                    {
                        frame = capture.QueryGrayFrame();
                        ++frameNumber;
                        CheckKeyFrameAndAddIfInList(keyFrames, keyFramesInformation, frame, framesDirName, frameNumber, frameWidth, frameHeight);
                        if (frame != null)
                            keyFrameExtractedEvent(frameNumber, frameNumber + 1, false);
                        else
                            keyFrameExtractedEvent(frameNumber, frameNumber + 1, true);
                    }
                    while (frame != null);
                }
                else
                    keyFrameExtractedEvent(frameNumber, frameNumber + 1, true);
                return keyFrames;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Извлечение ключевых кадров по номерам из заданного списка
        /// </summary>
        /// <param name="data">Данные</param>
        /// <returns>Ключевые кадры</returns>
        public Task<List<GreyVideoFrame>> ExtractKeyFramesByListNumberAsync(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in ExtractKeyFrames");
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
                List<KeyFrameIOInformation> keyFramesInformation = ioData.KeyFrameIOInformation;
                if (keyFramesInformation == null)
                    throw new ArgumentNullException("Null keyFramesInformation in ExtractKeyFrames");
                
                return Task.Run(() =>
                {
                    return GetKeyFrames(videoFileName, frameWidth, frameHeight, keyFramesInformation);
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
