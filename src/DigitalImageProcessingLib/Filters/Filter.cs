using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters
{
    public abstract class Filter
    {
        public abstract void Apply(GreyImage image);
        public abstract void Apply(RGBImage image);
        public abstract GreyImage Apply(GreyImage image, int threadsNumber);
        protected abstract void ApplyThread(object data);
        public List<Thread> Threads { get; protected set; }
        public int ThreadsNumber { get; protected set; }
    }
}
