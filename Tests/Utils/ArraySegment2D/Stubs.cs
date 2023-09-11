namespace Negi0109.ColorGradientToTexture.Utils.Tests
{
    namespace ArraySegment2DTest
    {
        public class LineStub : ArraySegment2DBase<int>.Line
        {
            public override int GetLength() => _body.Length;
            public override int this[int i] { get => _body[i]; set => _body[i] = value; }

            private readonly int[] _body;
            public LineStub(int[] body)
            {
                _body = body;
            }
        }

        public class ArraySegment2DStub : ArraySegment2DBase<(int, int)>
        {
            public readonly (int, int)[,] body;

            public override (int, int) this[int i0, int i1]
            {
                set => body[i0, i1] = value;
                get => body[i0, i1];
            }
            public override int GetLength(int dimension) => body.GetLength(dimension);

            public ArraySegment2DStub(int s0, int s1)
            {
                body = new (int, int)[s0, s1];

                for (int i0 = 0; i0 < s0; i0++)
                    for (int i1 = 0; i1 < s1; i1++)
                        body[i0, i1] = (i0, i1);
            }
        }
    }
}
