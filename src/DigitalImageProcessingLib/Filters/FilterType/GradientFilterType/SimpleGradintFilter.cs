using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.GradientFilterType
{
    public class SimpleGradintFilter : GradientFilter
    {
        private GreyImage _gradientXMap = null;
        private GreyImage _gradientYMap = null;

        public GreyImage GradientXMap()
        {
            return this._gradientXMap;
        }
        public GreyImage GradientYMap()
        {
            return this._gradientYMap;
        }
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");

                this._gradientXMap = new GreyImage(image.Width, image.Height);
                this._gradientYMap = new GreyImage(image.Width, image.Height);

                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        this._gradientXMap.Pixels[i, j].Gradient.GradientX = image.Pixels[i, j].Color.Data - image.Pixels[i + 1, j].Color.Data;
                        this._gradientYMap.Pixels[i, j].Gradient.GradientY = image.Pixels[i, j].Color.Data - image.Pixels[i, j + 1].Color.Data;
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public override void Apply(RGBImage image)
        {
            throw new NotImplementedException();
        }
    }
}
