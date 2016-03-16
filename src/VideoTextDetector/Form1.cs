using DigitalVideoProcessingLib.Algorithms.KeyFrameExtraction;
using DigitalVideoProcessingLib.IO;
using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoTextDetector
{
    public partial class VideoForm : Form
    {
        public VideoForm()
        {
            InitializeComponent();
        }

        private void VideoForm_Load(object sender, EventArgs e)
        {
            progressBar.Step = 1;
        }

        private void сервисToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private async void загрузитьВидеоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    string fileName = dialog.FileName;

                    IOData ioData = new IOData() { FileName = fileName, FrameHeight = 600, FrameWidth = 800 };
                  
                    VideoLoader.frameLoaded += this.LoadingFramesProcessing;
                    VideoLoader videoLoader = new VideoLoader();
                    List<Image<Bgr, Byte>> frames = await videoLoader.LoadFramesAsync(ioData);
                    

                    EdgeBasedKeyFrameExtractor edgeBasedKeyFrameExtractor = new EdgeBasedKeyFrameExtractor();
                    List<GreyVideoFrame> keyFrames = await edgeBasedKeyFrameExtractor.ExtractKeyFrames(frames);

                    frames.Clear();

                    int a = 0;
                    a++;
                }           

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
                    progressBar.Invoke(new Action(() => progressBar.Value = progressBar.Maximum));
                    MessageBox.Show("Видео загружено!");
                }
                else
                    progressBar.Invoke(new Action(() => progressBar.PerformStep()));                
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}
