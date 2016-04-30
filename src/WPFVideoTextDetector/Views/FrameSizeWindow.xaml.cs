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
using System.Windows.Shapes;
using WPFVideoTextDetector.ViewModels;

namespace WPFVideoTextDetector.Views
{
    /// <summary>
    /// Логика взаимодействия для FrameSizeWidth.xaml
    /// </summary>
    public partial class FrameSizeWindow : Window
    {
        private static int FRAME_SIZE_WINDOW_HEADER_TEXT_PADDING_DEFAULT = 10;    
        public FrameSizeWindow()
        {
            FrameSizeViewModel viewModel = new FrameSizeViewModel(this);
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Инициализация экземпляра окна
        /// </summary>
        /// <returns></returns>
        public static FrameSizeWindow InitializeFrameSizeWindow()
        {
            FrameSizeWindow frameSizeWindow = new FrameSizeWindow();
            frameSizeWindow.capitalText.Background = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            frameSizeWindow.capitalText.Padding = new Thickness(FRAME_SIZE_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                FRAME_SIZE_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                FRAME_SIZE_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                FRAME_SIZE_WINDOW_HEADER_TEXT_PADDING_DEFAULT);
            return frameSizeWindow;
        }
    }
}
