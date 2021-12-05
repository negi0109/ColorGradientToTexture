using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Tests
{
    public class CoordinateFilterTest
    {
        CoordinateFilter coordinateFilter;

        [SetUp]
        public void SetUp()
        {
            coordinateFilter = new CoordinateFilter();
        }

        [TestCase(2, 2, 0.5f, 0.5f, 0f, 0f)]
        [TestCase(3, 1, 0.5f, 0.5f, 0.5f, 0.5f)]

        public void Tiling(float x, float y, float vx, float vy, float exceptedX, float exceptedY)
        {
            coordinateFilter.type = CoordinateFilter.Type.Tiling;
            coordinateFilter.value1 = x;
            coordinateFilter.value2 = y;

            Assert.AreEqual(new Vector2(exceptedX, exceptedY), coordinateFilter.Evaluate(new Vector2(vx, vy)));
        }

        [TestCase(1f, 0f, 0f, 1f)]
        [TestCase(2f, 3f, 3f, 2f)]
        public void Turn(float vx, float vy, float exceptedX, float exceptedY)
        {
            coordinateFilter.type = CoordinateFilter.Type.Turn;

            Assert.AreEqual(new Vector2(exceptedX, exceptedY), coordinateFilter.Evaluate(new Vector2(vx, vy)));
        }

        [TestCase(0, 0.2f, 0.2f, 0.8f, 0.2f, TestName = "x軸のみ")]
        [TestCase(1, 0.2f, 0.2f, 0.2f, 0.8f, TestName = "y軸のみ")]
        [TestCase(2, 0.2f, 0.2f, 0.8f, 0.8f, TestName = "x,y軸両方")]
        public void Reverse(int a, float vx, float vy, float exceptedX, float exceptedY)
        {
            coordinateFilter.type = CoordinateFilter.Type.Reverse;
            coordinateFilter.value1 = a;

            Assert.AreEqual(new Vector2(exceptedX, exceptedY), coordinateFilter.Evaluate(new Vector2(vx, vy)));
        }
    }
}
