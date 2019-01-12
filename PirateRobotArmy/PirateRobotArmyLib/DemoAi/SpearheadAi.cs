using GeometryLib;
using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib.DemoAi
{
    [TeamInfo("Nathan", "Spearhead", "Makes a spearhead. Moves in unision", "Cube", "0,255,255")]
    public class SpearheadAi : TeamAi
    {
        private int id = 0;
        public override BotAi CreateBotAi(Bot.BotControl controller)
        {
            return new SpearheadAiBotAi(controller, id++);
        }

        public override void OnGameStart(IEnumerable<BotAi> Ais)
        {
            SpearheadAiBotAi[] ais = Ais.OfType<SpearheadAiBotAi>().ToArray();

            var spearheadLeader = ais[0];
            var botsPerAxis = (ais.Length - 1) / 2;

            spearheadLeader.FormationOffset = Vector.CreateXY(0,0);
            spearheadLeader.FormationPosition = spearheadLeader.Control.TeamOrigin;

            var localAngle = new Angle(Vector.CreateXY(0, 0), spearheadLeader.Control.TeamOrigin);
            Angle leftAngle = localAngle + (Math.PI / 4);
            Angle rightAngle = localAngle - (Math.PI / 4);
            for (int i = 1; i < botsPerAxis + 1; i++)
            {
                ais[i].FormationOffset = Vector.CreateMA(i * 1.6, leftAngle.AbsoluteValue);
                ais[i].FormationPosition = spearheadLeader.Control.TeamOrigin;
                ais[i].LeftFlank = true;
            }
            for(int i = 1 + botsPerAxis; i < ais.Length; i++)
            {
                ais[i].FormationOffset = Vector.CreateMA((i-botsPerAxis) * 1.6, rightAngle.AbsoluteValue);
                ais[i].FormationPosition = spearheadLeader.Control.TeamOrigin;
                ais[i].LeftFlank = false;
            }
        }
        private int tick = 0;
        public override void BeforeUpdate(IEnumerable<BotAi> Ais)
        {
            SpearheadAiBotAi[] ais = Ais.OfType<SpearheadAiBotAi>().ToArray();
            if (ais.Length == 0)
                return;
            tick++;
            if(tick >= 120)
            {
                var moveTowardCenter = new Angle(Vector.CreateXY(0, 0), ais[0].FormationPosition);
                var moveAmt = moveTowardCenter.CreateVector(-0.03);
                foreach (var ai in ais)
                    ai.FormationPosition += moveAmt;
            }
        }
    }

    public class SpearheadAiBotAi : BotAi
    {
        public Vector FormationPosition;
        public Vector FormationOffset;

        public bool LeftFlank;
        public bool isInPosition = false;
        private int id;
        public SpearheadAiBotAi(Bot.BotControl controller, int id) : base(controller)
        {
            this.id = id;
        }
        public override void Init()
        {
            Control.Power.EnginePower = Control.Power.RemainingPower / 3;
            Control.Power.ShieldPower = Control.Power.RemainingPower / 2;
            Control.Power.WeaponPower = Control.Power.RemainingPower;

            Control.FiringRule = EFiringRules.FIRE_AT_MAX_ACCURACY;
        }
        public override void Update(AiInfo Info)
        {
            if(Info.EnemyBots.Count > 0)
            {
                foreach(var enemy in Info.EnemyBots)
                {
                    if (enemy.Position.Distance(Control.Position) >= Control.Constants.MaxRange * 0.9f)
                        continue;

                    var angleToEnemy = Angle.AngleBetween(FormationPosition, Vector.CreateXY(0, 0), enemy.Position);

                    if((angleToEnemy.RelativeValue <= 0 && LeftFlank) || (angleToEnemy.RelativeValue >= 0 && !LeftFlank))
                    {
                        Control.Aim = enemy.Position;
                        break;
                    }
                }
            }
            else
            {
                Control.Aim = null;
            }

            if(Control.Position.Distance(FormationPosition + FormationOffset) < 0.5f)
            {
                isInPosition = true;
                Control.PercentEngineThrust = 0;
            }
            else
            {
                Control.DriveToward(FormationPosition + FormationOffset, Math.Min(1, Control.Position.Distance(FormationPosition + FormationOffset)));
            }
        }
    }
}
