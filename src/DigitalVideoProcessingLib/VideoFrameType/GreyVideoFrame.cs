using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.VideoFrameType
{
    public class GreyVideoFrame
    {
        public GreyImage Frame { get; set; }
        public Image<Bgr, Byte> OriginalFrame { get; set; }
        public int FrameNumber { get; set; }
    }
}
