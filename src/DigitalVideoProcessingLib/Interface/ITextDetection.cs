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
        Task<bool> DetectText(GreyVideo video, int threadsNumber);
        Task<bool> DetectText(GreyVideoFrame videoFrame, int threadsNumber);
    }
}
