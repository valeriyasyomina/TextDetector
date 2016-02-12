using DigitalImageProcessingLib.ColorType;
using System;
using System.Collections.Generic;
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
