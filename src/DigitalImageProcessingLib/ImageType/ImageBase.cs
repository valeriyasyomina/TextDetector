using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.PixelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.ImageType
{
    public abstract class ImageBase<ColorType> where ColorType: ColorBase, new()
    {
        public ImageBase() { }
        public ImageBase(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException("Incorrect width value");
            if (height <= 0)
                throw new ArgumentException("Incorrect height value");
            this.Width = width;
            this.Height = height;            
            AllocatePixels();
        }
        public abstract bool IsColored();
        public int Height { get; protected set; }
        public int Width { get; protected set; }
        public PixelData<ColorType>[,] Pixels { get; protected set; }
        public abstract ImageBase<ColorType> Copy();   
     
        /// <summary>
        /// Выделение памяти на матрицу пикселей
        /// </summary>
        private void AllocatePixels()
        {
            try
            {
                this.Pixels = new PixelData.PixelData<ColorType>[this.Height, this.Width];
               // this.Pixels = new PixelData.PixelData<ColorType>[this.Width, this.Height];
                for (int i = 0; i < this.Height; i++)
                    for (int j = 0; j < this.Width; j++)                   
                        this.Pixels[i, j] = new PixelData.PixelData<ColorType>();               
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}

