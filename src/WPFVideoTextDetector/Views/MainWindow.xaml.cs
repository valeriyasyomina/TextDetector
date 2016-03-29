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
using WPFVideoTextDetector.ViewModels;

namespace WPFVideoTextDetector.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainWindowViewModel mainViewModel = new MainWindowViewModel(this);
            this.DataContext = mainViewModel;       

            InitializeComponent();
        }

        private void FullScreenMenuItemClick(object sender, RoutedEventArgs e)
        {
             if (this.FullScreenMenuItem.IsChecked)
                 this.WindowState = System.Windows.WindowState.Maximized;
             else
                 this.WindowState = System.Windows.WindowState.Normal;
        }

        private void ExitMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
