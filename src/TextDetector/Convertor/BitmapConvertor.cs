using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DigitalImageProcessingLib.ImageType;

namespace TextDetector.Convertor
{
    public class BitmapConvertor: IBitmapConvertor
    {
        public BitmapConvertor() { }
        public Bitmap ToBitmap(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in ToBitmap");

                int width = image.Width;
                int height = image.Height;
                Bitmap bitmap = new Bitmap(width, height);
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                    {
                        byte colorIntensity = image.Pixels[i, j].Color.Data;
                        bitmap.SetPixel(j, i, Color.FromArgb(colorIntensity, colorIntensity, colorIntensity));
                    }
                return bitmap;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public Bitmap ToBitmap(RGBImage image)
        {
            throw new NotImplementedException();
        }

        public GreyImage ToGreyImage(Bitmap bitmap)
        {
            try
            {
                if (bitmap == null)
                    throw new ArgumentNullException("Null bitmap in ToGreyImage");

                int width = bitmap.Width;
                int height = bitmap.Height;
                GreyImage greyImage = new GreyImage(width, height);
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                    {
                        Color color = bitmap.GetPixel(j, i);
                        if (color.R != color.G || color.R != color.B || color.G != color.B)
                            throw new ArgumentException("Bitmap must be greyscale in ToGreyImage");
                        greyImage.Pixels[i, j].Color.Data = color.R;
                    }
                return greyImage;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public RGBImage ToRGBImage(Bitmap bitmap)
        {
            throw new NotImplementedException();
        }
    }
}
