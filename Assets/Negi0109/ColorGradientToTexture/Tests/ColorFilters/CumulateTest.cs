using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class CumulateTest
    {
        [TestCase(1, 2, 3, 1, 3, 6, Filters.Cumulate.Division.One)]
        [TestCase(1, 2, 3, 1.0 / 6, 3.0 / 6, 6.0 / 6, Filters.Cumulate.Division.Max)]
        [TestCase(1, 2, 3, 1.0 / 3, 3.0 / 3, 6.0 / 3, Filters.Cumulate.Division.Volume)]
        public void EvaluateAll(double v1, double v2, double v3, double r1, double r2, double r3, Filters.Cumulate.Division division)
        {
            var filter = new Filters.Cumulate() { division = division };

            {
                filter.direction = Filters.Cumulate.Direction.X01;
                var ar = new double[,] { { v1 }, { v2 }, { v3 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new double[,] { { r1 }, { r2 }, { r3 } }), "X01");
            }
            {
                filter.direction = Filters.Cumulate.Direction.X10;
                var ar = new double[,] { { v3 }, { v2 }, { v1 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new double[,] { { r3 }, { r2 }, { r1 } }), "X10");
            }
            {
                filter.direction = Filters.Cumulate.Direction.Y01;
                var ar = new double[,] { { v1, v2, v3 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new double[,] { { r1, r2, r3 } }), "Y01");
            }
            {
                filter.direction = Filters.Cumulate.Direction.Y10;
                var ar = new double[,] { { v3, v2, v1 } };
                filter.EvaluateAll(ref ar);

                Assert.That(ar, Is.EqualTo(new double[,] { { r3, r2, r1 } }), "Y10");
            }
        }
    }
}
