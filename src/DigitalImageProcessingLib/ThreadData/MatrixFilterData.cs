using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.ThreadData
{
    public class MatrixFilterData
    {
        public MatrixFilterData(GreyImage image, int startI, int endI, int startJ, int endJ)
        {
            this.GreyImage = image;
            this.StartIndexI = startI;
            this.EndIndexI = endI;
            this.StartIndexJ = startJ;
            this.EndIndexJ = endJ;
            this.RGBImage = null;
        }
        public MatrixFilterData(RGBImage image, int startI, int endI, int startJ, int endJ)
        {
            this.RGBImage = image;
            this.StartIndexI = startI;
            this.EndIndexI = endI;
            this.StartIndexJ = startJ;
            this.EndIndexJ = endJ;
            this.GreyImage = null;
        }
        public GreyImage GreyImage { get; set; }
        public RGBImage RGBImage { get; set; }
        public int StartIndexI { get; set; }
        public int EndIndexI { get; set; }
        public int StartIndexJ { get; set; }
        public int EndIndexJ { get; set; }
    }
}
