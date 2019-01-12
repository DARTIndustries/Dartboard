using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PirateRobotArmyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmy
{
    public class TeamRenderInfo
    {
        public Texture2D Insignia { get; }
        public Color Color { get; private set; }

        public TeamRenderInfo(TeamInfoAttribute teamInfo, ContentManager cm)
        {
            try
            {
                var colorComponents = teamInfo.Color.Split(',');
                Color = new Color(int.Parse(colorComponents[0]), int.Parse(colorComponents[1]), int.Parse(colorComponents[2]), 255);
            }
            catch(Exception)
            {
                Color = new Color(255, 255, 255);
            }

            try
            {
                Insignia = cm.Load<Texture2D>("Insignias/" + teamInfo.Insignia);
            }
            catch(Exception)
            {
                Insignia = null;
            }
        }
    }
}
