using DigitalVideoProcessingLib.Interface;
using DigitalVideoProcessingLib.VideoFrameType;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class FrameLoader : IFrameLoader
    {
        public Task<GreyVideoFrame> LoadFrame(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in LoadFrames");
                IOData ioData = (IOData)data;
                string videoFileName = ioData.FileName;
                if (videoFileName == null || videoFileName.Length == 0)
                    throw new ArgumentNullException("Null videoFileName in LoadFrames");
                int frameWidth = ioData.FrameWidth;
                if (frameWidth <= 0)
                    throw new ArgumentException("Error frameWidth in LoadFrames");
                int frameHeight = ioData.FrameHeight;
                if (frameHeight <= 0)
                    throw new ArgumentException("Error frameHeight in LoadFrames");

                return Task.Run(() =>
                {
                    Capture capture = new Capture(videoFileName);
                    Image<Bgr, Byte> frame = capture.QueryFrame();

                    ImageConvertor imageConvertor = new ImageConvertor();
                    GreyVideoFrame greyVideoFrame = new GreyVideoFrame();
                    greyVideoFrame.OriginalFrame = frame;
                    greyVideoFrame.Frame = imageConvertor.ConvertColor(frame.Convert<Gray, Byte>());

                    return greyVideoFrame;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
