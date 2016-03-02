using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Interface
{
    public enum UnifyingFeature { StrokeWidth }
    public enum ConnectivityType { FourConnectedRegion, EightConnectedRegion }
    public interface IConnectedComponent
    {
        UnifyingFeature Feature { get; set; }
        ConnectivityType ConnectivityType { get; set; }       
        Dictionary<int, Region> Regions { get; set; }
        void FindComponents(GreyImage image);
        void FindComponents(RGBImage image);
    }
}
