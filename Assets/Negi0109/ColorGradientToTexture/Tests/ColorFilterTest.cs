using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Tests
{
    public class ColorFilterTest
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
    }
}
