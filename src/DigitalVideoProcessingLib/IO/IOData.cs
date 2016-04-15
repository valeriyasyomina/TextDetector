using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class IOData
    {
        public IOData()
        {
            this.KeyFrameIOInformation = new List<KeyFrameIOInformation>();
        }
        public string FileName { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public List<KeyFrameIOInformation> KeyFrameIOInformation { get; set; }
    }
}
