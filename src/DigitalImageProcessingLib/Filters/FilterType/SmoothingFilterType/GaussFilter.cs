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
    public class GaussFilter: SmoothingFilter
    {
        public static double TRESHOLD = 0.85;
        public double Sigma { get; protected set; }     
        public GaussFilter(int size, double sigma, int threadsNumber): base(size)
        {
            if (size <= 0)
                throw new ArgumentException("Error filter size in GaussFilter");
            if (sigma < 0)
                throw new ArgumentException("Error sigma in GaussFilter");
            if (threadsNumber <= 0)
                throw new ArgumentException("Error threadsNumber in GaussFilter");
            this.Sigma = sigma;
            this.NormalizationRatio = 0;
            this.ThreadList = new List<Thread>();
            this.ThreadsNumber = threadsNumber;
            FillKernel();
        }

        /// <summary>
        /// Применение фильтра Гаусса к серому изображению
        /// </summary>
        /// <param name="image">Серое изображение</param>
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");
                if (image.Height < this.Size)
                    throw new ArgumentException("Image height must be >= filter size");
                if (image.Width < this.Size)
                    throw new ArgumentException("Image width must be >= filter size");

                this.GreySmoothedImage = (GreyImage) image.Copy();
                StartThreads(image);       
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Свертки изображения в одном потоке
        /// </summary>
        /// <param name="data">Данные</param>
        private void ApplyThread(object data)
        {
            try
            {
                MatrixFilterData matrixFilterData = (MatrixFilterData) data;
                GreyImage copyImage = (GreyImage) matrixFilterData.GreyImage;

                int startIndexI = matrixFilterData.StartIndexI;
                int endIndexI = matrixFilterData.EndIndexI;

                int startIndexJ = matrixFilterData.StartIndexJ;
                int endIndexJ = matrixFilterData.EndIndexJ;

                int filterSize = this.Size;
                int lowIndex = filterSize / 2;
          
                for (int i = startIndexI; i < endIndexI; i++)
                    for (int j = startIndexJ; j < endIndexJ; j++)
                    {
                        int pixelColor = 0;
                        for (int k = 0; k < filterSize; k++)
                            for (int l = 0; l < filterSize; l++)
                                pixelColor += this.Kernel[k, l] * copyImage.Pixels[k + i - lowIndex, l + j - lowIndex].Color.Data;

                        pixelColor /= this.NormalizationRatio;
                        if (pixelColor > ColorBase.MAX_COLOR_VALUE)
                            pixelColor = ColorBase.MAX_COLOR_VALUE;
                        if (pixelColor < 0)
                            pixelColor = ColorBase.MIN_COLOR_VALUE;
                        this.GreySmoothedImage.Pixels[i, j].Color.Data = (byte)pixelColor;
                    }

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

        /// <summary>
        /// Запуск потоков на выполнение обработки
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void StartThreads(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;
                int filterSize = this.Size;
              
                int lowIndexJ = filterSize / 2;
                int highIndexJ = imageWidth - lowIndexJ;
                int deltaHeight = imageHeight / this.ThreadsNumber;

                int lowIndexI = filterSize / 2;
                int highIndexI = lowIndexI + deltaHeight;

                for (int i = 0; i < this.ThreadsNumber; i++)
                {
                    if (i == this.ThreadsNumber - 1)   // последний поток                    
                        highIndexI = imageHeight - filterSize / 2;                  

                    MatrixFilterData matrixFilterData = new MatrixFilterData(image, lowIndexI, highIndexI, lowIndexJ, highIndexJ);
                    Thread thread = new Thread(new ParameterizedThreadStart(this.ApplyThread));
                    this.ThreadList.Add(thread);
                    this.ThreadList[i].Start(matrixFilterData);

                    lowIndexI = highIndexI;
                    highIndexI += deltaHeight;                  
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Заполняет ядро свертки
        /// </summary>
        protected  override void FillKernel()
        {
            try
            {
             //   FillOptimizedKernel();

                int shift = this.Size / 2;
                double firstValue = 0.0;            

                for (int i = 0; i < this.Size; i++)
                    for (int j = 0; j < this.Size; j++)
                    {
                        double value = KernelFunction(i - shift, j - shift, Sigma);
                        if (i == 0 && j == 0)
                            firstValue = value;
                        value = value / firstValue * 2;
                        int integerPart = (int)Math.Floor(value);
                        if (value - (double)integerPart >= TRESHOLD)
                            this.Kernel[i, j] = (int)Math.Ceiling(value);
                        else
                            this.Kernel[i, j] = integerPart;
                        NormalizationRatio += Kernel[i, j];                        
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void FillOptimizedKernel()
        {
            int shift = this.Size / 2;
            double firstValue = 0.0;      

            for (int i = 0; i < this.Size; i++)
            {
                double x = Math.Exp((-((i - shift) * (i - shift))) / (2 * Sigma * Sigma));  

                if (i == 0)
                    firstValue = x;
                x = x / firstValue * 2;
                int integerPart = (int)Math.Floor(x);
                if (x - (double)integerPart >= TRESHOLD)
                    x = (int)Math.Ceiling(x);
                else
                    x = integerPart;
            }
        }

        /// <summary>
        /// Вычисляет функцию Гаусса двух переменных
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="sigma">СКО</param>
        /// <returns></returns>
        private double KernelFunction(double x, double y, double sigma)
        {
            return Math.Exp((-(x * x + y * y)) / (2 * sigma * sigma));
        }
    }
}
