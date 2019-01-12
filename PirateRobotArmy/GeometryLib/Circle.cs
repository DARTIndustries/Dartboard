using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryLib
{
    public struct Circle
    {
        public Vector Position;
        public double Radius;

        public Circle(Vector position, double radius = 1)
        {
            Position = position;
            Radius = radius;
        }

        public static Circle Uncollide(Circle a, Circle b, Angle moveDirection)
        {
            if (!a.Intersects(b))
                return a;
            Line movementLine = new Line(a.Position, a.Position + moveDirection.CreateVector(1));
            Circle imaginaryCircle = new Circle(b.Position, b.Radius + a.Radius);
            var intersections = imaginaryCircle.InfiniteLineIntersections(movementLine);

            if (intersections.Length == 0 || intersections[0].DistanceSq(a.Position) <= intersections[1].DistanceSq(a.Position))
                return new Circle(intersections[0], a.Radius);
            return new Circle(intersections[1], a.Radius);
        }
        public Circle Uncollide(Circle a, Circle b, Angle moveDirection, out bool wasColiding)
        {
            wasColiding = false;
            if (!a.Intersects(b))
                return a;
            wasColiding = true;
            Line movementLine = new Line(a.Position, a.Position + moveDirection.CreateVector(1));
            Circle imaginaryCircle = new Circle(b.Position, b.Radius + a.Radius);
            var intersections = imaginaryCircle.InfiniteLineIntersections(movementLine);

            return new Circle(intersections[1], a.Radius);
        }

        public bool Intersects(Circle other)
        {
            return Position.DistanceSq(other.Position) < Math.Pow(Radius + other.Radius, 2);
        }
        
        public Vector[] Intersections(Line line)
        {
            double dx, dy, A, B, C, det, t;

            dx = line.End.X - line.Start.X;
            dy = line.End.Y - line.Start.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (line.Start.X - Position.X) + dy * (line.Start.Y - Position.Y));
            C = (line.Start.X - Position.X) * (line.Start.X - Position.X) + (line.Start.Y - Position.Y) * (line.Start.Y - Position.Y) - Radius * Radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                return new Vector[0];
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                if (t > 1 || t < 0)
                    return new Vector[0];
                return new Vector[] { new Vector(line.Start.X + t * dx, line.Start.Y + t * dy, true) };
            }
            else
            {
                // Two solutions.
                t = ((-B + Math.Sqrt(det)) / (2 * A));
                double t2 = ((-B - Math.Sqrt(det)) / (2 * A));

                bool tValid = t >= 0 && t <= 1;
                bool t2Valid = t2 >= 0 && t2 <= 1;

                if (!tValid && !t2Valid)
                    return new Vector[0];
                if (tValid && !t2Valid)
                    return new Vector[] { new Vector(line.Start.X + t * dx, line.Start.Y + t * dy, true) };
                if (!tValid && t2Valid)
                    return new Vector[] { new Vector(line.Start.X + t2 * dx, line.Start.Y + t2 * dy, true) };

                return new Vector[] { new Vector(line.Start.X + t2 * dx, line.Start.Y + t2 * dy, true), new Vector(line.Start.X + t * dx, line.Start.Y + t * dy, true) };
            }
        }


        public Vector[] InfiniteLineIntersections(Line line)
        {
            double dx, dy, A, B, C, det, t;

            dx = line.End.X - line.Start.X;
            dy = line.End.Y - line.Start.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (line.Start.X - Position.X) + dy * (line.Start.Y - Position.Y));
            C = (line.Start.X - Position.X) * (line.Start.X - Position.X) + (line.Start.Y - Position.Y) * (line.Start.Y - Position.Y) - Radius * Radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                return new Vector[0];
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                return new Vector[] { new Vector(line.Start.X + t * dx, line.Start.Y + t * dy, true) };
            }
            else
            {
                // Two solutions.
                t = ((-B + Math.Sqrt(det)) / (2 * A));
                double t2 = ((-B - Math.Sqrt(det)) / (2 * A));

                return new Vector[] { new Vector(line.Start.X + t2 * dx, line.Start.Y + t2 * dy, true), new Vector(line.Start.X + t * dx, line.Start.Y + t * dy, true) };
            }
        }

        public Vector? FirstIntersection(Line l)
        {
            var both = Intersections(l);
            if (both.Length == 0)
                return null;
            return both[0];
        }
    }
}
