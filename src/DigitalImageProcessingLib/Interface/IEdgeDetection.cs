using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Interface
{
    interface IEdgeDetection
    {
        void Detect(GreyImage image);
        void Detect(RGBImage image);
        GreyImage GreySmoothedImage();
    }
}
