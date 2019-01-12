using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryLib
{
    public struct Line
    {
        public Vector Start;
        public Vector End;
        public Line(Vector start, Vector end)
        {
            Start = start;
            End = end;
        }

        public Line Clone()
        {
            return new Line(Start, End);
        }
    }
}
