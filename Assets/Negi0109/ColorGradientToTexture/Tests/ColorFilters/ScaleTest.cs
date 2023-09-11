using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class ScaleTest
    {
        [TestCase(2, 0.5, 1)]
        [TestCase(2, 1, 2)]
        public void Evaluate(double scale, double v, double expected)
        {
            var filter = new Filters.Scale() { value = scale };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
