using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class OneMinusTest
    {
        [TestCase(1f, 0f)]
        [TestCase(0.2f, 0.8f)]
        public void Evaluate(float v, float expected)
        {
            var filter = new Filters.OneMinus();

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
