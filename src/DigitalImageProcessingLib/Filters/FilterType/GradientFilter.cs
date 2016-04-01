using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType
{
    public abstract class GradientFilter: Filter
    {
        protected GreyImage _gradientXMap = null;
        protected GreyImage _gradientYMap = null;

        public GreyImage GradientXMap()
        {
            return this._gradientXMap;
        }
        public GreyImage GradientYMap()
        {
            return this._gradientYMap;
        }
    }
}
