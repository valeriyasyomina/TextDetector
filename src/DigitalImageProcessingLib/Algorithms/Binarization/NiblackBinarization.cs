using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Algorithms.Binarization
{
    public class NiblackBinarization : ILocalTresholdBinarization
    {
        public NiblackBinarization(int windowSize, double k = -0.2)
        {
            if (windowSize <= 0)
                throw new ArgumentException("Error windowSize in NiblackBinarization");
            this.WindowSize = windowSize;
            this.K = k;
            this._pixelsNumberInWindow = this.WindowSize * this.WindowSize;
        }
        public int WindowSize { get; set; }
        public double K { get; set; }
        private int _pixelsNumberInWindow = 0;

        /// <summary>
        /// Бинаризация методом Ниблака
        /// </summary>
        /// <param name="image"></param>
        public void Binarize(GreyImage image)
        {
            try
            {
                int windowSize = this.WindowSize;
                int lowIndex = windowSize / 2;
                int highIndexI = image.Height - lowIndex;
                int highIndexJ = image.Width - lowIndex;

                GreyImage copyImage = (GreyImage)image.Copy();

                for (int i = lowIndex; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
                    {
                        double matExp = CountMatExp(copyImage, i, j);
                        double sigma = CountSigma(copyImage, i, j, matExp);

                        int treshold = (int) (matExp + this.K * sigma);
                        if (image.Pixels[i, j].Color.Data < treshold)
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                        else
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;
                    }                                 
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Подсчет мат ожидания пикселей. попавших в окно
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="i">Индекс по строке</param>
        /// <param name="j">Индекс по столбцу</param>
        /// <returns>Мат ожидание</returns>
        private double CountMatExp(GreyImage image, int i, int j)
        {
            try
            {
                int windowSize = this.WindowSize;
                int lowIndex = windowSize / 2;
                double sumColor = 0.0;
                for (int k = 0; k < windowSize; k++)
                    for (int l = 0; l < windowSize; l++)
                        sumColor += image.Pixels[k + i - lowIndex, l + j - lowIndex].Color.Data;
                return (double)sumColor / this._pixelsNumberInWindow;   
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Подсчет СКО для пикселей. попавших в окно
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="i">Индекс по строке</param>
        /// <param name="j">Индекс по столбцу</param>
        /// <param name="matExp">Мат ожидание</param>
        /// <returns>СКО</returns>
        private double CountSigma(GreyImage image, int i, int j, double matExp)
        {
            try
            {
                int windowSize = this.WindowSize;
                int lowIndex = windowSize / 2;
                double sumColor = 0.0;
                for (int k = 0; k < windowSize; k++)
                    for (int l = 0; l < windowSize; l++)
                        sumColor += Math.Pow(image.Pixels[k + i - lowIndex, l + j - lowIndex].Color.Data - matExp, 2);

                return (double) Math.Sqrt(sumColor / this._pixelsNumberInWindow);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
