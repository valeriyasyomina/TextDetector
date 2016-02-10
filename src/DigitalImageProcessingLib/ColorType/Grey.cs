using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.ColorType
{
    public class Grey: ColorBase
    {
        public Grey()
        {
            this.Data = 0;
        }
        public byte Data { get; set; }        
    }
}
