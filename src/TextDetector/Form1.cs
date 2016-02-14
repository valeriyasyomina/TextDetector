using DigitalImageProcessingLib.Algorithms.EdgeDetection;
using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType;
using DigitalImageProcessingLib.Filters.FilterType.SmoothingFilterType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextDetector.Convertor;

namespace TextDetector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {    

            Bitmap bitmap = new Bitmap("9.jpg");

            BitmapConvertor conv = new BitmapConvertor();
            GreyImage image1 = conv.ToGreyImage(bitmap);

            MessageBox.Show("Converted");

            EdgeDetectionFilter sobel = new SobelFilter();

           // sobel.Apply(image1);
            SmoothingFilter gauss = new GaussFilter(5, 1.4);

           // gauss.Apply(image1);

            CannyEdgeDetection canny = new CannyEdgeDetection(gauss, sobel, 20, 80);

            canny.Detect(image1);

            MessageBox.Show("Edges detected");

            Bitmap convBitmap = conv.ToBitmap(image1);

            pictureBox1.Image = convBitmap;
            
            
          


         /*   GreyImage image = new GreyImage(5, 4);

            image.Pixels[0, 0].Color.Data = 50;
            image.Pixels[0, 1].Color.Data = 125;
            image.Pixels[0, 2].Color.Data = 22;
            image.Pixels[0, 3].Color.Data = 11;
            image.Pixels[0, 4].Color.Data = 104;

            image.Pixels[1, 0].Color.Data = 12;
            image.Pixels[1, 1].Color.Data = 17;
            image.Pixels[1, 2].Color.Data = 187;
            image.Pixels[1, 3].Color.Data = 0;
            image.Pixels[1, 4].Color.Data = 15;

            image.Pixels[2, 0].Color.Data = 201;
            image.Pixels[2, 1].Color.Data = 100;
            image.Pixels[2, 2].Color.Data = 45;
            image.Pixels[2, 3].Color.Data = 91;
            image.Pixels[2, 4].Color.Data = 17;

            image.Pixels[3, 0].Color.Data = 100;
            image.Pixels[3, 1].Color.Data = 15;
            image.Pixels[3, 2].Color.Data = 18;
            image.Pixels[3, 3].Color.Data = 205;
            image.Pixels[3, 4].Color.Data = 194;                    

            EdgeDetectionFilter f = new SobelFilter();

            f.Apply(image);           


            int a = 0;
            a++;
            int b = 2;   */       
        }
    }
}
