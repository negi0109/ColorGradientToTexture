using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Tests
{
    public class ColorFilterTest
    {
        ColorFilter colorFilter;

        [SetUp]
        public void SetUp()
        {
            colorFilter = new ColorFilter();
        }

        [TestCase(2, 0.5f, 0.5f)]
        [TestCase(2, 0.49f, 0f)]
        [TestCase(2, 1f, 1f)]
        public void Step(int step, float v, float excepted)
        {
            colorFilter.type = ColorFilter.Type.Step;
            colorFilter.value1 = step;

            Assert.AreEqual(excepted, colorFilter.Evaluate(v));
        }

        [TestCase(2, 0.5f, 1f)]
        [TestCase(2, 1f, 2f)]
        public void Scale(float scale, float v, float excepted)
        {
            colorFilter.type = ColorFilter.Type.Scale;
            colorFilter.value1 = scale;

            Assert.AreEqual(excepted, colorFilter.Evaluate(v));
        }

        [TestCase(3, 0.5f, 0.5f)]
        [TestCase(3, 1f, 0f)]
        public void Repeat(float repeat, float v, float excepted)
        {
            colorFilter.type = ColorFilter.Type.Repeat;
            colorFilter.value1 = repeat;

            Assert.AreEqual(excepted, colorFilter.Evaluate(v));
        }

        [TestCase(1f, 0.5f, 0)]
        [TestCase(1f, 1f, 1)]
        public void Threshold(float scale, float v, float excepted)
        {
            colorFilter.type = ColorFilter.Type.Threshold;
            colorFilter.value1 = scale;

            Assert.AreEqual(excepted, colorFilter.Evaluate(v));
        }

        [TestCase(1f, 0.5f, 1.5f)]
        [TestCase(1f, 1f, 2f)]
        public void Add(float add, float v, float excepted)
        {
            colorFilter.type = ColorFilter.Type.Add;
            colorFilter.value1 = add;

            Assert.AreEqual(excepted, colorFilter.Evaluate(v));
        }
    }
}
