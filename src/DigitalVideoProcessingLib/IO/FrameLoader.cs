using DigitalImageProcessingLib.IO;
using DigitalVideoProcessingLib.Interface;
using DigitalVideoProcessingLib.VideoFrameType;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class FrameLoader : IFrameLoader
    {
        public Task<GreyVideoFrame> LoadFrameAsync(object data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("Null data in LoadFrames");
                IOData ioData = (IOData)data;
                string frameFileName = ioData.FileName;
                if (frameFileName == null || frameFileName.Length == 0)
                    throw new ArgumentNullException("Null frameFileName in LoadFrames");                

                return Task.Run(() =>
                {
                    Bitmap bitmapFrame = new Bitmap(frameFileName);
                    BitmapConvertor bitmapConvertor = new BitmapConvertor();
                    GreyVideoFrame greyVideoFrame = new GreyVideoFrame();

                    greyVideoFrame.Frame = bitmapConvertor.ToGreyImage(bitmapFrame);
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
