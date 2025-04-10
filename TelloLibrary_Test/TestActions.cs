using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TelloLibrary;

namespace TelloLibrary_Test
{
    [TestClass]
    public class TestActions
    {
        static Tello drone = new Tello("192.168.10.1");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod1()
        {
            MoveUp mu = new MoveUp(drone, "dummy", 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod2()
        {
            MoveUp mu = new MoveUp(drone, "dummy", 501);
        }


        [TestMethod]
        public void TestMethod3()
        {
            MoveUp mu = new MoveUp(drone, "dummy", 20);
        }
    }
}
