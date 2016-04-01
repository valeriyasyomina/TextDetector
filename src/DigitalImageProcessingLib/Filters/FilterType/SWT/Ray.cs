using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.SWT
{
    public enum RayDirection { RIGHT, LEFT, DOWN, UP, RIGHT_UP, RIGHT_DOWN, LEFT_UP, LEFT_DOWN, UNDEFINED }
    public class Ray
    {
        public Ray()
        {
            this.Direction = RayDirection.UNDEFINED;
            this.Pixels = new List<RayPixel>();
        }
        public int RowBeginIndex { get; set; }
        public int RowEndIndex { get; set; }
        public int ColumnBeginIndex { get; set; }
        public int ColumnEndIndex { get; set; }
        public int ColumnStep { get; set; }
        public RayDirection Direction { get; set; }
        public List<RayPixel> Pixels { get; set; }
    }
}
