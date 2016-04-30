using DigitalVideoProcessingLib.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public delegate void ExceptionOccured(string message);
    public class FileReader: IFileReader
    {
        public static event ExceptionOccured ExceptionOccuredEvent;
        /// <summary>
        /// Считывание информации о ключевых кадрах из файла
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task<List<KeyFrameIOInformation>> ReadKeyFramesInformationAsync(string fileName, int frameWidth, int frameHeight)
        {
            try
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
                        List<string> informationList = line.Split(new[] { ' ' }).ToList();

                        KeyFrameIOInformation keyFrameIOInformation = new KeyFrameIOInformation();
                        keyFrameIOInformation.Number = Convert.ToInt32(informationList[0]);
                        keyFrameIOInformation.NeedProcess = Convert.ToInt32(informationList[1]) == 1 ? true : false;
                        keyFrameIOInformation.VarienceAverageSWRation = Convert.ToDouble(informationList[2]);
                        keyFrameIOInformation.AspectRatio = Convert.ToDouble(informationList[3]);
                        keyFrameIOInformation.DiamiterSWRatio = Convert.ToDouble(informationList[4]);
                        keyFrameIOInformation.BbPixelsNumberMinRatio = Convert.ToDouble(informationList[5]);
                        keyFrameIOInformation.BbPixelsNumberMaxRatio = Convert.ToDouble(informationList[6]);
                        keyFrameIOInformation.ImageRegionHeightRationMin = Convert.ToDouble(informationList[7]);
                        keyFrameIOInformation.ImageRegionWidthRatioMin = Convert.ToDouble(informationList[8]);
                        keyFrameIOInformation.PairsHeightRatio = Convert.ToDouble(informationList[9]);
                        keyFrameIOInformation.PairsIntensityRatio = Convert.ToDouble(informationList[10]);
                        keyFrameIOInformation.PairsSWRatio = Convert.ToDouble(informationList[11]);
                        keyFrameIOInformation.PairsWidthDistanceSqrRatio = Convert.ToDouble(informationList[12]);
                        keyFrameIOInformation.PairsOccupationRatio = Convert.ToDouble(informationList[13]);
                        keyFrameIOInformation.MinLettersNumberInTextRegion = Convert.ToInt32(informationList[14]);
                        keyFrameIOInformation.GaussFilterSize = Convert.ToInt32(informationList[15]);
                        keyFrameIOInformation.GaussFilterSigma = Convert.ToDouble(informationList[16]);
                        keyFrameIOInformation.CannyLowTreshold = Convert.ToInt32(informationList[17]);
                        keyFrameIOInformation.CannyHighTreshold = Convert.ToInt32(informationList[18]);                        
                        keyFrameIOInformation.MergeByDirectionAndChainEnds = Convert.ToInt32(informationList[19]) == 1 ? true : false;
                        keyFrameIOInformation.UseAdaptiveSmoothing = Convert.ToInt32(informationList[20]) == 1 ? true : false;
                        keyFrameIOInformation.Width = frameWidth;
                        keyFrameIOInformation.Height = frameHeight;
                        keyFrameIOInformationList.Add(keyFrameIOInformation);
                    }
                    return keyFrameIOInformationList;
                });
            }
            catch (Exception exception)
            {
                ExceptionOccuredEvent(exception.Message);
                return null;
            }
        }
    }
}
