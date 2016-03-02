using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Interface;
using DigitalImageProcessingLib.SWTData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalImageProcessingLib.PixelData;
using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.RegionData;

namespace DigitalImageProcessingLib.Algorithms.ConnectedComponent
{    
    public class TwoPassCCAlgorithm: IConnectedComponent
    {
        private static int MIN_PIXELS_NEIGHBOR_NUMBER = 3;
        public Dictionary<int, List<int>> RegionsEquivalents { get; set; }
        public Dictionary<int, Region> Regions { get; set; }
        public UnifyingFeature Feature { get; set; }
        public ConnectivityType ConnectivityType { get; set; }         
        public TwoPassCCAlgorithm(UnifyingFeature feature, ConnectivityType connectivityType)
        {
            this.Feature = feature;          
            this.ConnectivityType = connectivityType;
            this.RegionsEquivalents = new Dictionary<int, List<int>>();
            this.Regions = new Dictionary<int, Region>();
        }

        /// <summary>
        /// Выращивание регионов по заданному признаку
        /// </summary>
        /// <param name="image">Серое изображение</param>
        public void FindComponents(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in FindComponents");
                if (this.Feature == UnifyingFeature.StrokeWidth)
                {
                    if (this.ConnectivityType == Interface.ConnectivityType.FourConnectedRegion)                    
                        FirstPassStrokeWidth4Connectivity(image);               
                    else                    
                        FirstPassStrokeWidth8Connectivity(image);                                 
                }
                SecondPass(image);
                DeleteTrashLines(image);
                DefineRegions(image);
                CountAverageStrokeWidthAndRegionSquare();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void FindComponents(RGBImage image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Выращивание регионов на основании ширины штриха с использованием 4-х связной области (первый проход алгоритма)
        /// </summary>
        /// <param name="image">Серое изображенение</param>
        private void FirstPassStrokeWidth4Connectivity(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                int currentRegionLabel = 0;

                for (int i = 1; i < imageHeight; i++)
                    for (int j = 1; j < imageWidth; j++)
                    {
                        if (image.Pixels[i, j].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)
                        {
                            // Если нет соседей у пикселя
                            if (image.Pixels[i, j - 1].RegionNumber == PixelData<Grey>.UNDEFINED_REGION &&
                                image.Pixels[i - 1, j].RegionNumber == PixelData<Grey>.UNDEFINED_REGION)
                            {
                                ++currentRegionLabel;
                                image.Pixels[i, j].RegionNumber = currentRegionLabel;
                                List<int> labelsList = new List<int>();
                                this.RegionsEquivalents.Add(currentRegionLabel, labelsList);
                            }
                            else if (image.Pixels[i, j - 1].RegionNumber != PixelData<Grey>.UNDEFINED_REGION &&     // есть оба соседа
                            image.Pixels[i - 1, j].RegionNumber != PixelData<Grey>.UNDEFINED_REGION)
                            {
                                int leftPixelLabel = image.Pixels[i, j - 1].RegionNumber;
                                int upPixelLabel = image.Pixels[i - 1, j].RegionNumber;

                                int minLabel = leftPixelLabel < upPixelLabel ? leftPixelLabel : upPixelLabel;

                                image.Pixels[i, j].RegionNumber = minLabel;
                                AddRegionLabels4Connectivity(leftPixelLabel, upPixelLabel);
                            }
                            else if (image.Pixels[i, j - 1].RegionNumber != PixelData<Grey>.UNDEFINED_REGION)   // проверка пикселя слева 
                            {
                                image.Pixels[i, j].RegionNumber = image.Pixels[i, j - 1].RegionNumber;
                            }
                            else if (image.Pixels[i - 1, j].RegionNumber != PixelData<Grey>.UNDEFINED_REGION)   // проверка пикселя сверху
                            {
                                image.Pixels[i, j].RegionNumber = image.Pixels[i - 1, j].RegionNumber;
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
        /// Выращивание регионов на основании ширины штриха с использованием 8-ми связной области (первый проход алгоритма)
        /// </summary>
        /// <param name="image">Серое изображенение</param>
        private void FirstPassStrokeWidth8Connectivity(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                int currentRegionLabel = 0;

                for (int i = 1; i < imageHeight; i++)
                    for (int j = 1; j < imageWidth; j++)
                    {
                        if (image.Pixels[i, j].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)
                        {
                            int firstPixelRegionNumber = image.Pixels[i, j - 1].RegionNumber;
                            int secondPixelRegionNumber = image.Pixels[i - 1, j].RegionNumber;
                            int thirdPixelRegionNumber = image.Pixels[i - 1, j - 1].RegionNumber;
                            int fourthPixelRegionNumber = image.Pixels[i - 1, j + 1].RegionNumber;                     

                            // Если нет соседей у пикселя
                            if (firstPixelRegionNumber == PixelData<Grey>.UNDEFINED_REGION && secondPixelRegionNumber == PixelData<Grey>.UNDEFINED_REGION &&
                                thirdPixelRegionNumber == PixelData<Grey>.UNDEFINED_REGION && fourthPixelRegionNumber == PixelData<Grey>.UNDEFINED_REGION)
                            {
                                ++currentRegionLabel;
                                image.Pixels[i, j].RegionNumber = currentRegionLabel;
                                List<int> labelsList = new List<int>();
                                this.RegionsEquivalents.Add(currentRegionLabel, labelsList);
                            }
                            else
                            {
                                int minLabel = GetMinFromFourLabels(firstPixelRegionNumber, secondPixelRegionNumber, thirdPixelRegionNumber, fourthPixelRegionNumber);                                
                                image.Pixels[i, j].RegionNumber = minLabel;
                                AddRegionLabels8Connectivity(firstPixelRegionNumber, secondPixelRegionNumber, thirdPixelRegionNumber, fourthPixelRegionNumber);
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
        /// Выращивание регионов (второй проход алгоритма)
        /// </summary>
        /// <param name="image">Серое изображенение</param>
        private void SecondPass(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        int pixelLabel = image.Pixels[i, j].RegionNumber;
                        if (pixelLabel != PixelData<Grey>.UNDEFINED_REGION)
                        {
                            List<int> labelsList = this.RegionsEquivalents[pixelLabel];

                            if (labelsList.Count != 0)                        
                                image.Pixels[i, j].RegionNumber = pixelLabel < labelsList.Min() ? pixelLabel : labelsList.Min();                      
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Поиск минимальной среди 4-х мномеров 
        /// </summary>
        /// <param name="firstLabel">Первый номер</param>
        /// <param name="secondLabel">Второй номер</param>
        /// <param name="thirdLabel">Третий номер</param>
        /// <param name="fourthLabel">Четвертый номер</param>
        /// <returns></returns>
        private int GetMinFromFourLabels(int firstLabel, int secondLabel, int thirdLabel, int fourthLabel)
        {
            try
            {
                int minLabel = int.MaxValue;
                if (firstLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                    minLabel = minLabel > firstLabel ? firstLabel : minLabel;

                if (secondLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                    minLabel = minLabel > secondLabel ? secondLabel : minLabel;

                if (thirdLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                    minLabel = minLabel > thirdLabel ? thirdLabel : minLabel;

                if (fourthLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                    minLabel = minLabel > fourthLabel ? fourthLabel : minLabel;

                return minLabel;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Удаление лишних линий, связывающих регионы
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void DeleteTrashLines(GreyImage image)
        {
            try
            {
                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                for (int i = 1; i < imageHeight; i++)
                    for (int j = 1; j < imageWidth; j++)
                    {
                        int pixelRegionNumber = image.Pixels[i, j].RegionNumber;
                        if (pixelRegionNumber != PixelData<Grey>.UNDEFINED_REGION)
                        {
                            int counter = 0;
                            if (image.Pixels[i - 1, j - 1].RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i - 1, j].RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i - 1, j + 1].RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i, j - 1].RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i, j + 1].RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i + 1, j - 1].RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i + 1, j].RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i + 1, j + 1].RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (counter <= MIN_PIXELS_NEIGHBOR_NUMBER)
                            {
                                image.Pixels[i, j].RegionNumber = PixelData<Grey>.UNDEFINED_REGION;
                                image.Pixels[i, j].StrokeWidth.Width = StrokeWidthData.UNDEFINED_WIDTH;
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
        /// Определяет границы регионов и количество пикселей в регионах
        /// </summary>
        /// <param name="image">Серое изображение</param>
        private void DefineRegions(GreyImage image)
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
                            int pixelStrokeWidth = image.Pixels[i, j].StrokeWidth.Width;
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                            if (!this.Regions.ContainsKey(pixelRegionNumber))
                                this.Regions.Add(pixelRegionNumber, new Region() { PixelsNumber = 0, AverageStrokeWidth = 0,
                                MaxBorderIndexI = i, MaxBorderIndexJ = j, MinBorderIndexI = i, MinBorderIndexJ = j, SummaryStrokeWidth = 0,
                                TruePixelsNumber = 0, Square = 0, MaxStrokeWidth = pixelStrokeWidth, MinStrokeWidth = pixelStrokeWidth});

                            this.Regions[pixelRegionNumber].PixelsNumber++;
                            this.Regions[pixelRegionNumber].SummaryStrokeWidth += image.Pixels[i, j].StrokeWidth.Width;

                            if (i < this.Regions[pixelRegionNumber].MinBorderIndexI)
                                this.Regions[pixelRegionNumber].MinBorderIndexI = i;
                            if (i > this.Regions[pixelRegionNumber].MaxBorderIndexI)
                                this.Regions[pixelRegionNumber].MaxBorderIndexI = i;
                            if (j < this.Regions[pixelRegionNumber].MinBorderIndexJ)
                                this.Regions[pixelRegionNumber].MinBorderIndexJ = j;
                            if (j > this.Regions[pixelRegionNumber].MaxBorderIndexJ)
                                this.Regions[pixelRegionNumber].MaxBorderIndexJ = j;

                            if (pixelStrokeWidth < this.Regions[pixelRegionNumber].MinStrokeWidth)
                                this.Regions[pixelRegionNumber].MinStrokeWidth = pixelStrokeWidth;
                            if (pixelStrokeWidth > this.Regions[pixelRegionNumber].MaxStrokeWidth)
                                this.Regions[pixelRegionNumber].MaxStrokeWidth = pixelStrokeWidth;
                            //  if (!this.Regions.Contains(pixelRegionNumber))
                            //    this.Regions.Add(pixelRegionNumber);
                        }
                        else
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MAX_COLOR_VALUE;
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисляет среднюю ширину штриха в кадом регионе площадь региона
        /// </summary>
        private void CountAverageStrokeWidthAndRegionSquare()
        {
            try
            {
                foreach (var pair in this.Regions)
                {
                    pair.Value.AverageStrokeWidth = pair.Value.SummaryStrokeWidth / pair.Value.PixelsNumber;
                    pair.Value.Square = (pair.Value.MaxBorderIndexI - pair.Value.MinBorderIndexI) * 
                        (pair.Value.MaxBorderIndexJ - pair.Value.MinBorderIndexJ);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Обработка номеров регионов соседних пикселей для 4-х связной области
        /// </summary>
        /// <param name="currentRegionLabel">Номер текущео региона</param>
        /// <param name="leftPixelLabel">Номер региона левого пикселя</param>
        /// <param name="upPixelLabel">Номер региона верхнего пикселя</param>
        private void AddRegionLabels4Connectivity(int leftPixelLabel, int upPixelLabel)
        {
            try
            {
                if (leftPixelLabel != upPixelLabel)
                {
                    AddLabelToList(upPixelLabel, leftPixelLabel);
                    AddLabelToList(leftPixelLabel, upPixelLabel);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }      

        /// <summary>
        /// Обработка номеров регионов соседних пикселей для 8-х связной области
        /// </summary>
        /// <param name="currentRegionLabel">Номер текущео региона</param>
        /// <param name="leftPixelLabel">Номер региона левого пикселя</param>
        /// <param name="upPixelLabel">Номер региона верхнего пикселя</param>
        private void AddRegionLabels8Connectivity(int firstLabel, int secondLabel, int thirdLabel, int fourthLabel)
        {
            try
            {
                if (firstLabel != secondLabel && firstLabel != PixelData<ColorBase>.UNDEFINED_REGION && secondLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(firstLabel, secondLabel);
                    AddLabelToList(secondLabel, firstLabel);
                }
                if (firstLabel != thirdLabel && firstLabel != PixelData<ColorBase>.UNDEFINED_REGION && thirdLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(firstLabel, thirdLabel);
                    AddLabelToList(thirdLabel, firstLabel);
                }
                if (firstLabel != fourthLabel && firstLabel != PixelData<ColorBase>.UNDEFINED_REGION && fourthLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(firstLabel, fourthLabel);
                    AddLabelToList(fourthLabel, firstLabel);
                }
                if (secondLabel != thirdLabel && secondLabel != PixelData<ColorBase>.UNDEFINED_REGION && thirdLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(secondLabel, thirdLabel);
                    AddLabelToList(thirdLabel, secondLabel);
                }
                if (secondLabel != fourthLabel && secondLabel != PixelData<ColorBase>.UNDEFINED_REGION && fourthLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(secondLabel, fourthLabel);
                    AddLabelToList(fourthLabel, secondLabel);
                }
                if (thirdLabel != fourthLabel && thirdLabel != PixelData<ColorBase>.UNDEFINED_REGION && fourthLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(thirdLabel, fourthLabel);
                    AddLabelToList(fourthLabel, thirdLabel);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        /// <summary>
        /// Добавляет номер региона в список эквивалентов
        /// </summary>
        /// <param name="currentRegionLabel">Номер региона</param>
        /// <param name="label">Метка для добавления</param>
        private void AddLabelToList(int currentRegionLabel, int label)
        {
            try
            {  
                /*foreach (var pair in this.RegionsEquivalents)
                {
                    List<int> labelsList = pair.Value;                    
                    if ((labelsList.Contains(currentRegionLabel) || pair.Key == currentRegionLabel) && !labelsList.Contains(label))
                        labelsList.Add(label);
                }  */     

                int RegionsCount = this.RegionsEquivalents.Count;
                for (int i = 0; i < RegionsCount; i++)
                {
                    int index = i + 1;
                    List<int> labelsList = this.RegionsEquivalents[index];
                    if ((labelsList.Contains(currentRegionLabel) || index == currentRegionLabel) && !labelsList.Contains(label))
                            labelsList.Add(label);                  
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }       
        
    }
}
