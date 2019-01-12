using GeometryLib;

namespace PirateRobotArmyLib
{
    public interface IBotView
    {
        double CurrentAccuracy { get; }
        Vector? CurrentAim { get; }
        Vector TeamOrigin { get; }
        double CurrentReload { get; }
        double HP { get; }
        Vector Position { get; }
        double Shield { get; }
        int TeamNumber { get; }
        Vector Velocity { get; }
        ReadonlyPowerDistribution Power { get; }
        GameConstants Constants { get; }
        Circle Hitbox { get; }
    }
}