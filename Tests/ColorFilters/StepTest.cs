using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class StepTest
    {
        [TestCase(2, 0.5, 0.5)]
        [TestCase(2, 0.49, 0)]
        [TestCase(2, 1, 1)]
        public void Evaluate(int step, double v, double expected)
        {
            var filter = new Filters.Step() { step = step };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
