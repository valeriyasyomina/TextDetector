using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType
{
    public abstract class SmoothingFilter: MatrixFilter
    {
        public int[,] Kernel { get; protected set; }        
    }
}
