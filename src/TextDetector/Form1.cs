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
            
           // GreyImage g = new GreyImage(10, 7);
            //GreyImage d = new GreyImage(10, 7);
            //bool s = g == d;
           /* g.Pixels[0, 0].Color.Data = 50;
            g.Pixels[0, 1].Color.Data = 125;
            g.Pixels[0, 2].Color.Data = 22;

            g.Pixels[1, 0].Color.Data = 12;
            g.Pixels[1, 1].Color.Data = 17;
            g.Pixels[1, 2].Color.Data = 187;

            g.Pixels[2, 0].Color.Data = 201;
            g.Pixels[2, 1].Color.Data = 100;
            g.Pixels[2, 2].Color.Data = 45;*/

            //EdgeDetectionFilter fdd = new SobelFilter();
        //    SmoothingFilter f = new GaussFilter(5, 1.4);
       //     f.Apply(g);


            GreyImage image = new GreyImage(5, 4);

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


          

            

          //  bool s = image.IsEqual(patternImage);
           // SmoothingFilter f = new GaussFilter(5, 1.4);

            EdgeDetectionFilter f = new SobelFilter();

            f.Apply(image);

           


            int a = 0;
            a++;
            int b = 2;

           // StreamWriter s = new StreamWriter("1.txt", File.AppendText);

        }
    }
}
