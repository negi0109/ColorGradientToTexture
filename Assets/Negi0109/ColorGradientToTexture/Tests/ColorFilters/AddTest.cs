using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class AddTest
    {
        [TestCase(1, 0.5, 1.5)]
        [TestCase(1, 1, 2)]
        public void Evaluate(double value, double v, double expected)
        {
            var filter = new Filters.Add() { value = value };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
