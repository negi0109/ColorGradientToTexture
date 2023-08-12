using NUnit.Framework;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Tests
{
    public class ColorAxisTest
    {
        public const float xCurveValue = 1;
        public const float yCurveValue = 2;

        [TestCase(0, xCurveValue, TestName = "yDirection:0 => xの値を返却")]
        [TestCase(1, yCurveValue, TestName = "yDirection:1 => yの値を返却")]
        public void YDirectionTest(float yWeight, float expected)
        {
            var axis = new ColorAxis(2)
            {
                xCurve = AnimationCurve.Linear(0, xCurveValue, 1, xCurveValue),
                yCurve = AnimationCurve.Linear(0, yCurveValue, 1, yCurveValue),

                yWeight = yWeight
            };
            Assert.That(axis.Evaluate(0f, 0f), Is.EqualTo(expected));
        }

        [TestCase(1, xCurveValue, TestName = "xの値のみ反映")]
        [TestCase(2, yCurveValue, TestName = "yの値のみ反映")]
        public void AxesCountTest(int axesCount, float expected)
        {
            var axis = new ColorAxis(axesCount)
            {
                xCurve = AnimationCurve.Linear(0, xCurveValue, 1, xCurveValue),
                yCurve = AnimationCurve.Linear(0, yCurveValue, 1, yCurveValue),
                yWeight = 1
            };
            Assert.That(axis.Evaluate(0f, 1f), Is.EqualTo(expected));
        }
    }
}
