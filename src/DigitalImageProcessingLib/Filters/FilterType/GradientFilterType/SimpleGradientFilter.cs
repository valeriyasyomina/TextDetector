using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.ThreadData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.GradientFilterType
{
    public class SimpleGradientFilter : GradientFilter
    {
        /// <summary>
        /// Вычисление градиентной карты по горизонтали и вертикали
        /// </summary>
        /// <param name="image">Изображение</param>
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");

                this._gradientXMap = new GreyImage(image.Width, image.Height);
                this._gradientYMap = new GreyImage(image.Width, image.Height);

                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                for (int i = 0; i < imageHeight; i++)
                    for (int j = 0; j < imageWidth; j++)
                    {
                        this._gradientXMap.Pixels[i, j].Gradient.GradientX = image.Pixels[i, j].Color.Data - image.Pixels[i + 1, j].Color.Data;
                        this._gradientYMap.Pixels[i, j].Gradient.GradientY = image.Pixels[i, j].Color.Data - image.Pixels[i, j + 1].Color.Data;
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public override void Apply(RGBImage image)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вычисление градиентной карты (многопоточная)
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="threadsNumber">Число потоков</param>
        /// <returns></returns>
        public override GreyImage Apply(GreyImage image, int threadsNumber)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");               
                if (threadsNumber <= 0)
                    throw new ArgumentException("Error threadsNumber in Apply");

                this._gradientXMap = new GreyImage(image.Width, image.Height);
                this._gradientYMap = new GreyImage(image.Width, image.Height);
               
                this.Threads = new List<Thread>();
                int deltaI = image.Height / threadsNumber;           
                int lowIndexI = 0;
                int highIndexI = lowIndexI + deltaI;               

                for (int i = 0; i < threadsNumber; i++)
                {
                    if (i == threadsNumber - 1)
                        highIndexI = image.Height - 1;
                    MatrixFilterData matrixFilterData = new ThreadData.MatrixFilterData(image, lowIndexI, highIndexI, 0, image.Width - 1);
                    Thread thread = new Thread(new ParameterizedThreadStart(this.ApplyThread));
                    this.Threads.Add(thread);
                    this.Threads[i].Start(matrixFilterData);

                    lowIndexI = highIndexI;
                    highIndexI += deltaI;
                }
                WaitForThreads();
                return image;       
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Ожидание завершения потоков
        /// </summary>
        private void WaitForThreads()
        {
            try
            {
                int hreadsNumber = this.Threads.Count;
                for (int i = 0; i < hreadsNumber; i++)
                    this.Threads[i].Join();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Функция вычисления градиентной карты, выполняется в отдельном потоке
        /// </summary>
        /// <param name="data">Данные</param>
        protected override void ApplyThread(object data)
        {
            try
            {
                MatrixFilterData matrixFilterData = (MatrixFilterData)data;

                GreyImage image = matrixFilterData.GreyImage;
                int startI = matrixFilterData.StartIndexI;
                int endI = matrixFilterData.EndIndexI;
                int startJ = matrixFilterData.StartIndexJ;
                int endJ = matrixFilterData.EndIndexJ;             
              

                for (int i = startI; i < endI; i++)
                    for (int j = startJ; j < endJ; j++)
                    {
                        this._gradientXMap.Pixels[i, j].Gradient.GradientX = image.Pixels[i, j].Color.Data - image.Pixels[i + 1, j].Color.Data;
                        this._gradientYMap.Pixels[i, j].Gradient.GradientY = image.Pixels[i, j].Color.Data - image.Pixels[i, j + 1].Color.Data;
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
