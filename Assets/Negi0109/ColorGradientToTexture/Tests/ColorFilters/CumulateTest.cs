using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class CumulateTest
    {
        [TestCase(1, 2, 3, 1, 3, 6, Filters.Cumulate.Division.One)]
        [TestCase(1, 2, 3, 1 / 6f, 3 / 6f, 6f / 6f, Filters.Cumulate.Division.Max)]
        [TestCase(1, 2, 3, 1 / 3f, 3 / 3f, 6f / 3f, Filters.Cumulate.Division.Volume)]
        public void EvaluateAll(float v1, float v2, float v3, float r1, float r2, float r3, Filters.Cumulate.Division division)
        {
            var filter = new Filters.Cumulate() { division = division };

            {
                filter.direction = Filters.Cumulate.Direction.X01;
                var ar = new float[,] { { v1 }, { v2 }, { v3 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new float[,] { { r1 }, { r2 }, { r3 } }), "X01");
            }
            {
                filter.direction = Filters.Cumulate.Direction.X10;
                var ar = new float[,] { { v3 }, { v2 }, { v1 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new float[,] { { r3 }, { r2 }, { r1 } }), "X10");
            }
            {
                filter.direction = Filters.Cumulate.Direction.Y01;
                var ar = new float[,] { { v1, v2, v3 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new float[,] { { r1, r2, r3 } }), "Y01");
            }
            {
                filter.direction = Filters.Cumulate.Direction.Y10;
                var ar = new float[,] { { v3, v2, v1 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new float[,] { { r3, r2, r1 } }), "Y10");
            }
        }
    }
}
