using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.MorphologicalOperations.MorphologicalOperationsTypes
{
    public class Dilation: MorphologicalOperation
    {
        public Dilation(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Error size");
            this.Size = size;
        }

        /// <summary>
        /// Применение морфологической операции расширения к контурному изображению
        /// </summary>
        /// <param name="image"></param>
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");

                GreyImage copyImage = (GreyImage)image.Copy();

                int filterSize = this.Size;
                int lowIndex = filterSize / 2;
                int highIndexI = image.Height;
                int highIndexJ = image.Width - lowIndex;
                for (int i = 0; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
                    {
                        if (copyImage.Pixels[i, j].Color.Data == ColorBase.MIN_COLOR_VALUE)
                        {
                           //ool borderPixelFound = false;
                            for (int k = 0; k < filterSize; k++)
                                image.Pixels[i, k + j - lowIndex].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                                // (copyImage.Pixels[i, k + j - lowIndex].Color.Data == ColorBase.MIN_COLOR_VALUE)
                                  //borderPixelFound = true;
                          //  if (borderPixelFound)
                            //    image.Pixels[i, j].Color.Data = (byte) ColorBase.MIN_COLOR_VALUE;
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
