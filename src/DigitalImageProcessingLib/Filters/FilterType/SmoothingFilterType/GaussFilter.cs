using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.SmoothingFilterType
{
    public class GaussFilter: SmoothingFilter
    {
        public static double TRESHOLD = 0.85;
        public double Sigma { get; protected set; }
        public GaussFilter(int size, double sigma)
        {
            if (size <= 0)
                throw new ArgumentException("Error filter size in GaussFilter");
            if (sigma < 0)
                throw new ArgumentException("Error sigma in GaussFilter");
            this.Size = size;
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
                        {
                            int indexI = k + i - lowIndex;
                            for (int l = 0; l < filterSize; l++)
                            {
                                int indexJ = l + j - lowIndex;
                                pixelColor += this.Kernel[k, l] * copyImage.Pixels[indexI, indexJ].Color.Data;                               
                            }                            
                        }
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
        public override void Apply(RGBImage image)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Заполняет ядро свертки
        /// </summary>
        private void FillKernel()
        {
            try
            {
                Kernel = new int[Size, Size];
                int shift = Size / 2;
                double firstValue = 0.0;
                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                    {
                        double value = KernelFunction(i - shift, j - shift, Sigma);
                        if (i == 0 && j == 0)
                            firstValue = value;
                        value = value / firstValue * 2;
                        int integerPart = (int)Math.Floor(value);
                        if (value - (double)integerPart >= TRESHOLD)
                            Kernel[i, j] = (int)Math.Ceiling(value);
                        else
                            Kernel[i, j] = integerPart;
                        NormalizationRatio += Kernel[i, j];
                    }
            }
            catch (Exception exception)
            {
                throw exception;
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
