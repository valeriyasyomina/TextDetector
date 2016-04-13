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
using DigitalImageProcessingLib.IO;
using DigitalImageProcessingLib.MorphologicalOperations.MorphologicalOperationsTypes;
using DigitalImageProcessingLib.RegionData;
using DigitalImageProcessingLib.SWTData;
using DigitalVideoProcessingLib.IO;
using Emgu.CV;
using Emgu.CV.Structure;
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
         //   Image<Gray, Byte> img = new Image<Gray, byte>(800, 600, new Gray(155));
         //   Byte x = img.Data[0, 0, 0];
           // MessageBox.Show(x.ToString());

        /*    Image<Gray, Byte> frame1 = new Image<Gray, byte>(@"C:\Users\valeriya\Desktop\V1\frames\52.jpg");
            Image<Gray, Byte> frame2 = new Image<Gray, byte>(@"C:\Users\valeriya\Desktop\V1\frames\53.jpg");

            Image<Gray, Byte> absFr = new Image<Gray, byte>(frame1.Width, frame1.Height);

            for (int i = 0; i < frame1.Height; i++)
                for (int j = 0; j < frame1.Width; j++)
                {
                    absFr.Data[i, j, 0] = (byte)Math.Abs(frame2.Data[i, j, 0] - frame1.Data[i, j, 0]);
                }      */


            //Capture capture = new Capture(@"C:\Users\valeriya\Desktop\videoCut\Video_37_2_3.mp4");
           // Image<Gray, Byte> currentFrame = capture.QueryGrayFrame().Resize(640, 480, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

          //  for (int i = 1; i <= 62; i++ )
            //    currentFrame = capture.QueryGrayFrame().Resize(640, 480, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

           // currentFrame.Save("100500.jpg");

            Bitmap bitmap = new Bitmap(@"C:\Users\valeriya\Desktop\ICDAR\frames_51\230.jpg");

            BitmapConvertor conv = new BitmapConvertor();
            GreyImage image1 = conv.ToGreyImage(bitmap);

         //   EnhancingGradientFilter ench = new EnhancingGradientFilter();
         //   ench.Apply(image1);

            //GreyImage copyImageCanny = (GreyImage)image1.Copy();
          //  GreyImage copyImageGauss = (GreyImage)image1.Copy();
            

         //   ImageConvertor ImageConvertor = new DigitalVideoProcessingLib.IO.ImageConvertor();
           // GreyImage image1 = ImageConvertor.ConvertColor(currentFrame);

         //   GreyImage imageCopy = conv.ToGreyImage(bitmap);

         /*   GreyImage image1 = new GreyImage(frame1.Width, frame1.Height);

            for (int i = 0; i < frame1.Height; i++)
                for (int j = 0; j < frame1.Width; j++)
                {
                    image1.Pixels[i, j].Color.Data =  absFr.Data[i, j, 0];
                }*/


            

            MessageBox.Show("Converted");

            EdgeDetectionFilter sobel = new SobelFilter();

            EdgeDetectionFilter sobel1 = new SobelFilter();
           // sobel.Apply(image1);

          //  EdgeDetectionFilter prev = new PrewittFilter();

           // EdgeDetectionFilter lap = new LaplasFilter();

            

            OtsuBinarization otsu = new OtsuBinarization();
           // otsu.Countreshold(image1);
          //  otsu.Binarize(image1);

          //  NiblackBinarization nib = new NiblackBinarization(15);
          //  nib.Binarize(image1);



            GaussFilter gauss = new GaussFilter(5, 1.4);
            SmoothingFilter gauss1 = new AdaptiveGaussFilter(1.4);


            CannyEdgeDetection canny = new CannyEdgeDetection(gauss, sobel, 20, 80);

           // Stopwatch time10kOperations = Stopwatch.StartNew();
           // GreyImage imageCanny = canny.Detect(image1, 4);
          //  time10kOperations.Stop();
          //  long milliSec = time10kOperations.ElapsedMilliseconds;


           //  SmoothingFilter gauss = new AdaptiveGaussFilter(1.4);   // (1.4)

             
           // gauss.Apply(image1);
            //prev.Apply(image1);


           

         //   canny.Detect(copyImageCanny);
          //  gauss.Apply(copyImageGauss);


          //  CannyEdgeDetection canny1 = new CannyEdgeDetection(gauss, sobel, 20, 80);

          //  EnhancingGradientFilter gF = new EnhancingGradientFilter();

        /*    canny.Detect(image1);
            gauss.Apply(copyImage);

            SimpleGradientFilter SimpleGradintFilter = new DigitalImageProcessingLib.Filters.FilterType.GradientFilterType.SimpleGradientFilter();
            SimpleGradintFilter.Apply(copyImage);

            SWTFilterSmart SWTFilterSmart = new DigitalImageProcessingLib.Filters.FilterType.SWT.SWTFilterSmart(SimpleGradintFilter.GradientXMap(), SimpleGradintFilter.GradientYMap());
            SWTFilterSmart.Apply(image1);

            GreyImage min = SWTFilterSmart.MinIntensityDirectionImage();
            GreyImage max = SWTFilterSmart.MaxIntensityDirectionImage(); */


          //  image1.Negative();
        //    gF.Apply(image1);

          //  otsu.Binarize(image1);

            
          //  List<TextRegion> textRegions = null;
            TwoPassCCAlgorithm conCon = new TwoPassCCAlgorithm(DigitalImageProcessingLib.Interface.UnifyingFeature.StrokeWidth, 
                                                            DigitalImageProcessingLib.Interface.ConnectivityType.EightConnectedRegion);
           // SWTTextDetection stext = new SWTTextDetection(canny, 20, 80, 30);


            SimpleGradientFilter simpleGradintFilter = new DigitalImageProcessingLib.Filters.FilterType.GradientFilterType.SimpleGradientFilter();

            SWTTextDetection stext1 = new SWTTextDetection(canny, simpleGradintFilter, 0.5);

            
             stext1.DetectText(image1, 4);

            //stext.DetectText(imageCopy);



            //

           
        
            //sobel.Apply(image1);
           // canny.Detect(image1);

         /*   GradientEdgeBasedTextDetection textd = new GradientEdgeBasedTextDetection(canny, gF, otsu, new Dilation(7), 
               new Opening(new Dilation(7), new Erosion(9)));
            textd.DetectText(image1, out textRegions);*/


           

            MessageBox.Show("Edges detected");

          // SWTFilter swt = new SWTFilter(canny.GreySmoothedImage());
          // swt.Apply(image1);

            MessageBox.Show("SWT");

            

           // GreyImage min = swt.MinIntensityDirectionImage();
            //GreyImage max = swt.MaxIntensityDirectionImage();
            

           // Bitmap convBitmap = conv.ToBitmap(min);
            //pictureBox1.Image = convBitmap;



          /*  GreyImage imageSWT = new GreyImage(max.Width, max.Height);

            for (int i = 0; i < max.Height; i++)
                for (int j = 0; j < max.Width; j++)
                {
                    if (max.Pixels[i, j].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)
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

            Pen pen = new Pen(Color.Red, 2);
            Graphics g = Graphics.FromImage(convBitmap);

            List<TextRegion> textRegions = image1.TextRegions;

            for (int i = 0; i < textRegions.Count; i++)
                g.DrawRectangle(pen, textRegions[i].MinBorderIndexJ, textRegions[i].MinBorderIndexI,
                    textRegions[i].MaxBorderIndexJ - textRegions[i].MinBorderIndexJ, textRegions[i].MaxBorderIndexI - textRegions[i].MinBorderIndexI);

            pictureBox1.Image = convBitmap;

          //  convBitmap.Save("PR.png", ImageFormat.Png);
            

           // 
          

        }
    }
}
