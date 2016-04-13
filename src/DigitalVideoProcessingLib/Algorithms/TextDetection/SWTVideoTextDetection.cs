using DigitalImageProcessingLib.Algorithms.TextDetection;
using DigitalImageProcessingLib.Filters.FilterType;
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
        public IEdgeDetection EdgeDetector { get; set; }
        private GradientFilter GradientFilter { get; set; }
        public double VarienceAverageSWRation { get; set; }
        public double AspectRatio { get; set; }
        public double DiamiterSWRatio { get; set; }
        public double BbPixelsNumberMinRatio { get; set; }
        public double BbPixelsNumberMaxRatio { get; set; }
        public double ImageRegionHeightRationMin { get; set; }
        public double ImageRegionWidthRatioMin { get; set; }
        public SWTVideoTextDetection(IEdgeDetection edgeDetector, GradientFilter gradientFiler, double varienceAverageSWRation, double aspectRatio = 5.0,
            double diamiterSWRatio = 10, double bbPixelsNumberMinRatio = 1.5, double bbPixelsNumberMaxRatio = 25.0,
            double imageRegionHeightRationMin = 1.5, double imageRegionWidthRatioMin = 1.5)
        {
            if (edgeDetector == null)
                throw new ArgumentNullException("Null edgeDetector in SWTVideoTextDetection");            
            if (gradientFiler == null)
                throw new ArgumentNullException("Null gradientFiler in ctor");
            this.EdgeDetector = edgeDetector;
            this.GradientFilter = gradientFiler;

            this.VarienceAverageSWRation = varienceAverageSWRation;
            this.AspectRatio = aspectRatio;
            this.DiamiterSWRatio = diamiterSWRatio;
            this.BbPixelsNumberMinRatio = bbPixelsNumberMinRatio;
            this.BbPixelsNumberMaxRatio = bbPixelsNumberMaxRatio;
            this.ImageRegionHeightRationMin = imageRegionHeightRationMin;
            this.ImageRegionWidthRatioMin = imageRegionWidthRatioMin;            
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

              /*  SWTTextDetection SWTTextDetection = new SWTTextDetection(this.EdgeDetector, this.SWTPixelsDelta, this.PixelsStrokeWidthTreshold,
                                                                    this.MinimumPixelsNumberInRegion);
                */
                return Task.Run(() =>
                {
                   // for (int i = 0; i < video.Frames.Count; i++)
                     //   SWTTextDetection.DetectText(video.Frames[i].Frame);
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

                SWTTextDetection SWTTextDetection = new SWTTextDetection(this.EdgeDetector, this.GradientFilter, this.VarienceAverageSWRation,
                    this.AspectRatio, this.DiamiterSWRatio, this.BbPixelsNumberMinRatio, this.BbPixelsNumberMaxRatio, this.ImageRegionHeightRationMin, this.ImageRegionWidthRatioMin);
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
