using DigitalVideoProcessingLib.Interface;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class VideoLoader: IVideoLoader
    {
        public delegate void FrameLoaded(int frameNumber, bool isLastFrame);
        public static event FrameLoaded frameLoaded;

        public Task<int> CountFramesNumber(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in LoadFrames");
                IOData ioData = (IOData)data;
                string videoFileName = ioData.FileName;
                if (videoFileName == null)
                    throw new ArgumentNullException("Null videoFileName in LoadFrames");               

                return Task.Run(() =>
                {
                    List<Image<Bgr, Byte>> frames = new List<Image<Bgr, byte>>();

                    Capture capture = new Capture(videoFileName);
                    Image<Bgr, Byte> frame = null;
                    int frameNumber = 0;
                    do
                    {
                        frame = capture.QueryFrame();
                        ++frameNumber;                      
                    }
                    while (frame != null);

                    return frameNumber;
                });               
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public Task<List<Image<Bgr, Byte>>> LoadFramesAsync(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in LoadFrames");
                IOData ioData = (IOData)data;
                string videoFileName = ioData.FileName;
                if (videoFileName == null)
                    throw new ArgumentNullException("Null videoFileName in LoadFrames");
                int frameWidth = ioData.FrameWidth;
                if (frameWidth <= 0)
                    throw new ArgumentException("Error frameWidth in LoadFrames");
                int frameHeight = ioData.FrameHeight;
                if (frameHeight <= 0)
                    throw new ArgumentException("Error frameHeight in LoadFrames");

                return Task.Run(() =>
                {
                    List<Image<Bgr, Byte>> frames = new List<Image<Bgr, byte>>();

                    Capture capture = new Capture(videoFileName);
                    Image<Bgr, Byte> frame = null;
                    int frameNumber = 0;
                    do
                    {
                        frame = capture.QueryFrame();
                        ++frameNumber;                        
                        if (frame != null)
                        {
                            Image<Bgr, Byte> resizedFrame = frame.Resize(frameWidth, frameHeight, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                            frames.Add(resizedFrame);
                            frameLoaded(frameNumber, false);
                        }
                        else
                            frameLoaded(frameNumber, true);
                    }
                    while (frame != null);

                    return frames;
                });               
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
