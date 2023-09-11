using System;
using Negi0109.ColorGradientToTexture.Filters;
using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class NumberTest
    {
        [TestCase(3f)]
        public void Free(double value)
        {
            double[,] array = new double[,] { { 0, 1 } };
            var segment = new Utils.ArraySegment2D<double>(array);
            var number = new Number() { type = Number.Type.Free, value = value };

            Assert.That(number.GetValue(segment), Is.EqualTo(value));
        }

        [TestCase(0f, 1f, 0.5f)]
        public void Avg(double v0, double v1, double expected)
        {
            double[,] array = new double[,] { { v0, v1 } };
            var segment = new Utils.ArraySegment2D<double>(array);
            var number = new Number() { type = Number.Type.Avg };

            Assert.That(number.GetValue(segment), Is.EqualTo(expected));
        }

        [TestCase(0f, 1f, 1f)]
        public void Max(double v0, double v1, double expected)
        {
            double[,] array = new double[,] { { v0, v1 } };
            var segment = new Utils.ArraySegment2D<double>(array);
            var number = new Number() { type = Number.Type.Max };

            Assert.That(number.GetValue(segment), Is.EqualTo(expected));
        }

        [TestCase(0f, 1f, 0f)]
        public void Min(double v0, double v1, double expected)
        {
            double[,] array = new double[,] { { v0, v1 } };
            var segment = new Utils.ArraySegment2D<double>(array);
            var number = new Number() { type = Number.Type.Min };

            Assert.That(number.GetValue(segment), Is.EqualTo(expected));
        }
    }
}
