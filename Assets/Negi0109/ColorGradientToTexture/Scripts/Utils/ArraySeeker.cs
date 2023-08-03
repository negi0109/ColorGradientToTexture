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

        public ArraySeekerLine<T> GetLine(int index) => new ArraySeekerLine<T>(_body, _dimension, index, _backward);
        public ArraySeekerLineEnumerable<T> GetLines() => new ArraySeekerLineEnumerable<T>(new ArraySeekerLineEnumerator<T>(this));

        public ArraySeeker(T[,] body, int dimension, bool backward)
        {
            _body = body;
            _dimension = dimension;
            _backward = backward;
        }
    }

    public class ArraySeekerLineEnumerable<T> : IEnumerable<ArraySeekerLine<T>>
    {
        IEnumerator<ArraySeekerLine<T>> _enumerator;
        public ArraySeekerLineEnumerable(IEnumerator<ArraySeekerLine<T>> enumerator)
        {
            _enumerator = enumerator;
        }
        public IEnumerator<ArraySeekerLine<T>> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ArraySeekerLineEnumerator<T> : IEnumerator<ArraySeekerLine<T>>
    {
        private ArraySeeker<T> _seeker;
        private int _currentIndex;
        private int _length;

        public ArraySeekerLineEnumerator(ArraySeeker<T> seeker)
        {
            _seeker = seeker;
            _length = seeker.GetLineCount();
            _currentIndex = -1;
        }

        public ArraySeekerLine<T> Current
        {
            get => _seeker.GetLine(_currentIndex);
        }
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

        public void Dispose()
        {
        }
    }

    public class ArraySeekerLine<T>
    {
        private readonly T[,] _body;
        private readonly int _dimension;
        private readonly int _index;
        private readonly bool _backward;

        public T this[int i]
        {
            set
            {
                if (_backward)
                {
                    if (_dimension == 0) _body[_body.GetLength(0) - i - 1, _index] = value;
                    else _body[_index, _body.GetLength(1) - i - 1] = value;
                }
                else
                {
                    if (_dimension == 0) _body[i, _index] = value;
                    else _body[_index, i] = value;
                }
            }
            get
            {
                if (_backward)
                {
                    if (_dimension == 0) return _body[_body.GetLength(0) - i - 1, _index];
                    else return _body[_index, _body.GetLength(1) - i - 1];
                }
                else
                {
                    if (_dimension == 0) return _body[i, _index];
                    else return _body[_index, i];
                }
            }
        }

        internal ArraySeekerLine(T[,] body, int dimension, int index, bool backward)
        {
            _body = body;
            _dimension = dimension;
            _index = index;
            _backward = backward;
        }

        public int GetLength()
        {
            if (_dimension == 0) return _body.GetLength(0);
            return _body.GetLength(1);
        }
    }
}
