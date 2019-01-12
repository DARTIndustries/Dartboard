using GeometryLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib
{
    public class ShootInfo
    {
        public Bot Origin { get; }
        public Bot HitTarget { get; }

        public Line Trajectory { get; }

        public double ShieldDamage { get; }
        public double HpDamage { get; }

        public ShootInfo(Bot origin, Bot hitTarget, Line trajectory, double shieldDamage, double hpDamage)
        {
            Origin = origin;
            HitTarget = hitTarget;
            Trajectory = trajectory;
            ShieldDamage = shieldDamage;
            HpDamage = hpDamage;
        }
    }
}
