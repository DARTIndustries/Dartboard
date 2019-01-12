using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryLib
{
    public struct Vector
    {
        private const double TwoPi = Math.PI * 2;
        public static Vector CreateXY(double x, double y) => new Vector(x, y, true);
        public static Vector CreateMA(double m, double a) => new Vector(m, a, false);

        public static readonly Vector UnitX = CreateXY(1, 0);
        public static readonly Vector UnitY = CreateXY(0, 1);

        private double _X;
        private double _Y;
        private double _M;
        private Angle _A;
        private bool isXYValid;
        private bool isMAValid;

        public double X
        {
            get
            {
                if(!isXYValid)
                    RecalculateXY();
                return _X;
            }
            set
            {
                if (!isXYValid)
                    RecalculateXY();
                _X = value;
                isMAValid = false;
            }
        }

        public double Y
        {
            get
            {
                if (!isXYValid)
                    RecalculateXY();
                return _Y;
            }
            set
            {
                if (!isXYValid)
                    RecalculateXY();
                _Y = value;
                isMAValid = false;
            }
        }

        public double Magnitude
        {
            get
            {
                if (!isMAValid)
                    RecalculateMA();
                return _M;
            }
            set
            {
                if (!isMAValid)
                    RecalculateMA();
                _M = value;
                isXYValid = false;
            }
        }

        public Angle Angle
        {
            get
            {
                if (!isMAValid)
                    RecalculateMA();
                return _A;
            }
            set
            {
                if (!isMAValid)
                    RecalculateMA();
                _A = value;
                isXYValid = false;
            }
        }

        public Vector(double xm, double ya, bool isXY)
        {
            isXYValid = isXY;
            isMAValid = !isXYValid;

            _X = _M = xm;
            _Y = ya;
            _A = new Angle(ya);
        }

        public Vector(Vector v)
        {
            _X = v.X;
            _Y = v.Y;
            _M = v.Magnitude;
            _A = v.Angle;
            isXYValid = isMAValid = true;
        }

        public Vector Clone()
        {
            if (isXYValid)
                return new Vector(_X, _Y, true);
            else
                return new Vector(_M, _A.AbsoluteValue, false);
        }

        public double LengthSq() => isMAValid ? _M * _M : (_X * _X + _Y * _Y);

        private void RecalculateXY()
        {
            _X = _M * _A.Cos;
            _Y = _M * _A.Sin;
            isXYValid = true;
        }

        private void RecalculateMA()
        {
            _M = Math.Sqrt(_X * _X + _Y * _Y);
            _A = Math.Atan2(_Y, _X);
            isMAValid = true;
        }

        //public void Add(Vector other)
        //{
        //    X += other.X;
        //    Y += other.Y;
        //}

        //public void Subtract(Vector other)
        //{
        //    X -= other.X;
        //    Y -= other.Y;
        //}

        //public void ComponentMultiply(Vector other)
        //{
        //    X *= other.X;
        //    Y *= other.Y;
        //}

        //public void ComponentDivide(Vector other)
        //{
        //    X /= other.X;
        //    Y /= other.Y;
        //}

        //public void ScalarMultiply(double s)
        //{
        //    if(isXYValid)
        //    {
        //        _X *= s;
        //        _Y *= s;
        //        isMAValid = false;
        //    }
        //    else
        //    {
        //        _M *= s;
        //        isXYValid = false;
        //    }
        //}

        public double Dot(Vector other)
        {
            return X * other.X + Y * other.Y;
        }

        //public void Negate()
        //{
        //    if (isXYValid)
        //    {
        //        _X = -_X;
        //        _Y = -_Y;
        //        isMAValid = false;
        //    }
        //    else
        //    {
        //        _A += Math.PI;
        //        isXYValid = false;
        //    }
        //}

        public static Vector Add(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y, true);

        public static Vector Subtract(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y, true);

        public static Vector ComponentMultiply(Vector a, Vector b) => new Vector(a.X * b.X, a.Y * b.Y, true);

        public static Vector ComponentDivide(Vector a, Vector b) => new Vector(a.X / b.X, a.Y / b.Y, true);
        public static double Dot(Vector a, Vector b) => a.Dot(b);

        public static Vector Negate(Vector a) => new Vector(-a.X, -a.Y, true);

        public static Vector ScalarMultiply(Vector a, double s) => new Vector(a.X * s, a.Y * s, true);

        public static Vector operator +(Vector a, Vector b) => Add(a, b);
        public static Vector operator -(Vector a, Vector b) => Subtract(a, b);
        public static Vector operator -(Vector a) => Negate(a);
        public static double operator *(Vector a, Vector b) => Dot(a, b);
        public static Vector operator *(Vector a, double b) => ScalarMultiply(a, b);
        public static Vector operator *(double b, Vector a) => ScalarMultiply(a, b);
        public static Vector operator /(Vector a, double b) => ScalarMultiply(a, 1.0/b);

        public static Vector Normalize(Vector v)
        {
            Vector tmp = v.Clone();
            v.Magnitude = 1;
            return tmp;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector other)
            {
                if (isXYValid)
                {
                    if (!other.isXYValid)
                        other.RecalculateXY();
                    return _X == other._X && _Y == other.Y;
                }
                else
                {
                    if (!other.isMAValid)
                        other.RecalculateMA();
                    return _M == other._M && _A == other._A;
                }
            }
            return false;
        }
        public override int GetHashCode()
        {
            if (isMAValid)
                return _M.GetHashCode() ^ _A.GetHashCode();
            return _X.GetHashCode() ^ _Y.GetHashCode();
        }
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public double DistanceSq(Vector other)
        {
            var dx = other.X - X;
            var dy = other.Y - Y;
            return dx * dx + dy * dy;
        }
        public double Distance(Vector other)
        {
            return Math.Sqrt(DistanceSq(other));
        }

        public static implicit operator Microsoft.Xna.Framework.Vector2(Vector source)
        {
            return new Microsoft.Xna.Framework.Vector2((float)source.X, (float)source.Y);
        }
        public static explicit operator Microsoft.Xna.Framework.Point(Vector source)
        {
            return new Microsoft.Xna.Framework.Point((int)source.X, (int)source.Y);
        }
        public static implicit operator Vector(Microsoft.Xna.Framework.Vector2 source) => new Vector(source.X, source.Y, true);
        public static implicit operator Vector(Microsoft.Xna.Framework.Point source) => new Vector(source.X, source.Y, true);
    }
}
