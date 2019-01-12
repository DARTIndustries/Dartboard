using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryLib
{
    internal class ComparerFunctionWrapper<T> : IComparer<T>
    {
        public Func<T, T, int> CompareFunction { get; }
        public ComparerFunctionWrapper(Func<T, T, int> func)
        {
            CompareFunction = func;
        }
        public int Compare(T x, T y) => CompareFunction(x, y);
    }
}
