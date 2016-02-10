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
        private int _highTreshold = 0;

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
            this._highTreshold = highTreshold;
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
                NonMaximaSuppression(image);
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
        private void NonMaximaSuppression(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in NonMaximasuppression");
                int imageHeight = image.Height;
                int imageWidth = image.Width;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        GradientData.RoundGradientDirection gradientDirection = image.Pixels[i, j].Gradient.RoundGradientDirection;
                        if (gradientDirection == GradientData.RoundGradientDirection.DEGREE_0)
                        {
                            int upPixelIndex = i - 1;
                            int downPixelIndex = i + 1;
                            if (upPixelIndex >= 0 && downPixelIndex < imageHeight)
                            {
                                int gradientStrength = image.Pixels[i, j].Gradient.Strength;
                                if (!(gradientStrength > image.Pixels[upPixelIndex, j].Gradient.Strength &&
                                gradientStrength > image.Pixels[downPixelIndex, j].Gradient.Strength))
                                {
                                    image.Pixels[i, j].Gradient.Strength = 0;
                                    image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                                }
                            }
                            else
                            {
                                image.Pixels[i, j].Gradient.Strength = 0;
                                image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                            }
                        }
                        else if (gradientDirection == GradientData.RoundGradientDirection.DEGREE__45)
                        {
                            int upPixelIndexI = i - 1;
                            int upPixelIndexJ = j + 1;

                            int downPixelIndexI = i + 1;
                            int downPixelIndexJ = j - 1;

                            if (upPixelIndexI >= 0 && upPixelIndexJ < imageWidth && downPixelIndexI < imageHeight && downPixelIndexJ >= 0)
                            {
                                int gradientStrength = image.Pixels[i, j].Gradient.Strength;
                                if (!(gradientStrength > image.Pixels[upPixelIndexI, upPixelIndexJ].Gradient.Strength &&
                                gradientStrength > image.Pixels[downPixelIndexI, downPixelIndexJ].Gradient.Strength))
                                {
                                    image.Pixels[i, j].Gradient.Strength = 0;
                                    image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                                }
                            }
                            else
                            {
                                image.Pixels[i, j].Gradient.Strength = 0;
                                image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                            }
                        }
                        else if (gradientDirection == GradientData.RoundGradientDirection.DEGREE_90)
                        {
                            int rightPixelIndexJ = j + 1;
                            int leftPixelIndexJ = j - 1;

                            if (rightPixelIndexJ < imageWidth && leftPixelIndexJ >= 0)
                            {
                                int gradientStrength = image.Pixels[i, j].Gradient.Strength;
                                if (!(gradientStrength > image.Pixels[i, rightPixelIndexJ].Gradient.Strength &&
                                gradientStrength > image.Pixels[i, leftPixelIndexJ].Gradient.Strength))
                                {
                                    image.Pixels[i, j].Gradient.Strength = 0;
                                    image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                                }
                            }
                            else
                            {
                                image.Pixels[i, j].Gradient.Strength = 0;
                                image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                            }
                        }
                        else if (gradientDirection == GradientData.RoundGradientDirection.DEGREE_135)                                                                                                  
                        {
                            int upPixelIndexI = i - 1;
                            int upPixelIndexJ = j - 1;

                            int downPixelIndexI = i + 1;
                            int downPixelIndexJ = j + 1;

                            if (upPixelIndexI >= 0 && upPixelIndexJ >= 0 && downPixelIndexI < imageHeight && downPixelIndexJ < imageWidth)
                            {
                                int gradientStrength = image.Pixels[i, j].Gradient.Strength;
                                if (!(gradientStrength > image.Pixels[upPixelIndexI, upPixelIndexJ].Gradient.Strength &&
                                gradientStrength > image.Pixels[downPixelIndexI, downPixelIndexJ].Gradient.Strength))
                                {
                                    image.Pixels[i, j].Gradient.Strength = 0;
                                    image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                                }
                            }
                            else
                            {
                                image.Pixels[i, j].Gradient.Strength = 0;
                                image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                            }
                        }
                    }
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
                int imageHeight = image.Height;
                int imageWidth = image.Width;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        int gradientStrength = image.Pixels[i, j].Gradient.Strength;
                        if (gradientStrength >= this._lowTreshold)
                        {
                            if (gradientStrength < this._highTreshold)
                                image.Pixels[i, j].BorderType = BorderType.Border.WEAK;
                            else
                                image.Pixels[i, j].BorderType = BorderType.Border.STRONG;
                        }
                        else
                            image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                    }
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
                TrackEdgeImageCenter(image);
                TrackEdgeImageRows(image);
                TrackEdgeImageColumns(image);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void TrackEdgeImageRows(GreyImage image)
        {
            try
            {
                int imageWidth = image.Width - 1;
                int imageHeight = image.Height - 1;
                for (int i = 1; i < imageWidth; i++)
                {
                    if (image.Pixels[0, i].BorderType == BorderType.Border.WEAK)
                    {
                        bool condition = image.Pixels[0, i - 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[0, i + 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[1, i - 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[1, i].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[1, i + 1].BorderType == BorderType.Border.STRONG;
                        if (condition)
                            image.Pixels[0, i].BorderType = BorderType.Border.STRONG;
                        else
                            image.Pixels[0, i].BorderType = BorderType.Border.SUPPRESSED;
                    }
                    if (image.Pixels[imageHeight, i].BorderType == BorderType.Border.WEAK)
                    {
                        bool condition = image.Pixels[imageHeight, i - 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[imageHeight, i + 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[imageHeight - 1, i - 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[imageHeight - 1, i].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[imageHeight - 1, i + 1].BorderType == BorderType.Border.STRONG;
                        if (condition)
                            image.Pixels[imageHeight, i].BorderType = BorderType.Border.STRONG;
                        else
                            image.Pixels[imageHeight, i].BorderType = BorderType.Border.SUPPRESSED;
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        private void TrackEdgeImageColumns(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;
                for (int i = 1; i < imageHeight; i++)
                {
                    if (image.Pixels[i, 0].BorderType == BorderType.Border.WEAK)
                    {
                        bool condition = image.Pixels[i - 1, 0].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[i + 1, 0].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[i - 1, 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[i, 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[i + 1, 1].BorderType == BorderType.Border.STRONG;
                        if (condition)
                            image.Pixels[i, 0].BorderType = BorderType.Border.STRONG;
                        else
                            image.Pixels[i, 0].BorderType = BorderType.Border.SUPPRESSED;
                    }
                    if (image.Pixels[i, imageWidth].BorderType == BorderType.Border.WEAK)
                    {
                        bool condition = image.Pixels[i - 1, imageWidth].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[i + 1, imageWidth].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[i - 1, imageWidth - 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[i, imageWidth - 1].BorderType == BorderType.Border.STRONG ||
                            image.Pixels[i + 1, imageWidth - 1].BorderType == BorderType.Border.STRONG;
                        if (condition)
                            image.Pixels[i, imageWidth].BorderType = BorderType.Border.STRONG;
                        else
                            image.Pixels[i, imageWidth].BorderType = BorderType.Border.SUPPRESSED;
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        private void TrackEdgeImageCenter(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                for (int i = 1; i < imageHeight; i++)
                    for (int j = 1; j < imageWidth; j++)
                    {
                        if (image.Pixels[i, j].BorderType == BorderType.Border.WEAK)
                        {
                            bool condition = image.Pixels[i - 1, j - 1].BorderType == BorderType.Border.STRONG ||
                                image.Pixels[i - 1, j].BorderType == BorderType.Border.STRONG ||
                                image.Pixels[i - 1, j + 1].BorderType == BorderType.Border.STRONG ||
                                image.Pixels[i, j - 1].BorderType == BorderType.Border.STRONG ||
                                image.Pixels[i, j + 1].BorderType == BorderType.Border.STRONG ||
                                image.Pixels[i + 1, j - 1].BorderType == BorderType.Border.STRONG ||
                                image.Pixels[i + 1, j].BorderType == BorderType.Border.STRONG ||
                                image.Pixels[i + 1, j + 1].BorderType == BorderType.Border.STRONG;
                            if (condition)
                                image.Pixels[i, j].BorderType = BorderType.Border.STRONG;
                            else
                                image.Pixels[i, j].BorderType = BorderType.Border.SUPPRESSED;
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
