using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Interface
{
    public interface IVideoLoader
    {
        Task<List<Image<Bgr, Byte>>> LoadFramesAsync(object data);
        Task<int> CountFramesNumber(object data);
    }
}
