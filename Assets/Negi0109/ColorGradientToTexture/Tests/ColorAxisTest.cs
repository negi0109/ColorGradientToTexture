﻿using System.Collections;
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
        public void YDirectionTest(float yDirection, float excepted)
        {
            var axis = new ColorAxis(2);
            axis.xCurve = AnimationCurve.Linear(0, xCurveValue, 1, xCurveValue);
            axis.yCurve = AnimationCurve.Linear(0, yCurveValue, 1, yCurveValue);

            axis.yDirection = yDirection;
            Assert.AreEqual(excepted, axis.Evaluate(0, 0));
        }
    }
}
