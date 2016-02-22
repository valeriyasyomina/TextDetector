using DigitalImageProcessingLib.ColorType;
using DigitalImageProcessingLib.ImageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalImageProcessingLib.Filters.FilterType.SWT
{
    public class SWTFilter: Filter
    {
        private GreyImage _minIntensityDirectionImage = null;
        private GreyImage _maxIntensityDirectionImage = null;
        private GreyImage _gaussSmoothedImage = null;

        public delegate bool CompareIntensity(int firstIntensity, int secondIntensity);
        public SWTFilter(GreyImage gaussSmoothedImage) 
        {
            if (gaussSmoothedImage == null)
                throw new ArgumentNullException("Null gaussSmoothedImage in ctor");
            this._gaussSmoothedImage = gaussSmoothedImage;
        }

        public GreyImage MinIntensityDirectionImage() { return this._minIntensityDirectionImage; }
        public GreyImage MaxIntensityDirectionImage() { return this._maxIntensityDirectionImage; }

        /// <summary>
        /// Применение SWT фильтра к серому изображению
        /// </summary>
        /// <param name="image">Серое изображение</param>
        public override void Apply(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in Apply");
                if (image.Height != this._gaussSmoothedImage.Height || image.Width != this._gaussSmoothedImage.Width)
                    throw new ArgumentException("Image must be the same size with gaussSmoothedImage in ctor");
                this._maxIntensityDirectionImage = (GreyImage) image.Copy();
                if (this._maxIntensityDirectionImage == null)
                    throw new NullReferenceException("Null _minIntensityDirectionImage in Apply");
                this._minIntensityDirectionImage = (GreyImage)image.Copy();
                if (this._minIntensityDirectionImage == null)
                    throw new NullReferenceException("Null _minIntensityDirectionImage in Apply");

                FillMinIntensityImage(image);              
                FillMaxIntensityImage(image);                              
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
        /// Заполняет матрицу в направлении увеличения интенсивности
        /// </summary>
        /// <param name="image">Изображение</param>
        private void FillMaxIntensityImage(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in FillMaxIntensityDirectionImage");
                FillStrokeImage(image, this._maxIntensityDirectionImage, this.MinIntensity);               
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Заполняет матрицу в направлении уменьшения интенсивности
        /// </summary>
        /// <param name="image">Изображение</param>
        private void FillMinIntensityImage(GreyImage image)
        {
            try
            {
                if (image == null)
                    throw new ArgumentNullException("Null image in FillMinIntensityImage");
                FillStrokeImage(image, this._minIntensityDirectionImage, this.MaxIntensity);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Вычисляет карту SWT для изображения
        /// </summary>
        /// <param name="image">Изображение, представляющее границы изображения</param>
        /// <param name="fillingImage">Изображения для заполнения SWT карты</param>
        /// <param name="comparator">Функция сравнения интенсивностей пикселей</param>
        private void FillStrokeImage(GreyImage image, GreyImage fillingImage, CompareIntensity comparator)
        {
            try
            {
                int imageHeight = image.Height - 1;
                int imageWidth = image.Width - 1;

                for (int i = 1; i < imageHeight; i++)
                    for (int j = 1; j < imageWidth; j++)
                    {
                        if (image.Pixels[i, j].BorderType == BorderType.Border.STRONG && !fillingImage.Pixels[i, j].StrokeWidth.WasProcessed)
                        {
                            int intensityI = 0;
                            int intensityJ = 0;
                            GetNeighboringPixel(comparator, i, j, ref intensityI, ref intensityJ);

                            if (intensityI == i && intensityJ == j + 1)
                                TrackRayRight(image, fillingImage, comparator, i, j);
                            else if (intensityI == i && intensityJ == j - 1)
                                TrackRayLeft(image, fillingImage, comparator, i, j);
                            else if (intensityJ == j && intensityI == i + 1)
                                TrackRayDown(image, fillingImage, comparator, i, j);
                            else if (intensityJ == j && intensityI == i - 1)
                                TrackRayUp(image, fillingImage, comparator, i, j);
                            else if (intensityI == i - 1 && intensityJ == j + 1)
                                TrackRayRightUp(image, fillingImage, comparator, i, j);
                            else if (intensityI == i + 1 && intensityJ == j + 1)
                                TrackRayRightDown(image, fillingImage, comparator, i, j);
                            else if (intensityI == i - 1 && intensityJ == j - 1)
                                TrackRayLeftUp(image, fillingImage, comparator, i, j);
                            else if (intensityI == i + 1 && intensityJ == j - 1)
                                TrackRayLeftDown(image, fillingImage, comparator, i, j); ;                            
                        }
                    }
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }      

        /// <summary>
        /// Сравнивает 2 значения интенсивности
        /// </summary>
        /// <param name="firstIntensity">Первое значение интенсивности</param>
        /// <param name="secondIntensity">Второе значение интенсивности</param>
        /// <returns>1 - firstIntensity > secondIntensity, 0 - иначе</returns>
        private bool MaxIntensity(int firstIntensity, int secondIntensity)
        {
            return firstIntensity > secondIntensity ? true : false;
        }

        /// <summary>
        /// Сравнивает 2 значения интенсивности
        /// </summary>
        /// <param name="firstIntensity">Первое значение интенсивности</param>
        /// <param name="secondIntensity">Второе значение интенсивности</param>
        /// <returns>1 - firstIntensity < secondIntensity, 0 - иначе</returns>
        private bool MinIntensity(int firstIntensity, int secondIntensity)
        {
            return firstIntensity < secondIntensity ? true : false;
        }

       
        /// <summary>
        /// Находит индексы пискеля - соседа с максимальной или миннимальной интенсивнотью в зависимости от comparator
        /// </summary>
        /// <param name="comparator">Функция сравнения интенсивностей</param>
        /// <param name="row">Номер строки текущего пикселя</param>
        /// <param name="column">Номер столбца текущего пикселя</param>
        /// <param name="intensityI">Номер строки искомого пикселя</param>
        /// <param name="intensityJ">Номер столбца искомого пикселя</param>
        private void GetNeighboringPixel(CompareIntensity comparator, int row, int column, ref int intensityI, ref int intensityJ)
        {
            int intensity = _gaussSmoothedImage.Pixels[row - 1, column - 1].Color.Data;
            intensityI = row - 1;
            intensityJ = column - 1;
            if (comparator(intensity, _gaussSmoothedImage.Pixels[row - 1, column].Color.Data))
            {
                intensity = _gaussSmoothedImage.Pixels[row - 1, column].Color.Data;
                intensityI = row - 1;
                intensityJ = column;
            }           
            if (comparator(intensity, _gaussSmoothedImage.Pixels[row - 1, column + 1].Color.Data))
            {
                intensity = _gaussSmoothedImage.Pixels[row - 1, column + 1].Color.Data;
                intensityI = row - 1;
                intensityJ = column + 1;
            }
            if (comparator(intensity, _gaussSmoothedImage.Pixels[row, column - 1].Color.Data))
            {
                intensity = _gaussSmoothedImage.Pixels[row, column - 1].Color.Data;
                intensityI = row;
                intensityJ = column - 1;
            }
            if (comparator(intensity, _gaussSmoothedImage.Pixels[row, column + 1].Color.Data))
            {
                intensity = _gaussSmoothedImage.Pixels[row, column + 1].Color.Data;
                intensityI = row;
                intensityJ = column + 1;
            }
            if (comparator(intensity, _gaussSmoothedImage.Pixels[row + 1, column - 1].Color.Data))
            {
                intensity = _gaussSmoothedImage.Pixels[row + 1, column - 1].Color.Data;
                intensityI = row + 1;
                intensityJ = column - 1;
            }
            if (comparator(intensity, _gaussSmoothedImage.Pixels[row + 1, column].Color.Data))
            {
                intensity = _gaussSmoothedImage.Pixels[row + 1, column].Color.Data;
                intensityI = row + 1;
                intensityJ = column;
            }
            if (comparator(intensity, _gaussSmoothedImage.Pixels[row + 1, column + 1].Color.Data))
            {
                intensity = _gaussSmoothedImage.Pixels[row + 1, column + 1].Color.Data;
                intensityI = row + 1;
                intensityJ = column + 1;
            }
        }

        /// <summary>
        /// Строит луч от пикселя влево 
        /// </summary>
        /// <param name="image">Изображение, представляющее границы</param>
        /// <param name="fillingImage">Изображение для карты SWT</param>
        /// <param name="comparator">Функция сравнение интенсивностей пикселей</param>
        /// <param name="row">Номер строки пикселя, от которого строится луч</param>
        /// <param name="column">Номер столбца пикселя, от которого строится луч</param>
        private void TrackRayLeft(GreyImage image, GreyImage fillingImage, CompareIntensity comparator, int row, int column)
        {
            try
            {
                int imageWidth = image.Width;
                int columnFrom = column;
                column -= 1;
                while (column >= 0 && image.Pixels[row, column].BorderType != BorderType.Border.STRONG)
                    --column;

                if (column >= 0)
                {
                    int intensityI = 0;
                    int intensityJ = 0;
                    GetNeighboringPixel(comparator, row, column, ref intensityI, ref intensityJ);
                    // if (intensityI == row && intensityJ == column + 1)
                    // {
                    fillingImage.Pixels[row, column].StrokeWidth.WasProcessed = true;
                   // FillRowWithStrokeWidth(fillingImage, column, columnFrom + 1, row, columnFrom - column + 1);
                    // }
                    //  else
                    FillRowWithStrokeWidth(fillingImage, column + 1, columnFrom, row, columnFrom - column - 1);

                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Строит луч от пикселя вправо 
        /// </summary>
        /// <param name="image">Изображение, представляющее границы</param>
        /// <param name="fillingImage">Изображение для карты SWT</param>
        /// <param name="comparator">Функция сравнение интенсивностей пикселей</param>
        /// <param name="row">Номер строки пикселя, от которого строится луч</param>
        /// <param name="column">Номер столбца пикселя, от которого строится луч</param>
        private void TrackRayRight(GreyImage image, GreyImage fillingImage, CompareIntensity comparator, int row, int column)
        {
            try
            {
                int imageWidth = image.Width;
                int columnFrom = column;
                column += 1;
                while (column < imageWidth && image.Pixels[row, column].BorderType != BorderType.Border.STRONG)
                    ++column;

                if (column < imageWidth)
                {
                    int intensityI = 0;
                    int intensityJ = 0;
                    GetNeighboringPixel(comparator, row, column, ref intensityI, ref intensityJ);

                    // if (intensityI == row && intensityJ == column - 1)
                    //  {
                    fillingImage.Pixels[row, column].StrokeWidth.WasProcessed = true;
                //    FillRowWithStrokeWidth(fillingImage, columnFrom, column + 1, row, column - columnFrom + 1);
                    //  }
                    //  else
                    FillRowWithStrokeWidth(fillingImage, columnFrom + 1, column, row, column - columnFrom - 1);

                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }

        /// <summary>
        /// Строит луч от пикселя вверх 
        /// </summary>
        /// <param name="image">Изображение, представляющее границы</param>
        /// <param name="fillingImage">Изображение для карты SWT</param>
        /// <param name="comparator">Функция сравнение интенсивностей пикселей</param>
        /// <param name="row">Номер строки пикселя, от которого строится луч</param>
        /// <param name="column">Номер столбца пикселя, от которого строится луч</param>
        private void TrackRayUp(GreyImage image, GreyImage fillingImage, CompareIntensity comparator, int row, int column)
        {
            try
            {
                int imageHeight = image.Height;
                int rowFrom = row;
                row -= 1;

                while (row >= 0 && image.Pixels[row, column].BorderType != BorderType.Border.STRONG)
                    --row;

                if (row >= 0)
                {
                    int intensityI = 0;
                    int intensityJ = 0;
                    GetNeighboringPixel(comparator, row, column, ref intensityI, ref intensityJ);

                    // if (column == intensityJ && intensityI == row + 1)
                    //  {
                    fillingImage.Pixels[row, column].StrokeWidth.WasProcessed = true;
                    //  FillColumnWithStrokeWidth(fillingImage, row, rowFrom + 1, column, rowFrom - row + 1);
                    // }
                    // else
                    FillColumnWithStrokeWidth(fillingImage, row + 1, rowFrom, column, rowFrom - row - 1);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Строит луч от пикселя вниз 
        /// </summary>
        /// <param name="image">Изображение, представляющее границы</param>
        /// <param name="fillingImage">Изображение для карты SWT</param>
        /// <param name="comparator">Функция сравнение интенсивностей пикселей</param>
        /// <param name="row">Номер строки пикселя, от которого строится луч</param>
        /// <param name="column">Номер столбца пикселя, от которого строится луч</param>
        private void TrackRayDown(GreyImage image, GreyImage fillingImage, CompareIntensity comparator, int row, int column)
        {
            try
            {
                int imageHeight = image.Height;
                int rowFrom = row;
                row += 1;

                while (row < imageHeight && image.Pixels[row, column].BorderType != BorderType.Border.STRONG)
                    ++row;

                if (row < imageHeight)
                {
                    int intensityI = 0;
                    int intensityJ = 0;
                    GetNeighboringPixel(comparator, row, column, ref intensityI, ref intensityJ);

                    // if (column == intensityJ && intensityI == row - 1)
                    //    {
                    fillingImage.Pixels[row, column].StrokeWidth.WasProcessed = true;
                    //   FillColumnWithStrokeWidth(fillingImage, rowFrom, row + 1, column, row - rowFrom + 1);
                    //  }
                    // else
                    FillColumnWithStrokeWidth(fillingImage, rowFrom + 1, row, column, row - rowFrom - 1);
                }

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Строит луч от пикселя влево вверх
        /// </summary>
        /// <param name="image">Изображение, представляющее границы</param>
        /// <param name="fillingImage">Изображение для карты SWT</param>
        /// <param name="comparator">Функция сравнение интенсивностей пикселей</param>
        /// <param name="row">Номер строки пикселя, от которого строится луч</param>
        /// <param name="column">Номер столбца пикселя, от которого строится луч</param>
        private void TrackRayLeftUp(GreyImage image, GreyImage fillingImage, CompareIntensity comparator, int row, int column)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;
                int columnFrom = column;
                int rowfrom = row;

                column -= 1;
                row -= 1;

                while (row >= 0 && column >= 0 && image.Pixels[row, column].BorderType != BorderType.Border.STRONG)
                {
                    --row;
                    --column;
                }

                if (row >= 0 && column >= 0)
                {
                    int intensityI = 0;
                    int intensityJ = 0;
                    GetNeighboringPixel(comparator, row, column, ref intensityI, ref intensityJ);

                    //  if (row == intensityI - 1 && column == intensityJ - 1)
                    //  {
                    fillingImage.Pixels[row, column].StrokeWidth.WasProcessed = true;
                    // FillDiagonaWithStrokeWidth(fillingImage, row, rowfrom + 1, column, 1, rowfrom - row + 1);
                    //  }
                    // else
                    FillDiagonaWithStrokeWidth(fillingImage, row + 1, rowfrom, column + 1, 1, rowfrom - row - 1);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Строит луч от пикселя влево вниз
        /// </summary>
        /// <param name="image">Изображение, представляющее границы</param>
        /// <param name="fillingImage">Изображение для карты SWT</param>
        /// <param name="comparator">Функция сравнение интенсивностей пикселей</param>
        /// <param name="row">Номер строки пикселя, от которого строится луч</param>
        /// <param name="column">Номер столбца пикселя, от которого строится луч</param>
        private void TrackRayLeftDown(GreyImage image, GreyImage fillingImage, CompareIntensity comparator, int row, int column)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;
                int columnFrom = column;
                int rowfrom = row;

                column -= 1;
                row += 1;

                while (row < imageHeight && column >= 0 && image.Pixels[row, column].BorderType != BorderType.Border.STRONG)
                {
                    ++row;
                    --column;
                }

                if (row < imageHeight && column >= 0)
                {
                    int intensityI = 0;
                    int intensityJ = 0;
                    GetNeighboringPixel(comparator, row, column, ref intensityI, ref intensityJ);

                    // if (row == intensityI + 1 && column == intensityJ - 1)
                    // {
                    fillingImage.Pixels[row, column].StrokeWidth.WasProcessed = true;
                    //  FillDiagonaWithStrokeWidth(fillingImage, rowfrom, row + 1, columnFrom, -1, row - rowfrom + 1);
                    // }
                    //  else
                    FillDiagonaWithStrokeWidth(fillingImage, rowfrom + 1, row, columnFrom - 1, -1, row - rowfrom - 1);
                }

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Строит луч от пикселя вправо вверх 
        /// </summary>
        /// <param name="image">Изображение, представляющее границы</param>
        /// <param name="fillingImage">Изображение для карты SWT</param>
        /// <param name="comparator">Функция сравнение интенсивностей пикселей</param>
        /// <param name="row">Номер строки пикселя, от которого строится луч</param>
        /// <param name="column">Номер столбца пикселя, от которого строится луч</param>
        private void TrackRayRightUp(GreyImage image, GreyImage fillingImage, CompareIntensity comparator, int row, int column)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;
                int columnFrom = column;
                int rowfrom = row;

                column += 1;
                row -= 1;

                while (row >= 0 && column < imageWidth && image.Pixels[row, column].BorderType != BorderType.Border.STRONG)
                {
                    --row;
                    ++column;
                }

                if (column < imageWidth && row >= 0)
                {
                    int intensityI = 0;
                    int intensityJ = 0;
                    GetNeighboringPixel(comparator, row, column, ref intensityI, ref intensityJ);

                    //  if (row == intensityI - 1 && column == intensityJ + 1)
                    // {
                    fillingImage.Pixels[row, column].StrokeWidth.WasProcessed = true;
                    //   FillDiagonaWithStrokeWidth(fillingImage, row, rowfrom + 1, column, -1, rowfrom - row + 1);
                    //   }
                    //   else
                    FillDiagonaWithStrokeWidth(fillingImage, row + 1, rowfrom, column - 1, -1, rowfrom - row - 1);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        
        /// <summary>
        /// Строит луч от пикселя вправо вниз 
        /// </summary>
        /// <param name="image">Изображение, представляющее границы</param>
        /// <param name="fillingImage">Изображение для карты SWT</param>
        /// <param name="comparator">Функция сравнение интенсивностей пикселей</param>
        /// <param name="row">Номер строки пикселя, от которого строится луч</param>
        /// <param name="column">Номер столбца пикселя, от которого строится луч</param>
        private void TrackRayRightDown(GreyImage image, GreyImage fillingImage, CompareIntensity comparator, int row, int column)
        {
            try
            {
                int imageHeight = image.Height;
                int imageWidth = image.Width;
                int columnFrom = column;
                int rowfrom = row;

                column += 1;
                row += 1;

                while (row < imageHeight && column < imageWidth && image.Pixels[row, column].BorderType != BorderType.Border.STRONG)
                {
                    ++row;
                    ++column;
                }

                if (row < imageHeight && column < imageWidth)
                {
                    int intensityI = 0;
                    int intensityJ = 0;
                    GetNeighboringPixel(comparator, row, column, ref intensityI, ref intensityJ);

                    //  if (row == intensityI + 1 && column == intensityJ + 1)
                    // {
                    fillingImage.Pixels[row, column].StrokeWidth.WasProcessed = true;
                    //  FillDiagonaWithStrokeWidth(fillingImage, rowfrom, row + 1, columnFrom, 1, row - rowfrom + 1);
                    //  }
                    // else
                    FillDiagonaWithStrokeWidth(fillingImage, rowfrom + 1, row, columnFrom + 1, 1, row - rowfrom - 1);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Заполняет строку матрицы - изображения заданным значением ширины штриха
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="columnFrom">Номер столбца, от которого начинаем заполнять</param>
        /// <param name="columnTo">Номер столбца, на котором заканчиваем заполнять</param>
        /// <param name="row">Номер строки</param>
        /// <param name="strokeWidth">Ширина штриха</param>
        private void FillRowWithStrokeWidth(GreyImage image, int columnFrom, int columnTo, int row, int strokeWidth)
        {
            try
            {
                for (int i = columnFrom; i < columnTo; i++)
                    image.Pixels[row, i].StrokeWidth.Width = strokeWidth;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Заполняет столбец матрицы - изображения заданным значением ширины штриха
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="rowFrom">Номер строки, от которой начинаем заполнять</param>
        /// <param name="rowTo">Номер строки, на которой заканчиваем заполнять</param>
        /// <param name="coloumn">Номер столбца</param>
        /// <param name="strokeWidth">Ширина штриха</param>
        private void FillColumnWithStrokeWidth(GreyImage image, int rowFrom, int rowTo, int coloumn, int strokeWidth)
        {
            try
            {
                for (int i = rowFrom; i < rowTo; i++)
                    image.Pixels[i, coloumn].StrokeWidth.Width = strokeWidth;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Заполняет диагональ матрицы - изображения заданным значением ширины штриха
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="rowFrom">Номер строки, от которой начинаем заполнять</param>
        /// <param name="rowTo">Номер строки, на которой заканчиваем заполнять</param>
        /// <param name="columnFrom">Номер столбца, от которого начинаем заполнять</param>         
        /// <param name="deltaColumn">Шаг по столбцу</param>
        /// <param name="strokeWidth">Ширина штриха</param>
        private void FillDiagonaWithStrokeWidth(GreyImage image, int rowFrom, int rowTo, int columnFrom, int deltaColumn, int strokeWidth)
        {
            try
            {
                for (int i = rowFrom, j = columnFrom; i < rowTo; i++, j += deltaColumn)
                    image.Pixels[i, j].StrokeWidth.Width = strokeWidth;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
