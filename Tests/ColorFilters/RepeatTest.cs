using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class RepeatTest
    {
        [TestCase(3, 0.5, 0.5)]
        [TestCase(3, 1, 0)]
        public void Evaluate(int count, double v, double expected)
        {
            var filter = new Filters.Repeat() { count = count };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
