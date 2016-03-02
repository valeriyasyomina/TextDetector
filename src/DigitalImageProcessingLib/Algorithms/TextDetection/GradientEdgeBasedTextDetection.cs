using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Interface;
using DigitalImageProcessingLib.MorphologicalOperations;
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


                Thread edgeThread = new Thread(new ParameterizedThreadStart(this.EdgeBasedProcessThread));
                Thread gradientThread = new Thread(new ParameterizedThreadStart(this.GradientBasedProcessThread));

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

             //   this._dilation.Apply(image);
               // this._opening.Apply(image);
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
