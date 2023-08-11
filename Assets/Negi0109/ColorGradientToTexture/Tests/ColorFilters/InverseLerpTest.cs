using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class InverseLerpTest
    {
        public class Clamped
        {
            [TestCase(1f, 2f, 3f, 0f, 0.5f, 1f)]
            public void MinToMax(float v0, float v1, float v2, float e0, float e1, float e2)
            {
                var ar = new float[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number() { type = Filters.Number.Type.Min },
                    max = new Filters.Number() { type = Filters.Number.Type.Max },
                    clamped = true
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new float[,] { { e0, e1, e2 } }));
            }

            [TestCase(1f, 2f, 3f, 1f, 3f, 0f, 0.5f, 1f)]
            [TestCase(1f, 3f, 6f, 1f, 2f, 0f, 1f, 1f)]
            public void FreeToFree(float v0, float v1, float v2, float minValue, float maxValue, float e0, float e1, float e2)
            {
                var ar = new float[,] { { v0, v1, v2 } };

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
                Assert.That(ar, Is.EqualTo(new float[,] { { e0, e1, e2 } }));
            }

            [TestCase(1f, 2f, 4f, 0f, 0.25f, 0.5f, 1f)]
            [TestCase(1f, 3f, 5f, 1f, 0f, 0.5f, 1f)]
            public void FreeToMax(float v0, float v1, float v2, float minValue, float e0, float e1, float e2)
            {
                var ar = new float[,] { { v0, v1, v2 } };

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
                Assert.That(ar, Is.EqualTo(new float[,] { { e0, e1, e2 } }));
            }

            [TestCase(1f, 2f, 3f, 0f, 0f, 1f)]
            public void AvgToMax(float v0, float v1, float v2, float e0, float e1, float e2)
            {
                var ar = new float[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number() { type = Filters.Number.Type.Avg },
                    max = new Filters.Number() { type = Filters.Number.Type.Max },
                    clamped = true
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new float[,] { { e0, e1, e2 } }));
            }
        }

        public class Unclamped
        {
            [TestCase(1f, 2f, 3f, 0f, 0.5f, 1f)]
            public void MinToMax(float v0, float v1, float v2, float e0, float e1, float e2)
            {
                var ar = new float[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number() { type = Filters.Number.Type.Min },
                    max = new Filters.Number() { type = Filters.Number.Type.Max },
                    clamped = false
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new float[,] { { e0, e1, e2 } }));
            }

            [TestCase(1f, 2f, 3f, 1f, 3f, 0f, 0.5f, 1f)]
            [TestCase(1f, 3f, 6f, 1f, 2f, 0f, 2f, 5f)]
            public void FreeToFree(float v0, float v1, float v2, float minValue, float maxValue, float e0, float e1, float e2)
            {
                var ar = new float[,] { { v0, v1, v2 } };

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
                Assert.That(ar, Is.EqualTo(new float[,] { { e0, e1, e2 } }));
            }

            [TestCase(1f, 2f, 4f, 0f, 0.25f, 0.5f, 1f)]
            [TestCase(1f, 3f, 5f, 1f, 0f, 0.5f, 1f)]
            public void FreeToMax(float v0, float v1, float v2, float minValue, float e0, float e1, float e2)
            {
                var ar = new float[,] { { v0, v1, v2 } };

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
                Assert.That(ar, Is.EqualTo(new float[,] { { e0, e1, e2 } }));
            }

            [TestCase(1f, 2f, 3f, -1f, 0f, 1f)]
            public void AvgToMax(float v0, float v1, float v2, float e0, float e1, float e2)
            {
                var ar = new float[,] { { v0, v1, v2 } };

                var filter = new Filters.InverseLerp()
                {
                    min = new Filters.Number() { type = Filters.Number.Type.Avg },
                    max = new Filters.Number() { type = Filters.Number.Type.Max },
                    clamped = false
                };
                filter.EvaluateAll(ref ar);
                Assert.That(ar, Is.EqualTo(new float[,] { { e0, e1, e2 } }));
            }
        }
    }
}
