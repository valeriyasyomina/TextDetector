using DigitalImageProcessingLib.Algorithms.EdgeDetection;
using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType;
using DigitalImageProcessingLib.Filters.FilterType.GradientFilterType;
using DigitalImageProcessingLib.Filters.FilterType.SmoothingFilterType;
using DigitalImageProcessingLib.IO;
using DigitalImageProcessingLib.RegionData;
using DigitalVideoProcessingLib.Algorithms.KeyFrameExtraction;
using DigitalVideoProcessingLib.Algorithms.TextDetection;
using DigitalVideoProcessingLib.Graphics;
using DigitalVideoProcessingLib.IO;
using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TextDetectionAccuracyEstimationLib.AccuracyEstimation;
using TextDetectionAccuracyEstimationLib.IO;
using TextDetectionAccuracyEstimationLib.Metrics;
using WPFVideoTextDetector.Command;
using WPFVideoTextDetector.Convertors;
using WPFVideoTextDetector.Views;

namespace WPFVideoTextDetector.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        #region Windows
        private MainWindow window = null;
        private LoaderWindow loaderWindow = null;
        private ProgressWindow progressWindow = null;
        #endregion

        #region Constants
        private static int UNDEFINED_FRAME_INDEX = -1;
        private static string DETECT_TEXT_VIDEO_FRAME_STRING = "Пожалуйста, подождите!\nИдет обработка кадра...";
        private static string DETECT_TEXT_VIDEO_STRING = "Пожалуйста, подождите!\nИдет обработка видео...";
        private static string FRAME_PROCESS_STRING = "Обработка кадра";
        private static string FRAME_PROCESS_SUCCESS_STRING = "Кадр был успешно обработан";
        private static string VIDEO_PROCESS_STRING = "Обработка видео";
        private static string VIDEO_PROCESS_SUCCESS_STRING = "Видео было успешно обработано";
        private static string SAVE_FRAME_STRING = "Сохранение кадра";
        private static string SAVE_FRAME_SUCCESS_STRING = "Кадр был успешно сохранен";
        private static string SAVE_PROCESSED_VIDEO_FRAMES_STRING = "Сохранение обработанных кадров видео";
        private static string SAVE_PROCESSED_VIDEO_FRAMES_SUCCESS_STRING = "Обработанные кадры видео были успешно сохранены";
        private static string LOAD_FRAME_STRING = "Загрука кадра";
        private static string LOAD_FRAME_SUCCESS_STRING = "Изображение было успешно загружено";
        private static string LOAD_VIDEO_STRING = "Загрузка видео";
        private static string VIDEO_INITIALIZATION_STRING = "Инициализация видео файла";
        private static string LOAD_VIDEO_SUCCESS_STRING = "Видео было успешно загружено";
        private static string PROCESS_DATA_FOR_OUTPUT_STRING = "Подождите, идет обработка данных\nдля вывода на экран";
        private static string XML_FILES_LOADED_SUCCESS_STRING = "XML - файлы были успешно загружены";
        private static string XML_FILES_LOADED_STRING = "Загрузка XML - файлов";
        private static string FILE_WAS_NOT_CHOOSEN_STRING = "Файл не был выбран";
        private static string XML_FILES_SAVE_SUCCESS_STRING = "XML - файл был успешно сохранен";
        private static string XML_FILES_SAVE_STRING = "Сохранение XML - файла";
        #endregion

        private bool isVideoTabSelected = true;
        private bool isProcessedVideoFramesTabSelected = false;
        private bool isVideoFrameTabSelected = false;
        private bool isProcessedVideoFrameTabSelected = false;


        private GreyVideo video = null;
        private GreyVideoFrame videoFrame = null;
        private Uri videoFileName = null;

        #region Image source variables
        private ImageSource processedFrameSource = null;
        private ImageSource frameSource = null;
        private ImageSource currentProcessedVideoFrameSource = null;
        private ImageSource previousProcessedVideoFrameSource = null;
        private ImageSource nextProcessedVideoFrameSource = null;
        #endregion

        private List<BitmapImage> processedVideoFramesBitmap = null;

        private int currentProcessedVideoFrameNumber = UNDEFINED_FRAME_INDEX;
        private int nextProcessedVideoFrameNumber = UNDEFINED_FRAME_INDEX;
        private int previousProcessedVideoFrameNumber = UNDEFINED_FRAME_INDEX;        

        public MainWindowViewModel(MainWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("Null window in ctor");
            this.window = window;
        }
        

        #region Visibility variables
        private Visibility mediaPlayerNavigationVisibility = Visibility.Hidden;
        private Visibility arrowLeftVisibility = Visibility.Hidden;
        private Visibility arrowRightVisibility = Visibility.Hidden;
        private Visibility currentProcessedFrameVisibility = Visibility.Hidden;
        private Visibility previousProcessedFrameVisibility = Visibility.Hidden;
        private Visibility nextProcessedFrameVisibility = Visibility.Hidden;
        private Visibility videoWasNotLoaded = Visibility.Visible;
        #endregion        


        #region Properties

        public bool IsVideoTabSelected
        {
            get
            {
                return this.isVideoTabSelected;
            }
            set
            {
                this.isVideoTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsProcessedVideoFramesTabSelected
        {
            get
            {
                return this.isProcessedVideoFramesTabSelected;
            }
            set
            {
                this.isProcessedVideoFramesTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsVideoFrameTabSelected
        {
            get
            {
                return this.isVideoFrameTabSelected;
            }
            set
            {
                this.isVideoFrameTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsProcessedVideoFrameTabSelected
        {
            get
            {
                return this.isProcessedVideoFrameTabSelected;
            }
            set
            {
                this.isProcessedVideoFrameTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand AboutProgramCommand
        {
            get
            {
                return new DelegateCommand(o => this.AboutProgramFunction());
            }
        }
        public ICommand RightArrowClickCommand
        {
            get
            {
                return new DelegateCommand(o => this.RightArrowClickFunction());
            }
        }

        public ICommand LeftArrowClickCommand
        {
            get
            {
                return new DelegateCommand(o => this.LeftArrowClickFunction());
            }
        }

        public ICommand DetectTextOnVideoKeyFramesCommand
        {
            get
            {
                return new DelegateCommand(o => this.DetectVideoTextFunction());
            }
        }

        public ICommand DetectTextOnFrameCommand
        {
            get
            {
                return new DelegateCommand(o => this.DetectVideoFrameTextFunction());
            }
        }

        public ICommand SaveVideoFileCommand
        {
            get
            {
                return new DelegateCommand(o => this.SaveVideoFileFunction());
            }
        }   
        public ICommand SaveVideoFrameCommand
        {
            get
            {
                return new DelegateCommand(o => this.SaveVideoFrameFunction());
            }
        }
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
        public ICommand AccuracyEstimationCommand
        {
            get
            {
                return new DelegateCommand(o => this.AccuracyEstimationFunction());
            }
        }

        public ImageSource FrameSource
        {
            get
            {
                return this.frameSource;
            }
            set
            {
                this.frameSource = value;
                NotifyPropertyChanged();
            }
        }

        public ImageSource ProcessedFrameSource
        { 
            get
            {
                return this.processedFrameSource;
            }
            set
            {
                this.processedFrameSource = value;
                NotifyPropertyChanged();
            }
        }

        public ImageSource CurrentProcessedVideoFrameSource
        {
            get
            {
                return this.currentProcessedVideoFrameSource;
            }
            set
            {
                this.currentProcessedVideoFrameSource = value;
                NotifyPropertyChanged();
            }
        }

        public ImageSource NextProcessedVideoFrameSource
        {
            get
            {
                return this.nextProcessedVideoFrameSource;
            }
            set
            {
                this.nextProcessedVideoFrameSource = value;
                NotifyPropertyChanged();
            }
        }

        public ImageSource PreviousProcessedVideoFrameSource
        {
            get
            {
                return this.previousProcessedVideoFrameSource;
            }
            set
            {
                this.previousProcessedVideoFrameSource = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility CurrentProcessedFrameVisibility
        {
            get
            {
                return this.currentProcessedFrameVisibility;
            }
            set
            {
                this.currentProcessedFrameVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility NextProcessedFrameVisibility
        {
            get
            {
                return this.nextProcessedFrameVisibility;
            }
            set
            {
                this.nextProcessedFrameVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility PreviousProcessedFrameVisibility
        {
            get
            {
                return this.previousProcessedFrameVisibility;
            }
            set
            {
                this.previousProcessedFrameVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility ArrowLeftVisibility
        {
            get
            {
                return this.arrowLeftVisibility;
            }
            set
            {
                this.arrowLeftVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility ArrowRightVisibility
        {
            get
            {
                return this.arrowRightVisibility;
            }
            set
            {
                this.arrowRightVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility VideoWasNotLoaded
        {
            get
            {
                return this.videoWasNotLoaded;
            }
            set
            {
                this.videoWasNotLoaded = value;
                NotifyPropertyChanged();
            }
        }
        public Visibility MediaPlayerNavigationVisibility
        {
            get
            {
                return this.mediaPlayerNavigationVisibility;
            }
            set
            {
                this.mediaPlayerNavigationVisibility = value;
                NotifyPropertyChanged();
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
        #endregion        


        #region Save region

        /// <summary>
        /// Сохранение единичного кадра видео после обработки
        /// </summary>
        private async void SaveVideoFrameFunction()
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "JPEG Image (.jpg)|*.jpg";
                dialog.ShowDialog();

                string pathToSaveFile = System.IO.Path.GetDirectoryName(dialog.FileName);
                string XMLFileName = System.IO.Path.Combine(pathToSaveFile, System.IO.Path.GetFileNameWithoutExtension(dialog.FileName) + ".xml");

                VideoSaver videoSaver = new VideoSaver();              
                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2);

                await videoSaver.SaveVideoFrameAsync(videoFrame, pen, dialog.FileName);
                await DigitalVideoProcessingLib.IO.XMLWriter.WriteTextBlocksInformation(videoFrame, XMLFileName);

                OkButtonWindow okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();
                okButtonWindow.capitalText.Text = SAVE_FRAME_STRING;
                okButtonWindow.textInformation.Text = SAVE_FRAME_SUCCESS_STRING;
                okButtonWindow.ShowDialog();          
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);           
            }
        }

        /// <summary>
        /// Сохранение обработанного видео в виде набора ключевых кадров
        /// </summary>
        private async void SaveVideoFileFunction()
        {
            try
            {
                progressWindow = ProgressWindow.InitializeProgressWindow();
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.ShowDialog();

                string XMLFileName = System.IO.Path.Combine(dialog.FileName, System.IO.Path.GetFileNameWithoutExtension(dialog.FileName) + ".xml");
                
                VideoSaver videoSaver = new VideoSaver();
                VideoSaver.videoFrameSavedEvent += this.VideoFrameSavedProcessing;
                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2);

                progressWindow.pbStatus.Value = 0.0;
                progressWindow.pbStatus.SmallChange = (double)progressWindow.pbStatus.Maximum / (double)this.video.Frames.Count;
                progressWindow.capitalText.Text = SAVE_PROCESSED_VIDEO_FRAMES_STRING;
                progressWindow.textInformation.Text = "";

                progressWindow.Show();
                await videoSaver.SaveVideoAsync(video, pen, dialog.FileName, "ProcessedFrames", ".jpg");
                await DigitalVideoProcessingLib.IO.XMLWriter.WriteTextBlocksInformation(video, XMLFileName);

                OkButtonWindow okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();
                okButtonWindow.capitalText.Text = SAVE_PROCESSED_VIDEO_FRAMES_STRING;
                okButtonWindow.textInformation.Text = SAVE_PROCESSED_VIDEO_FRAMES_SUCCESS_STRING;

                progressWindow.Hide();    
                okButtonWindow.ShowDialog();                    
            }
            catch (Exception exception)
            {
                progressWindow.Hide();   
                ShowExceptionMessage(exception.Message);
            }
        }
        #endregion


        #region Video frames gallery

        /// <summary>
        /// Нажатие на кнопку "Влево" для просмотра предыдущего обработанного кадра видеоролика
        /// </summary>
        private void LeftArrowClickFunction()
        {
            try
            {
                this.nextProcessedVideoFrameNumber = this.currentProcessedVideoFrameNumber;
                this.currentProcessedVideoFrameNumber = this.previousProcessedVideoFrameNumber;
                --this.previousProcessedVideoFrameNumber;

                if (this.previousProcessedVideoFrameNumber == UNDEFINED_FRAME_INDEX)
                {
                    this.ArrowLeftVisibility = Visibility.Hidden;
                    this.PreviousProcessedFrameVisibility = Visibility.Hidden;
                    this.PreviousProcessedVideoFrameSource = null;
                }
                this.CurrentProcessedVideoFrameSource = this.processedVideoFramesBitmap[this.currentProcessedVideoFrameNumber];
                this.NextProcessedVideoFrameSource = this.processedVideoFramesBitmap[this.nextProcessedVideoFrameNumber];
                this.ArrowRightVisibility = Visibility.Visible;
                this.NextProcessedFrameVisibility = Visibility.Visible;
            }
            catch (Exception exception)
            {
                this.ShowExceptionMessage(exception.Message);
            }
        }
        /// <summary>
        /// Нажатие на кнопку "Вправо" для просмотра следующего обработанного кадра видеоролика
        /// </summary>
        private void RightArrowClickFunction()
        {
            try
            {
                this.previousProcessedVideoFrameNumber = this.currentProcessedVideoFrameNumber;
                this.currentProcessedVideoFrameNumber = this.nextProcessedVideoFrameNumber;                
                ++this.nextProcessedVideoFrameNumber;

                if (this.currentProcessedVideoFrameNumber == this.processedVideoFramesBitmap.Count - 1)
                {
                    this.ArrowRightVisibility = Visibility.Hidden;
                    this.NextProcessedFrameVisibility = Visibility.Hidden;
                    this.NextProcessedVideoFrameSource = null;
                }
                this.CurrentProcessedVideoFrameSource = this.processedVideoFramesBitmap[this.currentProcessedVideoFrameNumber];
                this.PreviousProcessedVideoFrameSource = this.processedVideoFramesBitmap[this.previousProcessedVideoFrameNumber];
                this.ArrowLeftVisibility = Visibility.Visible;
                this.PreviousProcessedFrameVisibility = Visibility.Visible;
            }
            catch (Exception exception)
            {
                this.ShowExceptionMessage(exception.Message);
            }
        }

        /// <summary>
        /// Инициализация галереи просмотра обработанных ключевых кадров видеоролика
        /// </summary>
        private void InitializeProcessedVideoFramesGallery()
        {
            try
            {
                this.currentProcessedVideoFrameNumber = 0;
                this.CurrentProcessedVideoFrameSource = this.processedVideoFramesBitmap[this.currentProcessedVideoFrameNumber];
                this.CurrentProcessedFrameVisibility = Visibility.Visible;

                if (this.processedVideoFramesBitmap.Count > 1)
                {
                    this.nextProcessedVideoFrameNumber = 1;
                    this.NextProcessedVideoFrameSource = this.processedVideoFramesBitmap[this.nextProcessedVideoFrameNumber];
                    this.NextProcessedFrameVisibility = Visibility.Visible;
                    this.ArrowRightVisibility = Visibility.Visible;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Запись обработанных ключевых кадров видео в виде BitmapImage, отрисовка прямоугольников текстовых областей
        /// </summary>
        private void CreateBitmapsFromProcessedVideoFrames()
        {
            try
            {
                this.processedVideoFramesBitmap = new List<BitmapImage>();
                BitmapConvertor bitmapConvertor = new BitmapConvertor();
                BitmapImageConvertor bitmapImageConvertor = new Convertors.BitmapImageConvertor();

                for (int i = 0; i < this.video.Frames.Count; i++)
                {
                    Bitmap bitmapFrame = bitmapConvertor.ToBitmap(this.video.Frames[i].Frame);
                    Draw.DrawTextBoundingBoxes(bitmapFrame, this.video.Frames[i].Frame.TextRegions, new System.Drawing.Pen(System.Drawing.Color.Red, 2));
                    this.processedVideoFramesBitmap.Add(bitmapImageConvertor.BitmapToBitmapImage(bitmapFrame));
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion


        #region Detect text region

        /// <summary>
        /// Выделение текста на на единичном кадре видео
        /// </summary>
        private async void DetectVideoFrameTextFunction()
        {
            try
            {
                SWTVideoTextDetection SWTVideoTextDetection = new SWTVideoTextDetection(0.5);
                
                this.StartLoader(DETECT_TEXT_VIDEO_FRAME_STRING);
                await SWTVideoTextDetection.DetectText(this.videoFrame, 4);
                this.StopLoader();

                OkButtonWindow okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();
                okButtonWindow.capitalText.Text = FRAME_PROCESS_STRING;
                okButtonWindow.textInformation.Text = FRAME_PROCESS_SUCCESS_STRING;
                okButtonWindow.ShowDialog();

                BitmapConvertor bitmapConvertor = new BitmapConvertor();
                Bitmap bitmapFrame = bitmapConvertor.ToBitmap(this.videoFrame.Frame);
                Draw.DrawTextBoundingBoxes(bitmapFrame, this.videoFrame.Frame.TextRegions, new System.Drawing.Pen(System.Drawing.Color.Red, 2));

                BitmapImageConvertor bitmapImageConvertor = new Convertors.BitmapImageConvertor();
                this.ProcessedFrameSource = bitmapImageConvertor.BitmapToBitmapImage(bitmapFrame);

                this.IsVideoFrameTabSelected = false;
                this.IsProcessedVideoFramesTabSelected = false;
                this.IsProcessedVideoFrameTabSelected = true;
                this.IsVideoTabSelected = false;
            }
            catch (Exception exception)
            {
                this.StopLoader();
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
                SWTVideoTextDetection SWTVideoTextDetection = new SWTVideoTextDetection(0.5);
                this.StartLoader(DETECT_TEXT_VIDEO_STRING);
                await SWTVideoTextDetection.DetectText(this.video, 4);
                this.StopLoader();

                OkButtonWindow okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();
                okButtonWindow.capitalText.Text = VIDEO_PROCESS_STRING;
                okButtonWindow.textInformation.Text = VIDEO_PROCESS_SUCCESS_STRING;
                okButtonWindow.ShowDialog();

                this.StartLoader(PROCESS_DATA_FOR_OUTPUT_STRING);
                this.CreateBitmapsFromProcessedVideoFrames();
                this.InitializeProcessedVideoFramesGallery();
                this.StopLoader();

                this.IsVideoFrameTabSelected = false;
                this.IsProcessedVideoFramesTabSelected = true;
                this.IsProcessedVideoFrameTabSelected = false;
                this.IsVideoTabSelected = false;
            }
            catch (Exception exception)
            {
                this.StopLoader();
                ShowExceptionMessage(exception.Message);              
            }
        }
        #endregion

        
        #region Load video and video frames region

        /// <summary>
        /// Загрузка единичного кадра видео 
        /// </summary>
        private async void LoadVideoFrameFunction()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "JPEG Image (.jpg)|*.jpg";
                dialog.ShowDialog();

                IOData ioData = new IOData() { FileName = dialog.FileName, FrameHeight = 600, FrameWidth = 800 };
                VideoLoader.frameLoadedEvent += this.LoadingFramesProcessing;
                VideoLoader videoLoader = new VideoLoader();

                FrameLoader frameLoader = new FrameLoader();
                videoFrame = await frameLoader.LoadFrameAsync(ioData);

                OkButtonWindow okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();
                okButtonWindow.textInformation.Text = LOAD_FRAME_SUCCESS_STRING;
                okButtonWindow.capitalText.Text = LOAD_FRAME_STRING;
                okButtonWindow.ShowDialog();

                Bitmap bitmapFrame = new Bitmap(dialog.FileName);
                BitmapImageConvertor bitmapImageConvertor = new Convertors.BitmapImageConvertor();
                this.FrameSource = bitmapImageConvertor.BitmapToBitmapImage(bitmapFrame);
                this.ProcessedFrameSource = null;

                this.IsVideoFrameTabSelected = true;
                this.IsProcessedVideoFramesTabSelected = false;
                this.IsProcessedVideoFrameTabSelected = false;
                this.IsVideoTabSelected = false;
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
                this.video = new GreyVideo();

                progressWindow = ProgressWindow.InitializeProgressWindow();
                OkButtonWindow okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "MP4 Image (.mp4)|*.mp4|3gp (.3gp)|*.3gp|Avi (.avi)|*.avi";
                dialog.ShowDialog();

                progressWindow.capitalText.Text = LOAD_VIDEO_STRING;
                progressWindow.textInformation.Text = VIDEO_INITIALIZATION_STRING;
                progressWindow.Show();

                string videoFilePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                string videoFileName = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                string keyFramesInormatyionFilePath = System.IO.Path.Combine(videoFilePath, videoFileName + ".txt");

                IOData ioData = new IOData() { FileName = dialog.FileName, FrameHeight = 480, FrameWidth = 640 };

                VideoLoader videoLoader = new VideoLoader();
                int framesNumber = await videoLoader.CountFramesNumberAsync(ioData);
                progressWindow.pbStatus.SmallChange = (double)progressWindow.pbStatus.Maximum / (double)framesNumber;

                /// Если существует файл с информацией о ключевых кадрах
                if (System.IO.File.Exists(keyFramesInormatyionFilePath))
                {
                    FileReader fileReader = new FileReader();
                    List<KeyFrameIOInformation> keyFrameIOInformation = await fileReader.ReadKeyFramesInformationAsync(keyFramesInormatyionFilePath);
                    ioData.KeyFrameIOInformation = keyFrameIOInformation;                    

                    EdgeBasedKeyFrameExtractor.keyFrameExtractedEvent += this.KeyFramesExtractionProcessing;
                    EdgeBasedKeyFrameExtractor edgeBasedKeyFrameExtractor = new EdgeBasedKeyFrameExtractor();
                    this.video.Frames = await edgeBasedKeyFrameExtractor.ExtractKeyFramesByListNumberAsync(ioData);
                }
                else   /// Если нет, то выделяем кадры алгоритмически
                {
                    EdgeBasedKeyFrameExtractor.keyFrameExtractedEvent += this.KeyFramesExtractionProcessing;
                    EdgeBasedKeyFrameExtractor.framesDifferenceEvent += this.FramesDifferenceCalculateProcessing;
                    EdgeBasedKeyFrameExtractor edgeBasedKeyFrameExtractor = new EdgeBasedKeyFrameExtractor();
                    this.video.Frames = await edgeBasedKeyFrameExtractor.ExtractKeyFramesTwoPassAsync(ioData);
                }
                okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();
                okButtonWindow.capitalText.Text = LOAD_VIDEO_STRING;
                okButtonWindow.textInformation.Text = LOAD_VIDEO_SUCCESS_STRING;
                okButtonWindow.ShowDialog();

                progressWindow.Close();                
                this.VideoFileName = new Uri(dialog.FileName);
                this.MediaPlayerNavigationVisibility = Visibility.Visible;
                this.VideoWasNotLoaded = Visibility.Hidden;

                this.IsVideoFrameTabSelected = false;
                this.IsProcessedVideoFramesTabSelected = false;
                this.IsProcessedVideoFrameTabSelected = false;
                this.IsVideoTabSelected = true;
            }
            catch (Exception exception)
            {
                progressWindow.Hide();
                ShowExceptionMessage(exception.Message);             
            }
        }
        #endregion


        #region Event handlers region

        private void VideoFrameSavedProcessing(int frameNumber, bool isLastFrame)
        {
            try
            {
                this.progressWindow.Dispatcher.Invoke(delegate
                {
                    this.progressWindow.textInformation.Text = "Сохранение кадра: " + frameNumber.ToString();                                                                
                });
                if (isLastFrame)
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.pbStatus.Value = this.progressWindow.pbStatus.Maximum; });
                else
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.pbStatus.Value += this.progressWindow.pbStatus.SmallChange; });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Подписка на событие о вычислении разниц кадров
        /// </summary>
        /// <param name="firstFrameNumber">Номер первого кадра</param>
        /// <param name="secondFrameNumber">Номер второго кадра</param>
        private void FramesDifferenceCalculateProcessing(int firstFrameNumber, int secondFrameNumber, bool isLastFrame)
        {
            try
            {
                this.progressWindow.Dispatcher.Invoke(delegate
                {
                    this.progressWindow.textInformation.Text = "Вычисление разницы кадров: " +
                                                                 firstFrameNumber.ToString() + " и " +
                                                                 secondFrameNumber.ToString();
                });
                if (isLastFrame)
                    this.progressWindow.Dispatcher.Invoke(delegate { this.progressWindow.pbStatus.Value = 0; });
                else
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
        #endregion


        #region Addition region
        /// <summary>
        /// Отобрадение окна об исключительной ситуации
        /// </summary>
        /// <param name="message"></param>
        private void ShowExceptionMessage(string message)
        {
            OkButtonWindow infoWindow = OkButtonWindow.InitializeOkButtonWindow();
            infoWindow.textInformation.Text = message;
            infoWindow.capitalText.Text = "Oops...";
            infoWindow.ShowDialog();
        }
        private void AboutProgramFunction()
        {           
        }
        
        #endregion

        #region Loader region

        /// <summary>
        /// Запуск загрузчика
        /// </summary>
        /// <param name="textInfromation">Текстовое сообщение</param>
        private void StartLoader(string textInfromation)
        {
            try
            {
                this.window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    loaderWindow = new LoaderWindow();
                    loaderWindow.Show();
                    LoaderWindowViewModel loaderWindowViewModel = (LoaderWindowViewModel)loaderWindow.DataContext;
                    loaderWindowViewModel.TextInformation = textInfromation;
                    loaderWindowViewModel.StartAnimation();                  
                }));
            }
            catch (Exception exception)
            {
                this.ShowExceptionMessage(exception.Message);
            }
        }

        /// <summary>
        /// Остановка загрузчика
        /// </summary>       
        private void StopLoader()
        {
            try
            {
                this.window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoaderWindowViewModel loaderWindowViewModel = (LoaderWindowViewModel)loaderWindow.DataContext;
                    loaderWindowViewModel.StopAnimation();
                    loaderWindow.Close();
                }));
            }
            catch (Exception exception)
            {
                this.ShowExceptionMessage(exception.Message);
            }
        }

        #endregion

        #region Accuracy estimation
        /// <summary>
        /// Подсчет значений метрик для алгоритма локализации, вычисление точности обнаружения текстовых областей
        /// </summary>
        private async void AccuracyEstimationFunction()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Выберите эталонный XML - файл";
                dialog.Filter = "XML - files (.xml)|*.xml";
                dialog.ShowDialog();
                string patternXMLFileName = dialog.FileName;
                if (patternXMLFileName.Length == 0)
                    ShowExceptionMessage(FILE_WAS_NOT_CHOOSEN_STRING);
                else
                {
                    dialog.Title = "Выберите XML файл, сгенерированный программой";
                    dialog.ShowDialog();
                    string generatedXMLFileName = dialog.FileName;
                    if (generatedXMLFileName.Length == 0)
                        ShowExceptionMessage(FILE_WAS_NOT_CHOOSEN_STRING);
                    else
                    {
                        Dictionary<int, List<TextRegion>> patternFramesTextBlocksInformation = await XMLReader.ReadTextBlocksInformation(patternXMLFileName);
                        Dictionary<int, List<TextRegion>> generatedFramesTextBlocksInformation = await XMLReader.ReadTextBlocksInformation(generatedXMLFileName);

                        OkButtonWindow okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();
                        okButtonWindow.capitalText.Text = XML_FILES_LOADED_STRING;
                        okButtonWindow.textInformation.Text = XML_FILES_LOADED_SUCCESS_STRING;
                        okButtonWindow.ShowDialog();

                        IAccuracyEstimation accuracyEstimator = new AccuracyEstimator();
                        Dictionary<int, List<Metric>> metrics = await accuracyEstimator.CalculateMetrics(patternFramesTextBlocksInformation, generatedFramesTextBlocksInformation);
                                                  
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.Filter = "XML - files (.xml)|*.xml";
                        saveDialog.ShowDialog();

                        await TextDetectionAccuracyEstimationLib.IO.XMLWriter.WriteMetricsXML(saveDialog.FileName, metrics);

                        okButtonWindow = OkButtonWindow.InitializeOkButtonWindow();
                        okButtonWindow.capitalText.Text = XML_FILES_SAVE_STRING;
                        okButtonWindow.textInformation.Text = XML_FILES_SAVE_SUCCESS_STRING;
                        okButtonWindow.ShowDialog();
                    }
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);
            }
        }

        #endregion
    }
}
