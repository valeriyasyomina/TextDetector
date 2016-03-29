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
using WPFVideoTextDetector.ViewModels;

namespace WPFVideoTextDetector.Views
{
    /// <summary>
    /// Логика взаимодействия для LoaderWindow.xaml
    /// </summary>
    public partial class LoaderWindow : Window
    {
        private static int DELAY_TIMER_MS = 500;
        public LoaderWindow()
        {
            LoaderWindowViewModel loaderWindowViewModel = new LoaderWindowViewModel(this, DELAY_TIMER_MS);
            this.DataContext = loaderWindowViewModel;                
            InitializeComponent();         
        }       
    }
}
