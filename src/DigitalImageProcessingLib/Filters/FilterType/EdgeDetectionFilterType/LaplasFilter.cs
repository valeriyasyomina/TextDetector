using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType
{
    public class LaplasFilter: EdgeDetectionFilter
    {
        static protected int FILTER_SIZE = 3;
        static protected int TRESHOLD = 128 * 128;
        public LaplasFilter()
        {
            try
            {
                this.Size = FILTER_SIZE;
                this.Gx = new int[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                this.Gy = this.Gx;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Применение фильтра Лапласа к серому изображению
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
                        int gradientStrengthX = copyImage.Pixels[i - 1, j].Color.Data * Gx[0, 1] +
                          copyImage.Pixels[i, j - 1].Color.Data * Gx[1, 0] + copyImage.Pixels[i, j].Color.Data * Gx[1, 1] +
                          copyImage.Pixels[i, j + 1].Color.Data * Gx[1, 2] + copyImage.Pixels[i + 1, j].Color.Data * Gx[2, 1];
                    
                        int gradientStrengthSqr = gradientStrengthX * gradientStrengthX + gradientStrengthX * gradientStrengthX;

                        if (gradientStrengthSqr > ColorBase.MAX_COLOR_VALUE)
                        {
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                            image.Pixels[i, j].BorderType = BorderType.Border.STRONG;
                        }
                        else
                        {
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;
                            image.Pixels[i, j].BorderType = BorderType.Border.WEAK;
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
