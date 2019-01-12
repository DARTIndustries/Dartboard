using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PirateRobotArmyLib
{
    public class GameConstants
    {
        public double MaxMoveSpeed { get; private set; }
        public double AccelPerPower { get; private set; }
        public double Friction { get; private set; }
        public double MaxRange { get; private set; }
        public double ViewRange { get; private set; }
        public double MinimumAccuracy { get; private set; }
        public double MaximumAccuracy { get; private set; }
        public double MovementAccuracyDegrade { get; private set; }
        public double AimAccuracyDegrade { get; private set; }
        public double AimImprovementPerTick { get; private set; }
        public double MaxShield { get; private set; }
        public double ShieldPerPower { get; private set; }
        public double ReloadPerPower { get; private set; }
        public double SpawnDistance { get; private set; }
        public double SpawnRadius { get; private set; }
        public double ArenaRadius { get; private set; }

        public double MaxStoredBullets { get; private set; }
        public double ShootAccuracyDegrade { get; private set; }

        public int TotalBots { get; private set; }
        public int MaxPower { get; private set; }
        public double MaxHP { get; private set; }

        public GameConstants(XmlNode fromXml)
        {
            foreach(var prop in typeof(GameConstants).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance))
            {
                XmlNode node = fromXml.SelectSingleNode(prop.Name);
                if (node != null)
                {
                    string strValue = node.InnerText;
                    if(prop.PropertyType == typeof(double))
                    {
                        prop.SetValue(this, double.Parse(strValue));
                    }
                    else if(prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(this, int.Parse(strValue));
                    }
                }
            }
        }
    }
}
