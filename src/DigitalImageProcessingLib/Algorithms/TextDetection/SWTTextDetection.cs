using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.Filters;
using DigitalImageProcessingLib.Filters.FilterType.SWT;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Interface;
using DigitalImageProcessingLib.PixelData;
using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Algorithms.TextDetection
{
    public class SWTTextDetection: ITextDetection
    {
        private IEdgeDetection _edgeDetector = null;
        private IConnectedComponent _connectedComponent = null;
        private SWTFilter _SWTFilter = null;
        private int _strokeWidthDelta = 0;
        private int _pixelsPercentTreshold = 0;
        private int _minPixelsNumberInRegion = 0;
        private int _regionSquareTheshold = 0;
        private int _regionWidthTreshold = 0;
        public SWTTextDetection(IEdgeDetection edgeDetector, IConnectedComponent connectedComponent, int swtDelta,
            int percentTreshold, int minPixelsNumber, int regionSquareThreshold, int regionWidthTreshold)
        {
            if (edgeDetector == null)
                throw new ArgumentNullException("Null edgeDetector");        
            if (connectedComponent == null)
                throw new ArgumentNullException("Null connectedComponent");
            if (swtDelta < 0)
                throw new ArgumentException("Error swtDelta");
            if (percentTreshold <= 0)
                throw new ArgumentException("Error percentTreshold");
            if (minPixelsNumber <= 0)
                throw new ArgumentException("Error minPixelsNumber");
            this._edgeDetector = edgeDetector;          
            this._connectedComponent = connectedComponent;
            this._strokeWidthDelta = swtDelta;
            this._pixelsPercentTreshold = percentTreshold;
            this._minPixelsNumberInRegion = minPixelsNumber;
            this._regionSquareTheshold = regionSquareThreshold;
            this._regionWidthTreshold = regionWidthTreshold;
        }
        public void DetectText(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in DetectText");

                this._edgeDetector.Detect(image);
                this._SWTFilter = new SWTFilter(this._edgeDetector.GreySmoothedImage());
                this._SWTFilter.Apply(image);

                GreyImage darkTextLightBg = this._SWTFilter.MinIntensityDirectionImage();
               // GreyImage lightTextDarkBg = this._SWTFilter.MaxIntensityDirectionImage();

                this._connectedComponent.FindComponents(darkTextLightBg);
                Dictionary<int, Region> darkRegions = this._connectedComponent.Regions;
                CountTruePixels(darkRegions, darkTextLightBg);
                DeleteTreshRegions(darkRegions, image.Width);
                SetRegionColor(darkRegions, image, darkTextLightBg);
                int a = 0;
                a++;
                //this._connectedComponent.FindComponents(lightTextDarkBg);                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void DetectText(RGBImage image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вычисляет количество пикселей в каждом регионе, значение ширины штриха которых +-= this._strokeWidthDelta
        /// </summary>
        /// <param name="regions">Области</param>
        /// <param name="image">Серое изображение</param>
        private void CountTruePixels(Dictionary<int, Region> regions, GreyImage image)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        int pixelRegionNumber = image.Pixels[i, j].RegionNumber;
                        if (pixelRegionNumber != PixelData<Grey>.UNDEFINED_REGION)
                        {
                            int averageStrokeWidth = regions[pixelRegionNumber].AverageStrokeWidth;
                            int pixelStrokeWidth = image.Pixels[i, j].StrokeWidth.Width;
                            if (Math.Abs(averageStrokeWidth - pixelStrokeWidth) <= this._strokeWidthDelta)
                                regions[pixelRegionNumber].TruePixelsNumber++;
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Удаляет области, в котрых процент пикселей попавших в диапазо среднего значения ширины штриха меньше заданного порога
        /// А также регионы с больши количеством пиксель и маленьким колическом пикселей и неплотные регионы
        /// </summary>
        /// <param name="regions"></param>
        private void DeleteTreshRegions(Dictionary<int, Region> regions, int imageWidth)
        {
            try
            {
                foreach (var pair in regions.ToList())
                {                   
                    double regionWidthRation = (pair.Value.MaxBorderIndexJ - pair.Value.MinBorderIndexJ) /  imageWidth;
                    double percent = (double)pair.Value.TruePixelsNumber / pair.Value.PixelsNumber;
                    double squareRatio = (double)pair.Value.PixelsNumber / pair.Value.Square;
                    if (percent * 100 < this._pixelsPercentTreshold  || pair.Value.PixelsNumber < this._minPixelsNumberInRegion
                      /*  squareRatio * 100 < this._regionSquareTheshold)*/  /*||  pair.Value.PixelsNumber > 30000*/)
                      //  regionWidthRation * 100 > this._regionWidthTreshold)
                        regions.Remove(pair.Key);
                }
           
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Выставляет черный цвет пикселям, которые принадлежат какому либо региону
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="image">Изображение, для которого устанавливается цвет</param>
        /// <param name="imageWithRegions">Изображение, на котором определены регионы</param>
        private void SetRegionColor(Dictionary<int, Region> regions, GreyImage image, GreyImage imageWithRegions)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;

                 for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                        image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE; 
                    

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        int pixelRegionNumber = imageWithRegions.Pixels[i, j].RegionNumber;
                        if (pixelRegionNumber != PixelData<Grey>.UNDEFINED_REGION && regions.ContainsKey(pixelRegionNumber))
                        {
                            if (regions[pixelRegionNumber].PixelsNumber < 30)
                            {
                                int a = 0;
                                a++;
                            }
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                        }
                      //  else
                         //   image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;                        
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
