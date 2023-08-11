using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class AddTest
    {
        [TestCase(1f, 0.5f, 1.5f)]
        [TestCase(1f, 1f, 2f)]
        public void Evaluate(float value, float v, float expected)
        {
            var filter = new Filters.Add() { value = value };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }
    }
}
