using DigitalImageProcessingLib.ImageType;
using DigitalVideoProcessingLib.Interface;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class ImageConvertor: IImageConvertor
    {
        /// <summary>
        /// Конвертация одното типа изображения в другое (переносится цвет)
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <returns>Изображение</returns>
        public GreyImage ConvertColor(Image<Gray, byte> image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in ConvertColor");                

                int imageHeight = image.Height;
                int imageWidth = image.Width;

                GreyImage newImage = new GreyImage(imageWidth, imageHeight);

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                        newImage.Pixels[i, j].Color.Data = image.Data[i, j, 0];
               return newImage;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
