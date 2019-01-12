using GeometryLib;
using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib.DemoAi
{
    [TeamInfo("Nathan", "Stop-N-Shoot", "Drives toward the middle, stopping to shoot baddies.", "Stripe", "0,255,255")]
    public class StopAndShootAi : TeamAi
    {
        private int id = 0;
        public override BotAi CreateBotAi(Bot.BotControl controller)
        {
            return new StopAndShootBotAi(controller, id++);
        }
    }

    public class StopAndShootBotAi : BotAi
    {
        Vector OriginOffset;
        private int id;
        public StopAndShootBotAi(Bot.BotControl controller, int id) : base(controller)
        {
            this.id = id;
        }
        public override void Init()
        {
            OriginOffset = Vector.Subtract(Control.Position, Control.TeamOrigin);

            Control.Power.EnginePower = Control.Power.RemainingPower / 3;
            Control.Power.ShieldPower = Control.Power.RemainingPower / 2;
            Control.Power.WeaponPower = Control.Power.RemainingPower;

            Control.FiringRule = EFiringRules.FIRE_AT_MAX_ACCURACY;
        }
        public override void Update(AiInfo Info)
        {
            if(Info.EnemyBots.Count > 0)
            {
                Control.Power.EnginePower = 1;
                Control.Power.WeaponPower += Control.Power.RemainingPower;
                
                // I see someone
                var targetBot = Info.EnemyBots[0];

                Control.Aim = targetBot.Position;

                if (Vector.Subtract(targetBot.Position, Control.Position).Magnitude >= Control.Constants.MaxRange * .9)
                {
                    Control.FiringRule = EFiringRules.HOLD_FIRE;
                    Control.DriveToward(targetBot.Position);
                }
                else
                {
                    Control.FiringRule = EFiringRules.FIRE_AT_MAX_ACCURACY;
                    Control.PercentEngineThrust = 0;
                }
            }
            else
            {
                Control.DriveToward(OriginOffset);
                Control.Power.WeaponPower = 0;
                Control.Power.EnginePower = Control.Power.RemainingPower;
                Control.Aim = null;
            }
        }
    }
}
