using System;

namespace DartboardEngine
{
    public class Cached<T>
    {
        public event Action OnInvalidated;
        public event Action<T> OnRecached;

        public virtual T Value
        {
            get
            {
                Recache();
                return _Value;
            }
        }

        protected T _Value;
        protected bool valid;
        protected readonly Func<T> _recache;

        public Cached(Func<T> recache)
        {
            _recache = recache;
            valid = false;
            _Value = default(T);
        }

        public virtual bool Recache(bool force = false)
        {
            if (!valid || force)
            {
                _Value = _recache();
                OnRecached?.Invoke(_Value);
                valid = true;
                return true;
            }
            return false;
        }

        public virtual void Invalidate()
        {
            valid = false;
            OnInvalidated?.Invoke();
        }

        public static implicit operator T(Cached<T> cache) => cache.Value;
    }
}
