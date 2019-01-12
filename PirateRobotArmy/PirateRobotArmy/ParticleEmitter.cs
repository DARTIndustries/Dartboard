using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmy
{
    public class ParticleManager
    {
        protected readonly LinkedList<Particle> _Particles;

        public ParticleManager()
        {
            _Particles = new LinkedList<Particle>();
        }

        public virtual void Tick()
        {
            if (_Particles.Count > 0)
            {
                LinkedListNode<Particle> p = _Particles.First;
                while (p != null)
                {
                    p.Value.Tick();
                    if (p.Value.LifeTime == 0)
                    {
                        var last = p;
                        p = p.Next;
                        _Particles.Remove(last);
                    }
                    else
                    {
                        p = p.Next;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            foreach (var p in _Particles)
            {
                p.Draw(sb);
            }
        }

        public void AddParticle(Particle p)
        {
            _Particles.AddFirst(p);
        }
    }
    public class ParticleEmitter : ParticleManager
    {
        public Func<ParticleEmitter, Particle> ParticleCreateFunction { get; }
        public Action<Particle> ParticleTickFunction { get; }

        public float DelayPerParticle { get; set; }
        private float pendingDelay;

        public Vector2 Position;

        public Random random;
        

        public ParticleEmitter(Vector2 position, float delayPerParticle, Func<ParticleEmitter, Particle>  create, Action<Particle> tick)
        {
            random = new Random();
            DelayPerParticle = delayPerParticle;
            pendingDelay = 0;
            Position = position;
            ParticleTickFunction = tick;
            ParticleCreateFunction = create;
        }

        public override void Tick()
        {
            base.Tick();
            pendingDelay++;
            var particles = pendingDelay / DelayPerParticle;
            pendingDelay -= particles * DelayPerParticle;

            for(int i = 0; i < (int)particles; i++)
            {
                _Particles.AddLast(ParticleCreateFunction(this));
            }
        }
    }

    public class Particle
    {
        public static Action<Particle> FrictionUpdate(float friction) => (p => p.Velocity *= friction);
        public static Action<Particle> Fade(int endOfLifeFade) => (p =>
        {
            if(p.LifeTime < endOfLifeFade)
            {
                var fadeAmt = 1.0 / endOfLifeFade * (endOfLifeFade - p.LifeTime);
                p.Opacity = Math.Min(p.Opacity, (float)fadeAmt);
            }
        });
        public static Action<Particle> Combine(params Action<Particle>[] actions) => p => { foreach (var a in actions) a(p); };

        private Texture2D Pix { get; }
        public Vector2 Position { get; private set; }
        public Vector2 Velocity;
        public int LifeTime { get; private set; }
        public int InitialLifetime { get; }
        public float Size { get; set; }
        public Color Color;
        public Action<Particle> Update { get; }
        public float Opacity;
        public Particle(Action<Particle> update, Texture2D texture, Vector2 position, Vector2 vel, int lifetime, float size, Color color)
        {
            Color = color;
            Pix = texture;
            Position = position;
            LifeTime = lifetime;
            InitialLifetime = LifeTime;
            Size = size;
            Velocity = vel;
            Opacity = 1;
            Update = update;
        }

        public void Tick()
        {
            Update(this);
            LifeTime--;
            Position += Velocity;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Pix, Position, null, Color * Opacity, 0, Vector2.Zero, Size, SpriteEffects.None, 0);
        }
    }
}
