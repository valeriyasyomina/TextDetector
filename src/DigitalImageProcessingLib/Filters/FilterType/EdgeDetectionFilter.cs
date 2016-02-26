using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType
{
    public abstract class EdgeDetectionFilter: MatrixFilter
    {
        static protected int FILTER_SIZE = 3;
        static protected int TRESHOLD = 128 * 128;
        public EdgeDetectionFilter() { }
        public EdgeDetectionFilter(int size) : base(size) { }
        public int[,] Gx { get; protected set; }
        public int[,] Gy { get; protected set; }
    }
}
