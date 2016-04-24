using DigitalVideoProcessingLib.IO;
using DigitalVideoProcessingLib.VideoFrameType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Interface
{
    public interface IFrameLoader
    {
        Task<GreyVideoFrame> LoadFrameAsync(object data);
        Task<GreyVideoFrame> LoadFrameAsync(string videoFileName, KeyFrameIOInformation keyFrameIOInformation);
    }
}
