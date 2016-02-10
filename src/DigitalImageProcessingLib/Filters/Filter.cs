using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters
{
    public abstract class Filter
    {
        public abstract void Apply(GreyImage image);
        public abstract void Apply(RGBImage image);
    }
}
