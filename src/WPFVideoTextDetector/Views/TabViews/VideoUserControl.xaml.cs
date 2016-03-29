using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFVideoTextDetector.Views.TabViews
{
    /// <summary>
    /// Логика взаимодействия для VideoUserControl.xaml
    /// </summary>
    public partial class VideoUserControl : UserControl
    {
        public VideoUserControl()
        {
            InitializeComponent();
        }

        private void PlayVideoButtonClick(object sender, RoutedEventArgs e)
        {
            this.VideoPlayer.Play();
        }

        private void StopVideoButtonClick(object sender, RoutedEventArgs e)
        {
            this.VideoPlayer.Stop();
        }

        private void PauseVideoButtonClick(object sender, RoutedEventArgs e)
        {
            this.VideoPlayer.Pause();
        }

        private void VideoWasLoaded(object sender, RoutedEventArgs e)
        {
            this.VideoPlayer.Play();
            this.VideoPlayer.Pause();
        }
    }
}
