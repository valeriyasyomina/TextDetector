using DigitalImageProcessingLib.ImageType;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Interface
{
    public interface IImageConvertor
    {
        GreyImage ConvertColor(Image<Gray, Byte> image);
    }
}
