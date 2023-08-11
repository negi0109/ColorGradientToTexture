using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class DiffTest
    {
        [TestCase(1, 2, 3, 1, 1, 1, Filters.Diff.Division.One)]
        [TestCase(1, 2, 3, 1, 1, 1, Filters.Diff.Division.Max)]
        [TestCase(1, 2, 4, 1 / 2f, 1 / 2f, 2 / 2f, Filters.Diff.Division.Max)]
        public void EvaluateAll(float v1, float v2, float v3, float r1, float r2, float r3, Filters.Diff.Division division)
        {
            var filter = new Filters.Diff() { division = division };

            {
                filter.direction = Filters.Diff.Direction.X01;
                var ar = new float[,] { { v1 }, { v2 }, { v3 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new float[,] { { r1 }, { r2 }, { r3 } }), "X01");
            }
            {
                filter.direction = Filters.Diff.Direction.X10;
                var ar = new float[,] { { v3 }, { v2 }, { v1 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new float[,] { { r3 }, { r2 }, { r1 } }), "X10");
            }
            {
                filter.direction = Filters.Diff.Direction.Y01;
                var ar = new float[,] { { v1, v2, v3 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new float[,] { { r1, r2, r3 } }), "Y01");
            }
            {
                filter.direction = Filters.Diff.Direction.Y10;
                var ar = new float[,] { { v3, v2, v1 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new float[,] { { r3, r2, r1 } }), "Y10");
            }
        }
    }
}
