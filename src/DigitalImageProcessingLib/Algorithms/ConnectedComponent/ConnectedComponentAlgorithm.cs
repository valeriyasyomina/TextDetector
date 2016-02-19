using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Algorithms.ConnectedComponent
{    
    public class ConnectedComponentAlgorithm: IConnectedComponent
    {
        public UnifyingFeature Feature { get; set; }
        public ConnectedComponentAlgorithm(UnifyingFeature feature)
        {
            this.Feature = feature;
        }

        /// <summary>
        /// Выращивание регионов по заданному признаку
        /// </summary>
        /// <param name="image">Серое изображение</param>
        public void FindComponents(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in FindComponents");
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void FindComponents(RGBImage image)
        {
            throw new NotImplementedException();
        }

        
    }
}
