using System.Collections;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Utils
{
    public class ArraySeeker<T>
    {
        private readonly T[,] _body;
        private readonly int _dimension;
        private bool _backward = false;

        public T this[int i0, int i1]
        {
            set
            {
                if (_backward)
                {
                    if (_dimension == 0) _body[_body.GetLength(0) - i0 - 1, i1] = value;
                    else _body[i1, _body.GetLength(1) - i0 - 1] = value;
                }
                else
                {
                    if (_dimension == 0) _body[i0, i1] = value;
                    else _body[i1, i0] = value;
                }
            }
            get
            {
                if (_backward)
                {
                    if (_dimension == 0) return _body[_body.GetLength(0) - i0 - 1, i1];
                    else return _body[i1, _body.GetLength(1) - i0 - 1];
                }
                else
                {
                    if (_dimension == 0) return _body[i0, i1];
                    else return _body[i1, i0];
                }
            }
        }

        public int GetLineCount()
        {
            if (_dimension == 0) return _body.GetLength(1);
            else return _body.GetLength(0);
        }

        public int GetLineLength()
        {
            if (_dimension == 0) return _body.GetLength(0);
            else return _body.GetLength(1);
        }

        public Line GetLine(int index) => new Line(this, index);
        public LineEnumerable GetLines() => new LineEnumerable(new LineEnumerator(this));

        public ArraySeeker(T[,] body, int dimension, bool backward)
        {
            _body = body;
            _dimension = dimension;
            _backward = backward;
        }

        public class LineEnumerable : IEnumerable<Line>
        {
            private readonly IEnumerator<Line> _enumerator;

            public LineEnumerable(IEnumerator<Line> enumerator)
            {
                _enumerator = enumerator;
            }

            public IEnumerator<Line> GetEnumerator() => _enumerator;

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class LineEnumerator : IEnumerator<Line>
        {
            private ArraySeeker<T> _seeker;
            private int _currentIndex;
            private int _length;

            public LineEnumerator(ArraySeeker<T> seeker)
            {
                _seeker = seeker;
                _length = seeker.GetLineCount();
                _currentIndex = -1;
            }

            public Line Current { get => _seeker.GetLine(_currentIndex); }

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

        public class Line
        {
            private readonly ArraySeeker<T> _seeker;
            private readonly int _index;

            public T this[int i]
            {
                set { _seeker[i, _index] = value; }
                get => _seeker[i, _index];
            }

            internal Line(ArraySeeker<T> seeker, int index)
            {
                _seeker = seeker;
                _index = index;
            }

            public int GetLength()
            {
                return _seeker.GetLineLength();
            }
        }
    }
}
