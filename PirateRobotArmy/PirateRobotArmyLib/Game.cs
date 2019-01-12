using GeometryLib;
using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib
{
    public class Game
    {
        const int MAX_COLISION_RESOLVE_TICKS = 3;

        private Random Random { get; }
        public GameConstants Constants { get; }
        public int NumTeams { get; }
        public int BotsPerTeam { get; }
        public List<Team> Teams { get; }

        public Team Winner { get; private set; }

        public List<Bot> DeathsLastTick { get; }
        public List<ShootInfo> ShotsLastTick { get; }

        public double CurrentArenaRadius { get; private set; }
        public bool Draw { get; private set; }


        public Game(GameConstants constants, int botsPerTeam, params TeamAi[] teamAis)
        {
            CurrentArenaRadius = constants.ArenaRadius;
            Random = new Random();
            if(teamAis.Length < 2)
            {
                throw new ArgumentException("Game needs at least 2 teams");
            }

            Constants = constants;
            NumTeams = teamAis.Length;
            BotsPerTeam = botsPerTeam;
            Teams = new List<Team>();
            DeathsLastTick = new List<Bot>();
            ShotsLastTick = new List<ShootInfo>();

            for(int team = 0; team < teamAis.Length; team++)
            {
                List<Bot> teamBots = new List<Bot>();
                for(int bot = 0; bot < botsPerTeam; bot++)
                {
                    teamBots.Add(new Bot(team, teamAis.Length, Random, constants));
                }

                Team teamControl = new Team(constants, team, teamAis[team], teamBots);
                Teams.Add(teamControl);
                UncollideBots(teamControl);
            }
        }

        // Un-collides bots. Ensures no two bots overlap
        public void UncollideBots(Team t)
        {
            for(int i = 0; i < MAX_COLISION_RESOLVE_TICKS; i++)
            {
                bool colided = false;
                foreach (var bot in t.BotAi.Keys)
                {
                    foreach (var otherbot in t.BotAi.Keys)
                    {
                        if(bot != otherbot)
                        {
                            if(bot.Position.DistanceSq(otherbot.Position) < 1)
                            {
                                double overlap = 1-bot.Position.Distance(otherbot.Position);
                                double shift = overlap / 2;
                                Vector DeltaVector = Vector.Subtract(bot.Position, otherbot.Position);
                                DeltaVector.Magnitude = 1;
                                bot.Position += shift * DeltaVector;
                                otherbot.Position -= shift * DeltaVector;
                                colided = true;
                            }
                        }
                    }
                }
                if (!colided)
                    return;
            }
        }

        public void Tick()
        {
            DeathsLastTick.Clear();
            ShotsLastTick.Clear();
            if (Winner != null || Draw)
                return;
            foreach(var team in Teams)
            {
                team.AiTick(this, Teams);
            }
            foreach (var team in Teams)
            {
                team.AttackTick(this);
            }
            int livingTeams = 0;
            for(int i = Teams.Count - 1; i >= 0; i--)
            {
                Team team = Teams[i];
                team.DieTick(this);
                if (team.BotAi.Count > 0)
                    livingTeams++;
            }
            if (livingTeams == 1)
            {
                Winner = Teams.First(x => x.BotAi.Count != 0);
            }
            if(livingTeams == 0)
            {
                Draw = true;
            }
        }

        public void BotShoots(Bot Source, Vector Target, double Accuracy)
        {
            Vector origin = new Vector(Source.Position);
            Vector AngledShot = Target - origin;
            AngledShot.Magnitude = Constants.MaxRange;
            AngledShot.Angle += (Random.NextDouble() * Accuracy) - (Accuracy / 2);
            Vector realTarget = origin + AngledShot;

            Bot colided = null;

            Line trajectory = new Line(origin, realTarget);

            foreach(Team team in Teams)
            {
                foreach(Bot otherbot in team.BotAi.Keys.Where(t=>t!=Source))
                {
                    Vector? colision = otherbot.Hitbox.FirstIntersection(trajectory);
                    if(colision.HasValue)
                    {
                        colided = otherbot;
                        trajectory.End = colision.Value;
                    }
                }
            }

            double hpdamage = 0, shielddamage = 0;
            colided?.TakeDamage(1, out hpdamage, out shielddamage);
            ShotsLastTick.Add(new ShootInfo(Source, colided, trajectory, shielddamage, hpdamage));
        }
    }
}
