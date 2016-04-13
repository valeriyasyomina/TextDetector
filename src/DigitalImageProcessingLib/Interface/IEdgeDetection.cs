using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Interface
{
    public interface IEdgeDetection
    {
        int LowTreshold { get; set; }
        int HighTreshold { get; set; }
        void Detect(GreyImage image);
        GreyImage Detect(GreyImage image, int threadsNumber);
        void Detect(RGBImage image);
        GreyImage GreySmoothedImage();
    }
}
