using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.MorphologicalOperations.MorphologicalOperationsTypes
{
    public class Erosion : MorphologicalOperation
    {
        public Erosion(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Error size");
            this.Size = size;
        }

        /// <summary>
        /// Применение морфологической операции эрозии к контурному изображению
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
                int highIndexI = image.Height - 1;
                int highIndexJ = image.Width - lowIndex;
                for (int i = 1; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
                    {
                        if (copyImage.Pixels[i, j].Color.Data == ColorBase.MIN_COLOR_VALUE)
                        {
                            int pixelsNumber = 0;
                            for (int k = 0; k < filterSize; k++)
                                if (copyImage.Pixels[i, k + j - lowIndex].Color.Data == ColorBase.MIN_COLOR_VALUE)
                                    ++pixelsNumber;
                            if (pixelsNumber == filterSize)
                            {
                                for (int k = 0; k < lowIndex; k++)
                                    image.Pixels[i, k + j - lowIndex].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;

                                for (int k = lowIndex + 1; k < filterSize; k++)
                                    image.Pixels[i, k + j - lowIndex].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;
                                image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                            }
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
