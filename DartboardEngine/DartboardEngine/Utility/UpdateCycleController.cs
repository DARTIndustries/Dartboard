using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DartboardEngine.Utility
{
    public class UpdateCycleController : IDisposable
    {
        const int MIN_MILIS_FOR_SLEEP = 10;

        public IReadOnlyList<IUpdateable> Items => _Items;
        private readonly List<IUpdateable> _Items;

        private Thread _ActorThread;
        private Stopwatch _ActorTimer;
        private Timer _WakeupTimer;

        private bool shutdown;

        private readonly object _updateableObjectLock;

        public TimeSpan UpdateFrequency { get; }

        public UpdateCycleController(TimeSpan frequency)
        {
            UpdateFrequency = frequency;
            _Items = new List<IUpdateable>();
            _ActorTimer = new Stopwatch();
            _WakeupTimer = new Timer(_UpdateCallback);
            _updateableObjectLock = new object();
            shutdown = false;
        }

        public void Add(IUpdateable item)
        {
            lock (_updateableObjectLock)
                _Items.Add(item);
        }
        public bool Remove(IUpdateable item)
        {
            lock (_updateableObjectLock)
                return _Items.Remove(item);
        }
        public void Clear()
        {
            lock (_updateableObjectLock)
                _Items.Clear();
        }

        public void Start()
        {
            _WakeupTimer.Change(0, Timeout.Infinite);
        }

        private void _UpdateCallback(object _)
        {
            _ActorThread = Thread.CurrentThread;
            do
            {
                if (shutdown)
                    return;
                _ActorTimer.Restart();

                // Clone it, so that updates can change the cycle controller without breaking things.
                IUpdateable[] Clone;
                lock (_updateableObjectLock)
                {
                    Clone = _Items.ToArray();
                }

                foreach (var item in Clone)
                {
                    item.Update();
                    if (shutdown)
                        return;
                }

                _ActorTimer.Stop();

                // Check if the delay is too short to acurately sleep
            } while ((UpdateFrequency - _ActorTimer.Elapsed ).TotalMilliseconds < MIN_MILIS_FOR_SLEEP);

            // Otherwise, we sleep.
            var delta = (long)(UpdateFrequency - _ActorTimer.Elapsed).TotalMilliseconds;
            _WakeupTimer.Change(delta, Timeout.Infinite);
        }

        public void Dispose()
        {
            shutdown = true;
            _WakeupTimer.Dispose();
            _ActorThread?.Join();
        }
    }
}
