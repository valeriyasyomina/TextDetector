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
        public static int UNDEFINED_REGION = -1;
        public PixelData()
        {
            this.Gradient = new Gradient() { Strength = 0, Angle = 0, RoundGradientDirection = RoundGradientDirection.UNDEFINED };
            this.BorderType = Border.UNDEFINED;         
            this.Color = new ColorType();
            this.StrokeWidth = new StrokeWidthData();
            this.RegionNumber = UNDEFINED_REGION;
        }
        public Gradient Gradient { get; set; }
        public Border BorderType { get; set; }
        public ColorType Color { get; set; }
        public StrokeWidthData StrokeWidth { get; set; }
        public int RegionNumber { get; set; }
    }
}
