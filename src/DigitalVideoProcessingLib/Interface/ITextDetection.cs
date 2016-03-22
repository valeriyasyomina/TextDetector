using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Interface
{
    public interface ITextDetection
    {
        void DetectText(GreyVideo video);
        void DetectText(GreyVideoFrame videoFrame);
    }
}
