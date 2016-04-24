using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.IO
{
    public class KeyFrameIOInformation
    {
        /// <summary>
        /// Ширина кадра
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Высота кадра
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Номер кадра в видеоряде
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Нуждается ли кадр в обработке
        /// </summary>
        public bool NeedProcess { get; set; }
    }
}
