using DigitalVideoProcessingLib.Algorithms.KeyFrameExtraction;
using DigitalVideoProcessingLib.IO;
using DigitalVideoProcessingLib.VideoFrameType;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Threading;

namespace VideoTextDetectorWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProgressWindow _progressWindow = null;
        private OkButtonWindow _okButtonWindow = null;
        private static double PROGRESS_BAR_STEP_DEFAULT = 0.5;
        private static string INFO_TEXT_DEFAULT = "";
        private static int PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT = 10;
        private static int PROGRESS_WINDOW_TEXT_PADDING_DEFAULT = 5;
        public MainWindow()
        {
            InitializeComponent();
            InitializeProgressWindow();
            _okButtonWindow = InitializeOkButtonWindow();
        }

        private OkButtonWindow InitializeOkButtonWindow()
        {
            OkButtonWindow okButtonWindow = new OkButtonWindow();
            okButtonWindow.capitalText.Background = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            okButtonWindow.capitalText.Padding = new Thickness(PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT);
            okButtonWindow.textInformation.Padding = new Thickness(PROGRESS_WINDOW_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_TEXT_PADDING_DEFAULT);
            okButtonWindow.textInformation.Text = INFO_TEXT_DEFAULT;
            return okButtonWindow;
        }          

        private void InitializeProgressWindow()
        {
            _progressWindow = new ProgressWindow();
            _progressWindow.capitalText.Background = (System.Windows.Media.Brush)Application.Current.MainWindow.FindResource("Brush8B4789");
            _progressWindow.capitalText.Padding = new Thickness(PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT, 
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT,
                                                                PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT);
            _progressWindow.pbStatus.Value = 0;
            _progressWindow.pbStatus.SmallChange = PROGRESS_BAR_STEP_DEFAULT;
            _progressWindow.textInformation.Text = INFO_TEXT_DEFAULT;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ok");    
        }

        private void AboutProgram_Click(object sender, RoutedEventArgs e)
        {
            OkButtonWindow infoWindow = InitializeOkButtonWindow();
            infoWindow.capitalText.Text = "О программе";
            infoWindow.textInformation.Text = "О том, какое это дерьмовое ПО!\n И и о том, что жизнь боль,\n ад и содомия";
            infoWindow.ShowDialog();
        }

        private async void LoadVideoFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeProgressWindow();
                _okButtonWindow = InitializeOkButtonWindow();

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.ShowDialog();    

                this._progressWindow.capitalText.Text = "Загрузка видео";
                this._progressWindow.textInformation.Text = "Инициализация видео файла";
                this._progressWindow.Show();

                IOData ioData = new IOData() { FileName = dialog.FileName, FrameHeight = 600, FrameWidth = 800 };
                VideoLoader.frameLoaded += this.LoadingFramesProcessing;
                VideoLoader videoLoader = new VideoLoader();

                int framesNumber = await videoLoader.CountFramesNumberAsync(ioData);
                this._progressWindow.pbStatus.SmallChange = (double)this._progressWindow.pbStatus.Maximum / (double)framesNumber;

                List<Image<Bgr, Byte>> frames = await videoLoader.LoadFramesAsync(ioData);

                this._progressWindow.pbStatus.SmallChange = (double)(this._progressWindow.pbStatus.Maximum / 2.0) / (double)framesNumber;
                this._progressWindow.pbStatus.Value = 0;

                this._okButtonWindow.capitalText.Text = "Загрузка видео";
                this._okButtonWindow.textInformation.Text = "Видео было успешно загружено";
                this._okButtonWindow.ShowDialog();               

                this._progressWindow.capitalText.Text = "Извлечение ключевых кадров";
                this._progressWindow.textInformation.Text = INFO_TEXT_DEFAULT;             

                EdgeBasedKeyFrameExtractor.keyFrameExtractedEvent += this.KeyFramesExtractionProcessing;
                EdgeBasedKeyFrameExtractor.framesDifferenceEvent += this.FramesDifferenceCalculateProcessing;
                EdgeBasedKeyFrameExtractor edgeBasedKeyFrameExtractor = new EdgeBasedKeyFrameExtractor();
                List<GreyVideoFrame> keyFrames = await edgeBasedKeyFrameExtractor.ExtractKeyFrames(frames);
                frames.Clear();

                _okButtonWindow = InitializeOkButtonWindow();
                this._okButtonWindow.capitalText.Text = "Извлечение ключевых кадров";
                this._okButtonWindow.textInformation.Text = "Ключевые кадры были успешно извлечены";
                this._okButtonWindow.ShowDialog();

                this._progressWindow.Close();

                this.VideoPlayer.Source = new Uri(dialog.FileName);
            }
            catch (Exception exception)
            {
                OkButtonWindow infoWindow = InitializeOkButtonWindow();
                infoWindow.textInformation.Text = exception.Message;
                infoWindow.capitalText.Text = "Oops...";
                infoWindow.ShowDialog();
            }
        }


        private void FramesDifferenceCalculateProcessing(int firstFrameNumber, int secondFrameNumber)
        {
            try
            {
                this._progressWindow.Dispatcher.Invoke(delegate
                {
                    this._progressWindow.textInformation.Text = "Вычисление разницы кадров: " +
                                                                 firstFrameNumber.ToString() + " и " +
                                                                 secondFrameNumber.ToString();
                });
                this._progressWindow.Dispatcher.Invoke(delegate { this._progressWindow.pbStatus.Value += this._progressWindow.pbStatus.SmallChange; });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void KeyFramesExtractionProcessing(int firstFrameNumber, int secondFrameNumber, bool isLastFrame)
        {
            try
            {
                this._progressWindow.Dispatcher.Invoke(delegate { this._progressWindow.textInformation.Text = "Поиск ключевых кадров: " +
                                                                                                               firstFrameNumber.ToString() + " и " +
                                                                                                               secondFrameNumber.ToString();
                });
                if (isLastFrame)
                    this._progressWindow.Dispatcher.Invoke(delegate { this._progressWindow.pbStatus.Value = this._progressWindow.pbStatus.Maximum; });
                else
                    this._progressWindow.Dispatcher.Invoke(delegate { this._progressWindow.pbStatus.Value += this._progressWindow.pbStatus.SmallChange; });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void LoadingFramesProcessing(int frameNumber, bool isLastFrame)
        {
            try
            {
                if (isLastFrame)
                {
                    this._progressWindow.Dispatcher.Invoke( delegate { this._progressWindow.textInformation.Text = "Загрузка кадра: " + frameNumber.ToString();});
                    this._progressWindow.Dispatcher.Invoke(delegate { this._progressWindow.pbStatus.Value = this._progressWindow.pbStatus.Maximum; });                    
                }
                else
                {
                    this._progressWindow.Dispatcher.Invoke( delegate { this._progressWindow.textInformation.Text = "Загрузка кадра: " + frameNumber.ToString(); });
                    this._progressWindow.Dispatcher.Invoke(delegate { this._progressWindow.pbStatus.Value += this._progressWindow.pbStatus.SmallChange; });
                }   
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private async void LoadOneFrame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.ShowDialog();

                IOData ioData = new IOData() { FileName = dialog.FileName, FrameHeight = 600, FrameWidth = 800 };
                VideoLoader.frameLoaded += this.LoadingFramesProcessing;
                VideoLoader videoLoader = new VideoLoader();

                FrameLoader frameLoader = new FrameLoader();
                GreyVideoFrame videoFrame = await frameLoader.LoadFrame(ioData);

                OkButtonWindow okButtonWindow = InitializeOkButtonWindow();
                okButtonWindow.textInformation.Text = "Изображение было успешно загружено";
                okButtonWindow.capitalText.Text = "Загрука кадра";
                okButtonWindow.ShowDialog();
          
                ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                ImageSource imageSource = new BitmapImage(new Uri(dialog.FileName));
               
                this.VideoFramePicture.Source = imageSource;
            }
            catch (Exception exception)
            {
                OkButtonWindow infoWindow = InitializeOkButtonWindow();
                infoWindow.textInformation.Text = exception.Message;
                infoWindow.capitalText.Text = "Oops...";
                infoWindow.ShowDialog();
            }
        }
    }
}
