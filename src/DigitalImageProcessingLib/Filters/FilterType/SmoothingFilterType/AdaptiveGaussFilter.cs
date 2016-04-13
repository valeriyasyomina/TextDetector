using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.SmoothingFilterType
{
    public class AdaptiveGaussFilter: SmoothingFilter
    {
        private static int DEFAULT_ITERATIONS_NUMBER = 1;
        public double Sigma { get; set; }
        public int IterationsNumber { get; set; }
        public AdaptiveGaussFilter(double sigma, int iterationNumber = 1) 
        {
            if (sigma < 0)
                throw new ArgumentException("Error sigma in GaussFilter"); 
            this.Sigma = sigma;
            this.IterationsNumber = iterationNumber;
        }        

        /// <summary>
        /// Применение адаптивного фильтра Гаусса к серому изображению
        /// </summary>
        /// <param name="image">Серое изображение</param>
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");
                while (this.IterationsNumber > 0)
                {
                    ProcessImage(image);
                    --this.IterationsNumber;
                }
                this.IterationsNumber = DEFAULT_ITERATIONS_NUMBER;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Применение адаптивного фильтра Гаусса к серому изображению
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void ProcessImage(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height - 2;
                int imageWidth = image.Width - 2;

                GreyImage copyImage = (GreyImage) image.Copy();

                for (int i = 1; i < imageHeight; i++)
                    for (int j = 1; j < imageWidth; j++)
                    {
                        double w1 = CalculateWeight(copyImage, i - 1, j - 1);
                        double w2 = CalculateWeight(copyImage, i - 1, j);
                        double w3 = CalculateWeight(copyImage, i - 1, j + 1);
                        double w4 = CalculateWeight(copyImage, i, j - 1);
                        double w5 = CalculateWeight(copyImage, i, j + 1);
                        double w6 = CalculateWeight(copyImage, i + 1, j - 1);
                        double w7 = CalculateWeight(copyImage, i + 1, j);
                        double w8 = CalculateWeight(copyImage, i + 1, j + 1);
                        double wCurrentPixel = CalculateWeight(copyImage, i, j);

                        double N = w1 + w2 + w3 + w4 + w5 + w6 + w7 + w8 + wCurrentPixel;
                        double newPixelValue = w1 * copyImage.Pixels[i - 1, j - 1].Color.Data + w2 * copyImage.Pixels[i - 1, j].Color.Data +
                            w3 * copyImage.Pixels[i - 1, j + 1].Color.Data + w4 * copyImage.Pixels[i, j - 1].Color.Data +
                            w5 * copyImage.Pixels[i, j + 1].Color.Data + w6 * copyImage.Pixels[i + 1, j - 1].Color.Data +
                            w7 * copyImage.Pixels[i + 1, j].Color.Data + w8 * copyImage.Pixels[i + 1, j + 1].Color.Data +
                            wCurrentPixel * copyImage.Pixels[i, j].Color.Data;
                        if (N == 0.0)
                            image.Pixels[i, j].Color.Data = (byte) ColorBase.MIN_COLOR_VALUE;
                        else
                        {
                            int newColor = (int) (newPixelValue / N);
                            if (newColor > ColorBase.MAX_COLOR_VALUE)
                                newColor = ColorBase.MAX_COLOR_VALUE;
                            if (newColor < ColorBase.MIN_COLOR_VALUE)
                                newColor = ColorBase.MIN_COLOR_VALUE;
                            image.Pixels[i, j].Color.Data = (byte)newColor;
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисление веса пикселя изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="i">Номер пикселя в строке</param>
        /// <param name="j">Номер пикселя в столбце</param>
        /// <returns></returns>
        private double CalculateWeight(GreyImage image, int i, int j)
        {
            try
            {
                int gradientX = Math.Abs(image.Pixels[i, j].Color.Data - image.Pixels[i + 1, j].Color.Data);
                int gradientY = Math.Abs(image.Pixels[i, j].Color.Data - image.Pixels[i, j + 1].Color.Data);

                double gradientSqr = Math.Sqrt((double) (gradientX * gradientX + gradientY * gradientY));

                double weight = Math.Exp((double) -Math.Sqrt(gradientSqr) / (2.0 * this.Sigma * this.Sigma));
                return weight;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public override void Apply(RGBImage image)
        {
            throw new NotImplementedException();
        }

        protected override void FillKernel()
        {
            throw new NotImplementedException();
        }

        public override GreyImage Apply(GreyImage image, int threadsNumber)
        {
            throw new NotImplementedException();
        }

        protected override void ApplyThread(object data)
        {
            throw new NotImplementedException();
        }
    }
}
