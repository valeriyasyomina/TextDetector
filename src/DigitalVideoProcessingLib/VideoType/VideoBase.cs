using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.ColorType;

namespace DigitalVideoProcessingLib.VideoType
{
    public abstract class VideoBase<ColorType> where ColorType : ColorBase, new()
    {
        public List<ImageBase<ColorType>> Frames { get; set; }
    }
}
