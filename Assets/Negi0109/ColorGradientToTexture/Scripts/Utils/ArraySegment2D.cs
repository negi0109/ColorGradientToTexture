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

        public static ArraySegment2DBase<T> GetSegment<T>(T[,] body, int dimension = 0, bool backward = false)
        {
            return new ArraySegment2D<T>(body).Dimension(dimension).Backward(backward);
        }
    }

    public abstract class ArraySegment2DBase<T>
    {
        public abstract T this[int i0, int i1] { get; set; }

        public OneLine GetLine(int index) => new OneLine(this, index);
        public AllLine GetAll() => new AllLine(this);
        public LineEnumerable GetLines() => new LineEnumerable(GetEnumerator());

        public IEnumerator<OneLine> GetEnumerator()
        {
            for (int i = 0; i < this.GetLineCount(); i++) yield return this.GetLine(i);
        }

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

        public abstract class Line : IList<T>
        {
            public int Count => GetLength();
            public abstract int GetLength();
            public abstract T this[int i] { get; set; }

            public void SetValues(Func<T, T> func) => SetValues((v, i) => func(v));
            public void SetValues(Func<T, int, T> func)
            {
                for (int i = 0; i < Count; i++) this[i] = func(this[i], i);
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < Count; i++) yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(T item)
            {
                for (int i = 0; i < Count; i++) if (this[i].Equals(item)) return i;
                return -1;
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                for (int i = 0; i < Count; i++)
                {
                    array[arrayIndex + i] = this[i];
                }
            }

            public bool IsReadOnly => false;
            public bool Contains(T item) => IndexOf(item) >= 0;
            public void Clear() => throw new NotImplementedException();
            public void Add(T item) => throw new NotImplementedException();
            public void Insert(int index, T item) => throw new NotImplementedException();
            public bool Remove(T item) => throw new NotImplementedException();
            public void RemoveAt(int index) => throw new NotImplementedException();
        }

        public class OneLine : Line
        {
            private readonly ArraySegment2DBase<T> _segment;
            private readonly int _index;

            public override T this[int i]
            {
                set { _segment[i, _index] = value; }
                get => _segment[i, _index];
            }

            internal OneLine(ArraySegment2DBase<T> segment, int index)
            {
                _segment = segment;
                _index = index;
            }

            public override int GetLength()
            {
                return _segment.GetLineLength();
            }
        }

        public class AllLine : Line
        {
            private readonly ArraySegment2DBase<T> _segment;
            public override T this[int i]
            {
                set { _segment[i % _segment.GetLineLength(), i / _segment.GetLineLength()] = value; }
                get => _segment[i % _segment.GetLineLength(), i / _segment.GetLineLength()];
            }

            internal AllLine(ArraySegment2DBase<T> segment)
            {
                _segment = segment;
            }

            public override int GetLength()
            {
                return _segment.GetLineLength() * _segment.GetLineCount();
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
            private readonly ArraySegment2DBase<T> _segment;

            public override T this[int i0, int i1]
            {
                set => _segment[i1, i0] = value;
                get => _segment[i1, i0];
            }

            public DimensionReverse(ArraySegment2DBase<T> segment) { _segment = segment; }
            public override int GetLength(int dimension) => _segment.GetLength(1 - dimension);
        }

        public sealed class SeekBackward : ArraySegment2DBase<T>
        {
            private readonly ArraySegment2DBase<T> _segment;

            public override T this[int i0, int i1]
            {
                set => _segment[_segment.GetLength(0) - i0 - 1, _segment.GetLength(1) - i1 - 1] = value;
                get => _segment[_segment.GetLength(0) - i0 - 1, _segment.GetLength(1) - i1 - 1];
            }

            public SeekBackward(ArraySegment2DBase<T> segment) { _segment = segment; }
            public override int GetLength(int dimension) => _segment.GetLength(dimension);
        }
    }
}
