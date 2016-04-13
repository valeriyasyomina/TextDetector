using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.ThreadData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType
{
    public class SobelFilter: EdgeDetectionFilter
    {
        private GreyImage _copyImage = null;
        public SobelFilter()
        {
            try
            {
                this.Size = FILTER_SIZE;
                this.Gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                this.Gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };   // { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } 
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Применение фильтра Собеля к серому изображению
        /// </summary>
        /// <param name="image">Серое изображение</param>
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");
                if (image.Height < this.Size)
                    throw new ArgumentException("Image height must be >= filter size");
                if (image.Width < this.Size)
                    throw new ArgumentException("Image width must be >= filter size");

                GreyImage copyImage = (GreyImage)image.Copy();
                if (copyImage == null)
                    throw new NullReferenceException("Null copy image in Apply");

                int lowIndex = Size / 2;
                int highIndexI = image.Height - lowIndex;
                int highIndexJ = image.Width - lowIndex;
                for (int i = lowIndex; i < highIndexI; i++)
                    for (int j = lowIndex; j < highIndexJ; j++)
                    {
                        byte pixelI_1J_1 = copyImage.Pixels[i - 1, j - 1].Color.Data;
                        byte pixelI_1J1 = copyImage.Pixels[i - 1, j + 1].Color.Data;
                        byte pixelI1J1 = copyImage.Pixels[i + 1, j + 1].Color.Data;


                        int gradientStrengthY = pixelI_1J_1 * Gy[0, 0] +
                          copyImage.Pixels[i - 1, j].Color.Data * Gy[0, 1] + pixelI_1J1 * Gy[0, 2] +
                          copyImage.Pixels[i + 1, j - 1].Color.Data * Gy[2, 0] + copyImage.Pixels[i + 1, j].Color.Data * Gy[2, 1] +
                          pixelI1J1 * Gy[2, 2];

                        int gradientStrengthX = pixelI_1J_1 * Gx[0, 0] +
                          pixelI_1J1 * Gx[0, 2] + copyImage.Pixels[i, j - 1].Color.Data * Gx[1, 0] +
                          copyImage.Pixels[i, j + 1].Color.Data * Gx[1, 2] + copyImage.Pixels[i + 1, j - 1].Color.Data * Gx[2, 0] +
                          pixelI1J1 * Gx[2, 2];

                        int gradientStrengthSqr = gradientStrengthX * gradientStrengthX + gradientStrengthY * gradientStrengthY;
                        image.Pixels[i, j].Gradient.Strength = (int)Math.Sqrt((double)gradientStrengthSqr);

                        if (gradientStrengthSqr > TRESHOLD)
                        {
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                            image.Pixels[i, j].BorderType = BorderType.Border.STRONG;
                        }
                        else
                        {
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;
                            image.Pixels[i, j].BorderType = BorderType.Border.WEAK;
                        }

                        if (gradientStrengthX == 0)
                        {
                            if (gradientStrengthY == 0)
                                image.Pixels[i, j].Gradient.Angle = 0;
                            else
                                image.Pixels[i, j].Gradient.Angle = 90;
                        }
                        else
                            image.Pixels[i, j].Gradient.Angle = (int)((Math.Atan((double)gradientStrengthY / gradientStrengthX)) * (180 / Math.PI));
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public override void Apply(RGBImage image)
        {
            throw new NotImplementedException();
        }

        public override GreyImage Apply(GreyImage image, int threadsNumber)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");
                if (image.Height < this.Size)
                    throw new ArgumentException("Image height must be >= filter size");
                if (image.Width < this.Size)
                    throw new ArgumentException("Image width must be >= filter size");
                if (threadsNumber <= 0)
                    throw new ArgumentException("Error threadsNumber in Apply");

                this._copyImage = (GreyImage)image.Copy();
                if (this._copyImage == null)
                    throw new NullReferenceException("Null copy image in Apply");

                this.Threads = new List<Thread>();

                int deltaI = image.Height / threadsNumber;
                int filterSize = this.Size;
                int lowIndex = filterSize / 2;
                int lowIndexI = lowIndex;
                int highIndexI = lowIndexI + deltaI;
                int highIndexJ = image.Width - lowIndex;

                for (int i = 0; i < threadsNumber; i++)
                {
                    if (i == threadsNumber - 1)
                        highIndexI = image.Height - lowIndex;

                    MatrixFilterData matrixFilterData = new ThreadData.MatrixFilterData(image, lowIndexI, highIndexI, lowIndex, highIndexJ);
                    Thread thread = new Thread(new ParameterizedThreadStart(this.ApplyThread));
                    this.Threads.Add(thread);
                    this.Threads[i].Start(matrixFilterData);

                    lowIndexI = highIndexI;
                    highIndexI += deltaI;
                }
                WaitForThreads();

                return this._copyImage;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        private void WaitForThreads()
        {
            try
            {
                int hreadsNumber = this.Threads.Count;
                for (int i = 0; i < hreadsNumber; i++)
                    this.Threads[i].Join();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        protected override void ApplyThread(object data)
        {
            try
            {
                MatrixFilterData matrixFilterData = (MatrixFilterData)data;

                GreyImage image = matrixFilterData.GreyImage;
                int startI = matrixFilterData.StartIndexI;
                int endI = matrixFilterData.EndIndexI;
                int startJ = matrixFilterData.StartIndexJ;
                int endJ = matrixFilterData.EndIndexJ;
                int filterSize = this.Size;
                int lowIndex = filterSize / 2;

                for (int i = startI; i < endI; i++)
                    for (int j = startJ; j < endJ; j++)
                    {
                        byte pixelI_1J_1 = image.Pixels[i - 1, j - 1].Color.Data;
                        byte pixelI_1J1 = image.Pixels[i - 1, j + 1].Color.Data;
                        byte pixelI1J1 = image.Pixels[i + 1, j + 1].Color.Data;


                        int gradientStrengthY = pixelI_1J_1 * Gy[0, 0] +
                          image.Pixels[i - 1, j].Color.Data * Gy[0, 1] + pixelI_1J1 * Gy[0, 2] +
                          image.Pixels[i + 1, j - 1].Color.Data * Gy[2, 0] + image.Pixels[i + 1, j].Color.Data * Gy[2, 1] +
                          pixelI1J1 * Gy[2, 2];

                        int gradientStrengthX = pixelI_1J_1 * Gx[0, 0] +
                          pixelI_1J1 * Gx[0, 2] + image.Pixels[i, j - 1].Color.Data * Gx[1, 0] +
                          image.Pixels[i, j + 1].Color.Data * Gx[1, 2] + image.Pixels[i + 1, j - 1].Color.Data * Gx[2, 0] +
                          pixelI1J1 * Gx[2, 2];

                        int gradientStrengthSqr = gradientStrengthX * gradientStrengthX + gradientStrengthY * gradientStrengthY;
                        this._copyImage.Pixels[i, j].Gradient.Strength = (int)Math.Sqrt((double)gradientStrengthSqr);

                        if (gradientStrengthSqr > TRESHOLD)
                        {
                            this._copyImage.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                            this._copyImage.Pixels[i, j].BorderType = BorderType.Border.STRONG;
                        }
                        else
                        {
                            this._copyImage.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;
                            this._copyImage.Pixels[i, j].BorderType = BorderType.Border.WEAK;
                        }

                        if (gradientStrengthX == 0)
                        {
                            if (gradientStrengthY == 0)
                                this._copyImage.Pixels[i, j].Gradient.Angle = 0;
                            else
                                this._copyImage.Pixels[i, j].Gradient.Angle = 90;
                        }
                        else
                            this._copyImage.Pixels[i, j].Gradient.Angle = (int)((Math.Atan((double)gradientStrengthY / gradientStrengthX)) * (180 / Math.PI));
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
