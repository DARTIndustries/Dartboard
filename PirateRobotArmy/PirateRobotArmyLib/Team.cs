using GeometryLib;
using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib
{
    public class Team
    {
        public int TeamNumber { get; }
        public TeamAi Ai { get; }
        public Dictionary<Bot, BotAi> BotAi { get; }
        public MutableAiInfo Info { get; }
        private AiInfo ImmutableInfo { get; }
        public GameConstants Constants { get; }

        public Team(GameConstants c, int teamNumber, TeamAi ai, IEnumerable<Bot> bots)
        {
            Constants = c;
            TeamNumber = teamNumber;
            Ai = ai;
            Info = new MutableAiInfo(c.ArenaRadius);
            ImmutableInfo = new AiInfo(Info);
            BotAi = new Dictionary<Bot, BotAi>();
            foreach(var bot in bots)
            {
                BotAi.Add(bot, ai.CreateBotAi(bot.Control));
                Info.FriendlyBots.Add(bot.View);
                BotAi[bot].Init();
            }
            Ai.OnGameStart(BotAi.Values);
        }

        public void AiTick(Game g, IEnumerable<Team> AllTeams)
        {
            Info.EnemyBots.Clear();
            Info.CurrentArenaRadius = g.CurrentArenaRadius;
            foreach(var team in AllTeams.Where(t => t != this))
            {
                foreach(var enemyBot in team.BotAi.Keys)
                {
                    foreach(var friendlyBot in BotAi.Keys)
                    {
                        if(Vector.Subtract(enemyBot.Position, friendlyBot.Position).Magnitude <= Constants.ViewRange)
                        {
                            Info.EnemyBots.Add(enemyBot.View);
                            break;
                        }
                    }
                }
            }
            Ai.BeforeUpdate(BotAi.Values);
            foreach(var ai in BotAi)
            {
                ai.Value.Update(ImmutableInfo);
            }
            Ai.AfterUpdate(BotAi.Values);
        }

        public void AttackTick(Game g)
        {
            foreach (var bot in BotAi.Keys)
            {
                bot.ShootingTick(g);
            }
            foreach(var bot in BotAi.Keys)
            {
                bot.Tick(g.Teams.SelectMany(t => t.BotAi.Keys));
            }
        }

        public void DieTick(Game g)
        {
            foreach(var bot in BotAi.Keys.ToArray())
            {
                double _, __;
                if (bot.Position.DistanceSq(new Vector(0, 0, true)) >= g.CurrentArenaRadius * g.CurrentArenaRadius)
                    bot.TakeDamage(9999, out _, out __);
                if(bot.HP <= 0)
                {
                    BotAi.Remove(bot);
                    g.DeathsLastTick.Add(bot);
                }
            }
        }
    }
}
