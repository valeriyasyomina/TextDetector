using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.SWT
{
    public class RayPixel
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public double StrokeWidth { get; set; }
    }
}
