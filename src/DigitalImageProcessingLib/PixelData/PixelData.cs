using DigitalImageProcessingLib.BorderType;
using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.GradientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.PixelData
{    
    public class PixelData<ColorType> where ColorType: ColorBase, new()
    {
        public PixelData()
        {
            this.Gradient = new Gradient();
            this.BorderType = Border.UNDEFINED;         
            this.Color = new ColorType();
        }
        public Gradient Gradient { get; set; }
        public Border BorderType { get; set; }
        public ColorType Color { get; set; }        
    }
}
