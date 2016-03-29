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
    /// Логика взаимодействия для ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private static string INFO_TEXT_DEFAULT = "";
        private static int PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT = 10;
        private static int PROGRESS_WINDOW_TEXT_PADDING_DEFAULT = 5;
        private static double PROGRESS_BAR_STEP_DEFAULT = 0.5;

        public ProgressWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Инициализация экземпляра окна с pgorgess bar
        /// </summary>
        /// <returns></returns>
        public static ProgressWindow InitializeProgressWindow()
        {
            ProgressWindow progressWindow = new ProgressWindow();
            progressWindow.capitalText.Background = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            progressWindow.capitalText.Padding = new Thickness(PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT);
            progressWindow.pbStatus.Value = 0;
            progressWindow.pbStatus.SmallChange = PROGRESS_BAR_STEP_DEFAULT;
            progressWindow.textInformation.Text = INFO_TEXT_DEFAULT;

            return progressWindow;
        }
    }
}
