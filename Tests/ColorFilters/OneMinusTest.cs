using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class OneMinusTest
    {
        [TestCase(1, 0)]
        [TestCase(0.2, 0.8)]
        public void Evaluate(double v, double expected)
        {
            var filter = new Filters.OneMinus();

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
