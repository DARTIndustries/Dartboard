using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmy
{
    static class ImageLoader
    {
        public static Texture2D BotCoreSprite { get; private set; }
        public static Texture2D BotWeaponSprite { get; private set; }
        public static Texture2D BotShieldSprite { get; private set; }
        public static Texture2D BotEngineSprite { get; private set; }

        public static Texture2D SinglePixel { get; private set; }

        public static Texture2D ArenaRing { get; private set; }
        public static Texture2D ArenaCenter { get; private set; }

        public static void Load(ContentManager cm, GraphicsDevice gd)
        {
            BotCoreSprite = cm.Load<Texture2D>("Robot_Base");
            BotWeaponSprite = cm.Load<Texture2D>("Robot_Gun");
            BotShieldSprite = cm.Load<Texture2D>("Robot_Shield");
            BotEngineSprite = cm.Load<Texture2D>("Robot_Engine");
            ArenaRing = cm.Load<Texture2D>("ArenaRing");
            ArenaCenter = cm.Load<Texture2D>("ArenaCenter");

            SinglePixel = new Texture2D(gd, 1, 1);
            SinglePixel.SetData(new Color[] { Color.White });
        }
    }
}
