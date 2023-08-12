using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class ColorFilterTest
    {
        public class FilterStub : ColorFilter
        {
            public float value;
            public override float Evaluate(float v) => value;
        }

        [TestCase(0f, 1f, 1f)]
        [TestCase(0.3f, 0.7f, 0.7f)]
        public void EvaluateAll(float v, float stubValue, float expected)
        {
            float[,] array = { { v, v, v, v } };
            var filter = new FilterStub() { value = stubValue };

            filter.EvaluateAll(ref array);

            Assert.That(array, Is.EqualTo(new float[,] { { expected, expected, expected, expected } }));
        }
    }
}
