using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.MorphologicalOperations
{
    public abstract class MorphologicalOperation
    {
        public int Size { get; set; }
        public abstract void Apply(GreyImage image);
    }
}
