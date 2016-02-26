using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters
{
    public abstract class MatrixFilter: Filter
    {
        public MatrixFilter() { }
        public MatrixFilter(int size) { this.Size = size; }
        public int Size { get; protected set; }
        public int NormalizationRatio { get; protected set; }
        public List<Thread> ThreadList { get; set; }
        public int ThreadsNumber { get; set; }
    }
}
