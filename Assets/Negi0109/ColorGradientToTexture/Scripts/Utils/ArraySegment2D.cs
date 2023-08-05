using System;
using System.Collections;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Utils
{

    public static class ArraySegment2DUtils
    {
        public static ArraySegment2DBase<T>.AllLine GetAll<T>(T[,] body)
        {
            return new ArraySegment2D<T>(body).GetAll();
        }

        public static void SetValues<T>(T[,] body, Func<T, T> func)
        {
            new ArraySegment2D<T>(body).GetAll().SetValues(func);
        }

        public static ArraySegment2DBase<T> GetSeeker<T>(T[,] body, int dimension = 0, bool backward = false)
        {
            return new ArraySegment2D<T>(body).Dimension(dimension).Backward(backward);
        }
    }

    public abstract class ArraySegment2DBase<T>
    {
        public abstract T this[int i0, int i1] { get; set; }

        public OneLine GetLine(int index) => new OneLine(this, index);
        public AllLine GetAll() => new AllLine(this);
        public LineEnumerable GetLines() => new LineEnumerable(new LineEnumerator(this));

        public int GetLineCount() => GetLength(1);
        public int GetLineLength() => GetLength(0);

        public abstract int GetLength(int dimension);

        public ArraySegment2DBase<T> Dimension(int dimension) => dimension == 0 ? this : new ArraySegment2D<T>.DimensionReverse(this);
        public ArraySegment2DBase<T> Backward(bool backward = true) => backward ? new ArraySegment2D<T>.SeekBackward(this) : this;

        public class LineEnumerable : IEnumerable<OneLine>
        {
            private readonly IEnumerator<OneLine> _enumerator;

            public LineEnumerable(IEnumerator<OneLine> enumerator)
            {
                _enumerator = enumerator;
            }

            public IEnumerator<OneLine> GetEnumerator() => _enumerator;

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class LineEnumerator : IEnumerator<OneLine>
        {
            private ArraySegment2DBase<T> _seeker;
            private int _currentIndex;
            private int _length;

            public LineEnumerator(ArraySegment2DBase<T> seeker)
            {
                _seeker = seeker;
                _length = seeker.GetLineCount();
                _currentIndex = -1;
            }

            public OneLine Current { get => _seeker.GetLine(_currentIndex); }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_currentIndex + 1 < _length)
                {
                    _currentIndex++;
                    return true;
                }
                else return false;
            }
            public void Reset()
            {
                _currentIndex = -1;
            }

            public void Dispose() { }
        }

        public abstract class Line
        {
            public int Length => GetLength();
            public abstract int GetLength();
            public abstract T this[int i] { get; set; }

            public void SetValues(Func<T, T> func)
            {
                var length = GetLength();
                for (int i = 0; i < length; i++)
                {
                    this[i] = func(this[i]);
                }
            }
        }

        public class OneLine : Line
        {
            private readonly ArraySegment2DBase<T> _seeker;
            private readonly int _index;

            public override T this[int i]
            {
                set { _seeker[i, _index] = value; }
                get => _seeker[i, _index];
            }

            internal OneLine(ArraySegment2DBase<T> seeker, int index)
            {
                _seeker = seeker;
                _index = index;
            }

            public override int GetLength()
            {
                return _seeker.GetLineLength();
            }
        }

        public class AllLine : Line
        {
            private readonly ArraySegment2DBase<T> _seeker;
            public override T this[int i]
            {
                set { _seeker[i % _seeker.GetLineLength(), i / _seeker.GetLineLength()] = value; }
                get => _seeker[i % _seeker.GetLineLength(), i / _seeker.GetLineLength()];
            }

            internal AllLine(ArraySegment2DBase<T> seeker)
            {
                _seeker = seeker;
            }

            public override int GetLength()
            {
                return _seeker.GetLineLength() * _seeker.GetLineCount();
            }
        }
    }

    public sealed class ArraySegment2D<T> : ArraySegment2DBase<T>
    {
        private readonly T[,] _body;

        public override T this[int i0, int i1]
        {
            set => _body[i0, i1] = value;
            get => _body[i0, i1];
        }

        public override int GetLength(int dimension) => _body.GetLength(dimension);

        public ArraySegment2D(T[,] body) { _body = body; }

        public sealed class DimensionReverse : ArraySegment2DBase<T>
        {
            private readonly ArraySegment2DBase<T> _seeker;

            public override T this[int i0, int i1]
            {
                set => _seeker[i1, i0] = value;
                get => _seeker[i1, i0];
            }

            public DimensionReverse(ArraySegment2DBase<T> seeker) { _seeker = seeker; }
            public override int GetLength(int dimension) => _seeker.GetLength(1 - dimension);
        }

        public sealed class SeekBackward : ArraySegment2DBase<T>
        {
            private readonly ArraySegment2DBase<T> _seeker;

            public override T this[int i0, int i1]
            {
                set => _seeker[_seeker.GetLength(0) - i0 - 1, _seeker.GetLength(1) - i1 - 1] = value;
                get => _seeker[_seeker.GetLength(0) - i0 - 1, _seeker.GetLength(1) - i1 - 1];
            }

            public SeekBackward(ArraySegment2DBase<T> seeker) { _seeker = seeker; }
            public override int GetLength(int dimension) => _seeker.GetLength(dimension);
        }
    }
}
