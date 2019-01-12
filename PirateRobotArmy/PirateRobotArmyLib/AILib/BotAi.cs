using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib.AILib
{
    public abstract class BotAi
    {
        public Bot.BotControl Control { get; }

        public BotAi(Bot.BotControl control)
        {
            Control = control;
        }

        public virtual void Init() { }

        public abstract void Update(AiInfo Info);
    }
}
