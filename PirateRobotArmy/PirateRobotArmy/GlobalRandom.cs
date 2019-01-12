using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmy
{
    public static class GlobalRandom
    {
        private static Random random = new Random();

        public static int Next() => random.Next();
        public static int Next(int max) => random.Next(max);
        public static int Next(int min, int max) => random.Next(min, max);
        public static float NextFloat() => (float)random.NextDouble();
        public static float NextFloat(float max) => (float)random.NextDouble() * max;
        public static float NextFloat(float min, float max) => min + ((float)random.NextDouble() * (max-min));
        public static double NextDouble() => random.NextDouble();
        public static double NextDouble(double max) => random.NextDouble() * max;
        public static double NextDouble(double min, double max) => min + (random.NextDouble() * (max - min));

        public static Color NextColor() => new Color(random.Next(255), random.Next(255), random.Next(255), 255);
        public static Color NextColor(int rmax, int gmax, int bmax) => new Color(random.Next(rmax), random.Next(gmax), random.Next(bmax), 255);
        public static Color NextColor(int rmin, int rmax, int gmin, int gmax, int bmin, int bmax, int amin = 255, int amax = 256) => new Color(random.Next(rmin, rmax), random.Next(gmin, gmax), random.Next(bmin, bmax), random.Next(amin, amax));

    }
}
