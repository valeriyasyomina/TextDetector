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

namespace WPFVideoTextDetector.Views
{
    /// <summary>
    /// Логика взаимодействия для OkButtonWindow.xaml
    /// </summary>
    public partial class OkButtonWindow : Window
    {
        private static string INFO_TEXT_DEFAULT = "";
        private static int OK_BUTTON_WINDOW_HEADER_TEXT_PADDING_DEFAULT = 10;
        private static int OK_BUTTON_WINDOW_TEXT_PADDING_DEFAULT = 5;       

        public OkButtonWindow()
        {
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
        public static OkButtonWindow InitializeOkButtonWindow()
        {
            OkButtonWindow okButtonWindow = new OkButtonWindow();
            okButtonWindow.capitalText.Background = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            okButtonWindow.capitalText.Padding = new Thickness(OK_BUTTON_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                OK_BUTTON_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                OK_BUTTON_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                OK_BUTTON_WINDOW_HEADER_TEXT_PADDING_DEFAULT);
            okButtonWindow.textInformation.Padding = new Thickness(OK_BUTTON_WINDOW_TEXT_PADDING_DEFAULT,
                                                                OK_BUTTON_WINDOW_TEXT_PADDING_DEFAULT,
                                                                OK_BUTTON_WINDOW_TEXT_PADDING_DEFAULT,
                                                                OK_BUTTON_WINDOW_TEXT_PADDING_DEFAULT);
            okButtonWindow.textInformation.Text = INFO_TEXT_DEFAULT;
            return okButtonWindow;
        }
    }
}
