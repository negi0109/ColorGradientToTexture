using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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


    public class FilterTest
    {
        [TestCase(2, 0.5f, 0.5f)]
        [TestCase(2, 0.49f, 0f)]
        [TestCase(2, 1f, 1f)]
        public void Step(int step, float v, float expected)
        {
            var filter = new Filters.Step() { step = step };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }

        [TestCase(2, 0.5f, 1f)]
        [TestCase(2, 1f, 2f)]
        public void Scale(float scale, float v, float expected)
        {
            var filter = new Filters.Scale() { value = scale };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }

        [TestCase(3, 0.5f, 0.5f)]
        [TestCase(3, 1f, 0f)]
        public void Repeat(int count, float v, float expected)
        {
            var filter = new Filters.Repeat() { count = count };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }

        [TestCase(1f, 0.5f, 0)]
        [TestCase(1f, 1f, 1)]
        public void Threshold(float value, float v, float expected)
        {
            var filter = new Filters.Threshold() { value = value };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }

        [TestCase(1f, 0.5f, 1.5f)]
        [TestCase(1f, 1f, 2f)]
        public void Add(float value, float v, float expected)
        {
            var filter = new Filters.Add() { value = value };

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }

        [TestCase(1f, 0f)]
        [TestCase(0.2f, 0.8f)]
        public void OneMinus(float v, float expected)
        {
            var filter = new Filters.OneMinus();

            Assert.That(filter.Evaluate(v), Is.EqualTo(expected));
        }

        public class InverseLerp
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

        [TestCase(1, 2, 3, 1, 3, 6, Filters.Cumulate.Division.One)]
        [TestCase(1, 2, 3, 1 / 6f, 3 / 6f, 6f / 6f, Filters.Cumulate.Division.Max)]
        [TestCase(1, 2, 3, 1 / 3f, 3 / 3f, 6f / 3f, Filters.Cumulate.Division.Volume)]
        public void Cumulate(float v1, float v2, float v3, float r1, float r2, float r3, Filters.Cumulate.Division division)
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

        [TestCase(1, 2, 3, 1, 1, 1, Filters.Diff.Division.One)]
        [TestCase(1, 2, 3, 1, 1, 1, Filters.Diff.Division.Max)]
        [TestCase(1, 2, 4, 1 / 2f, 1 / 2f, 2 / 2f, Filters.Diff.Division.Max)]
        public void Diff(float v1, float v2, float v3, float r1, float r2, float r3, Filters.Diff.Division division)
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
