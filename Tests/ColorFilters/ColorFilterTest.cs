using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class ColorFilterTest
    {
        public class FilterStub : ColorFilter
        {
            public double value;
            public override double Evaluate(double v) => value;
        }

        [TestCase(0, 1, 1)]
        [TestCase(0.3, 0.7, 0.7)]
        public void EvaluateAll(double v, double stubValue, double expected)
        {
            double[,] array = { { v, v, v, v } };
            var filter = new FilterStub() { value = stubValue };

            filter.EvaluateAll(ref array);

            Assert.That(array, Is.EqualTo(new double[,] { { expected, expected, expected, expected } }));
        }
    }
}
