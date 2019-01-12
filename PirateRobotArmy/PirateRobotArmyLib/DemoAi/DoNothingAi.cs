using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib.DemoAi
{
    [TeamInfo("Nathan", "DoNothing", "Gives itself shields, and chills.")]
    public class DoNothingAi : TeamAi
    {
        public override BotAi CreateBotAi(Bot.BotControl controller)
        {
            return new DoNothingBotAi(controller);
        }
    }

    public class DoNothingBotAi : BotAi
    {
        public DoNothingBotAi(Bot.BotControl controller) : base(controller)
        {
            controller.Power.ShieldPower = controller.Power.RemainingPower;
        }

        public override void Update(AiInfo Info)
        {
        }
    }
}
