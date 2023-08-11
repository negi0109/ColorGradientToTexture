using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class ScaleTest
    {
        [TestCase(2, 0.5f, 1f)]
        [TestCase(2, 1f, 2f)]
        public void Evaluate(float scale, float v, float expected)
        {
            var filter = new Filters.Scale() { value = scale };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
