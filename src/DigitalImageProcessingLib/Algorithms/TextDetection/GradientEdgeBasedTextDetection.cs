using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Interface;
using DigitalImageProcessingLib.MorphologicalOperations;
using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Algorithms.TextDetection
{
    public class GradientEdgeBasedTextDetection : ITextDetection
    {
        private IEdgeDetection _edgeDetector = null;
        private GradientFilter _gradientFilter = null;
        private IGlobalTresholdBinarization _binarizator = null;
        private GreyImage _gradientImage = null;
        private GreyImage _edgeImage = null;
        private MorphologicalOperation _dilation = null;
        private MorphologicalOperation _opening = null;

        public GradientEdgeBasedTextDetection(IEdgeDetection edgeDetector, GradientFilter gradientFilter, IGlobalTresholdBinarization binarizator,
            MorphologicalOperation dilation, MorphologicalOperation opening)
        {
            if (edgeDetector == null)
                throw new ArgumentNullException("Null edgeDetector");
            if (gradientFilter == null)
                throw new ArgumentNullException("Null gradientFilter");
            if (binarizator == null)
                throw new ArgumentNullException("Null binarizator");
            if (dilation == null)
                throw new ArgumentNullException("Null dilation");
            if (opening == null)
                throw new ArgumentNullException("Null opening");
            this._edgeDetector = edgeDetector;
            this._gradientFilter = gradientFilter;
            this._binarizator = binarizator;
            this._dilation = dilation;
            this._opening = opening;
        }

        /// <summary>
        /// Выделение текста на изображении гибрибным подходом
        /// </summary>
        /// <param name="image">Изображение</param>
        public void DetectText(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in DetectText");

               // textRegions = null;

                Thread edgeThread = new Thread(new ParameterizedThreadStart(this.EdgeBasedProcessThread));
                Thread gradientThread = new Thread(new ParameterizedThreadStart(this.GradientBasedProcessThread));

             //   GreyImage copyImage = (GreyImage)image.Copy();

                edgeThread.Start(image);
                gradientThread.Start(image);

                edgeThread.Join();
                gradientThread.Join();  

             
                

                for (int i = 0; i < image.Height; i++)
                    for (int j = 0; j < image.Width; j++)
                    {
                        if (this._gradientImage.Pixels[i, j].Color.Data == ColorBase.MAX_COLOR_VALUE &&
                            this._edgeImage.Pixels[i, j].Color.Data == ColorBase.MIN_COLOR_VALUE)
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                        else
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;

                        image.Pixels[i, j].BorderType = this._edgeImage.Pixels[i, j].BorderType;
                      //  image.Pixels[i, j].Color.Data = this._gradientImage.Pixels[i, j].Color.Data;
                    }

                this._dilation.Apply(image);

         /*       int[] heightHist = new int[copyImage.Height];
                int[] widthHist = new int[copyImage.Width];

                for (int i = 0; i < copyImage.Height; i++)
                {
                    int sum = 0;
                    for (int j = 0; j < copyImage.Width; j++)
                    {
                        if (copyImage.Pixels[i, j].Color.Data == (byte)ColorBase.MIN_COLOR_VALUE)
                            ++sum;
                    }
                    heightHist[i] = sum;
                }

                int maxH = heightHist.Max();

                for (int i = 0; i < copyImage.Width; i++)
                {
                    int sum = 0;
                    for (int j = 0; j < copyImage.Height; j++)
                    {
                        if (copyImage.Pixels[j, i].Color.Data == (byte)ColorBase.MIN_COLOR_VALUE)
                            ++sum;
                    }
                    widthHist[i] = sum;
                }
                int maxw = widthHist.Max();

                for (int i = 0; i < heightHist.Length; i++)
                {
                    if (maxH - heightHist[i] < 150)
                        heightHist[i] = 1;
                    else
                        heightHist[i] = 0;
                }

                for (int i = 0; i < widthHist.Length; i++)
                {
                    if (maxw - widthHist[i] < 50)
                        widthHist[i] = 1;
                    else
                        widthHist[i] = 0;
                }

                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        if (heightHist[i] == 1 && widthHist[j] == 1)
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                     //   else
                           // image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;

                    }
                }*/


              //  this._opening.Apply(image);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        /// <summary>
        /// Процедура вычисления градиентного изображения
        /// </summary>
        /// <param name="data">Исходное изображение</param>
        private void GradientBasedProcessThread(object data)
        {
            try
            {
                GreyImage image = (GreyImage)data;
                this._gradientImage = (GreyImage)image.Copy();

                this._gradientFilter.Apply(this._gradientImage);
                this._binarizator.Binarize(this._gradientImage);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Процедура вычисления контурного изображения
        /// </summary>
        /// <param name="data">Исходное изображение</param>
        private void EdgeBasedProcessThread(object data)
        {
            try
            {
                GreyImage image = (GreyImage)data;
                this._edgeImage = (GreyImage)image.Copy();

                this._edgeDetector.Detect(this._edgeImage);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void IntersectGradientAndBasedImages()
        {

        }

        public void DetectText(RGBImage image)
        {
            throw new NotImplementedException();
        }
    }
}
