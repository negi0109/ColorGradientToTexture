using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class RepeatTest
    {
        [TestCase(3, 0.5f, 0.5f)]
        [TestCase(3, 1f, 0f)]
        public void Evaluate(int count, float v, float expected)
        {
            var filter = new Filters.Repeat() { count = count };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
