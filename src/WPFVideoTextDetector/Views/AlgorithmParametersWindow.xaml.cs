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
    /// Логика взаимодействия для AlgorithmParametersWindow.xaml
    /// </summary>
    public partial class AlgorithmParametersWindow : Window
    {
        private static int ALGORITM_PARAMS_WINDOW_HEADER_TEXT_PADDING_DEFAULT = 10;    
        public AlgorithmParametersWindow()
        {
            AlgorithmParametersViewModel viewModel = new AlgorithmParametersViewModel(this);
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
        public static AlgorithmParametersWindow InitializeAlgorithmParametersWindow()
        {
            AlgorithmParametersWindow algorithmParametersWindow = new AlgorithmParametersWindow();
            algorithmParametersWindow.capitalText.Background = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            algorithmParametersWindow.capitalText.Padding = new Thickness(ALGORITM_PARAMS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                ALGORITM_PARAMS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                ALGORITM_PARAMS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                ALGORITM_PARAMS_WINDOW_HEADER_TEXT_PADDING_DEFAULT);
            return algorithmParametersWindow;
        }
    }
}
