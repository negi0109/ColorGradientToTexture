using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class ThresholdTest
    {
        [TestCase(1, 0.5, 0)]
        [TestCase(1, 1, 1)]
        public void Evaluate(double value, double v, double expected)
        {
            var filter = new Filters.Threshold() { value = value };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
