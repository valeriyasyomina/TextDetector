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
        /// <summary>
        /// Объединять ли цепочки по направлению и конечным элементам
        /// </summary>
        bool MergeByDirectionAndChainEnds { get; set; }
        /// <summary>
        /// Использовать или нет адаптивное сглаживание
        /// </summary>
        bool UseAdaptiveSmoothing { get; set; }
        #endregion
        public SWTVideoTextDetection(double varienceAverageSWRation, int gaussFilterSize = 5,
            double gaussFilterSigma = 1.4, int cannyLowTreshold = 20, int CannyHighTreshold = 80,  double aspectRatio = 5.0,
            double diamiterSWRatio = 10, double bbPixelsNumberMinRatio = 1.5, double bbPixelsNumberMaxRatio = 25.0,
            double imageRegionHeightRationMin = 1.5, double imageRegionWidthRatioMin = 1.5, double pairsHeightRatio = 2.0,
            double pairsIntensityRatio = 1.0, double pairsSWRatio = 1.5, double pairsWidthDistanceSqrRatio = 9.0,
            double pairsOccupationRatio = 2.0, int minLettersNumberInTextRegion = 2, bool mergeByDirectionAndChainEnds = false,
            bool useAdaptiveSmoothing = false)
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
            this.MergeByDirectionAndChainEnds = mergeByDirectionAndChainEnds;
            this.UseAdaptiveSmoothing = useAdaptiveSmoothing;
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

                            SWTTextDetection SWTTextDetection = new SWTTextDetection(canny, null, gradientFiler, this.VarienceAverageSWRation,
                                this.AspectRatio, this.DiamiterSWRatio, this.BbPixelsNumberMinRatio, this.BbPixelsNumberMaxRatio, this.ImageRegionHeightRationMin, 
                                this.ImageRegionWidthRatioMin, this.PairsHeightRatio, this.PairsIntensityRatio, this.PairsSWRatio,
                                this.PairsWidthDistanceSqrRatio, this.PairsOccupationRatio, this.MinLettersNumberInTextRegion, this.MergeByDirectionAndChainEnds);

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
                
                return Task.Run(() =>
                {
                    if (videoFrame.NeedProcess)
                    {
                        EdgeDetectionFilter sobel = new SobelFilter();
                        GradientFilter gradientFiler = new SimpleGradientFilter();
                        SmoothingFilter gauss = null; //new GaussFilter(this.GaussFilterSize, this.GaussFilterSigma);
                        SmoothingFilter gaussForCanny = null;                        

                        SWTTextDetection sWTTextDetection = null;
                        if (ParametersUndefined(videoFrame))
                        {
                            if (this.UseAdaptiveSmoothing)
                                gaussForCanny = new AdaptiveGaussFilter(this.GaussFilterSigma);
                            else
                                gaussForCanny = new GaussFilter(this.GaussFilterSize, this.GaussFilterSigma);
                            gauss = new GaussFilter(this.GaussFilterSize, this.GaussFilterSigma);
                            CannyEdgeDetection canny = new CannyEdgeDetection(gaussForCanny, sobel, this.CannyLowTreshold, this.CannyHighTreshold);

                            sWTTextDetection = new SWTTextDetection(canny, gauss, gradientFiler, this.VarienceAverageSWRation,
                                            this.AspectRatio, this.DiamiterSWRatio, this.BbPixelsNumberMinRatio, this.BbPixelsNumberMaxRatio, this.ImageRegionHeightRationMin,
                                            this.ImageRegionWidthRatioMin, this.PairsHeightRatio, this.PairsIntensityRatio, this.PairsSWRatio,
                                            this.PairsWidthDistanceSqrRatio, this.PairsOccupationRatio, this.MinLettersNumberInTextRegion, this.MergeByDirectionAndChainEnds);
                        }
                        else
                        {
                            if (videoFrame.UseAdaptiveSmoothing)
                                gaussForCanny = new AdaptiveGaussFilter(videoFrame.GaussFilterSigma);
                            else
                                gaussForCanny = new GaussFilter(videoFrame.GaussFilterSize, videoFrame.GaussFilterSigma);
                            gauss = new GaussFilter(videoFrame.GaussFilterSize, videoFrame.GaussFilterSigma); 
                            CannyEdgeDetection canny = new CannyEdgeDetection(gaussForCanny, sobel, videoFrame.CannyLowTreshold, videoFrame.CannyHighTreshold);

                            sWTTextDetection = new SWTTextDetection(canny, gauss, gradientFiler, videoFrame.VarienceAverageSWRation, videoFrame.AspectRatio,
                                videoFrame.DiamiterSWRatio, videoFrame.BbPixelsNumberMinRatio, videoFrame.BbPixelsNumberMaxRatio, videoFrame.ImageRegionHeightRationMin,
                                videoFrame.ImageRegionWidthRatioMin, videoFrame.PairsHeightRatio, videoFrame.PairsIntensityRatio, videoFrame.PairsSWRatio,
                                videoFrame.PairsWidthDistanceSqrRatio, videoFrame.PairsOccupationRatio, videoFrame.MinLettersNumberInTextRegion, videoFrame.MergeByDirectionAndChainEnds);
                        }
                        sWTTextDetection.DetectText(videoFrame.Frame, threadsNumber);
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
        /// Выяснение, есть ли неопределенные параметры у кадра (эвристки)
        /// </summary>
        /// <param name="videoFrame">Кард</param>
        /// <returns>1 - есть неопределенные параметр(ы), 0 - иначе</returns>
        private bool ParametersUndefined(GreyVideoFrame videoFrame)
        {
            try
            {
                bool result = false;
                int undefinedParameter = GreyVideoFrame.UNDEFINED_PARAMETER;

                if (videoFrame.AspectRatio == undefinedParameter || videoFrame.BbPixelsNumberMaxRatio == undefinedParameter ||
                    videoFrame.BbPixelsNumberMinRatio == undefinedParameter || videoFrame.DiamiterSWRatio == undefinedParameter ||
                    videoFrame.ImageRegionHeightRationMin == undefinedParameter || videoFrame.ImageRegionWidthRatioMin == undefinedParameter ||
                    videoFrame.PairsHeightRatio == undefinedParameter || videoFrame.PairsIntensityRatio == undefinedParameter ||
                    videoFrame.PairsOccupationRatio == undefinedParameter || videoFrame.PairsSWRatio == undefinedParameter ||
                    videoFrame.PairsWidthDistanceSqrRatio == undefinedParameter || videoFrame.VarienceAverageSWRation == undefinedParameter)
                    return true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
