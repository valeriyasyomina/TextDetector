using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFVideoTextDetector.Views;

namespace WPFVideoTextDetector.ViewModels
{
    public class AlgorithmParametersViewModel: ViewModelBase
    {
        private AlgorithmParametersWindow window = null;

        private int _gaussFilterSize = 5;
        private double _gaussSigma = 1.4;
        private int _cannyLowTreshold = 20;
        private int _cannyHighTreshold = 80;        

        #region Properties
        public int GaussFilterSize
        {
            get
            {
                return this._gaussFilterSize;
            }
            set
            {
                this._gaussFilterSize = value;
                NotifyPropertyChanged();
            }
        }
        public double GaussSigma
        {
            get
            {
                return this._gaussSigma;
            }
            set
            {
                this._gaussSigma = value;
                NotifyPropertyChanged();
            }
        }
        public int CannyLowTreshold
        {
            get
            {
                return this._cannyLowTreshold;
            }
            set
            {
                this._cannyLowTreshold = value;
                NotifyPropertyChanged();
            }
        }
        public int CannyHighTreshold
        {
            get
            {
                return this._cannyHighTreshold;
            }
            set
            {
                this._cannyHighTreshold = value;
                NotifyPropertyChanged();
            }
        }        
        #endregion
        public AlgorithmParametersViewModel(AlgorithmParametersWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("Null window in ctor AlgorithmParametersViewModel");
            this.window = window;
        }
    }
}
