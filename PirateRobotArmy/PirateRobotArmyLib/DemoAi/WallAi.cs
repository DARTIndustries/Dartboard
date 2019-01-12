using GeometryLib;
using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib.DemoAi
{
    [TeamInfo("Nathan", "Wall-e", "Makes a Wall. Moves in unision", "Cube", "0,255,255")]
    public class WallAi : TeamAi
    {
        public override BotAi CreateBotAi(Bot.BotControl controller)
        {
            return new WallBotAi(controller);
        }

        public override void OnGameStart(IEnumerable<BotAi> Ais)
        {
            WallBotAi[] ais = Ais.OfType<WallBotAi>().ToArray();
            
        }
        public override void BeforeUpdate(IEnumerable<BotAi> Ais)
        {
        }
    }

    public class WallBotAi : BotAi
    {

        public WallBotAi(Bot.BotControl controller) : base(controller)
        {
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
        }
    }
}
