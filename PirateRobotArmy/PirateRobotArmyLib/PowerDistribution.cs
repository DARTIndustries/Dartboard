using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib
{
    public class PowerDistribution
    {
        public int RemainingPower { get; private set; }
        private int _EnginePower;
        private int _ShieldPower;
        private int _WeaponPower;
        public int EnginePower
        {
            get
            {
                return _EnginePower;
            }
            set
            {
                if(value < _EnginePower)
                {
                    var old = _EnginePower;
                    _EnginePower = Math.Max(0, value);
                    RemainingPower += old - _EnginePower;
                }
                else
                {
                    _EnginePower = Math.Min(value, _EnginePower + RemainingPower);
                }
            }
        }
        public int ShieldPower
        {
            get
            {
                return _ShieldPower;
            }
            set
            {
                if (value < _ShieldPower)
                {
                    var old = _ShieldPower;
                    _ShieldPower = Math.Max(0, value);
                    RemainingPower += old - _ShieldPower;
                }
                else
                {
                    _ShieldPower = Math.Min(value, _ShieldPower + RemainingPower);
                }
            }
        }
        public int WeaponPower
        {
            get
            {
                return _WeaponPower;
            }
            set
            {
                if (value < _WeaponPower)
                {
                    var old = _WeaponPower;
                    _WeaponPower = Math.Max(0, value);
                    RemainingPower += old - _WeaponPower;
                }
                else
                {
                    _WeaponPower = Math.Min(value, _WeaponPower + RemainingPower);
                }
            }
        }

        public PowerDistribution(GameConstants constants)
        {
            RemainingPower = constants.MaxPower;
            _EnginePower = _ShieldPower = _WeaponPower = 0;
        }
    }

    public class ReadonlyPowerDistribution
    {
        private readonly PowerDistribution Power;
        public ReadonlyPowerDistribution(PowerDistribution p)
        {
            Power = p;
        }
        public int EnginePower => Power.EnginePower;
        public int ShieldPower => Power.ShieldPower;
        public int WeaponPower => Power.WeaponPower;
    }
}
