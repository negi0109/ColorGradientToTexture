using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Utils.Tests
{
    namespace ArraySegment2DTest
    {
        public class LineTest
        {
            [TestCase(1, 2, 3, 4)]
            public void SetValues(int v0, int v1, int v2, int overwrite)
            {
                var body = new int[] { v0, v1, v2 };
                var line = new LineStub(body);

                line.SetValues((_) => overwrite);
                Assert.That(body, Is.EqualTo(new int[] { overwrite, overwrite, overwrite }));
            }

            [TestCase(3, 3)]
            [TestCase(1, 3)]
            public void AllLine(int s0, int s1)
            {
                var all = new ArraySegment2DStub(s0, s1).GetAll();
                for (int i = 0; i < all.Length; i++)
                {
                    Assert.That(all[i], Is.EqualTo((i % s0, i / s0)));
                }
            }

            [Test]
            public void GetLine()
            {
                int s0 = 2;
                int s1 = 3;
                var segment = new ArraySegment2DStub(s0, s1);

                for (int i1 = 0; i1 < s1; i1++)
                {
                    var line = segment.GetLine(i1);
                    for (int i0 = 0; i0 < line.Length; i0++)
                        Assert.That(line[i0], Is.EqualTo((i0, i1)));
                }
            }
        }
    }
}
