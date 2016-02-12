using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType
{
    public abstract class SmoothingFilter: MatrixFilter
    {
        public SmoothingFilter(int size) : base(size) 
        {
            try
            {
                this.Kernel = new int[this.Size, this.Size];
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public SmoothingFilter() { }
        public int[,] Kernel { get; protected set; }
        protected abstract void FillKernel();
    }
}
