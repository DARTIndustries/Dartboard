using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TeamInfoAttribute : Attribute
    {
        public string Author { get; }
        public string Name { get; }
        public string Strategy { get; }

        public string Insignia { get; }
        public string Color { get; }

        public TeamInfoAttribute(string author, string name, string strategry = "Shoot the Baddies", string insignia = "", string color = "255,255,255")
        {
            Author = author;
            Name = name;
            Strategy = strategry;
            Color = color;
            Insignia = insignia;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
