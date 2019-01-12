using GeometryLib;
using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib.DemoAi
{
    [TeamInfo("Nathan", "Basic-Bot", "Drives toward the middle, shooting baddies.", "Stripe", "0,0,255")]
    public class BasicBotAi : TeamAi
    {
        public override BotAi CreateBotAi(Bot.BotControl controller)
        {
            return new BasicBotIndivAi(controller);
        }
    }

    public class BasicBotIndivAi : BotAi
    {
        Vector OriginOffset;
        public BasicBotIndivAi(Bot.BotControl controller) : base(controller)
        {

        }

        public override void Init()
        {
            OriginOffset = Vector.Subtract(Control.Position, Control.TeamOrigin);

            Control.Power.EnginePower = Control.Power.RemainingPower / 3;
            Control.Power.ShieldPower = Control.Power.RemainingPower / 2;
            Control.Power.WeaponPower = Control.Power.RemainingPower;

            Control.FiringRule = EFiringRules.ALWAYS_FIRE;
        }

        public override void Update(AiInfo Info)
        {
            Control.DriveToward(OriginOffset);

            if(Info.EnemyBots.Count > 0)
            {
                // I see someone
                var targetBot = Info.EnemyBots[0];

                Control.Aim = targetBot.Position;
            }
            else
            {
                Control.Aim = null;
            }
        }
    }
}
