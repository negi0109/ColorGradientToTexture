using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class InverseLerpTest
    {
        public class Clamped
        {
            [TestCase(1, 2, 3, 0, 0.5, 1)]
            public void MinToMax(double v0, double v1, double v2, double e0, double e1, double e2)
            {
                var ar = new double[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number() { type = Filters.Number.Type.Min },
                    max = new Filters.Number() { type = Filters.Number.Type.Max },
                    clamped = true
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new double[,] { { e0, e1, e2 } }));
            }

            [TestCase(1, 2, 3, 1, 3, 0, 0.5, 1)]
            [TestCase(1, 3, 6, 1, 2, 0, 1, 1)]
            public void FreeToFree(double v0, double v1, double v2, double minValue, double maxValue, double e0, double e1, double e2)
            {
                var ar = new double[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number()
                    {
                        type = Filters.Number.Type.Free,
                        value = minValue
                    },
                    max = new Filters.Number()
                    {
                        type = Filters.Number.Type.Free,
                        value = maxValue
                    },
                    clamped = true
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new double[,] { { e0, e1, e2 } }));
            }

            [TestCase(1, 2, 4, 0, 0.25, 0.5, 1)]
            [TestCase(1, 3, 5, 1, 0, 0.5, 1)]
            public void FreeToMax(double v0, double v1, double v2, double minValue, double e0, double e1, double e2)
            {
                var ar = new double[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number()
                    {
                        type = Filters.Number.Type.Free,
                        value = minValue
                    },
                    max = new Filters.Number()
                    {
                        type = Filters.Number.Type.Max
                    },
                    clamped = true
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new double[,] { { e0, e1, e2 } }));
            }

            [TestCase(1, 2, 3, 0, 0, 1)]
            public void AvgToMax(double v0, double v1, double v2, double e0, double e1, double e2)
            {
                var ar = new double[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number() { type = Filters.Number.Type.Avg },
                    max = new Filters.Number() { type = Filters.Number.Type.Max },
                    clamped = true
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new double[,] { { e0, e1, e2 } }));
            }
        }

        public class Unclamped
        {
            [TestCase(1, 2, 3, 0, 0.5, 1)]
            public void MinToMax(double v0, double v1, double v2, double e0, double e1, double e2)
            {
                var ar = new double[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number() { type = Filters.Number.Type.Min },
                    max = new Filters.Number() { type = Filters.Number.Type.Max },
                    clamped = false
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new double[,] { { e0, e1, e2 } }));
            }

            [TestCase(1, 2, 3, 1, 3, 0, 0.5, 1)]
            [TestCase(1, 3, 6, 1, 2, 0f, 2, 5)]
            public void FreeToFree(double v0, double v1, double v2, double minValue, double maxValue, double e0, double e1, double e2)
            {
                var ar = new double[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number()
                    {
                        type = Filters.Number.Type.Free,
                        value = minValue
                    },
                    max = new Filters.Number()
                    {
                        type = Filters.Number.Type.Free,
                        value = maxValue
                    },
                    clamped = false
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new double[,] { { e0, e1, e2 } }));
            }

            [TestCase(1f, 2f, 4f, 0f, 0.25f, 0.5f, 1f)]
            [TestCase(1f, 3f, 5f, 1f, 0f, 0.5f, 1f)]
            public void FreeToMax(double v0, double v1, double v2, double minValue, double e0, double e1, double e2)
            {
                var ar = new double[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number()
                    {
                        type = Filters.Number.Type.Free,
                        value = minValue
                    },
                    max = new Filters.Number()
                    {
                        type = Filters.Number.Type.Max
                    },
                    clamped = false
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new double[,] { { e0, e1, e2 } }));
            }

            [TestCase(1f, 2f, 3f, -1f, 0f, 1f)]
            public void AvgToMax(double v0, double v1, double v2, double e0, double e1, double e2)
            {
                var ar = new double[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number() { type = Filters.Number.Type.Avg },
                    max = new Filters.Number() { type = Filters.Number.Type.Max },
                    clamped = false
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new double[,] { { e0, e1, e2 } }));
            }
        }
    }
}
