using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartboardEngine.Utility
{
    public class AccessQueue<T> : IReadOnlyCollection<T>
    {
        const int INITIAL_LENGTH = 10;
        const float RESIZE_MULT = 1.5f;

        private T[] _data;
        private int _dataStart;
        private int _dataEnd;

        public AccessQueue()
        {
            _data = new T[INITIAL_LENGTH];
            _dataStart = _dataEnd = 0;
        }

        public int Count
        {
            get
            {
                if (_dataEnd >= _dataStart)
                {
                    return _dataEnd - _dataStart;
                }
                else
                {
                    return _data.Length - _dataStart + _dataEnd;
                }
            }
        }

        public T this[int queuePosition]
        {
            get
            {
                if (queuePosition >= Count)
                    throw new IndexOutOfRangeException();
                return _data[(_dataStart + queuePosition) % _data.Length];
            }
        }

        public void Enqueue(T data)
        {
            if (Incr(_dataEnd) == _dataStart)
            {
                Grow();
            }
            _data[_dataEnd] = data;
            _dataEnd = Incr(_dataEnd);
        }

        public T Dequeue()
        {
            if (_dataStart == _dataEnd)
                throw new IndexOutOfRangeException();
            var tmp = _data[_dataStart];
            _dataStart = Incr(_dataStart);

            return tmp;
        }

        private int Incr(int i)
        {
            return (i + 1) % _data.Length;
        }

        public void Clear()
        {
            _dataStart = _dataEnd = 0;
        }

        private void Grow(int targetSize = -1)
        {
            targetSize = targetSize == -1 ? (int)(_data.Length * RESIZE_MULT) : targetSize;
            T[] newData = new T[targetSize];
            if(_dataEnd >= _dataStart)
            {
                Array.ConstrainedCopy(_data, _dataStart, newData, 0, _dataEnd - _dataStart);
            }
            else
            {
                Array.ConstrainedCopy(_data, _dataStart, newData, 0, _data.Length - _dataStart);
                Array.ConstrainedCopy(_data, 0, newData, _data.Length - _dataStart, _dataEnd);
            }

            var len = Count;

            _data = newData;
            _dataStart = 0;
            _dataEnd = len;
        }

        public IEnumerator<T> GetEnumerator() => Enumerable.Range(0, Count).Select(x => this[x]).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
