using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPFVideoTextDetector.Convertors
{
    public class BitmapImageConvertor: IImageConvertor
    {
        /// <summary>
        /// Конвертация bitmap в bitmapImage
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <returns>bitmapImage</returns>
        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {           
            MemoryStream memoryStream = new MemoryStream();
            ((System.Drawing.Bitmap)bitmap).Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);
            image.StreamSource = memoryStream;
            image.EndInit();
            return image; 
        }
    }
}
