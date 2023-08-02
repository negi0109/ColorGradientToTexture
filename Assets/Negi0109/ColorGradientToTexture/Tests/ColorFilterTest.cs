using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Tests
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
        public void EvaluateAll(float v, float stubValue, float excepted)
        {
            float[,] array = { { v, v, v, v } };
            var filter = new FilterStub() { value = stubValue };

            filter.EvaluateAll(ref array);

            Assert.That(array, Is.EqualTo(new float[,] { { excepted, excepted, excepted, excepted } }));
        }


        public class FilterTest
        {
            [TestCase(2, 0.5f, 0.5f)]
            [TestCase(2, 0.49f, 0f)]
            [TestCase(2, 1f, 1f)]
            public void Step(int step, float v, float excepted)
            {
                var filter = new Filters.Step() { step = step };

                Assert.That(filter.Evaluate(v), Is.EqualTo(excepted));
            }

            [TestCase(2, 0.5f, 1f)]
            [TestCase(2, 1f, 2f)]
            public void Scale(float scale, float v, float excepted)
            {
                var filter = new Filters.Scale() { value = scale };

                Assert.That(filter.Evaluate(v), Is.EqualTo(excepted));
            }

            [TestCase(3, 0.5f, 0.5f)]
            [TestCase(3, 1f, 0f)]
            public void Repeat(int count, float v, float excepted)
            {
                var filter = new Filters.Repeat() { count = count };

                Assert.That(filter.Evaluate(v), Is.EqualTo(excepted));
            }

            [TestCase(1f, 0.5f, 0)]
            [TestCase(1f, 1f, 1)]
            public void Threshold(float value, float v, float excepted)
            {
                var filter = new Filters.Threshold() { value = value };

                Assert.That(filter.Evaluate(v), Is.EqualTo(excepted));
            }

            [TestCase(1f, 0.5f, 1.5f)]
            [TestCase(1f, 1f, 2f)]
            public void Add(float value, float v, float excepted)
            {
                var filter = new Filters.Add() { value = value };

                Assert.That(filter.Evaluate(v), Is.EqualTo(excepted));
            }

            [TestCase(1f, 0f)]
            [TestCase(0.2f, 0.8f)]
            public void OneMinus(float v, float excepted)
            {
                var filter = new Filters.OneMinus();

                Assert.That(filter.Evaluate(v), Is.EqualTo(excepted));
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

                    Assert.That(ar, Is.EqualTo(new float[,] { { r1 }, { r2 }, { r3 } }));
                }
                {
                    filter.direction = Filters.Cumulate.Direction.X10;
                    var ar = new float[,] { { v3 }, { v2 }, { v1 } };
                    filter.EvaluateAll(ref ar);

                    Assert.That(ar, Is.EqualTo(new float[,] { { r3 }, { r2 }, { r1 } }));
                }
                {
                    filter.direction = Filters.Cumulate.Direction.Y01;
                    var ar = new float[,] { { v1, v2, v3 } };
                    filter.EvaluateAll(ref ar);

                    Assert.That(ar, Is.EqualTo(new float[,] { { r1, r2, r3 } }));
                }
                {
                    filter.direction = Filters.Cumulate.Direction.Y10;
                    var ar = new float[,] { { v3, v2, v1 } };
                    filter.EvaluateAll(ref ar);

                    Assert.That(ar, Is.EqualTo(new float[,] { { r3, r2, r1 } }));
                }
            }
        }
    }
}
