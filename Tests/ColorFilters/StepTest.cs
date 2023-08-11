using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class StepTest
    {
        [TestCase(2, 0.5f, 0.5f)]
        [TestCase(2, 0.49f, 0f)]
        [TestCase(2, 1f, 1f)]
        public void Evaluate(int step, float v, float expected)
        {
            var filter = new Filters.Step() { step = step };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
