using DigitalImageProcessingLib.Algorithms.ConnectedComponent;
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
using System.Threading;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Algorithms.TextDetection
{
    public class SWTTextDetection: ITextDetection
    {
        private IEdgeDetection _edgeDetector = null;
        private IConnectedComponent _lightTextConnectedComponent = null;
        private IConnectedComponent _darkTextConnectedComponent = null;
        private SWTFilter _SWTFilter = null;
        private int _strokeWidthDelta = 0;
        private double _pixelsPercentTreshold = 0;
        private int _minPixelsNumberInRegion = 0;
       // private int _regionSquareTheshold = 0;
        //private int _regionWidthTreshold = 0;

        private static double STROKE_WIDTH_RATIO = 2.0;
        private static double HEIGHT_RATIO = 2.0;
        private static double WIDTH_RATIO = 2.0;
        private static double INTENSITY_DIFFERENCE = 1.0;
        private static double WIDTH_DISTANCE_RATIO = 3.0;
        private static double ASPECT_RATIO = 5.0;
        private static double DIAMETER_SW_RATIO = 5.0;
        private static double BB_PIXELS_NUMBER_MIN_RATIO = 1.0;
        private static double BB_PIXELS_NUMBER_MAX_RATIO = 30.0;
        private static double IMAGE_REGION_HEIGHT_RATIO_MAX = 50.0;
        private static double IMAGE_REGION_WIDTH_RATIO_MAX = 50.0;
        private static double IMAGE_REGION_HEIGHT_RATIO_MIN = 1.5;
        private static double IMAGE_REGION_WIDTH_RATIO_MIN = 1.5;

        private static int MIN_LETTERS_IN_TEXT_REGION = 2;


        private GreyImage _darkTextLightBg = null;
        private GreyImage _lightTextDarkBg = null;

        private List<TextRegion> _lightTextRegions = null;
        private List<TextRegion> _darkTextRegions = null;

        private Dictionary<int, Region> _darkRegions = null;
        private Dictionary<int, Region> _lightRegions = null;

        private static int ERROR_VALUE = -1;

        public SWTTextDetection(IEdgeDetection edgeDetector, int swtDelta, double percentTreshold, int minPixelsNumber)
        {
            if (edgeDetector == null)
                throw new ArgumentNullException("Null edgeDetector");        
            if (swtDelta < 0)
                throw new ArgumentException("Error swtDelta");
            if (percentTreshold <= 0)
                throw new ArgumentException("Error percentTreshold");
            if (minPixelsNumber <= 0)
                throw new ArgumentException("Error minPixelsNumber");
            this._edgeDetector = edgeDetector;          
          //  this._lightTextConnectedComponent = new TwoPassCCAlgorithm(connectedComponent);

            this._lightTextConnectedComponent = new TwoPassCCAlgorithm(DigitalImageProcessingLib.Interface.UnifyingFeature.StrokeWidth,
                                                            DigitalImageProcessingLib.Interface.ConnectivityType.EightConnectedRegion);
            this._darkTextConnectedComponent = new TwoPassCCAlgorithm(DigitalImageProcessingLib.Interface.UnifyingFeature.StrokeWidth,
                                                            DigitalImageProcessingLib.Interface.ConnectivityType.EightConnectedRegion);
            this._strokeWidthDelta = swtDelta;
            this._pixelsPercentTreshold = percentTreshold;
            this._minPixelsNumberInRegion = minPixelsNumber;
           // this._regionSquareTheshold = regionSquareThreshold;
            //this._regionWidthTreshold = regionWidthTreshold;

            this._lightRegions = new Dictionary<int, Region>();
            this._darkRegions = new Dictionary<int, Region>();
            this._lightTextRegions = new List<TextRegion>();
            this._darkTextRegions = new List<TextRegion>();
        }
        public void DetectText(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in DetectText");
                FreeResources();

             //   textRegions = null;

         

                GreyImage copyImage = (GreyImage)image.Copy();

                this._edgeDetector.Detect(copyImage);
                this._SWTFilter = new SWTFilter(this._edgeDetector.GreySmoothedImage());
                this._SWTFilter.Apply(copyImage);

               // darkTextLightBg = this._SWTFilter.MaxIntensityDirectionImage();
               // GreyImage lightTextDarkBg = this._SWTFilter.MaxIntensityDirectionImage();

              /*  this._connectedComponent.FindComponents(darkTextLightBg);
                Dictionary<int, Region> darkRegions = this._connectedComponent.Regions;
                CountTruePixels(darkRegions, darkTextLightBg);
                DeleteTreshRegions(darkRegions, image.Width, image.Height);
                               
                GetTextRegions(darkRegions, out textRegions);*/
                //DeleteInnerRegions(darkRegions);

           /*     this.lightTextDarkBg = this._SWTFilter.MaxIntensityDirectionImage();  
                this._lightTextConnectedComponent.FindComponents(lightTextDarkBg);
                this.lightRegions = new Dictionary<int,Region>(this._lightTextConnectedComponent.Regions);
                CountTruePixels(this.lightRegions, this.lightTextDarkBg);
                DeleteTreshRegions(this.lightRegions, image.Width, image.Height);
                GetTextRegions(this.lightRegions, out this.lightTextRegions, image.Width, image.Height);*/
            


           /*     this._darkTextLightBg = this._SWTFilter.MinIntensityDirectionImage();
                this._darkTextConnectedComponent.FindComponents(_darkTextLightBg);
                this._darkRegions = new Dictionary<int,Region>(this._darkTextConnectedComponent.Regions);
                CountTruePixels(this._darkRegions, this._darkTextLightBg);
                DeleteTreshRegions(this._darkRegions, image.Width, image.Height);
                GetTextRegions(this._darkRegions, out this._darkTextRegions, image.Width, image.Height);*/

                Thread lightTextThread = new Thread(new ParameterizedThreadStart(this.DetectLightTextOnDarkBackGroundThread));
                Thread darkTextThread = new Thread(new ParameterizedThreadStart(this.DetectDarkTextOnLightBackGroundThread));

                darkTextThread.Start(image);
                lightTextThread.Start(image);

                darkTextThread.Join();
                lightTextThread.Join();

                this._lightTextRegions.AddRange(this._darkTextRegions);
                image.TextRegions = new List<TextRegion>(this._lightTextRegions);
              //  textRegions = this._lightTextRegions;
             //   SetRegionColor(this._darkRegions, image, this._darkTextLightBg);            
                          
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

        private void FreeResources()
        {
            try
            {
                this._lightRegions.Clear();
                this._darkRegions.Clear();
                this._lightTextRegions.Clear();
                this._darkTextRegions.Clear();
                this._lightTextDarkBg = null;
                this._darkTextLightBg = null;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Выделение текста, который светлее фона (поточная функция)
        /// </summary>
        /// <param name="data">Данные</param>
        private void DetectLightTextOnDarkBackGroundThread(object data)
        {
            try
            {
                GreyImage image = (GreyImage) data;

                this._lightTextDarkBg = this._SWTFilter.MaxIntensityDirectionImage();
                this._lightTextConnectedComponent.FindComponents(_lightTextDarkBg);
                this._lightRegions = new Dictionary<int, Region>(this._lightTextConnectedComponent.Regions);
                CountTruePixels(this._lightRegions, this._lightTextDarkBg);
                DeleteTreshRegions(this._lightRegions, image.Width, image.Height);
            //    DeleteInnerRegions(this._lightRegions);
                GetTextRegions(this._lightRegions, out this._lightTextRegions, image.Width, image.Height);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Выделение текста, который темнее фона (поточная функция)
        /// </summary>
        /// <param name="data">Данные</param>
        private void DetectDarkTextOnLightBackGroundThread(object data)
        {
            try
            {
                GreyImage image = (GreyImage)data;

                this._darkTextLightBg = this._SWTFilter.MinIntensityDirectionImage();
                this._darkTextConnectedComponent.FindComponents(this._darkTextLightBg);
                this._darkRegions = new Dictionary<int, Region>(this._darkTextConnectedComponent.Regions);
                CountTruePixels(this._darkRegions, this._darkTextLightBg);
                DeleteTreshRegions(this._darkRegions, image.Width, image.Height);
            //    DeleteInnerRegions(this._darkRegions);
                GetTextRegions(this._darkRegions, out this._darkTextRegions, image.Width, image.Height);                
            }
            catch (Exception exception)
            {
                throw exception;
            }
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
                            double averageStrokeWidth = regions[pixelRegionNumber].AverageStrokeWidth;
                            double pixelStrokeWidth = image.Pixels[i, j].StrokeWidth.Width;
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


        private void DeleteInnerRegions(Dictionary<int, Region> regions)
        {
            try
            {
                List<KeyValuePair<int, Region>> lisOfRegions = regions.ToList();

                for (int i = 0; i < lisOfRegions.Count; i++)
                {
                    bool regionIndexIDeleted = false;
                    for (int j = 0; j < lisOfRegions.Count && !regionIndexIDeleted; j++)
                    {
                        if (lisOfRegions[j].Value.CenterPointIndexI > lisOfRegions[i].Value.MinBorderIndexI &&
                                lisOfRegions[j].Value.CenterPointIndexI < lisOfRegions[i].Value.MaxBorderIndexI &&
                                lisOfRegions[j].Value.CenterPointIndexJ > lisOfRegions[i].Value.MinBorderIndexJ &&
                                lisOfRegions[j].Value.MinBorderIndexJ < lisOfRegions[i].Value.MaxBorderIndexJ &&
                                lisOfRegions[j].Value.Width < lisOfRegions[i].Value.Width &&
                                lisOfRegions[j].Value.Height < lisOfRegions[i].Value.Height)
                        {
                            regions.Remove(lisOfRegions[j].Key);
                            j--;
                        }
                        else if (lisOfRegions[i].Value.CenterPointIndexI > lisOfRegions[j].Value.MinBorderIndexI &&
                            lisOfRegions[i].Value.CenterPointIndexI < lisOfRegions[j].Value.MaxBorderIndexI &&
                            lisOfRegions[i].Value.CenterPointIndexJ > lisOfRegions[j].Value.MinBorderIndexJ &&
                            lisOfRegions[i].Value.MinBorderIndexJ < lisOfRegions[j].Value.MaxBorderIndexJ &&
                            lisOfRegions[i].Value.Width < lisOfRegions[j].Value.Width &&
                            lisOfRegions[i].Value.Height < lisOfRegions[j].Value.Height)
                        {
                            regions.Remove(lisOfRegions[i].Key);
                            i--;
                            regionIndexIDeleted = true;
                        }
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
        private void DeleteTreshRegions(Dictionary<int, Region> regions, int imageWidth, int imageHeight)
        {
            try
            {
                foreach (var pair in regions.ToList())
                {
                    double percent = (double) pair.Value.TruePixelsNumber / (double) pair.Value.PixelsNumber;
                    double aspectRatio = ERROR_VALUE;
                    double diameterStrokeWidthRatio = ERROR_VALUE;
                    double bbPixelsNumberRation = ERROR_VALUE;
                    double imageRegionHeightRatio = ERROR_VALUE;
                    double imageRegionWidthRatio = ERROR_VALUE;

                    bool isZeroHeightOrWidth = pair.Value.Height == 0 || pair.Value.Width == 0;
                    if (!isZeroHeightOrWidth)
                    {
                        aspectRatio = pair.Value.Height > pair.Value.Width ? (double)pair.Value.Height / (double)pair.Value.Width :
                                                                             (double)pair.Value.Width / (double)pair.Value.Height;

                        diameterStrokeWidthRatio = (double)pair.Value.Diameter / (double)pair.Value.AverageStrokeWidth;

                        bbPixelsNumberRation = (double)pair.Value.PixelsNumber / (double)pair.Value.BoundingBoxPixelsNumber;

                        imageRegionHeightRatio = (double)imageHeight / (double)pair.Value.Height;
                        imageRegionWidthRatio = (double)imageWidth / (double) pair.Value.Width;
                    }

                //    double squareRatio = (double)pair.Value.PixelsNumber / pair.Value.Square;
                    if (percent * 100.0 < this._pixelsPercentTreshold  || pair.Value.PixelsNumber < this._minPixelsNumberInRegion ||
                         isZeroHeightOrWidth || (aspectRatio != ERROR_VALUE && aspectRatio > ASPECT_RATIO) ||
                         (diameterStrokeWidthRatio != ERROR_VALUE && diameterStrokeWidthRatio > DIAMETER_SW_RATIO) ||
                        (bbPixelsNumberRation != ERROR_VALUE && (bbPixelsNumberRation < BB_PIXELS_NUMBER_MIN_RATIO ||
                         bbPixelsNumberRation > BB_PIXELS_NUMBER_MAX_RATIO)) ||
                         (imageRegionHeightRatio != ERROR_VALUE && imageRegionHeightRatio > IMAGE_REGION_HEIGHT_RATIO_MAX || imageRegionHeightRatio < IMAGE_REGION_HEIGHT_RATIO_MIN) ||
                         (imageRegionWidthRatio != ERROR_VALUE && imageRegionWidthRatio > IMAGE_REGION_WIDTH_RATIO_MAX || imageRegionWidthRatio < IMAGE_REGION_WIDTH_RATIO_MIN))
                   
                        regions.Remove(pair.Key);
                }             
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Объединение символов в текстовые области
        /// </summary>
        /// <param name="regions">Области букв</param>
        private void GetTextRegions(Dictionary<int, Region> regions, out List<TextRegion> textRegions, int imageWidth, int imageHeight)
        {
            try
            {
                textRegions = new List<TextRegion>();

                 /*  foreach (var pair in regions)
                   {
                       TextRegion textRegion = new TextRegion()
                       {
                           MaxBorderIndexI = pair.Value.MaxBorderIndexI,
                           MaxBorderIndexJ = pair.Value.MaxBorderIndexJ,
                           MinBorderIndexI = pair.Value.MinBorderIndexI,
                           MinBorderIndexJ = pair.Value.MinBorderIndexJ
                       };

                       textRegions.Add(textRegion);
                   }*/

            /*    List<RegionPair> letterPairs = null;
                GetLetterPairs(regions, out letterPairs);
                DeleteBigLetterPairs(letterPairs, imageWidth, imageHeight);

                   for (int i = 0; i < letterPairs.Count; i++)
                   {
                       int n1 = letterPairs[i].FirstRegionNumber;
                       int n2 = letterPairs[i].SecondRegionNumber;

                       Region fR = regions[n1];
                       Region sR = regions[n2];

                       TextRegion textRegion = new TextRegion()
                       {
                           MaxBorderIndexI = Math.Max(fR.MaxBorderIndexI, sR.MaxBorderIndexI) ,
                           MaxBorderIndexJ = Math.Max(fR.MaxBorderIndexJ, sR.MaxBorderIndexJ),
                           MinBorderIndexI = Math.Min(fR.MinBorderIndexI, sR.MinBorderIndexI),
                           MinBorderIndexJ = Math.Min(fR.MinBorderIndexJ, sR.MinBorderIndexJ)
                       };

                       textRegions.Add(textRegion);
                   } */

                List<RegionPair> letterPairs = null;
                GetLetterPairs(regions, out letterPairs);
                DeleteBigLetterPairs(letterPairs, imageWidth, imageHeight);

                List<RegionChain> mergedBigRegions = null;

                DeleteRegionsThatDoesnotBelongToAnyRagionsPais(regions, letterPairs);

                MergePairsToBigRegions(regions, letterPairs, out mergedBigRegions);

                CreateTextRegions(regions, mergedBigRegions, out textRegions);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Удаление рагионов, котрые не принадлежат ни одной паре регионов
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="letterPairs">Пары регионов</param>
        private void DeleteRegionsThatDoesnotBelongToAnyRagionsPais(Dictionary<int, Region> regions, List<RegionPair> letterPairs)
        {
            try
            {
                foreach (var pair in regions.ToList())
                {
                    bool contains = false;
                    for (int i = 0; i < letterPairs.Count && !contains; i++)
                        if (letterPairs[i].FirstRegionNumber == pair.Key || letterPairs[i].SecondRegionNumber == pair.Key)
                            contains = true;
                    if (!contains)
                        regions.Remove(pair.Key);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Удаление слишком больших пар регионов
        /// </summary>
        /// <param name="letterPairs">Пары регионов</param>
        /// <param name="imageWidth">Ширина изображения</param>
        /// <param name="imageHeight">Высота изображения</param>
        private void DeleteBigLetterPairs(List<RegionPair> letterPairs, int imageWidth, int imageHeight)
        {
            try
            {
                for (int i = 0; i < letterPairs.Count; i++)
                {
                    double heightRatio = (double)imageHeight / (double)letterPairs[i].Height;
                    double widthHeight = (double)imageWidth / (double)letterPairs[i].Width;
                    if (heightRatio < IMAGE_REGION_HEIGHT_RATIO_MIN || widthHeight < IMAGE_REGION_WIDTH_RATIO_MIN)
                        letterPairs.RemoveAt(i);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Удаление дублирующихся пар регионов
        /// </summary>
        /// <param name="letterPairs">Список пар регионов</param>
     /*   private void DeleteDoublePairs(List<List<int>> letterPairs)
        {
            try
            {
                for (int i = 0; i < letterPairs.Count; i++)
                    for (int j = i + 1; j < letterPairs.Count; j++)
                    {
                        List<int> firstPair = letterPairs[i];
                        List<int> secondPair = letterPairs[j];
                        if (firstPair[0] == secondPair[1] && firstPair[1] == secondPair[0])
                            letterPairs.RemoveAt(j);
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }*/

        /// <summary>
        /// Формирует текстовые области по большим областям
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="regionsMerged">Большие области</param>
        /// <param name="textRegions">Текстовые регионы</param>
        private void CreateTextRegions(Dictionary<int, Region> regions, List<RegionChain> regionsMerged, out List<TextRegion> textRegions)
        {
            try
            {
                textRegions = new List<TextRegion>();
                int regionsMergedNumber = regionsMerged.Count;
                for (int i = 0; i < regionsMergedNumber; i++)
                {
                    List<int> currentRegionsNumbersList = regionsMerged[i].RegionsNumber;

                    if (currentRegionsNumbersList.Count > MIN_LETTERS_IN_TEXT_REGION)
                    {
                        TextRegion textRegion = CreateTextRegion(regions, currentRegionsNumbersList);
                        textRegions.Add(textRegion);
                    }
                }            
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Формировани списка больших областей
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="letterPairsNumbers">Пары номеров регионов</param>
        /// <param name="mergedRegions">Список списков номеров больших областей</param>
        private void MergePairsToBigRegions(Dictionary<int, Region> regions, List<RegionPair> letterPairsNumbers, out List<RegionChain> mergedRegions)
        {
            try
            {
                mergedRegions = new List<RegionChain>();
                foreach (var pair in regions)
                {
                    int currentRegionNumber = pair.Key;
   
                    int numberOfMergedRegion = GetMergedRegionNumberByRegionNumber(mergedRegions, currentRegionNumber);
                    if (numberOfMergedRegion == ERROR_VALUE)
                    {
                        RegionChain chain = new RegionChain();
                        chain.RegionsNumber = new List<int>();
                        chain.RegionsNumber.Add(currentRegionNumber);
                        mergedRegions.Add(chain);
                        /*List<int> mergeRegionsSubList = new List<int>();
                        mergeRegionsSubList.Add(currentRegionNumber);
                        mergedRegions.Add(mergeRegionsSubList);*/
                        numberOfMergedRegion = mergedRegions.Count - 1;
                    }

                    List<int> listOfPairs = GetNumbersOfPairsByRegionNumber(letterPairsNumbers, currentRegionNumber);
                    for (int i = 0; i < listOfPairs.Count; i++)
                    {
                        int currentPairNumber = listOfPairs[i];
                     
                        int differentPairElement = letterPairsNumbers[currentPairNumber].FirstRegionNumber != currentRegionNumber ? 
                                                   letterPairsNumbers[currentPairNumber].FirstRegionNumber : letterPairsNumbers[currentPairNumber].SecondRegionNumber;
                        int pairMergedRegionIndex = GetMergedRegionNumberByRegionNumber(mergedRegions, differentPairElement);

                        if (pairMergedRegionIndex == ERROR_VALUE)
                        {
                            mergedRegions[numberOfMergedRegion].RegionsNumber.Add(differentPairElement);
                            RecalculateAngleDirectionForChain(mergedRegions[numberOfMergedRegion], regions);
                        }
                        else
                        {
                            if (pairMergedRegionIndex != numberOfMergedRegion)
                                UniteTwoMergedRegions(mergedRegions, regions, numberOfMergedRegion, pairMergedRegionIndex);
                            numberOfMergedRegion = GetMergedRegionNumberByRegionNumber(mergedRegions, currentRegionNumber);
                        }             
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Находит номер "большой" области по номеру региона. Большая область может включать себя разное кол-во регионов
        /// </summary>
        /// <param name="mergedRegions">Номера регионов, составляющих большую область</param>
        /// <param name="regionNumber">Номер региона</param>
        /// <returns>Номер "большой" области, кот принадлежит регион с заданным номером</returns>
        private int GetMergedRegionNumberByRegionNumber(List<RegionChain> mergedRegions, int regionNumber)
        {
            try
            {
                int mergedRegionIndex = ERROR_VALUE;
                int mergedRegionsNumber = mergedRegions.Count;
                for (int i = 0; i < mergedRegionsNumber && mergedRegionIndex == ERROR_VALUE; i++)
                    if (mergedRegions[i].RegionsNumber.Contains(regionNumber))
                        mergedRegionIndex = i;

               return mergedRegionIndex;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Нахождение номеров пар регионов, кот принадлежат регион с заданным номером
        /// </summary>
        /// <param name="letterPairs">Номера пар регионов</param>
        /// <param name="regionNumber">Номер региона</param>
        /// <returns>Номера пар регионов, кот принадлежит заданный регион</returns>
        private List<int> GetNumbersOfPairsByRegionNumber(List<RegionPair> letterPairs, int regionNumber)
        {
            try
            {
                List<int> listOfPairs = new List<int>();
                int pairsNumber = letterPairs.Count;
                for (int i = 0; i < pairsNumber; i++)
                    if (letterPairs[i].FirstRegionNumber == regionNumber || letterPairs[i].SecondRegionNumber == regionNumber)
                        listOfPairs.Add(i);
                return listOfPairs;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Объединение двух больших областей
        /// </summary>
        /// <param name="mergedRegions">Больште области</param>
        /// <param name="firstRegionIndexToUnit">Номер первой большой области для объединения</param>
        /// <param name="secondRegionIndexToUnit">Номер второй большой области для объединения</param>
        private void UniteTwoMergedRegions(List<RegionChain> mergedRegions, Dictionary<int, Region> regions, int firstRegionIndexToUnit, int secondRegionIndexToUnit)
        {
            try
            {
                RegionChain firstChain = mergedRegions[firstRegionIndexToUnit];
                int firstChainNumber = firstChain.RegionsNumber.Count;

                RegionChain secondChain = mergedRegions[secondRegionIndexToUnit];
                int secondChainNumber = secondChain.RegionsNumber.Count;

             /*   if (Math.Abs(firstChain.AngleDirection - secondChain.AngleDirection) < 15  && (firstChain.RegionsNumber[0] == secondChain.RegionsNumber[0] ||
                    firstChain.RegionsNumber[0] == secondChain.RegionsNumber[secondChainNumber - 1] ||
                    firstChain.RegionsNumber[firstChainNumber - 1] == secondChain.RegionsNumber[0] ||
                    firstChain.RegionsNumber[firstChainNumber - 1] == secondChain.RegionsNumber[secondChainNumber - 1]))
                {*/
                    for (int i = 0; i < secondChainNumber; i++)
                    {
                        int indexFromRegion = secondChain.RegionsNumber[i];
                        if (!mergedRegions[firstRegionIndexToUnit].RegionsNumber.Contains(indexFromRegion))
                            mergedRegions[firstRegionIndexToUnit].RegionsNumber.Add(indexFromRegion);
                    }
                    RecalculateAngleDirectionForChain(mergedRegions[firstRegionIndexToUnit], regions);
                    mergedRegions.RemoveAt(secondRegionIndexToUnit);
               // }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void RecalculateAngleDirectionForChain(RegionChain chain, Dictionary<int, Region> regions)
        {
            try
            {
                int firstRegionNumber = chain.RegionsNumber[0];
                int lastRegionNumber = chain.RegionsNumber[chain.RegionsNumber.Count - 1];

                int deltaI = regions[firstRegionNumber].CenterPointIndexI - regions[lastRegionNumber].CenterPointIndexI;
                int deltaJ = regions[firstRegionNumber].CenterPointIndexJ - regions[lastRegionNumber].CenterPointIndexJ;

                chain.AngleDirection = Math.Atan((double)deltaI / (double)deltaJ) * (180 / Math.PI);             
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Составляет 1 текстовый регион по номерам обалстей, входящим в него
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="regionsNumbersList">Номера областей будущего текстового региона</param>
        /// <return>Текстовый регион</return>
        private TextRegion CreateTextRegion(Dictionary<int, Region> regions, List<int> regionsNumbersList)
        {
            try
            {
                int firstRegionNumber = regionsNumbersList[0];
                Region firstRegion = regions[firstRegionNumber];

                int regionlistNumber = regionsNumbersList.Count;

                int minI = firstRegion.MinBorderIndexI;
                int minJ = firstRegion.MinBorderIndexJ;
                int maxI = firstRegion.MaxBorderIndexI;
                int maxJ = firstRegion.MaxBorderIndexJ;

                for (int i = 1; i < regionlistNumber; i++)
                {
                    int currentRegionNumber = regionsNumbersList[i];
                    Region currentRegion = regions[currentRegionNumber];

                    if (currentRegion.MinBorderIndexI < minI)
                        minI = currentRegion.MinBorderIndexI;
                    if (currentRegion.MinBorderIndexJ < minJ)
                        minJ = currentRegion.MinBorderIndexJ;
                    if (currentRegion.MaxBorderIndexI > maxI)
                        maxI = currentRegion.MaxBorderIndexI;
                    if (currentRegion.MaxBorderIndexJ > maxJ)
                        maxJ = currentRegion.MaxBorderIndexJ;
                }
                return new TextRegion() { MinBorderIndexI = minI, MinBorderIndexJ = minJ, MaxBorderIndexI = maxI, MaxBorderIndexJ = maxJ };                           
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        

        /// <summary>
        /// Объединяет буквы в пары номеров областей, используя эвристические правила
        /// </summary>
        /// <param name="regions">Области символов</param>
        /// <param name="letterPairs">Пары номеров символов</param>
        private void GetLetterPairs(Dictionary<int, Region> regions, out List<RegionPair> letterPairsNumbers)
        {
            try
            {
                List<KeyValuePair<int, Region> > listOfRegions = regions.ToList();
                letterPairsNumbers = new List<RegionPair>();

                int regionsNumber = listOfRegions.Count;

                for (int i = 0; i < regionsNumber; i++)
                    for (int j = i + 1; j < regionsNumber; j++)
                    {
                        double firstRegionAverageSW = listOfRegions[i].Value.AverageStrokeWidth;
                        double secondRegionAverageSW = listOfRegions[j].Value.AverageStrokeWidth;

                        int firstRegionHeight = listOfRegions[i].Value.Height;
                        int secondRegionHeight = listOfRegions[j].Value.Height;

                        int firstRegionWidth = listOfRegions[i].Value.Width;
                        int secondRegionWidth = listOfRegions[j].Value.Width;

                        int firstRegionAverageIntensity = listOfRegions[i].Value.AverageIntensity;
                        int secondRegionAverageIntensity = listOfRegions[i].Value.AverageIntensity;

                        int maxRegionWidth = firstRegionWidth > secondRegionWidth ? firstRegionWidth : secondRegionWidth;

                        double swRatio = firstRegionAverageSW > secondRegionAverageSW ? (double)firstRegionAverageSW / (double)secondRegionAverageSW :
                                                                                        (double)secondRegionAverageSW / (double)firstRegionAverageSW;
                        double heightRatio = firstRegionHeight > secondRegionHeight ? (double)firstRegionHeight / (double)secondRegionHeight :
                                                                                      (double)secondRegionHeight / (double)firstRegionHeight;
                        double widthRatio = firstRegionWidth > secondRegionWidth ? (double)firstRegionWidth / (double)secondRegionWidth :
                                                                                   (double)secondRegionWidth / (double)firstRegionWidth;

                        double distance = GetDistanceBetweenRegions(listOfRegions[i].Value.CenterPointIndexI, listOfRegions[i].Value.CenterPointIndexJ,
                            listOfRegions[j].Value.CenterPointIndexI, listOfRegions[j].Value.CenterPointIndexJ);
                        double ditanseWidthRatio = (double)distance / (double) maxRegionWidth;

                        if (heightRatio < HEIGHT_RATIO && swRatio < STROKE_WIDTH_RATIO && ditanseWidthRatio < WIDTH_DISTANCE_RATIO &&
                            widthRatio < WIDTH_RATIO && Math.Abs(firstRegionAverageIntensity - secondRegionAverageIntensity) < INTENSITY_DIFFERENCE)
                        {
                           // List<int> pair = new List<int>();
                            RegionPair pair = new RegionPair();
                            pair.FirstRegionNumber = listOfRegions[i].Value.Number;
                            pair.SecondRegionNumber = listOfRegions[j].Value.Number;

                            int deltaI = listOfRegions[i].Value.CenterPointIndexI - listOfRegions[j].Value.CenterPointIndexI;
                            int deltaJ = listOfRegions[i].Value.CenterPointIndexJ - listOfRegions[j].Value.CenterPointIndexJ;

                            pair.AngleDirection = Math.Atan((double)deltaI / (double)deltaJ) * (180 / Math.PI);

                            pair.MinBorderIndexI = Math.Min(listOfRegions[i].Value.MinBorderIndexI, listOfRegions[j].Value.MinBorderIndexI);
                            pair.MinBorderIndexJ = Math.Min(listOfRegions[i].Value.MinBorderIndexJ, listOfRegions[j].Value.MinBorderIndexJ);

                            pair.MaxBorderIndexI = Math.Max(listOfRegions[i].Value.MaxBorderIndexI, listOfRegions[j].Value.MaxBorderIndexI);
                            pair.MaxBorderIndexJ = Math.Max(listOfRegions[i].Value.MaxBorderIndexJ, listOfRegions[j].Value.MaxBorderIndexJ);

                            pair.Height = pair.MaxBorderIndexI - pair.MinBorderIndexI;
                            pair.Width = pair.MaxBorderIndexJ - pair.MinBorderIndexJ;
                         //   pair.Add(listOfRegions[i].Value.Number);
                          //  pair.Add(listOfRegions[j].Value.Number);
                            letterPairsNumbers.Add(pair);
                        }
                    }        
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Нахождение расстояния мужду двумя регионами
        /// </summary>
        /// <param name="firstRegionCenterI">Индекс центра первого региона по строке</param>
        /// <param name="firstRegionCenterJ">Индекс центра первого региона по столбцу</param>
        /// <param name="secondRegionCenterI">Индекс центра второго региона по строке</param>
        /// <param name="secondRegionCenterJ">Индекс центра второго региона по столбцу</param>
        /// <returns></returns>
        private double GetDistanceBetweenRegions(int firstRegionCenterI, int firstRegionCenterJ, int secondRegionCenterI, int secondRegionCenterJ)
        {
            return Math.Sqrt((double) ((firstRegionCenterI - secondRegionCenterI) * (firstRegionCenterI - secondRegionCenterI) +
                (firstRegionCenterJ - secondRegionCenterJ) * (firstRegionCenterJ - secondRegionCenterJ)));
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
