using DigitalImageProcessingLib.Filters;
using DigitalImageProcessingLib.Filters.FilterType.SWT;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Algorithms.TextDetection
{
    public class SWTTextDetection: ITextDetection
    {
        private IEdgeDetection _edgeDetector = null;
        private IConnectedComponent _connectedComponent = null;
        private SWTFilter _SWTFilter = null;
        public SWTTextDetection(IEdgeDetection edgeDetector, IConnectedComponent connectedComponent)
        {
            if (edgeDetector == null)
                throw new ArgumentNullException("Null edgeDetector");        
            if (connectedComponent == null)
                throw new ArgumentNullException("Null connectedComponent");
            this._edgeDetector = edgeDetector;          
            this._connectedComponent = connectedComponent;           
        }
        public void DetectText(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in DetectText");

                this._edgeDetector.Detect(image);
                this._SWTFilter = new SWTFilter(this._edgeDetector.GreySmoothedImage());
                this._SWTFilter.Apply(image);

                GreyImage darkTextLightBg = this._SWTFilter.MinIntensityDirectionImage();
                GreyImage lightTextDarkBg = this._SWTFilter.MaxIntensityDirectionImage();

                this._connectedComponent.FindComponents(darkTextLightBg);
                this._connectedComponent.FindComponents(lightTextDarkBg);                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void DetectText(RGBImage image)
        {
            throw new NotImplementedException();
        }
    }
}
