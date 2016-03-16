using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Interface
{
    public interface ITextDetection
    {
        void DetectText(GreyImage image, out List<TextRegion> textRegions);
        void DetectText(RGBImage image);
    }
}
