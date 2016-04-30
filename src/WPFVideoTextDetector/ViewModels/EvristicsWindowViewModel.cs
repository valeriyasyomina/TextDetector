using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFVideoTextDetector.Views;

namespace WPFVideoTextDetector.ViewModels
{
    public class EvristicsWindowViewModel : ViewModelBase
    {
        private EvristicsWindow window = null;
        private double _varienceAverageSWRation = 0.5;
        private double _aspectRatio = 5.0;
        private double _diamiterSWRatio = 10.0;
        private double _bbPixelsNumberMinRatio = 1.5;
        private double _bbPixelsNumberMaxRatio = 25;
        private double _imageRegionHeightRationMin = 1.5;
        private double _imageRegionWidthRatioMin = 1.5;
        private double _pairsHeightRatio = 2.0;
        private double _pairsIntensityRatio = 1.0;
        private double _pairsSWRatio = 1.5;
        private double _pairsWidthDistanceSqrRatio = 9;
        private double _pairsOccupationRatio = 2.0;
        private int _minLettersNumberInTextRegion = 2;
        private bool _mergeByDirectionAndChainEnds = false;
        private bool _useAdaptiveSmoothing = false;
        #region Properties
        /// <summary>
        /// Изменение ширины штриха в пределах одного региона
        /// </summary>
        public double VarienceAverageSWRation 
        { 
            get
            {
                return this._varienceAverageSWRation;
            }
            set
            {
                this._varienceAverageSWRation = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Соотношение сторон регионов
        /// </summary>
        public double AspectRatio 
        { 
            get
            {
                return this._aspectRatio;
            }
            set
            {
                this._aspectRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Соотношение средней ширины штриха и диаметра региона
        /// </summary>
        public double DiamiterSWRatio
        {
            get
            {
                return this._diamiterSWRatio;
            }
            set
            {
                this._diamiterSWRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Минимальное число пикселей в рамке
        /// </summary>
        public double BbPixelsNumberMinRatio
        {
            get
            {
                return this._bbPixelsNumberMinRatio;
            }
            set
            {
                this._bbPixelsNumberMinRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Максимальное число пикселей в рамке
        /// </summary>
        public double BbPixelsNumberMaxRatio
        {
            get
            {
                return this._bbPixelsNumberMaxRatio;
            }
            set
            {
                this._bbPixelsNumberMaxRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Соотношение высоты региона и высоты изображения (для удаления слишком больших регмонов)
        /// </summary>
        public double ImageRegionHeightRationMin
        {
            get
            {
                return this._imageRegionHeightRationMin;
            }
            set
            {
                this._imageRegionHeightRationMin = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Соотношение ширины региона и ширины изображения (для удаления слишком больших регмонов)
        /// </summary>
        public double ImageRegionWidthRatioMin
        {
            get
            {
                return this._imageRegionWidthRatioMin;
            }
            set
            {
                this._imageRegionWidthRatioMin = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Соотношение высот пар регионов
        /// </summary>
        public double PairsHeightRatio
        {
            get
            {
                return this._pairsHeightRatio;
            }
            set
            {
                this._pairsHeightRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Разница интенсивности пар регионов 
        /// </summary>
        public double PairsIntensityRatio
        {
            get
            {
                return this._pairsIntensityRatio;
            }
            set
            {
                this._pairsIntensityRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Соотношение средних ширин штрихов пар регионов
        /// </summary>
        public double PairsSWRatio
        {
            get
            {
                return this._pairsSWRatio;
            }
            set
            {
                this._pairsSWRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Соотношение расстояния и ширины наименьшего региона из пары
        /// </summary>
        public double PairsWidthDistanceSqrRatio
        {
            get
            {
                return this._pairsWidthDistanceSqrRatio;
            }
            set{
                this._pairsWidthDistanceSqrRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Соотношение числа пикселей для двух регионов
        /// </summary>
        public double PairsOccupationRatio
        {
            get
            {
                return this._pairsOccupationRatio;
            }
            set
            {
                this._pairsOccupationRatio = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Минимальное число букв в текстовой области
        /// </summary>
        public int MinLettersNumberInTextRegion
        {
            get
            {
                return this._minLettersNumberInTextRegion;
            }
            set
            {
                this._minLettersNumberInTextRegion = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Объединять ли пары по направлению и по концам цепочек
        /// </summary>
        public bool MergeByDirectionAndChainEnds
        {
            get
            {
                return this._mergeByDirectionAndChainEnds;
            }
            set
            {
                this._mergeByDirectionAndChainEnds = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Использовать ли адаптивный алгоритм сглаживания
        /// </summary>
        public bool UseAdaptiveSmoothing
        {
            get
            {
                return this._useAdaptiveSmoothing;
            }
            set
            {
                this._useAdaptiveSmoothing = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        public EvristicsWindowViewModel(EvristicsWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("Null window in ctor EvristicsWindowViewModel");
            this.window = window;
        }
    }
}
