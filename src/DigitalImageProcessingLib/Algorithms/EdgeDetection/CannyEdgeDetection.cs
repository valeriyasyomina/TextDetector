using DigitalImageProcessingLib.ColorType;
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
        private GreyImage _greySmoothedImage = null;
        public int LowTreshold { get; set; }
        public int HighTreshold { get; set; }

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
            if (lowTreshold >= highTreshold)
                throw new ArgumentException("highTreshold must be > lowTreshold");
            this._smoothingFilter = smoothingFilter;
            this._edgeDetectionFilter = edgeDetectionFilter;
            this.LowTreshold = lowTreshold;
            this.HighTreshold = highTreshold;
        }

        public GreyImage GreySmoothedImage() { return this._greySmoothedImage; }

        /// <summary>
        /// Определяет границы серого изображения (многопоточная)
        /// </summary>
        /// <param name="image">Серое изображение</param>
        /// <param name="threadsNumber">Число потоков</param>
        public GreyImage Detect(GreyImage image, int threadsNumber)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Detect");
                if (threadsNumber <= 0)
                    throw new ArgumentException("Error threadsNumber in Detect");

                GreyImage smoothedImage = _smoothingFilter.Apply(image, threadsNumber);
                _greySmoothedImage = (GreyImage)smoothedImage.Copy();
                smoothedImage = _edgeDetectionFilter.Apply(smoothedImage, threadsNumber);
                SetGradientDirection(smoothedImage);
                NonMaximaSuppression(smoothedImage);
                DoubleTresholding(smoothedImage);
                EdgeTrackingByHysteresis(smoothedImage);
                SetEdgesColor(smoothedImage);

                return smoothedImage;
            }
            catch (Exception exception)
            {
                throw exception;
            }
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
                _greySmoothedImage = (GreyImage)image.Copy();
                _edgeDetectionFilter.Apply(image);
                SetGradientDirection(image);
                NonMaximaSuppression(image);
                DoubleTresholding(image);
                EdgeTrackingByHysteresis(image);
                SetEdgesColor(image);
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
        /// Устанавливает округленное значение направления градиента для каждого пикселя изображения
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void SetGradientDirection(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in SetGradientDirection");

                int lowIndex = _edgeDetectionFilter.Size / 2;
                int highIndexI = image.Height - lowIndex;
                int highIndexJ = image.Width - lowIndex;
                for (int i = lowIndex; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
                    {
                        int gradientDirection = image.Pixels[i, j].Gradient.Angle;

                        if ((gradientDirection >= -90 && gradientDirection <= -68) || (gradientDirection > 67 && gradientDirection <= 90))
                            image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE_90;
                        else if (gradientDirection > -68 && gradientDirection <= -23)
                            image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE_135;
                        else if (gradientDirection > -23 && gradientDirection <= 22)
                            image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE_0;
                        else if (gradientDirection > 22 && gradientDirection <= 67)
                            image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE__45;                   
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
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

                GreyImage copyImage = (GreyImage)image.Copy();
                if (copyImage == null)
                    throw new NullReferenceException("Null copy image in NonMaximaSuppression");

                int lowIndex = _edgeDetectionFilter.Size / 2;
                int highIndexI = image.Height - lowIndex;
                int highIndexJ = image.Width - lowIndex;
                for (int i = lowIndex; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
                    {
                        GradientData.RoundGradientDirection gradientDirection = image.Pixels[i, j].Gradient.RoundGradientDirection;
                        if (gradientDirection == GradientData.RoundGradientDirection.DEGREE_0)
                        {
                            int gradientStrength = copyImage.Pixels[i, j].Gradient.Strength;
                            if (!(gradientStrength > copyImage.Pixels[i, j - 1].Gradient.Strength &&   // i, j - 1
                                gradientStrength > copyImage.Pixels[i, j + 1].Gradient.Strength))   // i, j + 1                            
                                image.Pixels[i, j].Gradient.Strength = 0;                                                  
                        }
                        else if (gradientDirection == GradientData.RoundGradientDirection.DEGREE__45)
                        {
                            int gradientStrength = copyImage.Pixels[i, j].Gradient.Strength;
                            if (!(gradientStrength > copyImage.Pixels[i + 1, j - 1].Gradient.Strength &&
                            gradientStrength > copyImage.Pixels[i - 1, j + 1].Gradient.Strength))                            
                                image.Pixels[i, j].Gradient.Strength = 0;                                                    
                        }
                        else if (gradientDirection == GradientData.RoundGradientDirection.DEGREE_90)
                        {
                            int gradientStrength = copyImage.Pixels[i, j].Gradient.Strength;
                            if (!(gradientStrength > copyImage.Pixels[i + 1, j].Gradient.Strength &&   // i + 1, j
                            gradientStrength > copyImage.Pixels[i - 1, j].Gradient.Strength))    // i - 1, j                            
                                image.Pixels[i, j].Gradient.Strength = 0;                      
                        }
                        else if (gradientDirection == GradientData.RoundGradientDirection.DEGREE_135)                                                                                                  
                        {
                            int gradientStrength = copyImage.Pixels[i, j].Gradient.Strength;
                            if (!(gradientStrength > copyImage.Pixels[i - 1, j - 1].Gradient.Strength &&
                            gradientStrength > copyImage.Pixels[i + 1, j + 1].Gradient.Strength))                            
                                image.Pixels[i, j].Gradient.Strength = 0;                   
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

                int lowIndex = this._edgeDetectionFilter.Size / 2;
                int imageHeight = image.Height - lowIndex;
                int imageWidth = image.Width - lowIndex;

                for (int i = lowIndex; i < imageHeight; i++)
                    for (int j = lowIndex; j < imageWidth; j++)
                    {
                        int gradientStrength = image.Pixels[i, j].Gradient.Strength;
                        if (gradientStrength >= this.LowTreshold)  
                        {
                              if (gradientStrength < this.HighTreshold)
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

        /// <summary>
        /// Трассировка области неоднозначности (крайние строки матрицы, представляющей изображение) 
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void TrackEdgeImageRows(GreyImage image)
        {
            try
            {
                int lowIndex = _edgeDetectionFilter.Size / 2;
                int imageWidth = image.Width - lowIndex;
                int imageHeight = image.Height - lowIndex;
                for (int i = lowIndex; i < imageWidth; i++)
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

        /// <summary>
        /// Трассировка области неоднозначности (крайние столбцы матрицы, представляющей изображение) 
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void TrackEdgeImageColumns(GreyImage image)
        {
            try
            {
                int lowIndex = _edgeDetectionFilter.Size / 2;
                int imageHeight = image.Height - lowIndex;
                int imageWidth = image.Width - lowIndex;
                for (int i = lowIndex; i < imageHeight; i++)
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

        /// <summary>
        /// Трассировка области неоднозначности (середина матрицы начиная с элемента [1, 1] и заканчивая элементом [height - 1, width - 1]) 
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void TrackEdgeImageCenter(GreyImage image)
        {
            try
            {
                int lowIndex = _edgeDetectionFilter.Size / 2;
                int imageHeight = image.Height - lowIndex;
                int imageWidth = image.Width - lowIndex;

                for (int i = lowIndex; i < imageHeight; i++)
                    for (int j = lowIndex; j < imageWidth; j++)
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

        /// <summary>
        /// Установка цвета изображения. Границы выделяются черным, все остальное - белым цветом
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void SetEdgesColor(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in SetEdgesColor");
                int imageHeight = image.Height;
                int imageWidth = image.Width;
                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        if (image.Pixels[i, j].BorderType == BorderType.Border.STRONG)
                            image.Pixels[i, j].Color.Data = (byte) ColorBase.MIN_COLOR_VALUE;
                        else
                            image.Pixels[i, j].Color.Data = (byte) ColorBase.MAX_COLOR_VALUE;
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        
    }
}
