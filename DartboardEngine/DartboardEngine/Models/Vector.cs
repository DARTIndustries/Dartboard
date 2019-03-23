using System;
using System.Runtime.InteropServices;

namespace DartboardEngine
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 12)]
    public struct Vector
    {
        public static Vector Zero { get; } = new Vector(0, 0, 0);

        [FieldOffset(0)]
        public readonly float X;

        [FieldOffset(4)]
        public readonly float Y;

        [FieldOffset(8)]
        public readonly float Z;

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector(Vector clone)
        {
            X = clone.X;
            Y = clone.Y;
            Z = clone.Z;
        }

        public Vector With(float? x = null, float? y = null, float? z = null)
        {
            return new Vector(x ?? X, y ?? Y, z ?? Z);
        }

        public float MagnitudeSquared() => X * X + Y * Y + Z * Z;
        public float Magnitude() => (float)Math.Sqrt(MagnitudeSquared());

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public static Vector Add(Vector a, Vector b, params Vector[] others)
        {
            float x = a.X + b.X, 
                  y = a.Y + b.Y,
                  z = a.Z + b.Z;

            foreach(var v in others)
            {
                x += v.X;
                y += v.Y;
                z += v.Z;
            }

            return new Vector(x, y, z);
        }

        public static Vector Negate(Vector a) => new Vector(-a.X, -a.Y, -a.Z);
        public static Vector Subtract(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector MultiplyScalar(Vector a, float b) => new Vector(a.X * b, a.Y * b, a.Z * b);
        public static Vector DivideScalar(Vector a, float b) => new Vector(a.X / b, a.Y / b, a.Z / b);
        public static float Length(Vector a) => (float)Math.Sqrt((a.X * a.X) + (a.Y * a.Y) + (a.Z * a.Z));
        public static float LengthSq(Vector a) => (a.X * a.X) + (a.Y * a.Y) + (a.Z * a.Z);
        public static Vector Normal(Vector v)
        {
            return DivideScalar(v, Length(v));
        }
        public static float Dot(Vector a, Vector b) => (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);


        public static Vector operator +(Vector a, Vector b) => Add(a, b);
        public static Vector operator -(Vector a, Vector b) => Subtract(a, b);
        public static Vector operator -(Vector a) => Negate(a);
        public static Vector operator *(Vector a, float b) => MultiplyScalar(a, b);
        public static Vector operator *(float s, Vector a) => MultiplyScalar(a, s);
        public static Vector operator /(Vector a, float b) => DivideScalar(a, b);
    }
}
