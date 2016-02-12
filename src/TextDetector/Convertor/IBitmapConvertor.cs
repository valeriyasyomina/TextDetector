using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDetector.Convertor
{
    interface IBitmapConvertor
    {
        Bitmap ToBitmap(GreyImage image);
        Bitmap ToBitmap(RGBImage image);
        GreyImage ToGreyImage(Bitmap bitmap);
        RGBImage ToRGBImage(Bitmap bitmap);
    }
}
