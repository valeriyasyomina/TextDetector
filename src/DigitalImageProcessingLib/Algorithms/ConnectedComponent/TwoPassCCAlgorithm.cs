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
//        public Dictionary<int, List<int>> RegionsEquivalents { get; set; }
        public List<List<int>> RegionsEquivalents { get; set; }
        public Dictionary<int, Region> Regions { get; set; }
        public UnifyingFeature Feature { get; set; }
        public ConnectivityType ConnectivityType { get; set; }         
        public TwoPassCCAlgorithm(UnifyingFeature feature, ConnectivityType connectivityType)
        {
            this.Feature = feature;          
            this.ConnectivityType = connectivityType;
            this.RegionsEquivalents = new List<List<int>>();// Dictionary<int, List<int>>();
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

                this.Regions.Clear();
                this.RegionsEquivalents.Clear();

                DeleteTrashLines(image);
                
                if (this.Feature == UnifyingFeature.StrokeWidth)
                {
                    if (this.ConnectivityType == Interface.ConnectivityType.FourConnectedRegion)                    
                        FirstPassStrokeWidth4Connectivity(image);               
                    else                    
                        FirstPassStrokeWidth8Connectivity(image);                                 
                }           
                SecondPass(image);
                DefineRegions(image);
                CountRegionsParameters();
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
                                this.RegionsEquivalents.Add(labelsList);                               

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
                                // Создание нового региона: создаём список с одним элементом (новый регион пока эквивалентен только себе)
                                ++currentRegionLabel;
                                image.Pixels[i, j].RegionNumber = currentRegionLabel;
                                List<int> labelsList = new List<int>();
                                labelsList.Add(currentRegionLabel);
                                this.RegionsEquivalents.Add(labelsList);
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
                            // Ищем множество (список) эквивалентных регионов, в котором есть и текущий регион

                          //  bool labelFound = false;
                            int listIndex = 0;
                            List<int> labelList = GetRegionsListByRegionNumber(pixelLabel, ref listIndex);
                          //  int RegionsCount = this.RegionsEquivalents.Count;

                          /*  for (int k = 0; k < RegionsCount && !labelFound; k++)
                            {
                                labelList = this.RegionsEquivalents[k];
                                if (labelList.Contains(pixelLabel))
                                    labelFound = true;
                            }*/

                            // Находим мин. элемент из этого списка
                            if (labelList.Count != 0)
                                image.Pixels[i, j].RegionNumber = labelList.Min();// pixelLabel < labelList.Min() ? pixelLabel : labelsList.Min();                      
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

                        // Т.к. мы убираем штрихи ДО связывания регионов, то ориентироваться на номер региона нельзя - только на ширину штриха

                        if (image.Pixels[i, j].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//pixelRegionNumber != PixelData<Grey>.UNDEFINED_REGION)
                        {
                            int counter = 0;
                            if (image.Pixels[i - 1, j - 1].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//...RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i - 1, j].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//.RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i - 1, j + 1].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//.RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i, j - 1].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//.RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i, j + 1].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//.RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i + 1, j - 1].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//.RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i + 1, j].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//.RegionNumber == pixelRegionNumber)
                                ++counter;
                            if (image.Pixels[i + 1, j + 1].StrokeWidth.Width != StrokeWidthData.UNDEFINED_WIDTH)//.RegionNumber == pixelRegionNumber)
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
                            double pixelStrokeWidth = image.Pixels[i, j].StrokeWidth.Width;
                            image.Pixels[i, j].Color.Data = (byte)ColorBase.MIN_COLOR_VALUE;
                            if (!this.Regions.ContainsKey(pixelRegionNumber))
                                this.Regions.Add(pixelRegionNumber, new Region() { PixelsNumber = 0, AverageStrokeWidth = 0,
                                MaxBorderIndexI = i, MaxBorderIndexJ = j, MinBorderIndexI = i, MinBorderIndexJ = j, SummaryStrokeWidth = 0,
                                TruePixelsNumber = 0, Square = 0, MaxStrokeWidth = pixelStrokeWidth, MinStrokeWidth = pixelStrokeWidth,
                                Width = 0, Height = 0, CenterPointIndexI = 0, CenterPointIndexJ = 0, Number = pixelRegionNumber, Diameter = 0,
                                BoundingBoxPixelsNumber = 0, AverageIntensity = 0, SummaryIntensity = 0 });

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

                            this.Regions[pixelRegionNumber].SummaryIntensity += image.Pixels[i, j].Color.Data;

                          //  this.Regions[pixelRegionNumber].Number = pixelRegionNumber;
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
        /// Вычисляет среднюю ширину штриха в кадом регионе, площадь региона, ширину, высоту региона, координаты центра региона
        /// </summary>
        private void CountRegionsParameters()
        {
            try
            {
                foreach (var pair in this.Regions)
                {
                    pair.Value.AverageStrokeWidth = (double)pair.Value.SummaryStrokeWidth / (double)pair.Value.PixelsNumber;
                    pair.Value.Square = (pair.Value.MaxBorderIndexI - pair.Value.MinBorderIndexI) * 
                        (pair.Value.MaxBorderIndexJ - pair.Value.MinBorderIndexJ);
                    pair.Value.Width = pair.Value.MaxBorderIndexJ - pair.Value.MinBorderIndexJ;
                    pair.Value.Height = pair.Value.MaxBorderIndexI - pair.Value.MinBorderIndexI;

                    pair.Value.CenterPointIndexI = pair.Value.MinBorderIndexI + pair.Value.Height / 2;
                    pair.Value.CenterPointIndexJ = pair.Value.MinBorderIndexJ + pair.Value.Width / 2;

                    pair.Value.Diameter = pair.Value.Height > pair.Value.Width ? pair.Value.Height / 2 : pair.Value.Width / 2;   ///???
                    pair.Value.BoundingBoxPixelsNumber = pair.Value.Width * 2 + pair.Value.Height * 2;

                    pair.Value.AverageIntensity = pair.Value.SummaryIntensity / pair.Value.PixelsNumber;
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
                    // теперь можно не делать второе добавление
                   // AddLabelToList(leftPixelLabel, upPixelLabel);
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
                    // теперь можно не делать второе добавление
                   // AddLabelToList(secondLabel, firstLabel);
                }
                if (firstLabel != thirdLabel && firstLabel != PixelData<ColorBase>.UNDEFINED_REGION && thirdLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(firstLabel, thirdLabel);
                    // теперь можно не делать второе добавление
                  //  AddLabelToList(thirdLabel, firstLabel);
                }
                if (firstLabel != fourthLabel && firstLabel != PixelData<ColorBase>.UNDEFINED_REGION && fourthLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(firstLabel, fourthLabel);
                    // теперь можно не делать второе добавление
                   // AddLabelToList(fourthLabel, firstLabel);
                }
                if (secondLabel != thirdLabel && secondLabel != PixelData<ColorBase>.UNDEFINED_REGION && thirdLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(secondLabel, thirdLabel);
                    // теперь можно не делать второе добавление
                  //  AddLabelToList(thirdLabel, secondLabel);
                }
                if (secondLabel != fourthLabel && secondLabel != PixelData<ColorBase>.UNDEFINED_REGION && fourthLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(secondLabel, fourthLabel);
                    // теперь можно не делать второе добавление
                 //   AddLabelToList(fourthLabel, secondLabel);
                }
                if (thirdLabel != fourthLabel && thirdLabel != PixelData<ColorBase>.UNDEFINED_REGION && fourthLabel != PixelData<ColorBase>.UNDEFINED_REGION)
                {
                    AddLabelToList(thirdLabel, fourthLabel);
                    // теперь можно не делать второе добавление
                 //   AddLabelToList(fourthLabel, thirdLabel);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Нахождение списка и его номера, содержащего номер текущего региона 
        /// </summary>
        /// <param name="currentRegionLabel">Номер текущего региона</param>
        /// <param name="listNumberInEquivalents">Номер списка в списке эквивалетных областей</param>
        /// <returns>Список, содержащий номер текущего регмона</returns>
        private List<int> GetRegionsListByRegionNumber(int currentRegionLabel, ref int listNumberInEquivalents)
        {
            try
            {
                int regionsCount = this.RegionsEquivalents.Count;
                bool currentRegionLabelFound = false;
                List<int> currentRegionLabelList = null;
                int currentRegionLabelIndex = 0;
                for (int i = 0; i < regionsCount && !currentRegionLabelFound; i++)
                {
                    currentRegionLabelIndex = i;
                    currentRegionLabelList = this.RegionsEquivalents[currentRegionLabelIndex];
                    if (currentRegionLabelList.Contains(currentRegionLabel))
                        currentRegionLabelFound = true;
                }
                listNumberInEquivalents = currentRegionLabelIndex;
                return currentRegionLabelList;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Сливание двух списков, результат записывается в первый список
        /// </summary>
        /// <param name="firstList">Первый список</param>
        /// <param name="secondList">Второй список</param>
        private void MergeLists(List<int> firstList, List<int> secondList)
        {
            try
            {
                int secondListCount = secondList.Count;
                for (int i = 0; i < secondListCount; i++)
                    firstList.Add(secondList[i]);                
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

                // Находим множество (список), содержащий номер первого региона: currentRegionLabel
              //  int RegionsCount = this.RegionsEquivalents.Count;
              //  bool currentRegionLabelFound = false;
                int currentRegionLabelIndex = 0;
                List<int> currentRegionLabelList = GetRegionsListByRegionNumber(currentRegionLabel, ref currentRegionLabelIndex);
                
             /*   for (int i = 0; i < RegionsCount && !currentRegionLabelFound; i++)
                {
                    currentRegionLabelIndex = i;
                    currentRegionLabelList = this.RegionsEquivalents[currentRegionLabelIndex];                    
                    if (currentRegionLabelList.Contains(currentRegionLabel))
                        currentRegionLabelFound = true;
                }*/

                // Находим множество (список), содержащий номер первого региона: label
              //  bool labelFound = false;
                int labelIndex = 0;
                List<int> labelList = GetRegionsListByRegionNumber(label, ref labelIndex);
                
             /*   for (int i = 0; i < RegionsCount && !labelFound; i++)
                {
                    labelIndex = i;
                    labelList = this.RegionsEquivalents[labelIndex];
                    if (labelList.Contains(label))
                        labelFound = true;
                }*/

                // Если номера регионов не принадлежат одному множеству (списку), то объединяем эти множества/списки
                if (labelIndex != currentRegionLabelIndex)
                {
                   /* int labelIndexCount = labelList.Count;
                    for (int i = 0; i < labelIndexCount; i++)
                        currentRegionLabelList.Add(labelList[i]);*/
                    MergeLists(currentRegionLabelList, labelList);
                    this.RegionsEquivalents.RemoveAt(labelIndex);
                }            
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }       
        
    }
}
