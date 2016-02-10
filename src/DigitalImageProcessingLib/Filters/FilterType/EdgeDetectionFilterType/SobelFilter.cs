using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType
{
    public class SobelFilter: EdgeDetectionFilter
    {
        static protected int FilterSize = 3;
        public SobelFilter()
        {
            try
            {
                this.Size = FilterSize;
                this.Gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                this.Gy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Применение фильтра Собеля к серому изображению
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

                int lowIndex = Size / 2;
                int highIndexI = image.Height - lowIndex;
                int highIndexJ = image.Width - lowIndex;
                for (int i = lowIndex; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
                    {
                        int gradientStrengthX = image.Pixels[i - 1, j - 1].Color.Data * Gx[0, 0] +
                            image.Pixels[i - 1, j + 1].Color.Data * Gx[0, 2] + image.Pixels[i, j - 1].Color.Data * Gx[1, 0] +
                            image.Pixels[i, j + 1].Color.Data * Gx[1, 2] + image.Pixels[i + 1, j - 1].Color.Data * Gx[2, 0] +
                            image.Pixels[i + 1, j + 1].Color.Data * Gx[2, 2];

                        int gradientStrengthY = image.Pixels[i - 1, j - 1].Color.Data * Gy[0, 0] +
                            image.Pixels[i - 1, j].Color.Data * Gy[0, 1] + image.Pixels[i - 1, j + 1].Color.Data * Gy[0, 2] +
                            image.Pixels[i + 1, j - 1].Color.Data * Gy[2, 0] + image.Pixels[i + 1, j].Color.Data * Gy[2, 1] +
                            image.Pixels[i + 1, j + 1].Color.Data * Gy[2, 2];

                        image.Pixels[i, j].Gradient.Strength = (int)Math.Sqrt(gradientStrengthX * gradientStrengthX + gradientStrengthY * gradientStrengthY);

                        if (gradientStrengthX == 0)
                            if (gradientStrengthY == 0)
                            {
                                image.Pixels[i, j].Gradient.Angle = 0;
                                image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE_0;
                            }
                            else
                            {
                                image.Pixels[i, j].Gradient.Angle = 90;
                                image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE_90;
                            }
                        else
                        {
                            int gradientDirection = (int)((Math.Atan((double)gradientStrengthY / gradientStrengthX)) * (180 / Math.PI));
                            gradientDirection += 90;
                            image.Pixels[i, j].Gradient.Angle = gradientDirection;

                            if ((gradientDirection >= 0 && gradientDirection <= 22) || (gradientDirection > 157 && gradientDirection <= 180))
                                image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE_0;
                            else if (gradientDirection > 22 && gradientDirection <= 67)
                                image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE__45;
                            else if (gradientDirection > 67 && gradientDirection <= 112)
                                image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE_90;
                            else if (gradientDirection > 112 && gradientDirection <= 157)
                                image.Pixels[i, j].Gradient.RoundGradientDirection = GradientData.RoundGradientDirection.DEGREE_135;
                        }                  
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
    }
}
