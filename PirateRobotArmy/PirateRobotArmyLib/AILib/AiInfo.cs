using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib.AILib
{
    public class AiInfo
    {
        private readonly MutableAiInfo Source;

        public IReadOnlyList<IBotView> FriendlyBots => Source.FriendlyBots;
        public IReadOnlyList<IBotView> EnemyBots => Source.EnemyBots;

        public double CurrentArenaRadius => Source.CurrentArenaRadius;

        public AiInfo(MutableAiInfo source)
        {
            Source = source;
        }
    }

    public class MutableAiInfo
    {
        public List<IBotView> FriendlyBots { get; }
        public List<IBotView> EnemyBots { get; }

        public double CurrentArenaRadius;

        public MutableAiInfo(double arenaSize)
        {
            CurrentArenaRadius = arenaSize;
            FriendlyBots = new List<IBotView>();
            EnemyBots = new List<IBotView>();
        }
    }
}
