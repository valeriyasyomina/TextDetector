using DigitalImageProcessingLib.Algorithms.Binarization;
using DigitalImageProcessingLib.Algorithms.ConnectedComponent;
using DigitalImageProcessingLib.Algorithms.EdgeDetection;
using DigitalImageProcessingLib.Algorithms.TextDetection;
using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.Filters;
using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType;
using DigitalImageProcessingLib.Filters.FilterType.GradientFilterType;
using DigitalImageProcessingLib.Filters.FilterType.SmoothingFilterType;
using DigitalImageProcessingLib.Filters.FilterType.SWT;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.MorphologicalOperations.MorphologicalOperationsTypes;
using DigitalImageProcessingLib.SWTData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

            Bitmap bitmap = new Bitmap("20.jpg");

            BitmapConvertor conv = new BitmapConvertor();
            GreyImage image1 = conv.ToGreyImage(bitmap);

            

            MessageBox.Show("Converted");

            EdgeDetectionFilter sobel = new SobelFilter();

          //  EdgeDetectionFilter prev = new PrewittFilter();

           // EdgeDetectionFilter lap = new LaplasFilter();

            

         //   OtsuBinarization otsu = new OtsuBinarization();
          //  otsu.Countreshold(image1);
          //  otsu.Binarize(image1);

          //  NiblackBinarization nib = new NiblackBinarization(15);
          //  nib.Binarize(image1);
          
            
            
           // SmoothingFilter gauss = new GaussFilter(5, 1.4);
             SmoothingFilter gauss = new AdaptiveGaussFilter(1.4);
           // gauss.Apply(image1);
            //prev.Apply(image1);


            CannyEdgeDetection canny = new CannyEdgeDetection(gauss, sobel, 20, 80);

          //  EnhancingGradientFilter gF = new EnhancingGradientFilter();

          //  image1.Negative();
        //    gF.Apply(image1);

          //  otsu.Binarize(image1);

            

            TwoPassCCAlgorithm conCon = new TwoPassCCAlgorithm(DigitalImageProcessingLib.Interface.UnifyingFeature.StrokeWidth, 
                                                            DigitalImageProcessingLib.Interface.ConnectivityType.EightConnectedRegion);
            SWTTextDetection stext = new SWTTextDetection(canny, conCon, 20, 95, 30, 90, 85);
            stext.DetectText(image1); 



            //Stopwatch time10kOperations = Stopwatch.StartNew();

           
        
            //sobel.Apply(image1);
         //   canny.Detect(image1);

          /*  GradientEdgeBasedTextDetection textd = new GradientEdgeBasedTextDetection(canny, gF, otsu, new Dilation(7), 
               new Opening(new Dilation(7), new Erosion(9)));
            textd.DetectText(image1);*/


           // time10kOperations.Stop();
           // long milliSec = time10kOperations.ElapsedMilliseconds;

            MessageBox.Show("Edges detected");

         //  SWTFilter swt = new SWTFilter(canny.GreySmoothedImage());
           //swt.Apply(image1);

            MessageBox.Show("SWT");

            

         //   GreyImage min = swt.MinIntensityDirectionImage();
           // GreyImage max = swt.MaxIntensityDirectionImage();
            

           // Bitmap convBitmap = conv.ToBitmap(min);
            //pictureBox1.Image = convBitmap;



         /*  GreyImage imageSWT = new GreyImage(min.Width, min.Height);

            for (int i = 0; i < min.Height; i++)
                for (int j = 0; j < min.Width; j++)
                {
                    if (min.Pixels[i, j].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)
                        imageSWT.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                    else
                        imageSWT.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;
                }*/



           // min.ToTxt("swt.txt");
         //   max.ToTxt("max.txt");

           // TwoPassCCAlgorithm cc = new TwoPassCCAlgorithm(DigitalImageProcessingLib.Interface.UnifyingFeature.StrokeWidth, 
             //                                               DigitalImageProcessingLib.Interface.ConnectivityType.EightConnectedRegion);
            //cc.FindComponents(max);

            MessageBox.Show("CC");

            Bitmap convBitmap = conv.ToBitmap(image1);
            pictureBox1.Image = convBitmap;

          //  convBitmap.Save("PR.png", ImageFormat.Png);
            

           // 
          

        }
    }
}
