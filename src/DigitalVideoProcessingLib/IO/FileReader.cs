using DigitalVideoProcessingLib.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class FileReader: IFileReader
    {
        /// <summary>
        /// Считывание информации о ключевых кадрах из файла
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task<List<KeyFrameIOInformation>> ReadKeyFramesInformationAsync(string fileName, int frameWidth, int frameHeight)
        {
            if (fileName == null)
                throw new ArgumentNullException("Null fileName in ReadKeyFramesInformationAsync");
            if (frameWidth <= 0)
                throw new ArgumentException("Error frameWidth in ReadKeyFramesInformationAsync");
            if (frameHeight <= 0)
                throw new ArgumentException("Error frameHeight in ReadKeyFramesInformationAsync");

            return Task.Run(() =>
            {
                List<KeyFrameIOInformation> keyFrameIOInformationList = new List<KeyFrameIOInformation>();

                foreach (string line in File.ReadLines(fileName))
                {
                    Console.WriteLine("-- {0}", line);
                    List<int> informationList = line.Split(new[] { ' ' }).Select(Int32.Parse).ToList();

                    KeyFrameIOInformation keyFrameIOInformation = new KeyFrameIOInformation();
                    keyFrameIOInformation.Number = informationList[0];
                    keyFrameIOInformation.NeedProcess = informationList[1] == 1 ? true : false;
                    keyFrameIOInformation.Width = frameWidth;
                    keyFrameIOInformation.Height = frameHeight;
                    keyFrameIOInformationList.Add(keyFrameIOInformation);
                }
                return keyFrameIOInformationList;
            });
        }
    }
}
