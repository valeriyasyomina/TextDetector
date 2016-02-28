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
    public class OtsuBinarization: IGlobalTresholdBinarization
    {
        /// <summary>
        /// Порог бинаризации
        /// </summary>
        public int Treshold { get; set; }

        /// <summary>
        /// Вычисляет порог бинаризации для изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <returns></returns>
        public int Countreshold(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;

                byte minColor = 0;
                byte maxColor = 0;

                GetMinMaxColorValues(image, ref minColor, ref maxColor);
                int[] histogram = CreateHistogram(image, minColor, maxColor - minColor + 1);

                int P = 0, iP = 0;
                CountMatExp(histogram, ref P, ref iP);

                this.Treshold = GetTreshold(histogram, P, iP) + minColor;

                return this.Treshold;     
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисление порогра бинаризации
        /// </summary>
        /// <param name="histogram">Гистограмма</param>
        /// <param name="P">Мат ожидание 1 класса пикселей</param>
        /// <param name="iP">Мат ожидание 2 класса пикселей</param>
        /// <returns></returns>
        private int GetTreshold(int[] histogram, int P, int iP)
        {
            try
            {
                int histogramSize = histogram.Length;

                int alpha = 0, beta = 0;
                int treshold = 0;
                double sigma = 0.0, weight = 0.0, koffSqr = 0.0;
                double maxSigma = Double.MinValue;

                for (int i = 0; i < histogramSize; i++)
                {
                    alpha += i * histogram[i];
                    beta += histogram[i];

                    weight = (double)beta / P;
                    koffSqr = (double) alpha / beta - (double) (iP - alpha) / (P - beta);

                    sigma = weight * (1 - weight) * koffSqr * koffSqr;
                    if (sigma > maxSigma)
                    {
                        maxSigma = sigma;
                        treshold = i;
                    }
                }
                return treshold;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Подсчет мат. ожидания для двух классов изображения
        /// </summary>
        /// <param name="histogram">Гистограмма</param>
        /// <param name="P">Первый класс</param>
        /// <param name="iP">Второй класс</param>
        private void CountMatExp(int[] histogram, ref int P, ref int iP)
        {
            try
            {
                P = 0;
                iP = 0;
                int histogramSize = histogram.Length;
                for (int i = 0; i < histogramSize; i++)
                {
                    P += histogram[i];
                    iP += i * histogram[i];
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Построение гистрограммы изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="minColor">Минимальное значение полутона изображения</param>
        /// <param name="histogramSize">Размер гистограммы</param>
        /// <returns></returns>
        private int[] CreateHistogram(GreyImage image, byte minColor, int histogramSize)
        {
            try
            {
                int[] histogram = new int[histogramSize];

                int imageHeight = image.Height;
                int imageWidth = image.Width;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                        ++histogram[image.Pixels[i, j].Color.Data - minColor];                    

                return histogram;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Бинаризация Отцу изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        public void Binarize(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Binarize");
                this.Treshold = Countreshold(image);
                SetColor(image);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Получение минимального и максимального тонов изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="minColor">Минимальный тон</param>
        /// <param name="maxColor">Максимальный тон</param>
        private void GetMinMaxColorValues(GreyImage image, ref byte minColor, ref byte maxColor)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;

                maxColor = minColor = image.Pixels[0, 0].Color.Data;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        byte currentColor = image.Pixels[i, j].Color.Data;
                        if (currentColor < minColor)
                            minColor = currentColor;
                        if (currentColor > maxColor)
                            maxColor = currentColor;
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }

        /// <summary>
        /// Устанавливает цвет пикселей изображения на основе порога
        /// </summary>
        /// <param name="image"></param>
        private void SetColor(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        if (image.Pixels[i, j].Color.Data < this.Treshold)
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
