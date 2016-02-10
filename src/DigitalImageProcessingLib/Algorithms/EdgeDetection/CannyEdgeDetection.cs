using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Algorithms.EdgeDetection
{
    public class CannyEdgeDetection: IEdgeDetection
    {
        private SmoothingFilter _smoothingFilter = null;
        private EdgeDetectionFilter _edgeDetectionFilter = null;
        private int _lowTreshold = 0;
        private int _highTreahold = 0;

        public CannyEdgeDetection(SmoothingFilter smoothingFilter, EdgeDetectionFilter edgeDetectionFilter, int lowTreshold,
                                    int highTreshold)
        {
            if (smoothingFilter == null)
                throw new ArgumentNullException("Null smoothingFilter");
            if (edgeDetectionFilter == null)
                throw new ArgumentNullException("Null edgeDetectionFilter");
            if (lowTreshold <= 0)
                throw new ArgumentException("lowTreshold must be > 0");
            if (highTreshold <= 0)
                throw new ArgumentException("highTreshold must be > 0");
            if (lowTreshold <= highTreshold)
                throw new ArgumentException("highTreshold must be > lowTreshold");
            this._smoothingFilter = smoothingFilter;
            this._edgeDetectionFilter = edgeDetectionFilter;
            this._lowTreshold = lowTreshold;
            this._highTreahold = highTreshold;
        }
         
        /// <summary>
        /// Определяет границы серого изображения
        /// </summary>
        /// <param name="image">Серое изображение</param>
        public void Detect(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Detect");
                _smoothingFilter.Apply(image);
                _edgeDetectionFilter.Apply(image);
                NonMaximasuppression(image);
                DoubleTresholding(image);
                EdgeTrackingByHysteresis(image);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void Detect(RGBImage image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Подавление не максимумов. Границами считаются только точки локального максимума
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void NonMaximasuppression(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in NonMaximasuppression");
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Двойная пороговая фильтрация
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void DoubleTresholding(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in DoubleTresholding");
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Трассировка области неоднозначности
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void EdgeTrackingByHysteresis(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in EdgeTrackingByHysteresis");
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
