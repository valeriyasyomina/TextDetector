using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters
{
    public abstract class MatrixFilter: Filter
    {        
        public int Size { get; protected set; }
        public int NormalizationRatio { get; protected set; }
    }
}
