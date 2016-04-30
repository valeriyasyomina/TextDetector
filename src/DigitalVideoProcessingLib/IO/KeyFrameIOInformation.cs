using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class KeyFrameIOInformation
    {
        public KeyFrameIOInformation()
        {
            this.MergeByDirectionAndChainEnds = false;
            this.UseAdaptiveSmoothing = false;
        }
        /// <summary>
        /// Ширина кадра
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Высота кадра
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Номер кадра в видеоряде
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Нуждается ли кадр в обработке
        /// </summary>
        public bool NeedProcess { get; set; }
        /// <summary>
        /// Изменение ширины штриха в пределах одного региона
        /// </summary>
        public double VarienceAverageSWRation { get; set; }
        /// <summary>
        /// Соотношение сторон регионов
        /// </summary>
        public double AspectRatio { get; set; }
        /// <summary>
        /// Соотношение средней ширины штриха и диаметра региона
        /// </summary>
        public double DiamiterSWRatio { get; set; }
        /// <summary>
        /// Минимальное число пикселей в рамке
        /// </summary>
        public double BbPixelsNumberMinRatio { get; set; }
        /// <summary>
        /// Максимальное число пикселей в рамке
        /// </summary>
        public double BbPixelsNumberMaxRatio { get; set; }
        /// <summary>
        /// Соотношение высоты региона и высоты изображения (для удаления слишком больших регмонов)
        /// </summary>
        public double ImageRegionHeightRationMin { get; set; }
        /// <summary>
        /// Соотношение ширины региона и ширины изображения (для удаления слишком больших регмонов)
        /// </summary>
        public double ImageRegionWidthRatioMin { get; set; }
        /// <summary>
        /// Соотношение высот пар регионов
        /// </summary>
        public double PairsHeightRatio { get; set; }
        /// <summary>
        /// Разница интенсивности пар регионов 
        /// </summary>
        public double PairsIntensityRatio { get; set; }
        /// <summary>
        /// Соотношение средних ширин штрихов пар регионов
        /// </summary>
        public double PairsSWRatio { get; set; }
        /// <summary>
        /// Соотношение расстояния и ширины наименьшего региона из пары
        /// </summary>
        public double PairsWidthDistanceSqrRatio { get; set; }
        /// <summary>
        /// Соотношение числа пикселей для двух регионов
        /// </summary>
        public double PairsOccupationRatio { get; set; }
        /// <summary>
        /// Минимальное число букв в текстовой области
        /// </summary>
        public int MinLettersNumberInTextRegion { get; set; }       
        /// <summary>
        /// Объединять ли пары по направлению и по концам цепочек
        /// </summary>
        public bool MergeByDirectionAndChainEnds { get; set; }
        /// <summary>
        /// Использовать ли адаптивный алгоритм сглаживания
        /// </summary>
        public bool UseAdaptiveSmoothing { get; set; }
        /// <summary>
        /// Размер апертуры фильтра Гаусса
        /// </summary>
        public int GaussFilterSize { get; set; }
        /// <summary>
        /// Sigma фильтра Гаусса
        /// </summary>
        public double GaussFilterSigma { get; set; }
        /// <summary>
        /// Нижний порог для Кенни
        /// </summary>
        public int CannyLowTreshold { get; set; }
        /// <summary>
        /// Верхний порог для Кенни
        /// </summary>
        public int CannyHighTreshold { get; set; }
    }
}
