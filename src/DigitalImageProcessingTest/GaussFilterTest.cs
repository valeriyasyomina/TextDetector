using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigitalImageProcessingLib.ImageType;
using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.Filters.FilterType.SmoothingFilterType;

namespace DigitalImageProcessingTest
{
    [TestClass]
    public class GaussFilterTest
    {
        [TestMethod]
        public void TestGaussFilter1()
        {
            //arrange
            GreyImage image = new GreyImage(5, 5);

            image.Pixels[0, 0].Color.Data = 50;
            image.Pixels[0, 1].Color.Data = 125;
            image.Pixels[0, 2].Color.Data = 22;
            image.Pixels[0, 3].Color.Data = 11;
            image.Pixels[0, 4].Color.Data = 104;

            image.Pixels[1, 0].Color.Data = 12;
            image.Pixels[1, 1].Color.Data = 17;
            image.Pixels[1, 2].Color.Data = 187;
            image.Pixels[1, 3].Color.Data = 0;
            image.Pixels[1, 4].Color.Data = 15;

            image.Pixels[2, 0].Color.Data = 201;
            image.Pixels[2, 1].Color.Data = 100;
            image.Pixels[2, 2].Color.Data = 45;
            image.Pixels[2, 3].Color.Data = 91;
            image.Pixels[2, 4].Color.Data = 17;

            image.Pixels[3, 0].Color.Data = 100;
            image.Pixels[3, 1].Color.Data = 15;
            image.Pixels[3, 2].Color.Data = 18;
            image.Pixels[3, 3].Color.Data = 205;
            image.Pixels[3, 4].Color.Data = 194;

            image.Pixels[4, 0].Color.Data = 0;
            image.Pixels[4, 1].Color.Data = 134;
            image.Pixels[4, 2].Color.Data = 45;
            image.Pixels[4, 3].Color.Data = 94;
            image.Pixels[4, 4].Color.Data = 112;

            GreyImage patternImage = new GreyImage(5, 5);

            patternImage.Pixels[0, 0].Color.Data = 50;
            patternImage.Pixels[0, 1].Color.Data = 125;
            patternImage.Pixels[0, 2].Color.Data = 22;
            patternImage.Pixels[0, 3].Color.Data = 11;
            patternImage.Pixels[0, 4].Color.Data = 104;

            patternImage.Pixels[1, 0].Color.Data = 12;
            patternImage.Pixels[1, 1].Color.Data = 17;
            patternImage.Pixels[1, 2].Color.Data = 187;
            patternImage.Pixels[1, 3].Color.Data = 0;
            patternImage.Pixels[1, 4].Color.Data = 15;

            patternImage.Pixels[2, 0].Color.Data = 201;
            patternImage.Pixels[2, 1].Color.Data = 100;
            patternImage.Pixels[2, 2].Color.Data = 77;   // changed elem
            patternImage.Pixels[2, 3].Color.Data = 91;
            patternImage.Pixels[2, 4].Color.Data = 17;

            patternImage.Pixels[3, 0].Color.Data = 100;
            patternImage.Pixels[3, 1].Color.Data = 15;
            patternImage.Pixels[3, 2].Color.Data = 18;
            patternImage.Pixels[3, 3].Color.Data = 205;
            patternImage.Pixels[3, 4].Color.Data = 194;

            patternImage.Pixels[4, 0].Color.Data = 0;
            patternImage.Pixels[4, 1].Color.Data = 134;
            patternImage.Pixels[4, 2].Color.Data = 45;
            patternImage.Pixels[4, 3].Color.Data = 94;
            patternImage.Pixels[4, 4].Color.Data = 112;

            SmoothingFilter gaussFilter = new GaussFilter(5, 1.4);   
            //act
            gaussFilter.Apply(image);

            //assert           
            Assert.IsTrue(image.IsEqual(patternImage));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGaussFilter2()
        {
            //arrange
            GreyImage image = new GreyImage(5, 3);
            SmoothingFilter gaussFilter = new GaussFilter(5, 1.4); 
            //act
            gaussFilter.Apply(image);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGaussFilter3()
        {
            //arrange
            GreyImage image = new GreyImage(2, 3);
            SmoothingFilter gaussFilter = new GaussFilter(5, 1.4);
            //act
            gaussFilter.Apply(image);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGaussFilter4()
        {
            //arrange
            GreyImage image = new GreyImage(4, 10);
            SmoothingFilter gaussFilter = new GaussFilter(5, 1.4);
            //act
            gaussFilter.Apply(image);
        }

        [TestMethod]
        public void TestGaussFilter5()
        {
            //arrange
            GreyImage image = new GreyImage(10, 7);

            image.Pixels[0, 0].Color.Data = 50;
            image.Pixels[0, 1].Color.Data = 125;
            image.Pixels[0, 2].Color.Data = 22;
            image.Pixels[0, 3].Color.Data = 110;
            image.Pixels[0, 4].Color.Data = 104;
            image.Pixels[0, 5].Color.Data = 11;
            image.Pixels[0, 6].Color.Data = 104;
            image.Pixels[0, 7].Color.Data = 111;
            image.Pixels[0, 8].Color.Data = 104;
            image.Pixels[0, 9].Color.Data = 248;
   

            image.Pixels[1, 0].Color.Data = 122;
            image.Pixels[1, 1].Color.Data = 17;
            image.Pixels[1, 2].Color.Data = 187;
            image.Pixels[1, 3].Color.Data = 0;
            image.Pixels[1, 4].Color.Data = 158;
            image.Pixels[1, 5].Color.Data = 12;
            image.Pixels[1, 6].Color.Data = 174;
            image.Pixels[1, 7].Color.Data = 187;
            image.Pixels[1, 8].Color.Data = 230;
            image.Pixels[1, 9].Color.Data = 152;

            image.Pixels[2, 0].Color.Data = 201;
            image.Pixels[2, 1].Color.Data = 100;
            image.Pixels[2, 2].Color.Data = 45;
            image.Pixels[2, 3].Color.Data = 91;
            image.Pixels[2, 4].Color.Data = 17;
            image.Pixels[2, 5].Color.Data = 201;
            image.Pixels[2, 6].Color.Data = 101;
            image.Pixels[2, 7].Color.Data = 145;
            image.Pixels[2, 8].Color.Data = 191;
            image.Pixels[2, 9].Color.Data = 178;

            image.Pixels[3, 0].Color.Data = 100;
            image.Pixels[3, 1].Color.Data = 15;
            image.Pixels[3, 2].Color.Data = 18;
            image.Pixels[3, 3].Color.Data = 205;
            image.Pixels[3, 4].Color.Data = 194;
            image.Pixels[3, 5].Color.Data = 140;
            image.Pixels[3, 6].Color.Data = 131;
            image.Pixels[3, 7].Color.Data = 18;
            image.Pixels[3, 8].Color.Data = 201;
            image.Pixels[3, 9].Color.Data = 194;

            image.Pixels[4, 0].Color.Data = 0;
            image.Pixels[4, 1].Color.Data = 134;
            image.Pixels[4, 2].Color.Data = 45;
            image.Pixels[4, 3].Color.Data = 94;
            image.Pixels[4, 4].Color.Data = 196;
            image.Pixels[4, 5].Color.Data = 0;
            image.Pixels[4, 6].Color.Data = 134;
            image.Pixels[4, 7].Color.Data = 134;
            image.Pixels[4, 8].Color.Data = 94;
            image.Pixels[4, 9].Color.Data = 110;

            image.Pixels[5, 0].Color.Data = 100;
            image.Pixels[5, 1].Color.Data = 132;
            image.Pixels[5, 2].Color.Data = 220;
            image.Pixels[5, 3].Color.Data = 13;
            image.Pixels[5, 4].Color.Data = 0;
            image.Pixels[5, 5].Color.Data = 0;
            image.Pixels[5, 6].Color.Data = 48;
            image.Pixels[5, 7].Color.Data = 154;
            image.Pixels[5, 8].Color.Data = 11;
            image.Pixels[5, 9].Color.Data = 111;

            image.Pixels[6, 0].Color.Data = 89;
            image.Pixels[6, 1].Color.Data = 34;
            image.Pixels[6, 2].Color.Data = 132;
            image.Pixels[6, 3].Color.Data = 58;
            image.Pixels[6, 4].Color.Data = 96;
            image.Pixels[6, 5].Color.Data = 97;
            image.Pixels[6, 6].Color.Data = 0;
            image.Pixels[6, 7].Color.Data = 57;
            image.Pixels[6, 8].Color.Data = 57;
            image.Pixels[6, 9].Color.Data = 200;

            GreyImage patternImage = new GreyImage(10, 7);

            patternImage.Pixels[0, 0].Color.Data = 50;
            patternImage.Pixels[0, 1].Color.Data = 125;
            patternImage.Pixels[0, 2].Color.Data = 22;
            patternImage.Pixels[0, 3].Color.Data = 110;
            patternImage.Pixels[0, 4].Color.Data = 104;
            patternImage.Pixels[0, 5].Color.Data = 11;
            patternImage.Pixels[0, 6].Color.Data = 104;
            patternImage.Pixels[0, 7].Color.Data = 111;
            patternImage.Pixels[0, 8].Color.Data = 104;
            patternImage.Pixels[0, 9].Color.Data = 248;


            patternImage.Pixels[1, 0].Color.Data = 122;
            patternImage.Pixels[1, 1].Color.Data = 17;
            patternImage.Pixels[1, 2].Color.Data = 187;
            patternImage.Pixels[1, 3].Color.Data = 0;
            patternImage.Pixels[1, 4].Color.Data = 158;
            patternImage.Pixels[1, 5].Color.Data = 12;
            patternImage.Pixels[1, 6].Color.Data = 174;
            patternImage.Pixels[1, 7].Color.Data = 187;
            patternImage.Pixels[1, 8].Color.Data = 230;
            patternImage.Pixels[1, 9].Color.Data = 152;

            patternImage.Pixels[2, 0].Color.Data = 201;
            patternImage.Pixels[2, 1].Color.Data = 100;
            patternImage.Pixels[2, 2].Color.Data = 87;
            patternImage.Pixels[2, 3].Color.Data = 93;
            patternImage.Pixels[2, 4].Color.Data = 106;
            patternImage.Pixels[2, 5].Color.Data = 113;
            patternImage.Pixels[2, 6].Color.Data = 125;
            patternImage.Pixels[2, 7].Color.Data = 140;
            patternImage.Pixels[2, 8].Color.Data = 191;
            patternImage.Pixels[2, 9].Color.Data = 178;

            patternImage.Pixels[3, 0].Color.Data = 100;
            patternImage.Pixels[3, 1].Color.Data = 15;
            patternImage.Pixels[3, 2].Color.Data = 90;
            patternImage.Pixels[3, 3].Color.Data = 98;
            patternImage.Pixels[3, 4].Color.Data = 108;
            patternImage.Pixels[3, 5].Color.Data = 111;
            patternImage.Pixels[3, 6].Color.Data = 115;
            patternImage.Pixels[3, 7].Color.Data = 125;
            patternImage.Pixels[3, 8].Color.Data = 201;
            patternImage.Pixels[3, 9].Color.Data = 194;

            patternImage.Pixels[4, 0].Color.Data = 0;
            patternImage.Pixels[4, 1].Color.Data = 134;
            patternImage.Pixels[4, 2].Color.Data = 93;
            patternImage.Pixels[4, 3].Color.Data = 96;
            patternImage.Pixels[4, 4].Color.Data = 94;
            patternImage.Pixels[4, 5].Color.Data = 92;
            patternImage.Pixels[4, 6].Color.Data = 93;
            patternImage.Pixels[4, 7].Color.Data = 103;
            patternImage.Pixels[4, 8].Color.Data = 94;
            patternImage.Pixels[4, 9].Color.Data = 110;

            patternImage.Pixels[5, 0].Color.Data = 100;
            patternImage.Pixels[5, 1].Color.Data = 132;
            patternImage.Pixels[5, 2].Color.Data = 220;
            patternImage.Pixels[5, 3].Color.Data = 13;
            patternImage.Pixels[5, 4].Color.Data = 0;
            patternImage.Pixels[5, 5].Color.Data = 0;
            patternImage.Pixels[5, 6].Color.Data = 48;
            patternImage.Pixels[5, 7].Color.Data = 154;
            patternImage.Pixels[5, 8].Color.Data = 11;
            patternImage.Pixels[5, 9].Color.Data = 111;

            patternImage.Pixels[6, 0].Color.Data = 89;
            patternImage.Pixels[6, 1].Color.Data = 34;
            patternImage.Pixels[6, 2].Color.Data = 132;
            patternImage.Pixels[6, 3].Color.Data = 58;
            patternImage.Pixels[6, 4].Color.Data = 96;
            patternImage.Pixels[6, 5].Color.Data = 97;
            patternImage.Pixels[6, 6].Color.Data = 0;
            patternImage.Pixels[6, 7].Color.Data = 57;
            patternImage.Pixels[6, 8].Color.Data = 57;
            patternImage.Pixels[6, 9].Color.Data = 200;

            SmoothingFilter gaussFilter = new GaussFilter(5, 1.4);
            //act
            gaussFilter.Apply(image);

            //assert           
            Assert.IsTrue(image.IsEqual(patternImage));
        }
    }
}
