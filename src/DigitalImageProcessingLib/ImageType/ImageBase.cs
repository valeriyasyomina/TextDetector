using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.PixelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.ImageType
{
    public abstract class ImageBase<ColorType> where ColorType: ColorBase, new()
    {
        public abstract bool IsColored();
        public int Height { get; protected set; }
        public int Width { get; protected set; }
        public PixelData<ColorType>[,] Pixels { get; protected set; }
        public abstract ImageBase<ColorType> Copy();        
    }
}

