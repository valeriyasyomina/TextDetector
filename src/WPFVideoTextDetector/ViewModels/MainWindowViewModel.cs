using DigitalVideoProcessingLib.Algorithms.KeyFrameExtraction;
using DigitalVideoProcessingLib.IO;
using DigitalVideoProcessingLib.VideoFrameType;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFVideoTextDetector.Command;
using WPFVideoTextDetector.Views;

namespace WPFVideoTextDetector.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        private ProgressWindow progressWindow = null;
        private Uri videoFileName = null;
        private Uri videoFrameFileName = null;

        private static double PROGRESS_BAR_STEP_DEFAULT = 0.5;
        private static string INFO_TEXT_DEFAULT = "";
        private static int PROGRESS_WINDOW_HEADER_TEXT_PADDING_DEFAULT = 10;
        private static int PROGRESS_WINDOW_TEXT_PADDING_DEFAULT = 5;

        public ICommand LoadVideoFileCommand
        {
            get
            {
                return new DelegateCommand(o => this.LoadVideoFileFunction());
            }
        }
        public ICommand LoadVideoFrameCommand
        {
            get
            {
                return new DelegateCommand(o => this.LoadVideoFrameFunction());
            }
        }
             

        public Uri VideoFileName
        {
            get
            {
                return this.videoFileName;
            }
            set
            {
                this.videoFileName = value;
                NotifyPropertyChanged();
            }
        }

        public Uri VideoFrameFileName
        {
            get
            {
                return this.videoFrameFileName;
            }
            set
            {
                this.videoFrameFileName = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Загрузка единичного кадра видео 
        /// </summary>
        private async void LoadVideoFrameFunction()
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

                this.VideoFrameFileName = new Uri(dialog.FileName);
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);
            }
        }

        /// <summary>
        /// Сохранение обработанного видео в виде набора ключевых кадров
        /// </summary>
        private async void SaveVideoFile()
        {
            try
            {

            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);
            }
        }

        /// <summary>
        /// Сохранение одного обработанного кадра видео
        /// </summary>
        private async void SaveVideoFrame()
        {
            try
            {

            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);
            }
        }

        /// <summary>
        /// Выделение текста на на единичном кадре видео
        /// </summary>
        private async void DetectVideoFrameTextFunction()
        {
            try
            {

            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);
            }
        }

        /// <summary>
        /// Выделение текста на ключевых кадрах видеоролика
        /// </summary>
        private async void DetectVideoTextFunction()
        {
            try
            {

            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);              
            }
        }


        /// <summary>
        /// Загрузка видео
        /// </summary>
        private async void LoadVideoFileFunction()
        {
            try
            {
                progressWindow = InitializeProgressWindow();                
                OkButtonWindow okButtonWindow = InitializeOkButtonWindow();

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.ShowDialog();

                progressWindow.capitalText.Text = "Загрузка видео";
                progressWindow.textInformation.Text = "Инициализация видео файла";
                progressWindow.Show();

                IOData ioData = new IOData() { FileName = dialog.FileName, FrameHeight = 600, FrameWidth = 800 };
                VideoLoader.frameLoaded += this.LoadingFramesProcessing;
                VideoLoader videoLoader = new VideoLoader();

                int framesNumber = await videoLoader.CountFramesNumberAsync(ioData);
                progressWindow.pbStatus.SmallChange = (double)progressWindow.pbStatus.Maximum / (double)framesNumber;

                List<Image<Bgr, Byte>> frames = await videoLoader.LoadFramesAsync(ioData);

                progressWindow.pbStatus.SmallChange = (double)(progressWindow.pbStatus.Maximum / 2.0) / (double)framesNumber;
                progressWindow.pbStatus.Value = 0;

                okButtonWindow.capitalText.Text = "Загрузка видео";
                okButtonWindow.textInformation.Text = "Видео было успешно загружено";
                okButtonWindow.ShowDialog();

                progressWindow.capitalText.Text = "Извлечение ключевых кадров";
                progressWindow.textInformation.Text = INFO_TEXT_DEFAULT;

                EdgeBasedKeyFrameExtractor.keyFrameExtractedEvent += this.KeyFramesExtractionProcessing;
                EdgeBasedKeyFrameExtractor.framesDifferenceEvent += this.FramesDifferenceCalculateProcessing;
                EdgeBasedKeyFrameExtractor edgeBasedKeyFrameExtractor = new EdgeBasedKeyFrameExtractor();
                List<GreyVideoFrame> keyFrames = await edgeBasedKeyFrameExtractor.ExtractKeyFrames(frames);
                frames.Clear();

                okButtonWindow = InitializeOkButtonWindow();
                okButtonWindow.capitalText.Text = "Извлечение ключевых кадров";
                okButtonWindow.textInformation.Text = "Ключевые кадры были успешно извлечены";
                okButtonWindow.ShowDialog();

                progressWindow.Close();
                this.VideoFileName = new Uri(dialog.FileName);        
            }
            catch (Exception exception)
            {
                progressWindow.Hide();
                ShowExceptionMessage(exception.Message);
            }
        }

        /// <summary>
        /// Подписка на событие о вычислении разниц кадров
        /// </summary>
        /// <param name="firstFrameNumber">Номер первого кадра</param>
        /// <param name="secondFrameNumber">Номер второго кадра</param>
        private void FramesDifferenceCalculateProcessing(int firstFrameNumber, int secondFrameNumber)
        {
            try
            {
                this.progressWindow.Dispatcher.Invoke(delegate
                {
                    this.progressWindow.textInformation.Text = "Вычисление разницы кадров: " +
                                                                 firstFrameNumber.ToString() + " и " +
                                                                 secondFrameNumber.ToString();
                });
                this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.pbStatus.Value += this.progressWindow.pbStatus.SmallChange; });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// Подписака на событие об извлечении ключевых кадров
        /// </summary>
        /// <param name="firstFrameNumber">Номер первого кадра</param>
        /// <param name="secondFrameNumber">Номер второго кадра</param>
        /// <param name="isLastFrame">Последний кадр или нет</param>
        private void KeyFramesExtractionProcessing(int firstFrameNumber, int secondFrameNumber, bool isLastFrame)
        {
            try
            {
                this.progressWindow.Dispatcher.Invoke(delegate
                {
                    this.progressWindow.textInformation.Text = "Поиск ключевых кадров: " +
                                                                 firstFrameNumber.ToString() + " и " +
                                                                 secondFrameNumber.ToString();
                });
                if (isLastFrame)
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.pbStatus.Value = this.progressWindow.pbStatus.Maximum; });
                else
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.pbStatus.Value += this.progressWindow.pbStatus.SmallChange; });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// Подписка на событие о загрузке кадров
        /// </summary>
        /// <param name="frameNumber">Номер кадра</param>
        /// <param name="isLastFrame">Последний кадр или нет</param>
        private void LoadingFramesProcessing(int frameNumber, bool isLastFrame)
        {
            try
            {
                if (isLastFrame)
                {
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.textInformation.Text = "Загрузка кадра: " + frameNumber.ToString(); });
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.pbStatus.Value = this.progressWindow.pbStatus.Maximum; });
                }
                else
                {
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.textInformation.Text = "Загрузка кадра: " + frameNumber.ToString(); });
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.pbStatus.Value += this.progressWindow.pbStatus.SmallChange; }); 
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// Отобрадение окна об исключительной ситуации
        /// </summary>
        /// <param name="message"></param>
        private void ShowExceptionMessage(string message)
        {
            OkButtonWindow infoWindow = InitializeOkButtonWindow();
            infoWindow.textInformation.Text = message;
            infoWindow.capitalText.Text = "Oops...";
            infoWindow.ShowDialog();
        }
        

        /// <summary>
        /// Инициализация экземпляра окна
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Инициализация экземпляра окна с pgorgess bar
        /// </summary>
        /// <returns></returns>
        private ProgressWindow InitializeProgressWindow()
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
