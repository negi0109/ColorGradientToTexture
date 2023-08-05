using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Tests
{
    public class CoordinateFilterTest
    {
        [TestCase(2, 2, 0.5f, 0.5f, 0f, 0f)]
        [TestCase(3, 1, 0.5f, 0.5f, 0.5f, 0.5f)]

        public void Tiling(float x, float y, float vx, float vy, float expectedX, float expectedY)
        {
            var filter = new CoordinateFilters.Tiling()
            {
                value = new Vector2(x, y)
            };
            Assert.That(filter.Evaluate(new Vector2(vx, vy)), Is.EqualTo(new Vector2(expectedX, expectedY)));
        }

        [TestCase(1f, 0f, 0f, 1f)]
        [TestCase(2f, 3f, 3f, 2f)]
        public void Turn(float vx, float vy, float expectedX, float expectedY)
        {
            var filter = new CoordinateFilters.Turn();

            Assert.That(filter.Evaluate(new Vector2(vx, vy)), Is.EqualTo(new Vector2(expectedX, expectedY)));
        }

        [TestCase(CoordinateFilters.Reverse.Mode.X, 0.2f, 0.2f, 0.8f, 0.2f, TestName = "x軸のみ")]
        [TestCase(CoordinateFilters.Reverse.Mode.Y, 0.2f, 0.2f, 0.2f, 0.8f, TestName = "y軸のみ")]
        [TestCase(CoordinateFilters.Reverse.Mode.Both, 0.2f, 0.2f, 0.8f, 0.8f, TestName = "x,y軸両方")]
        public void Reverse(CoordinateFilters.Reverse.Mode mode, float vx, float vy, float expectedX, float expectedY)
        {
            var filter = new CoordinateFilters.Reverse()
            {
                mode = mode
            };

            Assert.That(filter.Evaluate(new Vector2(vx, vy)), Is.EqualTo(new Vector2(expectedX, expectedY)));
        }
    }
}
