using GeometryLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameResources;
using PirateRobotArmyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmy
{
    class BotRenderInfo
    {
        private ParticleEmitter EffectEmitter;
        private ParticleEmitter FlameEmitter;
        private static Vector2 Origin { get; } = new Vector2(16, 16);
        private static Vector2 Size { get; } = new Vector2(1, 1);

        public Bot Bot { get; }
        public Vector2 target;

        public Angle Rotation;
        private double lastHp;

        public BotRenderInfo(Bot bot)
        {
            Bot = bot;
            lastHp = bot.HP;
            target = new Vector2((float)bot.Position.X, (float)bot.Position.Y);
            Rotation = 0;

        }

        public void Update()
        {
            var smokeAt = Bot.Constants.MaxHP / 2;
            if((lastHp > smokeAt && Bot.HP <= smokeAt) || (EffectEmitter == null && FlameEmitter != null))
            {
                EffectEmitter = new ParticleEmitter(new Vector2((float)Bot.Position.X, (float)Bot.Position.Y), 1, CreateSmoke, TickSmoke);
            }
            var flameAt = Bot.Constants.MaxHP / 3;
            if(lastHp > flameAt && Bot.HP <= flameAt)
            {
                FlameEmitter = new ParticleEmitter(new Vector2((float)Bot.Position.X, (float)Bot.Position.Y), 1, CreateFlame, TickFlame);
            }
            

            lastHp = Bot.HP;
            if(EffectEmitter != null)
            {
                float percentDamage = 0.5f - (float)(Bot.HP / Bot.Constants.MaxHP);
                EffectEmitter.DelayPerParticle = 2f - percentDamage;
                EffectEmitter.Position.X = (float)Bot.Position.X;
                EffectEmitter.Position.Y = (float)Bot.Position.Y;
            }
            if(FlameEmitter != null)
            {

                float percentDamage = 0.5f - (float)(Bot.HP / Bot.Constants.MaxHP);
                FlameEmitter.DelayPerParticle = (1.6f - percentDamage)/2;
                FlameEmitter.Position.X = (float)Bot.Position.X;
                FlameEmitter.Position.Y = (float)Bot.Position.Y;
            }
            EffectEmitter?.Tick();
            FlameEmitter?.Tick();
        }

        private Particle CreateSmoke(ParticleEmitter e)
        {
            int grey = e.random.Next(0, 150);
            return new Particle(e.ParticleTickFunction,
                ImageLoader.SinglePixel,
                e.Position + new Vector2((float)e.random.NextDouble() * 2 - 1, (float)e.random.NextDouble() * 2 - 1),
                new Vector2((float)e.random.NextDouble() * .02f - .01f, -((float)e.random.NextDouble() * .1f + .1f)),
                100 + e.random.Next(0, 80),
                (float)e.random.NextDouble() * .8f + .1f, new Color(Math.Min(255, grey + e.random.Next(10)), grey, grey, 50))
            { Opacity = .2f } ;
        }

        private void TickSmoke(Particle p)
        {
            p.Velocity *= .98f;
            p.Velocity.X += GlobalRandom.NextFloat() * .01f - .002f;
            p.Size += GlobalRandom.NextFloat(0.1f);
            p.Opacity -= GlobalRandom.NextFloat(.001f);
        }


        private Particle CreateFlame(ParticleEmitter e)
        {
            int grey = e.random.Next(0, 50);
            if (e.random.Next(3) != 0)
            {
                return new Particle(e.ParticleTickFunction,
                    ImageLoader.SinglePixel,
                    e.Position + new Vector2((float)e.random.NextDouble() - 0.5f, (float)e.random.NextDouble() - 0.5f),
                    new Vector2((float)e.random.NextDouble() * .02f - .01f, -((float)e.random.NextDouble() * .1f + .1f)),
                    10 + e.random.Next(0, 5),
                    (float)e.random.NextDouble() * .6f + .05f,
                    new Color(255 - grey, grey, grey, 50))
                { Opacity = 0.8f };
            }

            return new Particle(e.ParticleTickFunction,
                ImageLoader.SinglePixel,
                e.Position + new Vector2((float)e.random.NextDouble() - 0.5f, (float)e.random.NextDouble() - 0.5f),
                new Vector2((float)e.random.NextDouble() * .02f - .01f, -((float)e.random.NextDouble() * .6f + .2f)),
                10 + e.random.Next(0, 5),
                (float)e.random.NextDouble() * .3f + .05f,
                new Color(255 - grey, 255 - grey, grey, 120))
            { Opacity = 0.9f };
        }

        private void TickFlame(Particle p)
        {
            p.Velocity *= .98f;
            p.Size -= GlobalRandom.NextFloat(0.1f);
            p.Opacity -= GlobalRandom.NextFloat(.001f);
        }

        public void Draw(SpriteBatch sb, TeamRenderInfo team)
        {
            target.X = (float)Bot.Position.X;
            target.Y = (float)Bot.Position.Y;


            bool isAiming;
            if(Bot.CurrentAim != null)
            {
                Rotation = (float)Vector.Subtract(Bot.CurrentAim.Value, Bot.Position).Angle;
                isAiming = true;
            }
            else
            {
                Rotation = (float)Bot.Velocity.Angle;
                isAiming = false;
            }

            if(Bot.HP > 0 && isAiming)
            {
                Angle leftMostAccuracy = Rotation + Bot.CurrentAccuracy;
                Angle rightMostAccuracy = Rotation - Bot.CurrentAccuracy;
                RenderHelper.DrawLine(sb, Bot.Position, Bot.Position + leftMostAccuracy.CreateVector(1.2), 0.01, ImageLoader.SinglePixel, Color.Red);
                RenderHelper.DrawLine(sb, Bot.Position, Bot.Position + rightMostAccuracy.CreateVector(1.2), 0.01, ImageLoader.SinglePixel, Color.Red);
            }

            sb.Draw(ImageLoader.BotCoreSprite, target, null, Color.White * (Bot.HP <= 0 ? 0.2f : 1f), (float)Rotation.AbsoluteValue, Origin, 1 / 32f, SpriteEffects.None, 0);

            if (team.Insignia != null)
            {
                sb.Draw(team.Insignia, target, null, Bot.HP <= 0 ? Color.Gray : team.Color, (float)Rotation.AbsoluteValue, Origin, 1 / 32f, SpriteEffects.None, 0);
            }


            if (Bot.HP > 0)
            {
                if (Bot.Power.EnginePower != 0)
                {
                    float alpha = Bot.Power.EnginePower / (float)Bot.Constants.MaxPower;
                    sb.Draw(ImageLoader.BotEngineSprite, target, null, Color.White * alpha, (float)Rotation.AbsoluteValue, Origin, 1 / 32f, SpriteEffects.None, 0);
                }

                if (Bot.Power.WeaponPower != 0)
                {
                    float alpha = Bot.Power.WeaponPower / (float)Bot.Constants.MaxPower;
                    sb.Draw(ImageLoader.BotWeaponSprite, target, null, Color.White * alpha, (float)Rotation.AbsoluteValue, Origin, 1 / 32f, SpriteEffects.None, 0);
                }

                if (Bot.Power.ShieldPower != 0)
                {
                    float alpha = Bot.Power.ShieldPower / (float)Bot.Constants.MaxPower;
                    sb.Draw(ImageLoader.BotShieldSprite, target, null, Color.White * alpha, (float)Rotation.AbsoluteValue, Origin, 1 / 32f, SpriteEffects.None, 0);
                }
            }
        }

        public void OverDraw(SpriteBatch sb, TeamRenderInfo info)
        {
            //EffectEmitter?.Draw(sb);
            //FlameEmitter?.Draw(sb);
        }
    }
}
