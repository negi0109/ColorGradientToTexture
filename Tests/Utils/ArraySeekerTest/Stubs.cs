using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Negi0109.ColorGradientToTexture.Utils.Tests
{
    namespace ArraySeekerTest
    {
        public class LineStub : Utils.ArraySeekerBase<int>.Line
        {
            public override int GetLength() => _body.Length;
            public override int this[int i] { get => _body[i]; set => _body[i] = value; }

            private readonly int[] _body;
            public LineStub(int[] body)
            {
                _body = body;
            }
        }

        public class ArraySeekerStub : Utils.ArraySeekerBase<(int, int)>
        {
            public readonly (int, int)[,] body;

            public override (int, int) this[int i0, int i1]
            {
                set => body[i0, i1] = value;
                get => body[i0, i1];
            }
            public override int GetLength(int dimension) => body.GetLength(dimension);

            public ArraySeekerStub(int s0, int s1)
            {
                this.body = new (int, int)[s0, s1];

                for (int i0 = 0; i0 < s0; i0++)
                    for (int i1 = 0; i1 < s1; i1++)
                        this.body[i0, i1] = (i0, i1);
            }
        }
    }
}
