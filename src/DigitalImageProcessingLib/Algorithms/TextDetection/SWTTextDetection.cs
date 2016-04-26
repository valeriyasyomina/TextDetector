using DigitalImageProcessingLib.Algorithms.ConnectedComponent;
using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.Filters;
using DigitalImageProcessingLib.Filters.FilterType;
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

        private SWTFilterSmart _SWTFilter = null;

        private double _varienceAverageSWRation = 0.0;
        private double _aspectRatio = 0.0;        
        private double _diamiterSWRatio = 0.0;
        private double _bbPixelsNumberMinRatio = 0.0;
        private double _bbPixelsNumberMaxRatio = 0.0;
        private double _imageRegionHeightRationMin = 0.0;
        private double _imageRegionWidthRatioMin = 0.0;

        private double _pairsHeightRatio = 0.0;
        private double _pairsIntensityRatio = 0.0;
        private double _pairsSWRatio = 0.0;
        private double _pairsWidthDistanceSqrRatio = 0.0;
        private double _pairsOccupationRatio = 0.0;

        private int _minLettersNumberInTextRegion = 0;
        private bool _mergeByDirectionAndChainEnds = false;       

        private static double STRICTNESS = Math.PI / 6.0;

        private GradientFilter _gradientFilter = null;
        private SmoothingFilter _smoothingFilter = null;

        private GreyImage _darkTextLightBg = null;
        private GreyImage _lightTextDarkBg = null;

        private List<TextRegion> _lightTextRegions = null;
        private List<TextRegion> _darkTextRegions = null;

        private Dictionary<int, Region> _darkRegions = null;
        private Dictionary<int, Region> _lightRegions = null;

        private static int ERROR_VALUE = -1;

        public SWTTextDetection(IEdgeDetection edgeDetector, SmoothingFilter smoothingFilter, GradientFilter gradientFiler, double varienceAverageSWRation, double aspectRatio = 5.0,
            double diamiterSWRatio = 10, double bbPixelsNumberMinRatio = 1.5, double bbPixelsNumberMaxRatio = 25.0,
            double imageRegionHeightRationMin = 1.5, double imageRegionWidthRatioMin = 1.5, double pairsHeightRatio = 2.0,
            double pairsIntensityRatio = 1.0, double pairsSWRatio = 1.5, double pairsWidthDistanceSqrRatio = 9.0,
            double pairsOccupationRatio = 2.0, int minLettersNumberInTextRegion = 2, bool mergeByDirectionAndChainEnds = false)                       
        {
            if (edgeDetector == null)
                throw new ArgumentNullException("Null edgeDetector in ctor");           
            if (gradientFiler == null)
                throw new ArgumentNullException("Null gradientFiler in ctor");
            if (smoothingFilter == null)
                throw new ArgumentNullException("Null smoothingFilter in ctor");
            
            this._edgeDetector = edgeDetector;         
            this._gradientFilter = gradientFiler;
            this._smoothingFilter = smoothingFilter;

            this._lightTextConnectedComponent = new TwoPassCCAlgorithm(DigitalImageProcessingLib.Interface.UnifyingFeature.StrokeWidth,
                                                            DigitalImageProcessingLib.Interface.ConnectivityType.EightConnectedRegion);
            this._darkTextConnectedComponent = new TwoPassCCAlgorithm(DigitalImageProcessingLib.Interface.UnifyingFeature.StrokeWidth,
                                                            DigitalImageProcessingLib.Interface.ConnectivityType.EightConnectedRegion);

            this._varienceAverageSWRation = varienceAverageSWRation;
            this._aspectRatio = aspectRatio;
            this._diamiterSWRatio = diamiterSWRatio;
            this._bbPixelsNumberMinRatio = bbPixelsNumberMinRatio;
            this._bbPixelsNumberMaxRatio = bbPixelsNumberMaxRatio;
            this._imageRegionHeightRationMin = imageRegionHeightRationMin;
            this._imageRegionWidthRatioMin = imageRegionWidthRatioMin;

            this._pairsHeightRatio = pairsHeightRatio;
            this._pairsIntensityRatio = pairsIntensityRatio;
            this._pairsSWRatio = pairsSWRatio;
            this._pairsWidthDistanceSqrRatio = pairsWidthDistanceSqrRatio;
            this._pairsOccupationRatio = pairsOccupationRatio;

            this._minLettersNumberInTextRegion = minLettersNumberInTextRegion;
            this._mergeByDirectionAndChainEnds = mergeByDirectionAndChainEnds;          

            this._lightRegions = new Dictionary<int, Region>();
            this._darkRegions = new Dictionary<int, Region>();
            this._lightTextRegions = new List<TextRegion>();
            this._darkTextRegions = new List<TextRegion>();
        }

        /// <summary>
        /// Выделение текста на изображении (многопоточный метод)
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="threadsNumber">Количество кадров</param>
        public void DetectText(GreyImage image, int threadsNumber)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in DetectText");                

                GreyImage imageCanny = this._edgeDetector.Detect(image, threadsNumber);
                GreyImage smoothedImage = null;
                if (this._edgeDetector.SmoothingFilter.GetType() != this._smoothingFilter.GetType())                
                    smoothedImage = this._smoothingFilter.Apply(image, threadsNumber);                
                else
                    smoothedImage = this._edgeDetector.GreySmoothedImage();
                this._gradientFilter.Apply(smoothedImage);   // threadsNumber

              //  GreyImage gradienMapX = this._gradientFilter.GradientXMap();
             //   GreyImage gradienMapY = this._gradientFilter.GradientYMap();

                this._SWTFilter = new SWTFilterSmart(smoothedImage, smoothedImage);
                this._SWTFilter.Apply(imageCanny);              

                Thread lightTextThread = new Thread(new ParameterizedThreadStart(this.DetectLightTextOnDarkBackGroundThread));
                Thread darkTextThread = new Thread(new ParameterizedThreadStart(this.DetectDarkTextOnLightBackGroundThread));

                darkTextThread.Start(image);
                lightTextThread.Start(image);

                darkTextThread.Join();
                lightTextThread.Join();

                this._lightTextRegions.AddRange(this._darkTextRegions);
                image.TextRegions = new List<TextRegion>(this._lightTextRegions);

                FreeResources();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void DetectText(GreyImage image)
        {
            throw new NotImplementedException();
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
                CalculateStrokeWidthVariance(this._lightRegions, this._lightTextDarkBg);          
                DeleteTreshRegions(this._lightRegions, image.Width, image.Height);           
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
                CalculateStrokeWidthVariance(this._darkRegions, this._darkTextLightBg);             
                DeleteTreshRegions(this._darkRegions, image.Width, image.Height);        
                GetTextRegions(this._darkRegions, out this._darkTextRegions, image.Width, image.Height);                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

      /*  private void CalculateRegionsThatContanMoreThanTwoOther(Dictionary<int, Region> regions)
        {
            try
            {
                foreach (var firstPair in regions.ToList())
                {
                    foreach (var secondPair in regions)
                    {
                        if (firstPair.Key != secondPair.Key)
                            if (secondPair.Value.CenterPointIndexI > firstPair.Value.MinBorderIndexI && secondPair.Value.CenterPointIndexI <
                                firstPair.Value.MaxBorderIndexI && secondPair.Value.CenterPointIndexJ > firstPair.Value.MinBorderIndexJ &&
                                secondPair.Value.CenterPointIndexJ < firstPair.Value.MaxBorderIndexJ)
                                firstPair.Value.OtherRegionsNumberInIt++;
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }*/

        /// <summary>
        /// Вычисляет количество пикселей в каждом регионе, значение ширины штриха которых +-= this._strokeWidthDelta
        /// </summary>
        /// <param name="regions">Области</param>
        /// <param name="image">Серое изображение</param>
       /* private void CountTruePixels(Dictionary<int, Region> regions, GreyImage image)
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
        }*/

        /// <summary>
        /// Вычисление суммы отклонения ширины штриха
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="image">Изображение</param>
        private void CalculateVarianceSumm(Dictionary<int, Region> regions, GreyImage image)
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
                            regions[pixelRegionNumber].VarianceSumm += Math.Pow(pixelStrokeWidth - averageStrokeWidth, 2.0);
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Вычисление отклонения ширины штриха для регионов
        /// </summary>
        /// <param name="regions">Регионы</param>
        private void CalculateStrokeWidthVariance(Dictionary<int, Region> regions)
        {
            try
            {
                foreach (var pair in regions)
                {
                    if (pair.Value.PixelsNumber != 0)
                        pair.Value.StrokeWidthVarience = (double) pair.Value.VarianceSumm / (double)pair.Value.PixelsNumber;                     
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисление отклонения ширины штриха для регионов
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="image">Изображение</param>
        private void CalculateStrokeWidthVariance(Dictionary<int, Region> regions, GreyImage image)
        {
            try
            {
                CalculateVarianceSumm(regions, image);
                CalculateStrokeWidthVariance(regions);                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


     /*   private void DeleteInnerRegions(Dictionary<int, Region> regions)
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
        }*/

        /// <summary>
        /// Удаление регионов, не уловлетворяющих эвристикам
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="imageWidth">Ширина изображения</param>
        /// <param name="imageHeight">Высота изображения</param>
        private void DeleteTreshRegions(Dictionary<int, Region> regions, int imageWidth, int imageHeight)
        {
            try
            {
                foreach (var pair in regions.ToList())
                {
                    double aspectRatio = ERROR_VALUE;
                    double diameterStrokeWidthRatio = ERROR_VALUE;
                    double bbPixelsNumberRation = ERROR_VALUE;
                    double imageRegionHeightRatio = ERROR_VALUE;
                    double imageRegionWidthRatio = ERROR_VALUE;
                    double varienceAverageSWRatio = ERROR_VALUE;

                    bool isZeroHeightOrWidth = pair.Value.Height == 0 || pair.Value.Width == 0;
                    if (!isZeroHeightOrWidth)
                    {
                        aspectRatio = pair.Value.Height > pair.Value.Width ? (double)pair.Value.Height / (double)pair.Value.Width :
                                                                             (double)pair.Value.Width / (double)pair.Value.Height;
                        diameterStrokeWidthRatio = (double)pair.Value.Diameter / (double)pair.Value.AverageStrokeWidth;
                        bbPixelsNumberRation = (double)pair.Value.PixelsNumber / (double)pair.Value.BoundingBoxPixelsNumber;
                        imageRegionHeightRatio = (double)imageHeight / (double)pair.Value.Height;
                        imageRegionWidthRatio = (double)imageWidth / (double) pair.Value.Width;
                        varienceAverageSWRatio = pair.Value.StrokeWidthVarience / pair.Value.AverageStrokeWidth;
                    }
                    if (isZeroHeightOrWidth || varienceAverageSWRatio > this._varienceAverageSWRation || aspectRatio > this._aspectRatio ||
                         diameterStrokeWidthRatio > this._diamiterSWRatio || bbPixelsNumberRation < this._bbPixelsNumberMinRatio ||
                         bbPixelsNumberRation > this._bbPixelsNumberMaxRatio || imageRegionHeightRatio < this._imageRegionHeightRationMin ||
                         imageRegionWidthRatio < this._imageRegionWidthRatioMin)                   
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

             /*    foreach (var pair in regions)
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

               /* List<RegionChain> letterPairs = null;
                GetLetterPairs(regions, out letterPairs);
                DeleteBigLetterPairs(letterPairs, imageWidth, imageHeight);

                   for (int i = 0; i < letterPairs.Count; i++)
                   {
                       int n1 = letterPairs[i].FirstRegionNumber;
                       int n2 = letterPairs[i].LastRegionNumber;

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

                List<RegionChain> letterPairs = null;
                GetLetterPairs(regions, out letterPairs);
                DeleteBigLetterPairs(letterPairs, imageWidth, imageHeight);

                List<RegionChain> mergedBigRegions = null;
                DeleteRegionsThatDoesnotBelongToAnyRagionsPais(regions, letterPairs);
                if (this._mergeByDirectionAndChainEnds)
                    MergePairsToBigRegionsByDirectionAndChainEnds(regions, letterPairs, out mergedBigRegions);
                else
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
        private void DeleteRegionsThatDoesnotBelongToAnyRagionsPais(Dictionary<int, Region> regions, List<RegionChain> letterPairs)
        {
            try
            {
                foreach (var pair in regions.ToList())
                {
                    bool contains = false;
                    for (int i = 0; i < letterPairs.Count && !contains; i++)
                        if (letterPairs[i].FirstRegionNumber == pair.Key || letterPairs[i].LastRegionNumber == pair.Key)
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
        private void DeleteBigLetterPairs(List<RegionChain> letterPairs, int imageWidth, int imageHeight)
        {
            try
            {
                for (int i = 0; i < letterPairs.Count; i++)
                {
                    double heightRatio = (double)imageHeight / (double)letterPairs[i].Height;
                    double widthHeight = (double)imageWidth / (double)letterPairs[i].Width;
                    if (heightRatio < this._imageRegionHeightRationMin || widthHeight < this._imageRegionWidthRatioMin)
                        letterPairs.RemoveAt(i);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }      

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

                    if (currentRegionsNumbersList.Count > this._minLettersNumberInTextRegion)
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
        /// Формирование списка больших областей
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="letterPairsNumbers">Пары номеров регионов</param>
        /// <param name="mergedRegions">Список списков номеров больших областей</param>
        private void MergePairsToBigRegions(Dictionary<int, Region> regions, List<RegionChain> letterPairsNumbers, out List<RegionChain> mergedRegions)
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
                        numberOfMergedRegion = mergedRegions.Count - 1;
                    }

                    List<int> listOfPairs = GetNumbersOfPairsByRegionNumber(letterPairsNumbers, currentRegionNumber);
                    for (int i = 0; i < listOfPairs.Count; i++)
                    {
                        int currentPairNumber = listOfPairs[i];
                     
                        int differentPairElement = letterPairsNumbers[currentPairNumber].FirstRegionNumber != currentRegionNumber ? 
                                                   letterPairsNumbers[currentPairNumber].FirstRegionNumber : letterPairsNumbers[currentPairNumber].LastRegionNumber;
                        int pairMergedRegionIndex = GetMergedRegionNumberByRegionNumber(mergedRegions, differentPairElement);

                        if (pairMergedRegionIndex == ERROR_VALUE)
                        {
                            mergedRegions[numberOfMergedRegion].RegionsNumber.Add(differentPairElement);
                            //RecalculateAngleDirectionForChain(mergedRegions[numberOfMergedRegion], regions); РАСКОММЕНТИТь
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

        private void MergePairsToBigRegionsByDirectionAndChainEnds(Dictionary<int, Region> regions, List<RegionChain> letterPairsNumbers, out List<RegionChain> mergedRegions)
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
                        chain.FirstRegionNumber = currentRegionNumber;
                        chain.LastRegionNumber = currentRegionNumber;
                        mergedRegions.Add(chain);
                        numberOfMergedRegion = mergedRegions.Count - 1;
                    }

                    List<int> listOfPairs = GetNumbersOfPairsByRegionNumber(letterPairsNumbers, currentRegionNumber);
                    for (int i = 0; i < listOfPairs.Count; i++)
                    {
                        int currentPairNumber = listOfPairs[i];
                        int differentPairElement = letterPairsNumbers[currentPairNumber].FirstRegionNumber != currentRegionNumber ?
                                                   letterPairsNumbers[currentPairNumber].FirstRegionNumber : letterPairsNumbers[currentPairNumber].LastRegionNumber;
                        int pairMergedRegionIndex = GetMergedRegionNumberByRegionNumber(mergedRegions, differentPairElement);

                        if (pairMergedRegionIndex == ERROR_VALUE)
                        {
                            //-------------- ПРОВЕРКА НАПРАВЛЕНИЯ
                            int deltaI = regions[currentRegionNumber].CenterPointIndexI - regions[differentPairElement].CenterPointIndexI;
                            int deltaJ = regions[currentRegionNumber].CenterPointIndexJ - regions[differentPairElement].CenterPointIndexJ;
                            double PairAngleDirection = Math.Atan((double)deltaI / (double)deltaJ) * (180 / Math.PI);
                            //---------------

                            // ЕСЛИ НАПРАВЛЕНИЕ СОВПАЛО, ТО ДОБАВЛЯЕМ РЕГИОН
                            if (mergedRegions[numberOfMergedRegion].AngleDirection == 1000 || Math.Abs(mergedRegions[numberOfMergedRegion].AngleDirection - PairAngleDirection) < 30)
                            {
                                // mergedRegions[numberOfMergedRegion].RegionsNumber.Add(differentPairElement);

                                int addingElementI = regions[differentPairElement].CenterPointIndexI;
                                int addingElementJ = regions[differentPairElement].CenterPointIndexJ;

                                int chainFirstElementI = regions[mergedRegions[numberOfMergedRegion].FirstRegionNumber].CenterPointIndexI;
                                int chainFirstElementJ = regions[mergedRegions[numberOfMergedRegion].FirstRegionNumber].CenterPointIndexJ;

                                int chainLastElementI = regions[mergedRegions[numberOfMergedRegion].LastRegionNumber].CenterPointIndexI;
                                int chainLastElementJ = regions[mergedRegions[numberOfMergedRegion].LastRegionNumber].CenterPointIndexJ;

                                int scalar = GetScalyar(addingElementI - chainFirstElementI, addingElementJ - chainFirstElementJ, addingElementI - chainLastElementI, addingElementJ - chainLastElementJ);

                                if (mergedRegions[numberOfMergedRegion].FirstRegionNumber == currentRegionNumber ||
                                    mergedRegions[numberOfMergedRegion].LastRegionNumber == currentRegionNumber)
                                {
                                    mergedRegions[numberOfMergedRegion].RegionsNumber.Add(differentPairElement);
                                    if (scalar > 0)
                                    {
                                        //   mergedRegions[numberOfMergedRegion].FirstRegionNumber = differentPairElement;
                                        double dist1 = GetDistanceSqrBetweenRegions(regions[differentPairElement].CenterPointIndexI, regions[differentPairElement].CenterPointIndexJ,
                                            regions[mergedRegions[numberOfMergedRegion].FirstRegionNumber].CenterPointIndexI,
                                            regions[mergedRegions[numberOfMergedRegion].FirstRegionNumber].CenterPointIndexJ);

                                        double dist2 = GetDistanceSqrBetweenRegions(regions[differentPairElement].CenterPointIndexI, regions[differentPairElement].CenterPointIndexJ,
                                            regions[mergedRegions[numberOfMergedRegion].LastRegionNumber].CenterPointIndexI,
                                            regions[mergedRegions[numberOfMergedRegion].LastRegionNumber].CenterPointIndexJ);
                                        if (dist1 <= dist2)
                                            mergedRegions[numberOfMergedRegion].FirstRegionNumber = differentPairElement;
                                        else
                                            mergedRegions[numberOfMergedRegion].LastRegionNumber = differentPairElement;
                                    }
                                    RecalculateAngleDirectionForChain(mergedRegions[numberOfMergedRegion], regions);
                                }
                                /*    else if (mergedRegions[numberOfMergedRegion].LastRegionNumber == currentRegionNumber)
                                    {
                                        mergedRegions[numberOfMergedRegion].RegionsNumber.Add(differentPairElement);
                                        if (scalar > 0)
                                            mergedRegions[numberOfMergedRegion].LastRegionNumber = differentPairElement;
                                        RecalculateAngleDirectionForChain(mergedRegions[numberOfMergedRegion], regions);
                                    }*/

                                //RecalculateAngleDirectionForChain(mergedRegions[numberOfMergedRegion], regions);
                            }
                            /*       else // В ПРОТИВНОМ СЛУЧАЕ, ЭТОТ РЕГИОН ВЫДЕЛЯЕТСЯ В ОТДЕЛЬНУЮ ОБЛАСТЬ
                                   {
                                       RegionChain chain = new RegionChain();
                                       chain.RegionsNumber = new List<int>();
                                       chain.RegionsNumber.Add(currentRegionNumber);
                                       chain.RegionsNumber.Add(differentPairElement);
                                       chain.FirstRegionNumber = currentRegionNumber;
                                       chain.LastRegionNumber = differentPairElement;

                                       RecalculateAngleDirectionForChain(chain, regions);
                                       mergedRegions.Add(chain);
                                   }*/
                        }
                        else
                        {
                            if (pairMergedRegionIndex != numberOfMergedRegion)
                            {
                                // ЕСЛИ ВСТРЕТИЛИСЬ ДВЕ ОБЛАСТИ ПО 1 РЕГИОНУ, ТО ПРОСТО СЛИВАЕМ ИХ
                                if (mergedRegions[numberOfMergedRegion].RegionsNumber.Count == 1 && mergedRegions[pairMergedRegionIndex].RegionsNumber.Count == 1)
                                {
                                    mergedRegions[numberOfMergedRegion].RegionsNumber.Add(differentPairElement);
                                    mergedRegions[numberOfMergedRegion].LastRegionNumber = differentPairElement;
                                    RecalculateAngleDirectionForChain(mergedRegions[numberOfMergedRegion], regions);
                                    mergedRegions.RemoveAt(pairMergedRegionIndex);
                                }
                                else // ИНАЧЕ ПРОВЕРКА НА НАПРАВЛЕНИЕ, КАК БЫЛО ВЫШЕ
                                {
                                    //------------ ПРОВЕРКА НАПРАВЛЕНИЯ
                                    int deltaI = regions[currentRegionNumber].CenterPointIndexI - regions[differentPairElement].CenterPointIndexI;
                                    int deltaJ = regions[currentRegionNumber].CenterPointIndexJ - regions[differentPairElement].CenterPointIndexJ;
                                    double PairAngleDirection = Math.Atan((double)deltaI / (double)deltaJ) * (180 / Math.PI);
                                    //---------------

                                    // ЕСЛИ В ПЕРВОЙ ОБЛАСТИ ОДИН РЕГИОН, ТО ПРОБУЕМ ДОБАВИТЬ ЭТОТ РЕГИОН КО ВТОРОЙ ОБЛАСТИ
                                    if (mergedRegions[numberOfMergedRegion].RegionsNumber.Count == 1)
                                    {
                                        if (Math.Abs(mergedRegions[pairMergedRegionIndex].AngleDirection - PairAngleDirection) < 30)
                                        {
                                            mergedRegions[pairMergedRegionIndex].RegionsNumber.Add(currentRegionNumber);

                                            if (mergedRegions[pairMergedRegionIndex].FirstRegionNumber == differentPairElement)
                                                mergedRegions[pairMergedRegionIndex].FirstRegionNumber = currentRegionNumber;
                                            else if (mergedRegions[pairMergedRegionIndex].LastRegionNumber == differentPairElement)
                                                mergedRegions[pairMergedRegionIndex].LastRegionNumber = currentRegionNumber;

                                            RecalculateAngleDirectionForChain(mergedRegions[pairMergedRegionIndex], regions);
                                            mergedRegions.RemoveAt(numberOfMergedRegion);
                                        }
                                    }
                                    else // ЕСЛИ ВО ВТОРОЙ ОБЛАСТИ ОДИН РЕГИОН, ТО ПРОБУЕМ ДОБАВИТЬ ЭТОТ РЕГИОН К ПЕРВОЙ ОБЛАСТИ
                                        if (mergedRegions[pairMergedRegionIndex].RegionsNumber.Count == 1)
                                        {
                                            if (Math.Abs(mergedRegions[numberOfMergedRegion].AngleDirection - PairAngleDirection) < 30)
                                            {
                                                mergedRegions[numberOfMergedRegion].RegionsNumber.Add(differentPairElement);


                                                if (mergedRegions[numberOfMergedRegion].FirstRegionNumber == currentRegionNumber)
                                                    mergedRegions[numberOfMergedRegion].FirstRegionNumber = differentPairElement;
                                                else if (mergedRegions[numberOfMergedRegion].LastRegionNumber == currentRegionNumber)
                                                    mergedRegions[numberOfMergedRegion].LastRegionNumber = differentPairElement;


                                                RecalculateAngleDirectionForChain(mergedRegions[numberOfMergedRegion], regions);
                                                mergedRegions.RemoveAt(pairMergedRegionIndex);
                                            }
                                        }
                                        else // ЕСЛИ В ДВУХ ОБЛАСТЯХ МНОГО РЕГИОНОВ, ПЫТАЕМСЯ ИХ СЛИТЬ, КАК И РАНЬШЕ
                                        {
                                            //-------------- ПРОВЕРКА НАПРАВЛЕНИЯ
                                            deltaI = regions[currentRegionNumber].CenterPointIndexI - regions[differentPairElement].CenterPointIndexI;
                                            deltaJ = regions[currentRegionNumber].CenterPointIndexJ - regions[differentPairElement].CenterPointIndexJ;
                                            PairAngleDirection = Math.Atan((double)deltaI / (double)deltaJ) * (180 / Math.PI);
                                            //---------------

                                            // ЕСЛИ НАПРАВЛЕНИЕ СОВПАЛО, ТО ДОБАВЛЯЕМ РЕГИОН
                                            if (mergedRegions[numberOfMergedRegion].AngleDirection == 1000 || Math.Abs(mergedRegions[numberOfMergedRegion].AngleDirection - PairAngleDirection) < 30)
                                            {
                                                int addingElementI = regions[differentPairElement].CenterPointIndexI;
                                                int addingElementJ = regions[differentPairElement].CenterPointIndexJ;

                                                int chainFirstElementI = regions[mergedRegions[numberOfMergedRegion].FirstRegionNumber].CenterPointIndexI;
                                                int chainFirstElementJ = regions[mergedRegions[numberOfMergedRegion].FirstRegionNumber].CenterPointIndexJ;

                                                int chainLastElementI = regions[mergedRegions[numberOfMergedRegion].LastRegionNumber].CenterPointIndexI;
                                                int chainLastElementJ = regions[mergedRegions[numberOfMergedRegion].LastRegionNumber].CenterPointIndexJ;

                                                int scalar = GetScalyar(addingElementI - chainFirstElementI, addingElementJ - chainFirstElementJ, addingElementI - chainLastElementI, addingElementJ - chainLastElementJ);

                                                if (mergedRegions[numberOfMergedRegion].RegionsNumber.Contains(currentRegionNumber)) //.FirstRegionNumber == currentRegionNumber ||
                                                //mergedRegions[numberOfMergedRegion].LastRegionNumber == currentRegionNumber)
                                                {
                                                    mergedRegions[numberOfMergedRegion].RegionsNumber.Add(differentPairElement);
                                                    if (scalar > 0)
                                                    {
                                                        //   mergedRegions[numberOfMergedRegion].FirstRegionNumber = differentPairElement;
                                                        double dist1 = GetDistanceSqrBetweenRegions(regions[differentPairElement].CenterPointIndexI, regions[differentPairElement].CenterPointIndexJ,
                                                            regions[mergedRegions[numberOfMergedRegion].FirstRegionNumber].CenterPointIndexI,
                                                            regions[mergedRegions[numberOfMergedRegion].FirstRegionNumber].CenterPointIndexJ);

                                                        double dist2 = GetDistanceSqrBetweenRegions(regions[differentPairElement].CenterPointIndexI, regions[differentPairElement].CenterPointIndexJ,
                                                            regions[mergedRegions[numberOfMergedRegion].LastRegionNumber].CenterPointIndexI,
                                                            regions[mergedRegions[numberOfMergedRegion].LastRegionNumber].CenterPointIndexJ);
                                                        if (dist1 <= dist2)
                                                            mergedRegions[numberOfMergedRegion].FirstRegionNumber = differentPairElement;
                                                        else
                                                            mergedRegions[numberOfMergedRegion].LastRegionNumber = differentPairElement;
                                                    }
                                                    RecalculateAngleDirectionForChain(mergedRegions[numberOfMergedRegion], regions);
                                                    UniteTwoMergedRegionsByDirectionAndChainEnds(mergedRegions, regions, numberOfMergedRegion, pairMergedRegionIndex);
                                                }
                                            }


                                        }
                                }

                                numberOfMergedRegion = GetMergedRegionNumberByRegionNumber(mergedRegions, currentRegionNumber);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private int GetScalyar(int firstPointI, int firstPointJ, int secondPointI, int secondPointJ)
        {
            return firstPointI * secondPointI + firstPointJ * secondPointJ;           
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
        private List<int> GetNumbersOfPairsByRegionNumber(List<RegionChain> letterPairs, int regionNumber)
        {
            try
            {
                List<int> listOfPairs = new List<int>();
                int pairsNumber = letterPairs.Count;
                for (int i = 0; i < pairsNumber; i++)
                    if (letterPairs[i].FirstRegionNumber == regionNumber || letterPairs[i].LastRegionNumber == regionNumber)
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
        private void UniteTwoMergedRegionsByDirectionAndChainEnds(List<RegionChain> mergedRegions, Dictionary<int, Region> regions, int firstRegionIndexToUnit, int secondRegionIndexToUnit)
        {
            try
            {
                RegionChain firstChain = mergedRegions[firstRegionIndexToUnit];
                int firstChainNumber = firstChain.RegionsNumber.Count;

                RegionChain secondChain = mergedRegions[secondRegionIndexToUnit];
                int secondChainNumber = secondChain.RegionsNumber.Count;

                if (Math.Abs(firstChain.AngleDirection - secondChain.AngleDirection) < 30  && (firstChain.FirstRegionNumber == secondChain.FirstRegionNumber ||
                    firstChain.FirstRegionNumber == secondChain.LastRegionNumber ||
                    firstChain.LastRegionNumber == secondChain.FirstRegionNumber ||
                    firstChain.LastRegionNumber == secondChain.LastRegionNumber))
                {
                    if (secondChain.FirstRegionNumber == firstChain.FirstRegionNumber)
                        firstChain.FirstRegionNumber = secondChain.LastRegionNumber;
                    else if (secondChain.FirstRegionNumber == firstChain.LastRegionNumber)
                         firstChain.LastRegionNumber = secondChain.LastRegionNumber;
                    else  if (secondChain.LastRegionNumber == firstChain.LastRegionNumber)
                        firstChain.LastRegionNumber = secondChain.FirstRegionNumber;
                    else  if (secondChain.LastRegionNumber == firstChain.FirstRegionNumber)
                        firstChain.FirstRegionNumber = secondChain.FirstRegionNumber;

                    for (int i = 0; i < secondChainNumber; i++)
                    {
                        int indexFromRegion = secondChain.RegionsNumber[i];
                        if (!mergedRegions[firstRegionIndexToUnit].RegionsNumber.Contains(indexFromRegion))
                            mergedRegions[firstRegionIndexToUnit].RegionsNumber.Add(indexFromRegion);
                    }
                    RecalculateAngleDirectionForChain(mergedRegions[firstRegionIndexToUnit], regions); 
                    mergedRegions.RemoveAt(secondRegionIndexToUnit);
                }
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
                //   RecalculateAngleDirectionForChain(mergedRegions[firstRegionIndexToUnit], regions);  РАСКОММЕНТИИТЬ!!
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
                int firstRegionNumber = chain.FirstRegionNumber;
                int lastRegionNumber = chain.LastRegionNumber;

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
        /// Составляет 1 текстовый регион по номерам областей, входящим в него
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
        /// Формирование пар регионов
        /// </summary>
        /// <param name="regions">Регионы</param>
        /// <param name="letterPairsNumbers">Пары</param>
        
        private void GetLetterPairs(Dictionary<int, Region> regions, out List<RegionChain> letterPairsNumbers)
        {
            try
            {
                List<KeyValuePair<int, Region>> listOfRegions = regions.ToList();
                letterPairsNumbers = new List<RegionChain>();

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

                        double swRatio = firstRegionAverageSW > secondRegionAverageSW ? (double)firstRegionAverageSW / (double)secondRegionAverageSW :
                                                                                        (double)secondRegionAverageSW / (double)firstRegionAverageSW;
                        double heightRatio = firstRegionHeight > secondRegionHeight ? (double)firstRegionHeight / (double)secondRegionHeight :
                                                                                      (double)secondRegionHeight / (double)firstRegionHeight;
                      
                        double distance = GetDistanceSqrBetweenRegions(listOfRegions[i].Value.CenterPointIndexI, listOfRegions[i].Value.CenterPointIndexJ,
                            listOfRegions[j].Value.CenterPointIndexI, listOfRegions[j].Value.CenterPointIndexJ);                     

                        double widthHeightMetric = Math.Pow(Math.Max(Math.Min(firstRegionHeight, firstRegionWidth), Math.Min(secondRegionHeight, secondRegionWidth)), 2.0);
                        double occupationRationMetric = (double)Math.Abs(listOfRegions[i].Value.PixelsNumber - listOfRegions[j].Value.PixelsNumber) / (double)Math.Min(listOfRegions[i].Value.PixelsNumber, listOfRegions[j].Value.PixelsNumber);

                        if (heightRatio < this._pairsHeightRatio && swRatio <= this._pairsSWRatio && distance < this._pairsWidthDistanceSqrRatio * widthHeightMetric && 
                            occupationRationMetric <= this._pairsOccupationRatio  &&
                            Math.Abs(firstRegionAverageIntensity - secondRegionAverageIntensity) < this._pairsIntensityRatio)
                        {
                            RegionChain pair = new RegionChain();
                            pair.FirstRegionNumber = listOfRegions[i].Value.Number;
                            pair.LastRegionNumber = listOfRegions[j].Value.Number;
                            pair.DistanceSqr = distance;

                            pair.RegionsNumber.Add(listOfRegions[i].Value.Number);  ///new
                            pair.RegionsNumber.Add(listOfRegions[j].Value.Number);   // new

                          //  double dx = listOfRegions[i].Value.CenterPointIndexI - listOfRegions[j].Value.CenterPointIndexI;
                           // double dy = listOfRegions[i].Value.CenterPointIndexJ - listOfRegions[j].Value.CenterPointIndexJ;

                            int deltaI = listOfRegions[i].Value.CenterPointIndexI - listOfRegions[j].Value.CenterPointIndexI;
                            int deltaJ = listOfRegions[i].Value.CenterPointIndexJ - listOfRegions[j].Value.CenterPointIndexJ;

                            pair.AngleDirection = Math.Atan((double)deltaI / (double)deltaJ) * (180 / Math.PI);

                          //  double magnitude = Math.Sqrt(distance);

                          //  pair.DirectionX = dx / magnitude;
                         //   pair.DirectionY = dy / magnitude;                         

                            pair.MinBorderIndexI = Math.Min(listOfRegions[i].Value.MinBorderIndexI, listOfRegions[j].Value.MinBorderIndexI);
                            pair.MinBorderIndexJ = Math.Min(listOfRegions[i].Value.MinBorderIndexJ, listOfRegions[j].Value.MinBorderIndexJ);

                            pair.MaxBorderIndexI = Math.Max(listOfRegions[i].Value.MaxBorderIndexI, listOfRegions[j].Value.MaxBorderIndexI);
                            pair.MaxBorderIndexJ = Math.Max(listOfRegions[i].Value.MaxBorderIndexJ, listOfRegions[j].Value.MaxBorderIndexJ);

                            pair.Height = pair.MaxBorderIndexI - pair.MinBorderIndexI;
                            pair.Width = pair.MaxBorderIndexJ - pair.MinBorderIndexJ;
                        
                            letterPairsNumbers.Add(pair);
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }   

      /*  private void MakeChains(Dictionary<int, Region> regions, List<RegionChain> letterPairs, out List<RegionChain> chains)
        {
            try
            {
                int merges = 1;
                while (merges > 0)
                {
                    SetMergeToFalseForChains(letterPairs);
                    int letterPairsNumber = letterPairs.Count;                    
                    merges = 0;
                    List<RegionChain> temporaryChains = new List<RegionChain>();

                    for (int i = 0; i < letterPairsNumber; i++)
                        for (int j = 0; j < letterPairsNumber; j++)
                        {
                            if (i != j)
                            {
                                if (!letterPairs[i].WasMerged && !letterPairs[j].WasMerged && ShareOneEnd(letterPairs[i], letterPairs[j]))
                                {
                                    if (letterPairs[i].FirstRegionNumber == letterPairs[j].FirstRegionNumber)
                                        FirstFirstElementsCoincidence(regions, letterPairs[i], letterPairs[j], ref merges);
                                    else if (letterPairs[i].FirstRegionNumber == letterPairs[j].LastRegionNumber)
                                        FirstLastElementsCoincidence(regions, letterPairs[i], letterPairs[j], ref merges);
                                    else if (letterPairs[i].LastRegionNumber == letterPairs[j].FirstRegionNumber)
                                        LastFirstElementsCoincidence(regions, letterPairs[i], letterPairs[j], ref merges);
                                    else if (letterPairs[i].LastRegionNumber == letterPairs[j].LastRegionNumber)
                                        LastLastElementsCoincidence(regions, letterPairs[i], letterPairs[j], ref merges);
                                }
                            }
                        }
                    DistributeChains(letterPairs, temporaryChains);
                    letterPairs = temporaryChains;
                }
                chains = new List<RegionChain>(letterPairs);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void SetMergeToFalseForChains(List<RegionChain> chains)
        {
            try
            {
                int chainsNumber = chains.Count;
                for (int i = 0; i < chainsNumber; i++)
                    chains[i].WasMerged = false;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void DistributeChains(List<RegionChain> source, List<RegionChain> notMergedChains)
        {
            try
            {
                int sourceElementsNumber = source.Count;
                for (int i = 0; i < sourceElementsNumber; i++)
                {
                    if (!source[i].WasMerged)
                        notMergedChains.Add(source[i]);
                  //  else
                    //    mergedChains.Add(source[i]);
                }
                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Слияние двух цепочек, результат сливается в первую цепочку
        /// </summary>
        /// <param name="firstChain">Первая цепочка</param>
        /// <param name="secondChain">Вторая цепочка</param>
        private void MergeTwoChains(RegionChain firstChain, RegionChain secondChain)
        {
            try
            {
                int secondChainRegionsNumber = secondChain.RegionsNumber.Count;
                for (int i = 0; i < secondChainRegionsNumber; i++)
                {
                    int regionNumber = secondChain.RegionsNumber[i];
                    if (!firstChain.RegionsNumber.Contains(regionNumber))
                        firstChain.RegionsNumber.Add(regionNumber);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        void FirstFirstElementsCoincidence(Dictionary<int, Region> regions, RegionChain firstChain, RegionChain secondChain, ref int merges)
        {
            try
            {
                if (Math.Acos(firstChain.DirectionX * -secondChain.DirectionX + firstChain.DirectionY * -secondChain.DirectionY) < STRICTNESS)
                {
                    firstChain.FirstRegionNumber = secondChain.LastRegionNumber;
                    MergeTwoChains(firstChain, secondChain);

                    double d_x = regions[firstChain.FirstRegionNumber].CenterPointIndexI - regions[firstChain.LastRegionNumber].CenterPointIndexI;
                    double d_y = regions[firstChain.FirstRegionNumber].CenterPointIndexJ - regions[firstChain.LastRegionNumber].CenterPointIndexJ;
                    double distanceSqr = d_x * d_x + d_y * d_y;
                    double magitude = Math.Sqrt(distanceSqr);

                    firstChain.DirectionX = d_x / magitude;
                    firstChain.DirectionY = d_y / magitude;

                    firstChain.DistanceSqr = distanceSqr;

                    secondChain.WasMerged = true;
                    ++merges;
                }

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        void FirstLastElementsCoincidence(Dictionary<int, Region> regions, RegionChain firstChain, RegionChain secondChain, ref int merges)
        {
            try
            {
                if (Math.Acos(firstChain.DirectionX * secondChain.DirectionX + firstChain.DirectionY * secondChain.DirectionY) < STRICTNESS)
                {
                    firstChain.FirstRegionNumber = secondChain.FirstRegionNumber;
                    MergeTwoChains(firstChain, secondChain);

                    double d_x = regions[firstChain.FirstRegionNumber].CenterPointIndexI - regions[firstChain.LastRegionNumber].CenterPointIndexI;
                    double d_y = regions[firstChain.FirstRegionNumber].CenterPointIndexJ - regions[firstChain.LastRegionNumber].CenterPointIndexJ;
                    double distanceSqr = d_x * d_x + d_y * d_y;
                    double magitude = Math.Sqrt(distanceSqr);

                    firstChain.DirectionX = d_x / magitude;
                    firstChain.DirectionY = d_y / magitude;

                    firstChain.DistanceSqr = distanceSqr;

                    secondChain.WasMerged = true;
                    ++merges;
                }

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        void LastFirstElementsCoincidence(Dictionary<int, Region> regions, RegionChain firstChain, RegionChain secondChain, ref int merges)
        {
            try
            {
                //acos(chains[i].direction.x * chains[j].direction.x + chains[i].direction.y * chains[j].direction.y) < strictness
                if (Math.Acos(firstChain.DirectionX * secondChain.DirectionX + firstChain.DirectionY * secondChain.DirectionY) < STRICTNESS)
                {
                    firstChain.LastRegionNumber = secondChain.LastRegionNumber;
                    MergeTwoChains(firstChain, secondChain);

                    double d_x = regions[firstChain.FirstRegionNumber].CenterPointIndexI - regions[firstChain.LastRegionNumber].CenterPointIndexI;
                    double d_y = regions[firstChain.FirstRegionNumber].CenterPointIndexJ - regions[firstChain.LastRegionNumber].CenterPointIndexJ;
                    double distanceSqr = d_x * d_x + d_y * d_y;
                    double magitude = Math.Sqrt(distanceSqr);

                    firstChain.DirectionX = d_x / magitude;
                    firstChain.DirectionY = d_y / magitude;

                    firstChain.DistanceSqr = distanceSqr;

                    secondChain.WasMerged = true;
                    ++merges;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        void LastLastElementsCoincidence(Dictionary<int, Region> regions, RegionChain firstChain, RegionChain secondChain, ref int merges)
        {
            try
            {
                //acos(chains[i].direction.x * -chains[j].direction.x + chains[i].direction.y * -chains[j].direction.y) < strictness
                if (Math.Acos(firstChain.DirectionX * -secondChain.DirectionX + firstChain.DirectionY * -secondChain.DirectionY) < STRICTNESS)
                {
                    firstChain.LastRegionNumber = secondChain.FirstRegionNumber;
                    MergeTwoChains(firstChain, secondChain);

                    double d_x = regions[firstChain.FirstRegionNumber].CenterPointIndexI - regions[firstChain.LastRegionNumber].CenterPointIndexI;
                    double d_y = regions[firstChain.FirstRegionNumber].CenterPointIndexJ - regions[firstChain.LastRegionNumber].CenterPointIndexJ;
                    double distanceSqr = d_x * d_x + d_y * d_y;
                    double magitude = Math.Sqrt(distanceSqr);

                    firstChain.DirectionX = d_x / magitude;
                    firstChain.DirectionY = d_y / magitude;

                    firstChain.DistanceSqr = distanceSqr;

                    secondChain.WasMerged = true;
                    ++merges;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        bool ShareOneEnd(RegionChain firstChain, RegionChain secondChain)
        {
            if (firstChain.FirstRegionNumber == secondChain.FirstRegionNumber || firstChain.FirstRegionNumber == secondChain.LastRegionNumber ||
                firstChain.LastRegionNumber == secondChain.FirstRegionNumber || firstChain.LastRegionNumber == secondChain.LastRegionNumber)
                return true;
            return false;
        }*/

        /// <summary>
        /// Нахождение квадрата расстояния мужду двумя регионами
        /// </summary>
        /// <param name="firstRegionCenterI">Индекс центра первого региона по строке</param>
        /// <param name="firstRegionCenterJ">Индекс центра первого региона по столбцу</param>
        /// <param name="secondRegionCenterI">Индекс центра второго региона по строке</param>
        /// <param name="secondRegionCenterJ">Индекс центра второго региона по столбцу</param>
        /// <returns></returns>
        private double GetDistanceSqrBetweenRegions(int firstRegionCenterI, int firstRegionCenterJ, int secondRegionCenterI, int secondRegionCenterJ)
        {
            return ((double) ((firstRegionCenterI - secondRegionCenterI) * (firstRegionCenterI - secondRegionCenterI) +
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
                        int pixelRegionNumberL = this._lightTextDarkBg.Pixels[i, j].RegionNumber;
                        int pixelRegionNumberD = this._darkTextLightBg.Pixels[i, j].RegionNumber;
                        if ( (pixelRegionNumberL != PixelData<Grey>.UNDEFINED_REGION && this._lightRegions.ContainsKey(pixelRegionNumberL)) ||
                            (pixelRegionNumberD != PixelData<Grey>.UNDEFINED_REGION && this._darkRegions.ContainsKey(pixelRegionNumberD)))
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
