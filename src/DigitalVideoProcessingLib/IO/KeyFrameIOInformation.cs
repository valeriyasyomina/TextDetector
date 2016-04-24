using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class KeyFrameIOInformation
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Number { get; set; }
        public bool NeedProcess { get; set; }
    }
}
