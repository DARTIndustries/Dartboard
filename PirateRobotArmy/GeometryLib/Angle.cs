using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

namespace GeometryLib
{
    public struct Angle : IEquatable<Angle>, IComparable<Angle>
    {
        public static IComparer<Angle> GetAbsolueComparer() => new ComparerFunctionWrapper<Angle>((a, b) => a._Angle.CompareTo(b._Angle));
        public static IComparer<Angle> GetRelativeComparer() => new ComparerFunctionWrapper<Angle>((a, b) => a.RelativeValue.CompareTo(b.RelativeValue));
        public static IComparer<Angle> GetAcuteComparer() => new ComparerFunctionWrapper<Angle>((a, b) => Abs(a.RelativeValue).CompareTo(Abs(b.RelativeValue)));
        public static IComparer<Angle> GetObtuseComparer() => new ComparerFunctionWrapper<Angle>((a, b) => Abs(b.RelativeValue).CompareTo(Abs(a.RelativeValue)));


        private const double TwoPI = 2 * PI;

        private double _Angle;

        private Cached<double, double> _Cos;
        private Cached<double, double> _Sin;
        private Cached<double, double> _Tan;

        public double AbsoluteValue
        {
            get
            {
                return _Angle;
            }
            set
            {
                _SetAngle(_FixAngle(value));
            }
        }
        public double RelativeValue
        {
            get
            {
                if (_Angle <= PI)
                    return _Angle;
                return _Angle - TwoPI;
            }
            set
            {
                _SetAngle(_FixAngle(value));
            }
        }
        public double Sin
        {
            get
            {
                return _Sin.GetValue(_Angle);
            }
            set
            {
                _Sin.Assign(value);
                _Angle = _FixAngle(Asin(value));
                _Cos.Invalidate();
                _Tan.Invalidate();
            }
        }
        public double Cos
        {
            get
            {
                return _Cos.GetValue(_Angle);
            }
            set
            {
                _Cos.Assign(value);
                _Angle = _FixAngle(Acos(value));
                _Sin.Invalidate();
                _Tan.Invalidate();
            }
        }
        public double Tan
        {
            get
            {
                return _Tan.GetValue(_Angle);
            }
            set
            {
                _Tan.Assign(value);
                _Angle = _FixAngle(Atan(value));
                _Sin.Invalidate();
                _Cos.Invalidate();
            }
        }

        private void _SetAngle(double newAngle)
        {
            if (newAngle == _Angle)
                return;
            _Angle = newAngle;
            _Sin.Invalidate();
            _Cos.Invalidate();
            _Tan.Invalidate();
        }

        public Angle(double value)
        {
            // Don't ask.
            _Angle = 0;
            _Cos = new Cached<double, double>(d => Cos(d));
            _Sin = new Cached<double, double>(d => Sin(d));
            _Tan = new Cached<double, double>(d => Tan(d));
            _Angle = _FixAngle(value);
        }

        public Angle(double deltaX, double deltaY)
            : this(Atan2(deltaY, deltaX))
        {

        }
        public Angle(Vector a, Vector b)
            : this(
                  b.X - a.X,
                  b.Y - a.Y
            )
        {

        }

        /// <summary>
        /// Calculates the angle between a and b from the point of view of origin.
        /// </summary>
        public static Angle AngleBetween(Vector origin, Vector a, Vector b)
        {
            Angle toA = new Angle(origin, a);
            Angle toB = new Angle(origin, b);
            return toA.PositiveDistance(toB);
        }

        /// <summary>
        /// Calculates the angle between a and b from the global reference frame (0 = right)
        /// </summary>
        public static Angle AngleBetween(Vector source, Vector target)
        {
            return new Angle(source, target);
        }

        private double _FixAngle(double value)
        {
            if (value > 0 && value < TwoPI)
                return value;
            if(value < 0 && value > -TwoPI)
                return TwoPI + value;

            value = value % TwoPI;
            if (value < 0)
                return TwoPI + value;
            return value;
        }
        public static Angle Inverse(Angle a)
        {
            return new Angle(-a.AbsoluteValue);
        }
        public static Angle Add(Angle a, Angle b)
        {
            return new Angle(a.AbsoluteValue + b.AbsoluteValue);
        }
        public static Angle Subtract(Angle a, Angle b)
        {
            return new Angle(a.AbsoluteValue - b.AbsoluteValue);
        }
        public static Angle Multiply(Angle a, double b)
        {
            return new Angle(a.AbsoluteValue * b);
        }
        public static Angle Divide(Angle a, double b)
        {
            return new Angle(a.AbsoluteValue / b);
        }

        public Vector CreateVector(double scaleFactor)
        {
            return new Vector(scaleFactor, AbsoluteValue, false);
        }

        public static double Modulus(Angle a, double b)
        {
            return a.AbsoluteValue % b;
        }

        public override bool Equals(object obj)
        {
            if (obj is Angle)
                return Equals((Angle)obj);
            return false;
        }
        public override string ToString()
        {
            return _Angle.ToString();
        }
        public override int GetHashCode()
        {
            return _Angle.GetHashCode();
        }
        public bool Equals(Angle other)
        {
            return other._Angle == _Angle;
        }
        public int CompareTo(Angle other)
        {
            return _Angle.CompareTo(other._Angle);
        }

        public Angle PositiveDistance(Angle other)
        {
            return new Angle(other._Angle - _Angle);
        }
        public Angle NegativeDistance(Angle other)
        {
            return new Angle(_Angle - other._Angle);
        }
        public Angle Distance(Angle other)
        {
            return PositiveDistance(other).AcuteComponent();
        }

        public static Angle operator +(Angle a, Angle b) => Add(a, b);
        public static Angle operator -(Angle a, Angle b) => Subtract(a, b);
        public static Angle operator *(Angle a, double b) => Multiply(a, b);
        public static Angle operator /(Angle a, double b) => Divide(a, b);
        public static Angle operator %(Angle a, double b) => Modulus(a, b);
        public static bool operator ==(Angle a, Angle b) => a.Equals(b);
        public static bool operator !=(Angle a, Angle b) => !a.Equals(b);
        public static bool operator >(Angle a, Angle b) => a.CompareTo(b) > 0;
        public static bool operator <(Angle a, Angle b) => a.CompareTo(b) < 0;
        public static bool operator >=(Angle a, Angle b) => a.CompareTo(b) >= 0;
        public static bool operator <=(Angle a, Angle b) => a.CompareTo(b) <= 0;

        public static explicit operator double(Angle angle)
        {
            return angle.AbsoluteValue;
        }
        public static implicit operator Angle(double d)
        {
            return new Angle(d);
        }

        public Angle AcuteComponent()
        {
            return new Angle(Abs(RelativeValue));
        }
        public Angle ObtuseComponent()
        {
            if (_Angle >= PI)
                return new Angle(this._Angle);
            return new Angle(TwoPI - _Angle);
        }
    }
}
