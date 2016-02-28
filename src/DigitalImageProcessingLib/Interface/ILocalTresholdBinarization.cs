using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Interface
{
    public interface ILocalTresholdBinarization: IBinarization
    {
        int WindowSize { get; set; }
    }
}
