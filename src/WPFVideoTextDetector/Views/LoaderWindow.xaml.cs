using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFVideoTextDetector.Views
{
    /// <summary>
    /// Логика взаимодействия для LoaderWindow.xaml
    /// </summary>
    public partial class LoaderWindow : Window
    {
        private static int THREAD_TIME_DELAY = 500;
        public LoaderWindow()
        {
            InitializeComponent();
            this.InitializeLoaderColor();
        }

        private void InitializeLoaderColor()
        {
            this.Ellipse_1.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            this.Ellipse_2.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_3.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_4.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_5.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_6.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_7.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_8.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_9.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_10.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_11.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_12.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_13.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_14.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_15.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
            this.Ellipse_16.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
        }

        public void StartAnimation()
        {
           // while (true)
          //  {
               /* Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_1.Fill = new SolidColorBrush(Color.FromRgb(100, 50, 74));
                this.Ellipse_2.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_2.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_3.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_3.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_4.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_4.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_5.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_5.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_6.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_6.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_7.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_7.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_8.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_8.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_9.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_9.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_10.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                this.UpdateLayout();
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_10.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_11.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_11.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_12.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_12.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_13.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_13.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_14.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_14.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_15.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                Thread.Sleep(THREAD_TIME_DELAY);
                this.Ellipse_15.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_16.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
                Thread.Sleep(THREAD_TIME_DELAY);*/
                this.Ellipse_16.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brushe5cde4");
                this.Ellipse_1.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Ellipse_5.Fill = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
        }
    }
}
