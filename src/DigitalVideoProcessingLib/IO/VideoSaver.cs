using DigitalImageProcessingLib.IO;
using DigitalImageProcessingLib.RegionData;
using DigitalVideoProcessingLib.Graphics;
using DigitalVideoProcessingLib.Interface;
using DigitalVideoProcessingLib.VideoFrameType;
using DigitalVideoProcessingLib.VideoType;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public delegate void VideoFrameSaved(int frameNumber, bool isLastFrame);    
    public class VideoSaver: IVideoSaver
    {
        public static event VideoFrameSaved videoFrameSavedEvent;

        /// <summary>
        /// Сохранение видео как набора ключевых кадров
        /// </summary>
        /// <param name="video">Видео</param>
        /// <param name="pen">Кисть для отрисовки границ текстовых областей</param>
        /// <param name="fileName">Путь для сохранения</param>
        /// <param name="framesSubDir">Имя директории для сохранения ключевых кадров</param>
        /// <param name="frameExpansion">Расширение кадра</param>
        public Task<bool> SaveVideoAsync(GreyVideo video, System.Drawing.Pen pen, string fileName, string framesSubDir, string framesExpansion)
        {
            try
            {
                if (video == null)
                    throw new ArgumentNullException("Null video");
                if (fileName == null || fileName.Length == 0)
                    throw new ArgumentNullException("Null pathToSave");
                if (pen == null)
                    throw new ArgumentNullException("Null pen");

                return Task.Run(() =>
                {
                    SaveVideoFrames(video, pen, fileName, framesSubDir, framesExpansion);
                    return true;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        
        /// <summary>
        /// Сохранение одного кадра видео
        /// </summary>
        /// <param name="videoFrame"></param>
        /// <param name="pen"></param>
        /// <param name="saveFileName"></param>
        public Task<bool> SaveVideoFrameAsync(GreyVideoFrame videoFrame, System.Drawing.Pen pen, string saveFileName)
        {
            try
            {
                if (videoFrame == null || videoFrame.Frame == null)
                    throw new ArgumentNullException("Null videoFrame");
                if (saveFileName == null || saveFileName.Length == 0)
                    throw new ArgumentNullException("Null saveFileName");
                if (pen == null)
                    throw new ArgumentNullException("Null pen");

                return Task.Run(() =>
                {
                    BitmapConvertor bitmapConvertor = new BitmapConvertor();
                    Bitmap bitmapFrame = bitmapConvertor.ToBitmap(videoFrame.Frame);

                    Draw.DrawTextBoundingBoxes(bitmapFrame, videoFrame.Frame.TextRegions, pen);
                    bitmapFrame.Save(saveFileName);

                    return true;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }        

        /// <summary>
        /// Сохранение обработанных ключевых кадров видео
        /// </summary>
        /// <param name="video">Видео</param>
        /// <param name="pen">Кисть для выделения текстовых областей</param>
        /// <param name="fileName">Путь к сохранению</param>
        /// <param name="framesSubDir">Имя директории для сохранения ключевых кадров</param>
        /// <param name="frameExpansion">Расширение кадра</param>
        private void SaveVideoFrames(GreyVideo video, System.Drawing.Pen pen, string fileName, string framesSubDir, string frameExpansion)
        {
            try
            {
                if (video.Frames != null)
                {
                    int framesNumber = video.Frames.Count;
                    string framesDirName = Path.Combine(fileName, framesSubDir);
                    if (!Directory.Exists(framesDirName))
                        Directory.CreateDirectory(framesDirName);

                    for (int i = 0; i < framesNumber; i++)
                    {
                        string frameFileName = Path.Combine(framesDirName, i.ToString() + frameExpansion);
                        SaveVideoFrameAsync(video.Frames[i], pen, frameFileName);
                        if (i == framesNumber - 1)
                            videoFrameSavedEvent(i, true);
                        else
                            videoFrameSavedEvent(i, false);
                    }
                }                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

    }
}
