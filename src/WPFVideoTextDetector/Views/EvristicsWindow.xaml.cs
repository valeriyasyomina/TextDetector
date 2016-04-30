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
    /// Логика взаимодействия для EvristicsWindow.xaml
    /// </summary>
    public partial class EvristicsWindow : Window
    {
        private static int EVRISTICS_WINDOW_HEADER_TEXT_PADDING_DEFAULT = 10;       
        public EvristicsWindow()
        {
            EvristicsWindowViewModel evristicsWindowViewModel = new EvristicsWindowViewModel(this);
            this.DataContext = evristicsWindowViewModel;
            InitializeComponent();        
        }

        /// <summary>
        /// Инициализация экземпляра окна
        /// </summary>
        /// <returns></returns>
        public static EvristicsWindow InitializeEvristicsWindow()
        {
            EvristicsWindow evristicsWindow = new EvristicsWindow();
            evristicsWindow.capitalText.Background = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            evristicsWindow.capitalText.Padding = new Thickness(EVRISTICS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                EVRISTICS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                EVRISTICS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                EVRISTICS_WINDOW_HEADER_TEXT_PADDING_DEFAULT);
            return evristicsWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
