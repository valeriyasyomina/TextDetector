using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.GradientFilterType
{
    public class EnhancingGradientFilter: GradientFilter
    {
        /// <summary>
        /// Применение градиентного фильтра к изображению
        /// </summary>
        /// <param name="image">Изображение</param>
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");

                CountGradient(image);
                EnhanceGradientImage(image);                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисление градиента изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        private void CountGradient(GreyImage image)
        {
            try
            {
                GreyImage copyImage = (GreyImage)image.Copy();

                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        int gradientX = Math.Abs(copyImage.Pixels[i, j].Color.Data - copyImage.Pixels[i + 1, j].Color.Data);
                        int gradientY = Math.Abs(copyImage.Pixels[i, j].Color.Data - copyImage.Pixels[i, j + 1].Color.Data);

                        int gradient = gradientX + gradientY;
                        if (gradient > ColorBase.MAX_COLOR_VALUE)
                            gradient = ColorBase.MAX_COLOR_VALUE;
                        image.Pixels[i, j].Color.Data = (byte) gradient;
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        /// <summary>
        /// Усиление градиентного изображения фильтром 3 на 3
        /// </summary>
        /// <param name="image">Градиентное изображение </param>
        private void EnhanceGradientImage(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                GreyImage copyImage = (GreyImage)image.Copy();

                for (int i = 1; i < imageHeight; i++)
                    for (int j = 1; j < imageWidth; j++)                    
                        image.Pixels[i, j].Color.Data = (byte) GetMaximumIntensityFromNeibours(copyImage, i, j);                    
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисляет максимальное значение интенсивности среди 9 пикселей
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="pixelI">Номер строки текущего пикселя</param>
        /// <param name="pixelJ">Номер столбца текущего пикселя</param>
        private int GetMaximumIntensityFromNeibours(GreyImage image, int pixelI, int pixelJ)
        {
            try
            {
                int maxIntensity = image.Pixels[pixelI, pixelJ].Color.Data;

                if (image.Pixels[pixelI - 1, pixelJ - 1].Color.Data > maxIntensity)
                    maxIntensity = image.Pixels[pixelI - 1, pixelJ - 1].Color.Data;
                if (image.Pixels[pixelI - 1, pixelJ].Color.Data > maxIntensity)
                    maxIntensity = image.Pixels[pixelI - 1, pixelJ].Color.Data;
                if (image.Pixels[pixelI - 1, pixelJ + 1].Color.Data > maxIntensity)
                    maxIntensity = image.Pixels[pixelI - 1, pixelJ + 1].Color.Data;
                if (image.Pixels[pixelI, pixelJ - 1].Color.Data > maxIntensity)
                    maxIntensity = image.Pixels[pixelI, pixelJ - 1].Color.Data;
                if (image.Pixels[pixelI, pixelJ + 1].Color.Data > maxIntensity)
                    maxIntensity = image.Pixels[pixelI, pixelJ + 1].Color.Data;
                if (image.Pixels[pixelI + 1, pixelJ - 1].Color.Data > maxIntensity)
                    maxIntensity = image.Pixels[pixelI + 1, pixelJ - 1].Color.Data;
                if (image.Pixels[pixelI + 1, pixelJ].Color.Data > maxIntensity)
                    maxIntensity = image.Pixels[pixelI + 1, pixelJ].Color.Data;
                if (image.Pixels[pixelI + 1, pixelJ + 1].Color.Data > maxIntensity)
                    maxIntensity = image.Pixels[pixelI + 1, pixelJ + 1].Color.Data;

                return maxIntensity;
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
