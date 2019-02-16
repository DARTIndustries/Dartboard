using System;
using DartboardEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DartboardEngineUnitTests
{
    [TestClass]
    public class VectorTests
    {
        [TestMethod]
        public void BasicOperations()
        {
            Vector a = new Vector(1, 1, 1);
            Vector b = new Vector(1, 0, 1);

            Vector c = a + b;
            Vector d = a - b;

            Assert.AreEqual(2, c.X);
            Assert.AreEqual(1, c.Y);
            Assert.AreEqual(2, c.Z);


            Assert.AreEqual(0, d.X);
            Assert.AreEqual(1, d.Y);
            Assert.AreEqual(0, d.Z);
        }
    }
}
