using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Interface
{
    public interface IGlobalTresholdBinarization: IBinarization
    {
        int Treshold { get; set; }
        int Countreshold(GreyImage image);
    }
}
