using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Utils.Tests
{
    namespace ArraySeekerTest
    {
        public class ArraySeekerBaseTest
        {
            [TestCase(1, 2, 2)]
            public void GetLineCount(int s0, int s1, int expected)
            {
                var seeker = new ArraySeekerStub(s0, s1);
                Assert.That(seeker.GetLineCount(), Is.EqualTo(expected));
            }

            [TestCase(1, 2, 1)]
            public void GetLineLength(int s0, int s1, int expected)
            {
                var seeker = new ArraySeekerStub(s0, s1);
                Assert.That(seeker.GetLineLength(), Is.EqualTo(expected));
            }

            [TestCase(2, 2, 0, 0, 0, 0)]
            [TestCase(2, 2, 0, 1, 1, 0)]
            [TestCase(2, 2, 1, 0, 0, 1)]
            [TestCase(2, 2, 1, 1, 1, 1)]
            public void Dimension(int s0, int s1, int i0, int i1, int expected0, int expected1)
            {
                var seeker = new ArraySeekerStub(s0, s1).Dimension(1);

                Assert.That(seeker[i0, i1], Is.EqualTo((expected0, expected1)));
            }

            [TestCase(2, 2, 0, 0, 1, 1)]
            [TestCase(2, 2, 0, 1, 1, 0)]
            [TestCase(2, 2, 1, 0, 0, 1)]
            [TestCase(2, 2, 1, 1, 0, 0)]
            public void Backward(int s0, int s1, int i0, int i1, int expected0, int expected1)
            {
                var seeker = new ArraySeekerStub(s0, s1).Backward();

                Assert.That(seeker[i0, i1], Is.EqualTo((expected0, expected1)));
            }

            public class Integrations
            {
                [TestCase(3, 3, 0, 0, 2, 2)]
                [TestCase(3, 3, 0, 1, 1, 2)]
                [TestCase(3, 3, 0, 2, 0, 2)]
                [TestCase(3, 3, 2, 2, 0, 0)]
                public void DimensionAndBackward(int s0, int s1, int i0, int i1, int expected0, int expected1)
                {
                    var seeker = new ArraySeekerStub(s0, s1).Dimension(1).Backward();

                    Assert.That(seeker[i0, i1], Is.EqualTo((expected0, expected1)));
                }
            }
        }
    }
}
