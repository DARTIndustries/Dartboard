using DartboardEngine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartboardEngineUnitTests
{
    [TestClass]
    public class AccessQueueTests
    {
        [TestMethod]
        public void TestSimpleIO()
        {
            AccessQueue<int> q = new AccessQueue<int>();
            for(int i = 0; i < 1000; i++)
            {
                q.Enqueue(i);
            }
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(i, q.Dequeue());
            }
        }

        [TestMethod]
        public void TestHalfFill()
        {
            AccessQueue<int> q = new AccessQueue<int>();
            for (int i = 0; i < 10; i++)
            {
                q.Enqueue(i);
            }
            for(int i = 0; i < 5; i++)
            {
                Assert.AreEqual(i, q.Dequeue());
            }
            for (int i = 10; i < 20; i++)
            {
                q.Enqueue(i);
            }
            for (int i = 5; i < 20; i++)
            {
                Assert.AreEqual(i, q.Dequeue());
            }

        }

        [TestMethod]
        public void TestComplexIO()
        {
            Random rand = new Random();
            AccessQueue<int> q = new AccessQueue<int>();
            Queue<int> master = new Queue<int>();
            for (int i = 0; i < 1000; i++)
            {
                q.Enqueue(i);
                master.Enqueue(i);
                if(master.Count > 0 && rand.Next(3) == 0)
                {
                    Assert.AreEqual(master.Dequeue(), q.Dequeue());
                }
            }
            while (master.Count > 0)
            {
                Assert.AreEqual(master.Dequeue(), q.Dequeue());
            }
        }
    }
}
