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
        private GreyImage _copyImage = null;
        public static double TRESHOLD = 0.85;
        public double Sigma { get; protected set; }
        public GaussFilter(int size, double sigma): base(size)
        {
            if (size <= 0)
                throw new ArgumentException("Error filter size in GaussFilter");
            if (sigma < 0)
                throw new ArgumentException("Error sigma in GaussFilter");         
            this.Sigma = sigma;
            this.NormalizationRatio = 0;
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

                GreyImage copyImage = (GreyImage) image.Copy();
                if (copyImage == null)
                    throw new NullReferenceException("Null copy image in Apply");

                int filterSize = this.Size;
                int lowIndex = filterSize / 2;
                int highIndexI = image.Height - lowIndex;
                int highIndexJ = image.Width - lowIndex;
                for (int i = lowIndex; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
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
                        image.Pixels[i, j].Color.Data = (byte) pixelColor;
                    }            
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Применение фильтра Гаусса к серому изображению (многопоточная)
        /// </summary>
        /// <param name="image">Серое изображение</param>
        /// <param name="threadsNumber">Число потоков</param>
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
                int filterSize = this.Size;
                int lowIndex = filterSize / 2;
                int lowIndexI = lowIndex;           
                int highIndexI = lowIndexI + deltaI;       
                int highIndexJ = image.Width - lowIndex;
                
                for (int i = 0; i < threadsNumber; i++)
                {
                    if (i == threadsNumber - 1)
                        highIndexI = image.Height - lowIndex;                  

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
        /// <summary>
        /// Функция, выполняющаяся в отдельном потоке
        /// </summary>
        /// <param name="data">Данные</param>
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
                        int pixelColor = 0;
                        for (int k = 0; k < filterSize; k++)
                            for (int l = 0; l < filterSize; l++)
                                pixelColor += this.Kernel[k, l] * image.Pixels[k + i - lowIndex, l + j - lowIndex].Color.Data;

                        pixelColor /= this.NormalizationRatio;
                        if (pixelColor > ColorBase.MAX_COLOR_VALUE)
                            pixelColor = ColorBase.MAX_COLOR_VALUE;
                        if (pixelColor < 0)
                            pixelColor = ColorBase.MIN_COLOR_VALUE;
                        this._copyImage.Pixels[i, j].Color.Data = (byte)pixelColor;
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
        /// Заполняет ядро свертки
        /// </summary>
        protected  override void FillKernel()
        {
            try
            {
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
