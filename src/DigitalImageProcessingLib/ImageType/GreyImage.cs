using DigitalImageProcessingLib.ColorType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DigitalImageProcessingLib.ImageType
{
    public class GreyImage: ImageBase<Grey>
    {
        public GreyImage(int width, int height) : base(width, height) { }
     
        /// <summary>
        /// Проверяет, являестя ли изображение цветным
        /// </summary>
        /// <returns>1 - цветное, 0 - иначе</returns>
        public override bool IsColored()
        {
            return false;
        }

        /// <summary>
        /// Получение негатива изображения
        /// </summary>
        public override void Negative()
        {
            try
            {
                int imageHeight = this.Height;
                int imageWidth = this.Width;

                for (int i = 0; i < Height; i++)
                    for (int j = 0; j < Width; j++)                    
                        this.Pixels[i, j].Color.Data = (byte) (ColorBase.MAX_COLOR_VALUE - this.Pixels[i, j].Color.Data);                    
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void ToTxt(string filename)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename))
                {
                    int imageHeight = this.Height;
                    int imageWidth = this.Width;
                    for (int i = 0; i < imageHeight; i++)
                    {
                        for (int j = 0; j < imageWidth; j++)
                        {
                            int strokeWidth = this.Pixels[i, j].StrokeWidth.Width;
                            if (strokeWidth == SWTData.StrokeWidthData.UNDEFINED_WIDTH)
                                streamWriter.Write("UN   ");
                            else
                            {
                                streamWriter.Write(strokeWidth.ToString(), "{0:3}");
                                streamWriter.Write("  ");
                            }
                        }
                        streamWriter.WriteLine();
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Создает копию изображения
        /// </summary>
        /// <returns>Копия изображения</returns>
        public override ImageBase<Grey> Copy()
        {
            try
            {
                if (this.Pixels == null)
                    throw new NullReferenceException("Null object in copy");
                ImageBase<Grey> copyImage = new GreyImage(this.Width, this.Height);
                for (int i = 0; i < copyImage.Height; i++)
                    for (int j = 0; j < copyImage.Width; j++)
                    {
                        copyImage.Pixels[i, j].BorderType = this.Pixels[i, j].BorderType;
                        copyImage.Pixels[i, j].Color.Data = this.Pixels[i, j].Color.Data;
                        copyImage.Pixels[i, j].Gradient.Angle = this.Pixels[i, j].Gradient.Angle;
                        copyImage.Pixels[i, j].Gradient.RoundGradientDirection = this.Pixels[i, j].Gradient.RoundGradientDirection;
                        copyImage.Pixels[i, j].Gradient.Strength = this.Pixels[i, j].Gradient.Strength;

                        copyImage.Pixels[i, j].StrokeWidth.WasProcessed = this.Pixels[i, j].StrokeWidth.WasProcessed;
                        copyImage.Pixels[i, j].StrokeWidth.Width = this.Pixels[i, j].StrokeWidth.Width;

                        copyImage.Pixels[i, j].RegionNumber = this.Pixels[i, j].RegionNumber;
                    }

                return copyImage;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Сравнивает два серых изображения
        /// </summary>
        /// <param name="image">Изображение, с которым сравниваем исходное</param>
        /// <returns>1 - изображения равны, 0 - иначе</returns>
        public bool IsEqual(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in isEqual");
                if (this.Height != image.Height)
                    throw new ArgumentException("images must be the same height");
                if (this.Width != image.Width)
                    throw new ArgumentException("images must be the same width");
                for (int i = 0; i < this.Height; i++)
                    for (int j = 0; j < this.Width; j++)
                    {
                        bool isEqual = (this.Pixels[i, j].BorderType == image.Pixels[i, j].BorderType &&
                            this.Pixels[i, j].Color.Data == image.Pixels[i, j].Color.Data &&
                            this.Pixels[i, j].Gradient.Angle == image.Pixels[i, j].Gradient.Angle &&
                            this.Pixels[i, j].Gradient.RoundGradientDirection == image.Pixels[i, j].Gradient.RoundGradientDirection &&
                            this.Pixels[i, j].Gradient.Strength == image.Pixels[i, j].Gradient.Strength);
                        if (!isEqual)
                            return false;
                    }
                    return true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }     
        
    }
}
