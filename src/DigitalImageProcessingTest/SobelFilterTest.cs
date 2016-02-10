using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigitalImageProcessingLib.Filters.FilterType;
using DigitalImageProcessingLib.Filters.FilterType.EdgeDetectionFilterType;
using DigitalImageProcessingLib.ImageType;

namespace DigitalImageProcessingTest
{
    [TestClass]
    public class SobelFilterTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSobelFilter1()
        {
            //arrange
            EdgeDetectionFilter sobel = new SobelFilter();
            GreyImage image = new GreyImage(2, 2);

            //act
            sobel.Apply(image);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSobelFilter2()
        {
            //arrange
            EdgeDetectionFilter sobel = new SobelFilter();
            GreyImage image = new GreyImage(3, 2);

            //act
            sobel.Apply(image);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSobelFilter3()
        {
            //arrange
            EdgeDetectionFilter sobel = new SobelFilter();
            GreyImage image = new GreyImage(2, 3);

            //act
            sobel.Apply(image);
        }

        [TestMethod]
        public void TestSobelFilter4()
        {
            //arrange
            EdgeDetectionFilter sobel = new SobelFilter();
            GreyImage image = new GreyImage(3, 3);

            image.Pixels[0, 0].Color.Data = 50;
            image.Pixels[0, 1].Color.Data = 125;
            image.Pixels[0, 2].Color.Data = 22;

            image.Pixels[1, 0].Color.Data = 12;
            image.Pixels[1, 1].Color.Data = 17;
            image.Pixels[1, 2].Color.Data = 187;

            image.Pixels[2, 0].Color.Data = 201;
            image.Pixels[2, 1].Color.Data = 100;
            image.Pixels[2, 2].Color.Data = 45;

            GreyImage patternImage = new GreyImage(3, 3);

            patternImage.Pixels[0, 0].Color.Data = 50;
            patternImage.Pixels[0, 1].Color.Data = 125;
            patternImage.Pixels[0, 2].Color.Data = 22;

            patternImage.Pixels[1, 0].Color.Data = 12;
            patternImage.Pixels[1, 1].Color.Data = 17;
            patternImage.Pixels[1, 2].Color.Data = 187;

            patternImage.Pixels[2, 0].Color.Data = 201;
            patternImage.Pixels[2, 1].Color.Data = 100;
            patternImage.Pixels[2, 2].Color.Data = 45;

            patternImage.Pixels[1, 1].Gradient.Angle = 126;
            patternImage.Pixels[1, 1].Gradient.RoundGradientDirection = DigitalImageProcessingLib.GradientData.RoundGradientDirection.DEGREE_135;
            patternImage.Pixels[1, 1].Gradient.Strength = 207;

            //act
            sobel.Apply(image);

            //assert
            Assert.IsTrue(image.IsEqual(patternImage));
        }

        [TestMethod]
        public void TestSobelFilter5()
        {
            //arrange
            EdgeDetectionFilter sobel = new SobelFilter();
            GreyImage image = new GreyImage(5, 4);

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

            GreyImage patternImage = new GreyImage(5, 4);

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
            patternImage.Pixels[2, 2].Color.Data = 45;
            patternImage.Pixels[2, 3].Color.Data = 91;
            patternImage.Pixels[2, 4].Color.Data = 17;

            patternImage.Pixels[3, 0].Color.Data = 100;
            patternImage.Pixels[3, 1].Color.Data = 15;
            patternImage.Pixels[3, 2].Color.Data = 18;
            patternImage.Pixels[3, 3].Color.Data = 205;
            patternImage.Pixels[3, 4].Color.Data = 194;

            patternImage.Pixels[1, 1].Gradient.Strength = 207;
            patternImage.Pixels[1, 1].Gradient.Angle = 126;
            patternImage.Pixels[1, 1].Gradient.RoundGradientDirection = DigitalImageProcessingLib.GradientData.RoundGradientDirection.DEGREE_135;

            patternImage.Pixels[1, 2].Gradient.Strength = 186;
            patternImage.Pixels[1, 2].Gradient.Angle = 58;
            patternImage.Pixels[1, 2].Gradient.RoundGradientDirection = DigitalImageProcessingLib.GradientData.RoundGradientDirection.DEGREE__45;

            patternImage.Pixels[1, 3].Gradient.Strength = 305;
            patternImage.Pixels[1, 3].Gradient.Angle = 72;
            patternImage.Pixels[1, 3].Gradient.RoundGradientDirection = DigitalImageProcessingLib.GradientData.RoundGradientDirection.DEGREE_90;

            patternImage.Pixels[2, 1].Gradient.Strength = 234;
            patternImage.Pixels[2, 1].Gradient.Angle = 111;
            patternImage.Pixels[2, 1].Gradient.RoundGradientDirection = DigitalImageProcessingLib.GradientData.RoundGradientDirection.DEGREE_90;

            patternImage.Pixels[2, 2].Gradient.Strength = 205;
            patternImage.Pixels[2, 2].Gradient.Angle = 49;
            patternImage.Pixels[2, 2].Gradient.RoundGradientDirection = DigitalImageProcessingLib.GradientData.RoundGradientDirection.DEGREE__45;

            patternImage.Pixels[2, 3].Gradient.Strength = 423;
            patternImage.Pixels[2, 3].Gradient.Angle = 8;
            patternImage.Pixels[2, 3].Gradient.RoundGradientDirection = DigitalImageProcessingLib.GradientData.RoundGradientDirection.DEGREE_0;

            //act
            sobel.Apply(image);

            //assert
            Assert.IsTrue(image.IsEqual(patternImage));
        }
    }
}
