using DigitalImageProcessingLib.BorderType;
using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.GradientData;
using DigitalImageProcessingLib.SWTData;
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
            this.Gradient = new Gradient() { Strength = 0, Angle = 0, RoundGradientDirection = RoundGradientDirection.UNDEFINED };
            this.BorderType = Border.UNDEFINED;         
            this.Color = new ColorType();
            this.StrokeWidth = new StrokeWidthData();
        }
        public Gradient Gradient { get; set; }
        public Border BorderType { get; set; }
        public ColorType Color { get; set; }
        public StrokeWidthData StrokeWidth { get; set; }
    }
}
