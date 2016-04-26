using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.ThreadData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.SmoothingFilterType
{
    public class AdaptiveGaussFilter: SmoothingFilter
    {
        private static int DEFAULT_ITERATIONS_NUMBER = 1;
        private GreyImage _copyImage = null;
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
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");
                if (image.Height < this.Size)
                    throw new ArgumentException("Image height must be >= filter size");
                if (image.Width < this.Size)
                    throw new ArgumentException("Image width must be >= filter size");
                if (threadsNumber <= 0)
                    throw new ArgumentException("Error threadsNumber in Apply");

                this._copyImage = (GreyImage)image.Copy();
                if (this._copyImage == null)
                    throw new NullReferenceException("Null copy image in Apply");

                this.Threads = new List<Thread>();

                int deltaI = image.Height / threadsNumber;
                int lowIndex = 1;
                int lowIndexI = 1;           
                int highIndexI = lowIndexI + deltaI;       
                int highIndexJ = image.Width - 2;
                
                for (int i = 0; i < threadsNumber; i++)
                {
                    if (i == threadsNumber - 1)
                        highIndexI = image.Height - 2;                  

                    MatrixFilterData matrixFilterData = new ThreadData.MatrixFilterData(image, lowIndexI, highIndexI, lowIndex, highIndexJ);
                    Thread thread = new Thread(new ParameterizedThreadStart(this.ApplyThread));
                    this.Threads.Add(thread);
                    this.Threads[i].Start(matrixFilterData);                     
                  
                    lowIndexI = highIndexI;
                    highIndexI += deltaI;
                }
                WaitForThreads();

                return this._copyImage;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Ожидание завершения потоков
        /// </summary>
        private void WaitForThreads()
        {
            try
            {
                int hreadsNumber = this.Threads.Count;
                for (int i = 0; i < hreadsNumber; i++)
                    this.Threads[i].Join();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        protected override void ApplyThread(object data)
        {
            try
            {
                MatrixFilterData matrixFilterData = (MatrixFilterData)data;

                GreyImage image = matrixFilterData.GreyImage;
                int startI = matrixFilterData.StartIndexI;
                int endI = matrixFilterData.EndIndexI;
                int startJ = matrixFilterData.StartIndexJ;
                int endJ = matrixFilterData.EndIndexJ;
                int filterSize = this.Size;
                int lowIndex = filterSize / 2;

                for (int i = startI; i < endI; i++)
                    for (int j = startJ; j < endJ; j++)
                    {
                        double w1 = CalculateWeight(image, i - 1, j - 1);
                        double w2 = CalculateWeight(image, i - 1, j);
                        double w3 = CalculateWeight(image, i - 1, j + 1);
                        double w4 = CalculateWeight(image, i, j - 1);
                        double w5 = CalculateWeight(image, i, j + 1);
                        double w6 = CalculateWeight(image, i + 1, j - 1);
                        double w7 = CalculateWeight(image, i + 1, j);
                        double w8 = CalculateWeight(image, i + 1, j + 1);
                        double wCurrentPixel = CalculateWeight(image, i, j);

                        double N = w1 + w2 + w3 + w4 + w5 + w6 + w7 + w8 + wCurrentPixel;
                        double newPixelValue = w1 * image.Pixels[i - 1, j - 1].Color.Data + w2 * image.Pixels[i - 1, j].Color.Data +
                            w3 * image.Pixels[i - 1, j + 1].Color.Data + w4 * image.Pixels[i, j - 1].Color.Data +
                            w5 * image.Pixels[i, j + 1].Color.Data + w6 * image.Pixels[i + 1, j - 1].Color.Data +
                            w7 * image.Pixels[i + 1, j].Color.Data + w8 * image.Pixels[i + 1, j + 1].Color.Data +
                            wCurrentPixel * image.Pixels[i, j].Color.Data;
                        if (N == 0.0)
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                        else
                        {
                            int newColor = (int)(newPixelValue / N);
                            if (newColor > ColorBase.MAX_COLOR_VALUE)
                                newColor = ColorBase.MAX_COLOR_VALUE;
                            if (newColor < ColorBase.MIN_COLOR_VALUE)
                                newColor = ColorBase.MIN_COLOR_VALUE;
                            this._copyImage.Pixels[i, j].Color.Data = (byte)newColor;
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
