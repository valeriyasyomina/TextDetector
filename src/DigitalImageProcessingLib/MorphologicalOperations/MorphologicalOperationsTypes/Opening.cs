using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.MorphologicalOperations.MorphologicalOperationsTypes
{
    public class Opening: MorphologicalOperation
    {
        private Dilation _dilation = null;
        private Erosion _erosion = null;

        public Opening(Dilation dilation, Erosion erosion)
        {
            if (dilation == null)
                throw new ArgumentNullException("Null dilation");
            if (erosion == null)
                throw new ArgumentNullException("Null erosion");
            this._dilation = dilation;
            this._erosion = erosion;
        }

        /// <summary>
        /// Применение морфологической операции открытия к контурному изображению
        /// </summary>
        /// <param name="image"></param>
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");
                this._erosion.Apply(image);
               // this._dilation.Apply(image);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
