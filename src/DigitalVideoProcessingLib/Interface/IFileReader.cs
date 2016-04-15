using DigitalVideoProcessingLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Interface
{
    public interface IFileReader
    {
        Task<List<KeyFrameIOInformation>> ReadKeyFramesInformationAsync(string fileName);
    }
}
