using DigitalImageProcessingLib.Algorithms.TextDetection;
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
        public int SWTPixelsDelta { get; set; }
        public double PixelsStrokeWidthTreshold { get; set; }
        public int MinimumPixelsNumberInRegion { get; set; }
        public SWTVideoTextDetection(IEdgeDetection edgeDetector, int SWTPixelsDelta, double pixelsStrokeWidthTreshold,
            int minimumPixelsNumberInRegion)
        {
            if (edgeDetector == null)
                throw new ArgumentNullException("Null edgeDetector in SWTVideoTextDetection");
            this.EdgeDetector = edgeDetector;
            this.SWTPixelsDelta = SWTPixelsDelta;
            this.PixelsStrokeWidthTreshold = pixelsStrokeWidthTreshold;
            this.MinimumPixelsNumberInRegion = minimumPixelsNumberInRegion;
        }

        /// <summary>
        /// Выделение текста на кд\лючевых кадрах видеоролика
        /// </summary>
        /// <param name="video">Видеоролик</param>
        /// <returns>true</returns>
        public Task<bool> DetectText(GreyVideo video)
        {
            try
            {
                if (video == null)
                    throw new ArgumentNullException("Null video in DetectText");
                if (video.Frames == null)
                    throw new ArgumentNullException("Null video frames in DetectText");

                SWTTextDetection SWTTextDetection = new SWTTextDetection(this.EdgeDetector, this.SWTPixelsDelta, this.PixelsStrokeWidthTreshold,
                                                                    this.MinimumPixelsNumberInRegion);

                return Task.Run(() =>
                {
                    for (int i = 0; i < video.Frames.Count; i++)
                        SWTTextDetection.DetectText(video.Frames[i].Frame);
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
        public Task<bool> DetectText(GreyVideoFrame videoFrame)
        {
            try
            {
                if (videoFrame == null || videoFrame.Frame == null)
                    throw new ArgumentNullException("Null frame in DetectText");

                SWTTextDetection SWTTextDetection = new SWTTextDetection(this.EdgeDetector, this.SWTPixelsDelta, this.PixelsStrokeWidthTreshold,
                                                                    this.MinimumPixelsNumberInRegion);
                return Task.Run(() =>
                {
                        SWTTextDetection.DetectText(videoFrame.Frame);
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
