using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryLib
{
    internal interface ICached
    {
        void Invalidate();
    }
    internal class Cached<T> : ICached
    {
        bool isValid;
        Func<T> getT;
        T cache;

        public List<ICached> Dependants { get; }

        public T Value
        {
            get
            {
                if (!isValid)
                    cache = getT();
                return cache;
            }
            set
            {
                isValid = true;
                cache = value;
            }
        }

        public Cached(T value, Func<T> getValue):this(getValue)
        {
            cache = value;
            isValid = true;
            Dependants = new List<ICached>();
        }
        public Cached(Func<T> GetValue)
        {
            getT = GetValue;
            isValid = false;
            Dependants = new List<ICached>();
        }

        public void Invalidate()
        {
            isValid = false;
            foreach (var dep in Dependants)
                dep.Invalidate();
        }

        public static implicit operator T(Cached<T> cached)
        {
            return cached.Value;
        }
        public static explicit operator Cached<T>(T value)
        {
            return new Cached<T>(value, ()=>value);
        }
    }
    internal class Cached<IN, T> : ICached
    {
        bool isValid;
        Func<IN, T> getT;
        T cache;

        public List<ICached> Dependants { get; }
        public Cached(Func<IN, T> GetValue)
        {
            getT = GetValue;
            isValid = false;
            Dependants = new List<ICached>();
        }

        public void Invalidate()
        {
            isValid = false;
        }

        public static explicit operator T(Cached<IN, T> cached)
        {
            return cached.ValueOrFail();
        }

        public T ValueOrFail()
        {
            if (isValid)
                return cache;
            throw new InvalidOperationException("Cache not in valid state");
        }

        public void Assign(T value)
        {
            isValid = true;
            cache = value;
        }

        public T GetValue(IN source)
        {
            if(isValid)
            {
                return cache;
            }
            cache = getT(source);
            isValid = true;
            return cache;
        }
    }
}
