using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPFVideoTextDetector.Views;

namespace WPFVideoTextDetector.ViewModels
{
    public class LoaderWindowViewModel: ViewModelBase
    {
        private LoaderWindow window = null;        
        private int timerInterval = 0;
        private int currentEllipseNumber = 0;
        private string textInformation = null;

        private static int MAX_ELLIPSE_NUMBER = 16;
        private static int MIN_ELLIPSE_NUMBER = 1;

        #region Brushes
        private System.Windows.Media.Brush ellipse_1_brush = null;
        private System.Windows.Media.Brush ellipse_2_brush = null;
        private System.Windows.Media.Brush ellipse_3_brush = null;
        private System.Windows.Media.Brush ellipse_4_brush = null;
        private System.Windows.Media.Brush ellipse_5_brush = null;
        private System.Windows.Media.Brush ellipse_6_brush = null;
        private System.Windows.Media.Brush ellipse_7_brush = null;
        private System.Windows.Media.Brush ellipse_8_brush = null;
        private System.Windows.Media.Brush ellipse_9_brush = null;
        private System.Windows.Media.Brush ellipse_10_brush = null;
        private System.Windows.Media.Brush ellipse_11_brush = null;
        private System.Windows.Media.Brush ellipse_12_brush = null;
        private System.Windows.Media.Brush ellipse_13_brush = null;
        private System.Windows.Media.Brush ellipse_14_brush = null;
        private System.Windows.Media.Brush ellipse_15_brush = null;
        private System.Windows.Media.Brush ellipse_16_brush = null;
        #endregion

        #region Properties

        public System.Windows.Forms.Timer Timer { get; set; }

        public string TextInformation
        {
            get
            {
                return this.textInformation;
            }
            set
            {
                this.textInformation = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_1_Brush
        {
            get
            {
                return this.ellipse_1_brush;
            }
            set
            {
                this.ellipse_1_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_2_Brush
        {
            get
            {
                return this.ellipse_2_brush;
            }
            set
            {
                this.ellipse_2_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_3_Brush
        {
            get
            {
                return this.ellipse_3_brush;
            }
            set
            {
                this.ellipse_3_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_4_Brush
        {
            get
            {
                return this.ellipse_4_brush;
            }
            set
            {
                this.ellipse_4_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_5_Brush
        {
            get
            {
                return this.ellipse_5_brush;
            }
            set
            {
                this.ellipse_5_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_6_Brush
        {
            get
            {
                return this.ellipse_6_brush;
            }
            set
            {
                this.ellipse_6_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_7_Brush
        {
            get
            {
                return this.ellipse_7_brush;
            }
            set
            {
                this.ellipse_7_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_8_Brush
        {
            get
            {
                return this.ellipse_8_brush;
            }
            set
            {
                this.ellipse_8_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_9_Brush
        {
            get
            {
                return this.ellipse_9_brush;
            }
            set
            {
                this.ellipse_9_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_10_Brush
        {
            get
            {
                return this.ellipse_10_brush;
            }
            set
            {
                this.ellipse_10_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_11_Brush
        {
            get
            {
                return this.ellipse_11_brush;
            }
            set
            {
                this.ellipse_11_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_12_Brush
        {
            get
            {
                return this.ellipse_12_brush;
            }
            set
            {
                this.ellipse_12_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_13_Brush
        {
            get
            {
                return this.ellipse_13_brush;
            }
            set
            {
                this.ellipse_13_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_14_Brush
        {
            get
            {
                return this.ellipse_14_brush;
            }
            set
            {
                this.ellipse_14_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_15_Brush
        {
            get
            {
                return this.ellipse_15_brush;
            }
            set
            {
                this.ellipse_15_brush = value;
                NotifyPropertyChanged();
            }
        }
        public System.Windows.Media.Brush Ellipse_16_Brush
        {
            get
            {
                return this.ellipse_16_brush;
            }
            set
            {
                this.ellipse_16_brush = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        public LoaderWindowViewModel(LoaderWindow window, int timerInterval)
        {
            if (window == null)
                throw new ArgumentNullException("Null window in ctor");
            if (timerInterval <= 0)
                throw new ArgumentOutOfRangeException("Erro timerInterval value in ctor");
            this.window = window;
            this.timerInterval = timerInterval;
            this.currentEllipseNumber = MIN_ELLIPSE_NUMBER + 1;
            this.Timer = new System.Windows.Forms.Timer();
            this.Timer.Interval = this.timerInterval;
            this.Timer.Tick += this.RecolorEllipse;

            this.Ellipse_1_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            this.Ellipse_2_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_3_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_4_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_5_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_6_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_7_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_8_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_9_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_10_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_11_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_12_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_13_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_14_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_15_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_16_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
        }

        /// <summary>
        /// Перекрашивание элементов загрузчика по событию таймеру
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void RecolorEllipse(Object myObject, EventArgs myEventArgs)
        {
            switch (currentEllipseNumber)
            {
                case 1:
                    this.Ellipse_16_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_1_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 2:
                    this.Ellipse_1_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_2_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 3: 
                    this.Ellipse_2_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_3_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 4:
                    this.Ellipse_3_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_4_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 5:
                    this.Ellipse_4_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_5_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 6:
                    this.Ellipse_5_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_6_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 7:
                    this.Ellipse_6_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_7_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 8:
                    this.Ellipse_7_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_8_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 9:
                    this.Ellipse_8_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_9_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 10:
                    this.Ellipse_9_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_10_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 11:
                    this.Ellipse_10_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_11_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 12:
                    this.Ellipse_11_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_12_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 13:
                    this.Ellipse_12_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_13_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 14:
                    this.Ellipse_13_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_14_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 15:
                    this.Ellipse_14_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_15_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
                case 16:
                    this.Ellipse_15_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                    this.Ellipse_16_Brush = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                    break;
            }
            ++this.currentEllipseNumber;
            if (this.currentEllipseNumber > MAX_ELLIPSE_NUMBER)
                this.currentEllipseNumber = MIN_ELLIPSE_NUMBER;
        }

        /// <summary>
        /// Остановка анмации загрузки
        /// </summary>
        public void StopAnimation()
        {
            this.Timer.Stop();
        }

        /// <summary>
        /// Запуск анимации загрузки
        /// </summary>
        public void StartAnimation()
        {
            this.Timer.Start();  
        }
    }
}
