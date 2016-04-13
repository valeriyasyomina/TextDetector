using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType
{
    public class PrewittFilter : EdgeDetectionFilter
    {        
        public PrewittFilter()
        {
            try
            {
                this.Size = FILTER_SIZE;
                this.Gx = new int[,] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
                this.Gy = new int[,] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };   // { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } 
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Применение фильтра Превитта к серому изображению
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

                GreyImage copyImage = (GreyImage)image.Copy();
                if (copyImage == null)
                    throw new NullReferenceException("Null copy image in Apply");

                int lowIndex = Size / 2;
                int highIndexI = image.Height - lowIndex;
                int highIndexJ = image.Width - lowIndex;
                for (int i = lowIndex; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
                    {
                        byte pixelI_1J_1 = copyImage.Pixels[i - 1, j - 1].Color.Data;
                        byte pixelI_1J1 = copyImage.Pixels[i - 1, j + 1].Color.Data;
                        byte pixelI1J1 = copyImage.Pixels[i + 1, j + 1].Color.Data;


                        int gradientStrengthY = pixelI_1J_1 * Gy[0, 0] +
                          copyImage.Pixels[i - 1, j].Color.Data * Gy[0, 1] + pixelI_1J1 * Gy[0, 2] +
                          copyImage.Pixels[i + 1, j - 1].Color.Data * Gy[2, 0] + copyImage.Pixels[i + 1, j].Color.Data * Gy[2, 1] +
                          pixelI1J1 * Gy[2, 2];

                        int gradientStrengthX = pixelI_1J_1 * Gx[0, 0] +
                          pixelI_1J1 * Gx[0, 2] + copyImage.Pixels[i, j - 1].Color.Data * Gx[1, 0] +
                          copyImage.Pixels[i, j + 1].Color.Data * Gx[1, 2] + copyImage.Pixels[i + 1, j - 1].Color.Data * Gx[2, 0] +
                          pixelI1J1 * Gx[2, 2];

                        int gradientStrengthSqr = gradientStrengthX * gradientStrengthX + gradientStrengthY * gradientStrengthY;
                        image.Pixels[i, j].Gradient.Strength = (int)Math.Sqrt((double)gradientStrengthSqr);

                        if (gradientStrengthSqr > TRESHOLD)
                        {
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                            image.Pixels[i, j].BorderType = BorderType.Border.STRONG;
                        }
                        else
                        {
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;
                            image.Pixels[i, j].BorderType = BorderType.Border.WEAK;
                        }

                        if (gradientStrengthX == 0)
                        {
                            if (gradientStrengthY == 0)
                                image.Pixels[i, j].Gradient.Angle = 0;
                            else
                                image.Pixels[i, j].Gradient.Angle = 90;
                        }
                        else
                            image.Pixels[i, j].Gradient.Angle = (int)((Math.Atan((double)gradientStrengthY / gradientStrengthX)) * (180 / Math.PI));
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
