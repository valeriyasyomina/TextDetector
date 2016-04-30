using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFVideoTextDetector.Views;

namespace WPFVideoTextDetector.ViewModels
{
    public class FrameSizeViewModel: ViewModelBase
    {
        private FrameSizeWindow window = null;
        private int _frameWidth = 640;
        private int _frameHeight = 480;

        public int FrameWidth
        {
            get
            {
                return this._frameWidth;
            }
            set
            {
                this._frameWidth = value;
                NotifyPropertyChanged();
            }
        }
        public int FrameHeight
        {
            get
            {
                return this._frameHeight;
            }
            set
            {
                this._frameHeight = value;
                NotifyPropertyChanged();
            }
        }
        public FrameSizeViewModel(FrameSizeWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("Null window in ctor FrameSizeViewModel");
            this.window = window;
        }
    }
}
