using GeometryLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib
{
    /// <summary>
    /// Internal-ish class, used to actually track and modify the state of a bot
    /// </summary>
    public class Bot : IBotView
    {
        const double MIN_VELOCITY = .00001;

        /// <summary>
        /// Exposed to AI to view (but not modify) a bot
        /// </summary>
        public class BotView : IBotView
        {
            private readonly Bot Source;

            public BotView(Bot source)
            {
                Source = source;
            }

            public double CurrentAccuracy => Source.CurrentAccuracy;
            public Vector? CurrentAim => Source.CurrentAim;
            public double CurrentReload => Source.CurrentReload;
            public double HP => Source.HP;
            public Vector Position => Source.Position;
            public double Shield => Source.Shield;
            public int TeamNumber => Source.TeamNumber;
            public Vector Velocity => Source.Velocity;
            public ReadonlyPowerDistribution Power => Source.Power;
            public GameConstants Constants => Source.Constants;
            public Vector TeamOrigin => Source.TeamOrigin;
            public Circle Hitbox => Source.Hitbox;
        }

        /// <summary>
        /// Exposed to AI to allow the control of a bot
        /// </summary>
        public class BotControl : BotView
        {
            private readonly Bot Source;

            public Angle EngineAngle;
            public double PercentEngineThrust;

            public void DriveToward(Vector target, double enginePercent = 1)
            {
                EngineAngle = Angle.AngleBetween(Source.Position, target);
                PercentEngineThrust = enginePercent;
            }

            public EFiringRules FiringRule;

            public BotControl(Bot source) : base(source)
            {
                Source = source;
                EngineAngle = 0;
                PercentEngineThrust = 0;
                FiringRule = EFiringRules.HOLD_FIRE;
            }

            public new PowerDistribution Power => Source._Power;
            public Vector? Aim
            {
                get
                {
                    return Source.CurrentAim;
                }
                set
                {
                    if (value != null)
                        Source.CurrentAim = new Vector(value.Value);
                    else
                        Source.CurrentAim = null;
                }
            }
        }

        public BotView View { get; }
        public BotControl Control { get; }

        public int TeamNumber { get; }
        public double HP { get; private set; }
        public Vector Position { get; set; }
        private Circle _Hitbox;
        public Circle Hitbox => _Hitbox;
        public Vector Velocity { get; set; }
        public Vector? CurrentAim { get; set; }
        public Vector? LastAim;
        public Vector TeamOrigin { get; private set; }

        public double CurrentReload { get; private set; }
        public double Shield { get; private set; }
        public double CurrentAccuracy { get; private set; }
        

        private readonly PowerDistribution _Power;
        public ReadonlyPowerDistribution Power { get; }

        public GameConstants Constants { get; }
        public Bot(int team, int maxTeams, Random r, GameConstants gameSetup)
        {
            Constants = gameSetup;
            TeamNumber = team;
            Velocity = new Vector(0, 0, true);
            CurrentAim = null;

            CurrentAim = null;
            CurrentReload = 0;
            Shield = 0;
            CurrentAccuracy = Constants.MinimumAccuracy;

            _Power = new PowerDistribution(gameSetup);
            Power = new ReadonlyPowerDistribution(_Power);
            

            var teamTheta = (Math.PI * 2) / maxTeams;
            TeamOrigin = Vector.CreateMA(Constants.SpawnDistance, teamTheta * team);
            var teamOffset = Vector.CreateMA(Constants.SpawnRadius * r.NextDouble(), r.NextDouble() * Math.PI * 2);

            Position = Vector.Add(TeamOrigin, teamOffset);
            HP = Constants.MaxHP;

            View = new BotView(this);
            Control = new BotControl(this);
            _Hitbox = new Circle(Position, 0.5);
        }

        public void Tick(IEnumerable<Bot> allBots)
        {
            // Engine output is calculated as follows:
            // Power devoted to engines is multiplied by the Acceleration per point of power to get a total max engine power.
            // Then, the magnitude of the target engine vector is clamped between 0-1. 0 represents engines off, 1 represents full power.
            // Multiplying the max engine power by the target engine clamped magnitude gets desired engine power.
            var engineOutput = _Power.EnginePower * Constants.AccelPerPower * Math.Min(Math.Max(0, Control.PercentEngineThrust), 1);

            if(engineOutput != 0)
            {
                Vector newV = Control.EngineAngle.CreateVector(engineOutput);
                Velocity = Velocity + newV;
            }

            if (Velocity.Magnitude > Constants.MaxMoveSpeed)
                Velocity = Vector.Normalize(Velocity) * Constants.MaxMoveSpeed;
            if (Velocity.Magnitude < MIN_VELOCITY)
                Velocity = new Vector(0, 0, true);

            Position += Velocity;
            Velocity *= (1-Constants.Friction);
            _Hitbox.Position = Position;

            var colidingWith = allBots.Where(x => x != this && x.Hitbox.Intersects(this.Hitbox));
            foreach(var colision in colidingWith)
            {
                Circle fakeHitbox = new Circle(_Hitbox.Position, _Hitbox.Radius / 2);
                fakeHitbox = Circle.Uncollide(fakeHitbox, colision.Hitbox, (-Velocity).Angle);
                _Hitbox.Position = fakeHitbox.Position;

                colision._Hitbox = Circle.Uncollide(colision._Hitbox, Hitbox, Velocity.Angle);
                colision.Position = colision._Hitbox.Position;

                Position = _Hitbox.Position;
            }


            CurrentReload += _Power.WeaponPower * Constants.ReloadPerPower;
            CurrentReload = Math.Min(CurrentReload, Constants.MaxStoredBullets);

            Shield += _Power.ShieldPower * Constants.ShieldPerPower;
            if (Shield > Constants.MaxShield)
                Shield = Constants.MaxShield;

            if (CurrentAim.HasValue)
            {
                var aimValue = CurrentAim.Value;
                if (!LastAim.HasValue)
                {
                    CurrentAccuracy = Constants.MinimumAccuracy;
                }
                else
                {
                    CurrentAccuracy += Constants.AimAccuracyDegrade * Math.Abs((LastAim.Value - CurrentAim.Value).Magnitude);
                }

                CurrentAccuracy += engineOutput * Constants.MovementAccuracyDegrade;
                CurrentAccuracy -= Constants.AimImprovementPerTick;

                CurrentAccuracy = Math.Max(Math.Min(CurrentAccuracy, Constants.MinimumAccuracy), Constants.MaximumAccuracy);
            }
            else
            {
                CurrentAccuracy = Constants.MinimumAccuracy;
            }

            LastAim = CurrentAim;
        }

        public void ShootingTick(Game g)
        {
            while (CurrentReload >= 1 && CurrentAim.HasValue)
            {
                switch (Control.FiringRule)
                {
                    case EFiringRules.HOLD_FIRE:
                        return;
                    case EFiringRules.FIRE_AT_MAX_ACCURACY:
                        if (CurrentAccuracy == Constants.MaximumAccuracy)
                        {
                            g.BotShoots(this, CurrentAim.Value, CurrentAccuracy);
                            CurrentAccuracy = Math.Min(Constants.MinimumAccuracy, CurrentAccuracy + Constants.ShootAccuracyDegrade);
                            CurrentReload -= 1;
                        }
                        return;
                    case EFiringRules.ALWAYS_FIRE:
                        g.BotShoots(this, CurrentAim.Value, CurrentAccuracy);
                        CurrentAccuracy = Math.Min(Constants.MinimumAccuracy, CurrentAccuracy + Constants.ShootAccuracyDegrade);
                        CurrentReload-=1;
                        break;
                }
            }
        }

        public void TakeDamage(double damage, out double hpDamage, out double shieldDamage)
        {
            if(Shield >= damage)
            {
                shieldDamage = damage;
                hpDamage = 0;
                Shield -= damage;
            }
            else
            {
                shieldDamage = Shield;
                Shield = 0;
                HP -= damage;
                hpDamage = damage;
            }
        }
    }
}
