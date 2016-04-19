using DigitalImageProcessingLib.Algorithms.EdgeDetection;
using DigitalImageProcessingLib.Algorithms.TextDetection;
using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType;
using DigitalImageProcessingLib.Filters.FilterType.GradientFilterType;
using DigitalImageProcessingLib.Filters.FilterType.SmoothingFilterType;
using DigitalImageProcessingLib.Interface;
using DigitalVideoProcessingLib.Interface;
using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Algorithms.TextDetection
{
    public class SWTVideoTextDetection : DigitalVideoProcessingLib.Interface.ITextDetection
    {
        #region Properties
        /// <summary>
        /// Изменение ширины штриха в пределах одного региона
        /// </summary>
        public double VarienceAverageSWRation { get; set; }
        /// <summary>
        /// Соотношение сторон регионов
        /// </summary>
        public double AspectRatio { get; set; }
        /// <summary>
        /// Соотношение средней ширины штриха и диаметра региона
        /// </summary>
        public double DiamiterSWRatio { get; set; }
        /// <summary>
        /// Минимальное число пикселей в рамке
        /// </summary>
        public double BbPixelsNumberMinRatio { get; set; }
        /// <summary>
        /// Максимальное число пикселей в рамке
        /// </summary>
        public double BbPixelsNumberMaxRatio { get; set; }
        /// <summary>
        /// Соотношение высоты региона и высоты изображения (для удаления слишком больших регмонов)
        /// </summary>
        public double ImageRegionHeightRationMin { get; set; }
        /// <summary>
        /// Соотношение ширины региона и ширины изображения (для удаления слишком больших регмонов)
        /// </summary>
        public double ImageRegionWidthRatioMin { get; set; }
        /// <summary>
        /// Соотношение высот пар регионов
        /// </summary>
        public double PairsHeightRatio { get; set; }
        /// <summary>
        /// Разница интенсивности пар регионов 
        /// </summary>
        public double PairsIntensityRatio { get; set; }
        /// <summary>
        /// Соотношение средних ширин штрихов пар регионов
        /// </summary>
        public double PairsSWRatio { get; set; }
        /// <summary>
        /// Соотношение расстояния и ширины наименьшего региона из пары
        /// </summary>
        public double PairsWidthDistanceSqrRatio { get; set; }
        /// <summary>
        /// Соотношение числа пикселей для двух регионов
        /// </summary>
        public double PairsOccupationRatio { get; set; }
        /// <summary>
        /// Минимальное число букв в текстовой области
        /// </summary>
        public int MinLettersNumberInTextRegion { get; set; }
        /// <summary>
        /// Размер апертуры фильтра Гаусса
        /// </summary>
        public int GaussFilterSize { get; set; }
        /// <summary>
        /// Sigma фильтра Гаусса
        /// </summary>
        public double GaussFilterSigma { get; set; }
        /// <summary>
        /// Нижний порог для Кенни
        /// </summary>
        public int CannyLowTreshold { get; set; }
        /// <summary>
        /// Верхний порог для Кенни
        /// </summary>
        public int CannyHighTreshold { get; set; }
        #endregion
        public SWTVideoTextDetection(double varienceAverageSWRation, int gaussFilterSize = 5,
            double gaussFilterSigma = 1.4, int cannyLowTreshold = 20, int CannyHighTreshold = 80,  double aspectRatio = 5.0,
            double diamiterSWRatio = 10, double bbPixelsNumberMinRatio = 1.5, double bbPixelsNumberMaxRatio = 25.0,
            double imageRegionHeightRationMin = 1.5, double imageRegionWidthRatioMin = 1.5, double pairsHeightRatio = 2.0,
            double pairsIntensityRatio = 1.0, double pairsSWRatio = 1.5, double pairsWidthDistanceSqrRatio = 9.0,
            double pairsOccupationRatio = 2.0, int minLettersNumberInTextRegion = 2)
        {
            this.GaussFilterSize = gaussFilterSize;
            this.GaussFilterSigma = gaussFilterSigma;
            this.CannyLowTreshold = cannyLowTreshold;
            this.CannyHighTreshold = CannyHighTreshold;

            this.VarienceAverageSWRation = varienceAverageSWRation;
            this.AspectRatio = aspectRatio;
            this.DiamiterSWRatio = diamiterSWRatio;
            this.BbPixelsNumberMinRatio = bbPixelsNumberMinRatio;
            this.BbPixelsNumberMaxRatio = bbPixelsNumberMaxRatio;
            this.ImageRegionHeightRationMin = imageRegionHeightRationMin;
            this.ImageRegionWidthRatioMin = imageRegionWidthRatioMin;

            this.PairsHeightRatio = pairsHeightRatio;
            this.PairsIntensityRatio = pairsIntensityRatio;
            this.PairsSWRatio = pairsSWRatio;
            this.PairsWidthDistanceSqrRatio = pairsWidthDistanceSqrRatio;
            this.PairsOccupationRatio = pairsOccupationRatio;

            this.MinLettersNumberInTextRegion = minLettersNumberInTextRegion;
        }

        /// <summary>
        /// Выделение текста на кд\лючевых кадрах видеоролика
        /// </summary>
        /// <param name="video">Видеоролик</param>
        /// <returns>true</returns>
        public Task<bool> DetectText(GreyVideo video, int threadsNumber)
        {
            try
            {
                if (video == null)
                    throw new ArgumentNullException("Null video in DetectText");
                if (video.Frames == null)
                    throw new ArgumentNullException("Null video frames in DetectText");
                
                return Task.Run(() =>
                {
                    for (int i = 0; i < video.Frames.Count; i++)
                        if (video.Frames[i].NeedProcess)
                        {
                            EdgeDetectionFilter sobel = new SobelFilter();
                            SmoothingFilter gauss = new GaussFilter(this.GaussFilterSize, this.GaussFilterSigma);
                            GradientFilter gradientFiler = new SimpleGradientFilter();
                            CannyEdgeDetection canny = new CannyEdgeDetection(gauss, sobel, this.CannyLowTreshold, this.CannyHighTreshold);

                            SWTTextDetection SWTTextDetection = new SWTTextDetection(canny, gradientFiler, this.VarienceAverageSWRation,
                                this.AspectRatio, this.DiamiterSWRatio, this.BbPixelsNumberMinRatio, this.BbPixelsNumberMaxRatio, this.ImageRegionHeightRationMin, 
                                this.ImageRegionWidthRatioMin, this.PairsHeightRatio, this.PairsIntensityRatio, this.PairsSWRatio,
                                this.PairsWidthDistanceSqrRatio, this.PairsOccupationRatio, this.MinLettersNumberInTextRegion);

                            SWTTextDetection.DetectText(video.Frames[i].Frame, threadsNumber);
                        }
                   return true;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Выделение текста на единичном кадре видеопотока
        /// </summary>
        /// <param name="videoFrame">Кадр видео</param>
        /// <returns>true</returns>
        public Task<bool> DetectText(GreyVideoFrame videoFrame, int threadsNumber)
        {
            try
            {
                if (videoFrame == null || videoFrame.Frame == null)
                    throw new ArgumentNullException("Null frame in DetectText");

                EdgeDetectionFilter sobel = new SobelFilter();
                SmoothingFilter gauss = new GaussFilter(this.GaussFilterSize, this.GaussFilterSigma);
                GradientFilter gradientFiler = new SimpleGradientFilter();
                CannyEdgeDetection canny = new CannyEdgeDetection(gauss, sobel, this.CannyLowTreshold, this.CannyHighTreshold);

                SWTTextDetection SWTTextDetection = new SWTTextDetection(canny, gradientFiler, this.VarienceAverageSWRation,
                                this.AspectRatio, this.DiamiterSWRatio, this.BbPixelsNumberMinRatio, this.BbPixelsNumberMaxRatio, this.ImageRegionHeightRationMin,
                                this.ImageRegionWidthRatioMin, this.PairsHeightRatio, this.PairsIntensityRatio, this.PairsSWRatio,
                                this.PairsWidthDistanceSqrRatio, this.PairsOccupationRatio, this.MinLettersNumberInTextRegion);
                return Task.Run(() =>
                {
                        SWTTextDetection.DetectText(videoFrame.Frame, threadsNumber);
                        return true;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
