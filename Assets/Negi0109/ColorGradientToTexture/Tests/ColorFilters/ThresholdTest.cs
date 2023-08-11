using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class ThresholdTest
    {
        [TestCase(1f, 0.5f, 0)]
        [TestCase(1f, 1f, 1)]
        public void Evaluate(float value, float v, float expected)
        {
            var filter = new Filters.Threshold() { value = value };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
