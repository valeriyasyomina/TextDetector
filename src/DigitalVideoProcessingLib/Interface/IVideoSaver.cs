using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFVideoTextDetector.VideoSave
{
    public interface IVideoSaver
    {
        Task<bool> SaveVideoAsync(GreyVideo video, System.Drawing.Pen pen, string pathToSave, string framesSubDir, string framesExpansion);
        Task<bool> SaveVideoFrameAsync(GreyVideoFrame videoFrame, System.Drawing.Pen pen, string saveFileName);
    }
}
