using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.SWTData;
using DigitalImageProcessingLib.ThreadData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.SWT
{
    public class SWTFilterSmart : Filter
    {
        private GreyImage _minIntensityDirectionImage = null;
        private GreyImage _maxIntensityDirectionImage = null;
        private GreyImage _gradientMapX = null;
        private GreyImage _gradientMapY = null;

        private List<Ray> _rayMinIntensityDirection = null;
        private List<Ray> _rayMaxIntensityDirection = null;

        public GreyImage MinIntensityDirectionImage() { return this._minIntensityDirectionImage; }
        public GreyImage MaxIntensityDirectionImage() { return this._maxIntensityDirectionImage; }

        public SWTFilterSmart(GreyImage gradientMapX, GreyImage gradientMapY) 
        {
            if (gradientMapX == null)
                throw new ArgumentNullException("Null gradientMapX in ctor");
            if (gradientMapY == null)
                throw new ArgumentNullException("Null gradientMapY in ctor");
            this._rayMaxIntensityDirection = new List<Ray>();
            this._rayMinIntensityDirection = new List<Ray>();
            this._gradientMapX = gradientMapX;
            this._gradientMapY = gradientMapY;
        }

        /// <summary>
        /// Вычисление SWT карты изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        public override void Apply(GreyImage image)
        {
            if (image == null)
                throw new ArgumentNullException("Null image in Apply");        
            this._maxIntensityDirectionImage = (GreyImage)image.Copy();
            if (this._maxIntensityDirectionImage == null)
                throw new NullReferenceException("Null _minIntensityDirectionImage in Apply");
            this._minIntensityDirectionImage = (GreyImage)image.Copy();
            if (this._minIntensityDirectionImage == null)
                throw new NullReferenceException("Null _minIntensityDirectionImage in Apply");

            this._rayMaxIntensityDirection.Clear();
            this._rayMinIntensityDirection.Clear();

            Thread lightTextThread = new Thread(new ParameterizedThreadStart(this.FillMaxIntensityImage));
            Thread darkTextThread = new Thread(new ParameterizedThreadStart(this.FillMinIntensityImage));

            darkTextThread.Start(image);
            lightTextThread.Start(image);

            darkTextThread.Join();
            lightTextThread.Join();
        }
        /// <summary>
        /// Заполнение карты ширины штриха дл случая, когда текст светлее фона
        /// </summary>
        /// <param name="image">Изображение</param>
        private void FillMaxIntensityImage(object image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in FillMaxIntensityDirectionImage");
                FillStrokeImage((GreyImage)image, this._maxIntensityDirectionImage, this._rayMaxIntensityDirection, -1.0);
                TwoPassAlongRays(this._maxIntensityDirectionImage, this._rayMaxIntensityDirection);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }      
        /// <summary>
        /// Заполнение карты ширины штриха дл случая, когда текст темнее фона
        /// </summary>
        /// <param name="image">Изображение</param>
        private void FillMinIntensityImage(object image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in FillMaxIntensityDirectionImage");
                FillStrokeImage((GreyImage)image, this._minIntensityDirectionImage, this._rayMinIntensityDirection, 1.0);
                TwoPassAlongRays(this._minIntensityDirectionImage, this._rayMinIntensityDirection);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Заполнение swt карты
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="fillingImage">Заполняемое изображение</param>
        /// <param name="rays">МАссив лучей</param>
        /// <param name="multValue">Множитель (для темного текста или светлого)</param>
        private void FillStrokeImage(GreyImage image, GreyImage fillingImage, List<Ray> rays, double multValue)
        {
            try
            {
                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                double prec = 0.05;   //0.05

                for (int i = 1; i < imageHeight; i++)
                    for (int j = 1; j < imageWidth; j++)
                    {
                        if (image.Pixels[i, j].BorderType == BorderType.Border.STRONG) //&& !fillingImage.Pixels[i, j].StrokeWidth.WasProcessed)
                        {
                            Ray ray = new Ray();
                            RayPixel rayPixel = new RayPixel();
                            rayPixel.X = i;
                            rayPixel.Y = j;
                            
                            ray.Pixels.Add(rayPixel);

                            double curX = (double)i + 0.5;
                            double curY = (double)j + 0.5;

                            int curPixX = i;
                            int curPixY = j;

                            double gradientX = this._gradientMapX.Pixels[i, j].Gradient.GradientX;
                            double gradientY = this._gradientMapY.Pixels[i, j].Gradient.GradientY;

                            double magnitude = Math.Sqrt(gradientX * gradientX + gradientY * gradientY);

                            double stepX = gradientX / magnitude * multValue;
                            double stepY = gradientY / magnitude * multValue;
                           
                          //  double stepX = image.Pixels[i, j].Gradient.StepX * multValue;
                          //  double stepY = image.Pixels[i, j].Gradient.StepY * multValue;

                            while (true)
                            {
                                curX += stepX * prec;
                                curY += stepY * prec;

                                int nextPixelX = (int) Math.Floor(curX);
                                int nextPixelY = (int)Math.Floor(curY);

                                /// Если не тот же самый пискель
                                if (nextPixelX != curPixX || nextPixelY != curPixY)
                                {
                                    curPixX = nextPixelX;
                                    curPixY = nextPixelY;

                                    // если вышли за границы, обработка от 1 до N - 1, т.к. так построена градиентная карта Собелем
                                    if (curPixX < 1 || curPixX >= imageHeight || curPixY < 1 || curPixY >= imageWidth)
                                        break;

                                    RayPixel newRayPixel = new RayPixel();
                                    newRayPixel.X = curPixX;
                                    newRayPixel.Y = curPixY;
                                    ray.Pixels.Add(newRayPixel);

                                    // Если найденный пиксель тоже граничный
                                    if (image.Pixels[curPixX, curPixY].BorderType == BorderType.Border.STRONG)
                                    {
                                        double gradientXNewPixel = this._gradientMapX.Pixels[curPixX, curPixY].Gradient.GradientX;
                                        double gradientYNewPixel = this._gradientMapY.Pixels[curPixX, curPixY].Gradient.GradientY;

                                        double magnitudeNewPixel = Math.Sqrt(gradientX * gradientX + gradientY * gradientY);

                                        double stepXNewPixel = gradientXNewPixel / magnitudeNewPixel * multValue;
                                        double stepYNewPixel = gradientYNewPixel / magnitudeNewPixel * multValue;

                                       // double stepXNewPixel = image.Pixels[curPixY, curPixX].Gradient.StepX * multValue;
                                      //  double stepYNewPixel = image.Pixels[curPixY, curPixX].Gradient.StepY * multValue;

                                        // Нашли противоположный пиксель
                                        if (Math.Acos(stepX * -stepXNewPixel + stepY * -stepYNewPixel) < (double) (Math.PI / 2.0))
                                        {
                                            //float length = sqrt( ((float)r.q.x - (float)r.p.x)*((float)r.q.x - (float)r.p.x) + ((float)r.q.y - (float)r.p.y)*((float)r.q.y - (float)r.p.y));
                                            double length = Math.Sqrt(((double)newRayPixel.X - (double)rayPixel.X) * ((double)newRayPixel.X - (double)rayPixel.X) +
                                                                        ((double)newRayPixel.Y - (double)rayPixel.Y) * ((double)newRayPixel.Y - (double)rayPixel.Y));
                                            SetWidthValueToImage(fillingImage, ray, length);
                                            rays.Add(ray);
                                        }
                                        break;
                                    }                                    
                                }                                
                            }
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Установка значения ширины штриха в изображение вдоль луча
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="ray">Луч</param>
        /// <param name="widthValue">Ширина штриха</param>
        private void SetWidthValueToImage(GreyImage image, Ray ray, double widthValue)
        {
            try
            {
                int rayElementNumber = ray.Pixels.Count;

                for (int i = 0; i < rayElementNumber; i++)
                {
                    int pixelY = ray.Pixels[i].Y;
                    int pixelX = ray.Pixels[i].X;

                    double pixelStrokeWidth = image.Pixels[pixelX, pixelY].StrokeWidth.Width;

                    if (pixelStrokeWidth == StrokeWidthData.UNDEFINED_WIDTH || pixelStrokeWidth > widthValue)
                        image.Pixels[pixelX, pixelY].StrokeWidth.Width = widthValue;    
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Второй проход алгоритма
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="rays">Массив лучей</param>
        private void TwoPassAlongRays(GreyImage image, List<Ray> rays)
        {
            try
            {
                int raysNumber = rays.Count;
                for (int i = 0; i < raysNumber; i++)
                {
                    double mean = CalculeMeanStrokeWidth(image, rays[i]);
                    AveragingStrokeValue(image, rays[i], mean);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Усреднение значения ширин штриха для каждого пикселя каждого луча
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="ray">Луч</param>
        /// <param name="mean">Среднее значение ширины штриха</param>
        private void AveragingStrokeValue(GreyImage image, Ray ray, double mean)
        {
            try
            {
                int pixelsNumberInRay = ray.Pixels.Count;
                for (int i = 0; i < pixelsNumberInRay; i++)
                {
                    int pixelY = ray.Pixels[i].Y;
                    int pixelX = ray.Pixels[i].X;
                    if (image.Pixels[pixelX, pixelY].StrokeWidth.Width > mean)
                        image.Pixels[pixelX, pixelY].StrokeWidth.Width = mean;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Вычисление среднего значения ширины штриха для каждого луча
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="ray">Массив лучей</param>
        /// <returns></returns>
        private double CalculeMeanStrokeWidth(GreyImage image, Ray ray)
        {
            try
            {
                double summ = 0.0;
                int pixelsNumberInRay = ray.Pixels.Count;
                for (int i = 0; i < pixelsNumberInRay; i++)
                {
                    int pixelY = ray.Pixels[i].Y;
                    int pixelX = ray.Pixels[i].X;
                    summ += image.Pixels[pixelX, pixelY].StrokeWidth.Width;
                }
                return (double)summ / (double)pixelsNumberInRay;             
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

        public override GreyImage Apply(GreyImage image, int threadsNumber)
        {
            throw new NotImplementedException();
        }

        protected override void ApplyThread(object data)
        {
            throw new NotImplementedException();
        }
    }
}
