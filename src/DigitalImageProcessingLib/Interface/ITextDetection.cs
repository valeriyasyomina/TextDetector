using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Interface
{
    interface ITextDetection
    {
        void DetectText(GreyImage image);
        void DetectText(RGBImage image);
    }
}
