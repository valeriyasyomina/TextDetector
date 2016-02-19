using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Interface
{
    public enum UnifyingFeature { StrokeWidth }
    interface IConnectedComponent
    {
        UnifyingFeature Feature { get; set; }
        void FindComponents(GreyImage image);
        void FindComponents(RGBImage image);
    }
}
