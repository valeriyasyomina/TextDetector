using DigitalImageProcessingLib.ColorType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.ImageType
{
    public class RGBImage: ImageBase<RGB>
    {
        public override bool IsColored()
        {
            return true;
        }
        public override ImageBase<RGB> Copy()
        {
            throw new NotImplementedException();
        }
    }
}
