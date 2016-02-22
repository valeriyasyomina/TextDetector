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

namespace DigitalImageProcessingLib.Algorithms.ConnectedComponent
{    
    public class TwoPassCCAlgorithm: IConnectedComponent
    {
        private Dictionary<int, List<int>> _regionsEquivalents = null;      
        public UnifyingFeature Feature { get; set; }
        public int Treshold { get; set; }
        public TwoPassCCAlgorithm(UnifyingFeature feature, int strokeWidthTreshold = 0)
        {
            this.Feature = feature;
            this.Treshold = strokeWidthTreshold;
            this._regionsEquivalents = new Dictionary<int, List<int>>();
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
                    FirstPassStrokeWidth(image);
                    SecondPassStrokeWidth(image);
                }
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
        /// Выращивание регионов на основании ширины штриха (первый проход алгоритма)
        /// </summary>
        /// <param name="image">Серое изображенение</param>
        private void FirstPassStrokeWidth(GreyImage image)
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
                                this._regionsEquivalents.Add(currentRegionLabel, labelsList);
                            }
                            else if (image.Pixels[i, j - 1].StrokeWidth.Width != PixelData<Grey>.UNDEFINED_REGION &&     // есть оба соседа
                            image.Pixels[i - 1, j].StrokeWidth.Width != PixelData<Grey>.UNDEFINED_REGION)
                            {
                                int leftPixelLabel = image.Pixels[i, j - 1].RegionNumber;
                                int upPixelLabel = image.Pixels[i - 1, j].RegionNumber;

                                int minLabel = leftPixelLabel < upPixelLabel ? leftPixelLabel : upPixelLabel;

                                image.Pixels[i, j].RegionNumber = minLabel;
                                AddRegionLabels(currentRegionLabel, leftPixelLabel, upPixelLabel);
                            }
                            else if (image.Pixels[i, j - 1].StrokeWidth.Width != PixelData<Grey>.UNDEFINED_REGION)   // проверка пикселя слева 
                            {
                                image.Pixels[i, j].RegionNumber = image.Pixels[i, j - 1].RegionNumber;
                            }                            
                            else if (image.Pixels[i - 1, j].StrokeWidth.Width != PixelData<Grey>.UNDEFINED_REGION)   // проверка пикселя сверху
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
        /// Выращивание регионов на основании ширины штриха (второй проход алгоритма)
        /// </summary>
        /// <param name="image">Серое изображенение</param>
        private void SecondPassStrokeWidth(GreyImage image)
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
                            int minLabel = this._regionsEquivalents[pixelLabel].Min();
                            image.Pixels[i, j].RegionNumber = minLabel;                            
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Обработка номеров регионов соседних пикселей
        /// </summary>
        /// <param name="currentRegionLabel">Номер текущео региона</param>
        /// <param name="leftPixelLabel">Номер региона левого пикселя</param>
        /// <param name="upPixelLabel">Номер региона верхнего пикселя</param>
        private void AddRegionLabels(int currentRegionLabel, int leftPixelLabel, int upPixelLabel)
        {
            try
            {
                if (leftPixelLabel == upPixelLabel)
                    AddLabelToList(currentRegionLabel, leftPixelLabel);
                else
                {
                    AddLabelToList(currentRegionLabel, leftPixelLabel);
                    AddLabelToList(currentRegionLabel, upPixelLabel);
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
                List<int> labelsList = this._regionsEquivalents[currentRegionLabel];
                int listCount = labelsList.Count;

                bool alreadyExists = false;

                for (int i = 0; i < listCount && !alreadyExists; i++)
                    if (labelsList[i] == currentRegionLabel)
                        alreadyExists = true;

                if (!alreadyExists)
                    this._regionsEquivalents[currentRegionLabel].Add(label);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }       
        
    }
}
