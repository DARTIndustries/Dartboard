using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartboardEngine
{
    public class TimerCached<T> : Cached<T>
    {
        public override T Value
        {
            get
            {
                if(!Recache())
                {
                    if (DateTime.Now - LastCacheTime >= MaxTime)
                    {
                        Recache();
                    }
                }
                return _Value;
            }
        }

        public readonly TimeSpan MaxTime;
        private DateTime LastCacheTime;

        public TimerCached(Func<T> recache, TimeSpan maxCacheTime) : base(recache)
        {
            maxCacheTime = MaxTime;
            LastCacheTime = new DateTime(0);
        }

        public override bool Recache(bool force = false)
        {
            if(base.Recache(force))
            {
                LastCacheTime = DateTime.Now;
                return true;
            }
            return false;
        }
    }
}
