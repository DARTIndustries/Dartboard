using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib.AILib
{
    public abstract class TeamAi
    {
        public abstract BotAi CreateBotAi(Bot.BotControl controller);
        public virtual void OnGameStart(IEnumerable<BotAi> Ais) { }
        public virtual void BeforeUpdate(IEnumerable<BotAi> Ais) { }
        public virtual void AfterUpdate(IEnumerable<BotAi> Ais) { }
    }
}
