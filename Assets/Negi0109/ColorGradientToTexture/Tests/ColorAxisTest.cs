using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Tests
{
    public class ColorAxisTest
    {
        public const float xCurveValue = 1;
        public const float yCurveValue = 2;

        [TestCase(0, xCurveValue, TestName = "yDirection:0 => xの値を返却")]
        [TestCase(1, yCurveValue, TestName = "yDirection:1 => yの値を返却")]
        public void YDirectionTest(float yWeight, float excepted)
        {
            var axis = new ColorAxis(2);
            axis.xCurve = AnimationCurve.Linear(0, xCurveValue, 1, xCurveValue);
            axis.yCurve = AnimationCurve.Linear(0, yCurveValue, 1, yCurveValue);

            axis.yWeight = yWeight;
            Assert.That(axis.Evaluate(0f, 0f), Is.EqualTo(excepted));
        }

        [TestCase(1, xCurveValue, TestName = "xの値のみ反映")]
        [TestCase(2, yCurveValue, TestName = "yの値のみ反映")]
        public void AxesCountTest(int axesCount, float excepted)
        {
            var axis = new ColorAxis(axesCount);
            axis.xCurve = AnimationCurve.Linear(0, xCurveValue, 1, xCurveValue);
            axis.yCurve = AnimationCurve.Linear(0, yCurveValue, 1, yCurveValue);

            axis.yWeight = 1;
            Assert.That(axis.Evaluate(0f, 1f), Is.EqualTo(excepted));
        }
    }
}
