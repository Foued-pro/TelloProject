using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TelloLibrary;

namespace TelloLibrary_Test
{
    [TestClass]
    public class TestDrone
    {
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestMethod1()
        {
            Tello drone;
            // Should not run
            drone = new Tello("");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestMethod2()
        {
            Tello drone;
            // Should not run
            drone = new Tello("crashme");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestMethod3()
        {
            Tello drone;
            // Should not run
            drone = new Tello("2032.2032.2032.2032");
        }

        [TestMethod]
        public void TestMethod4()
        {
            Tello drone;
            // Should run
            drone = new Tello("127.0.0.1");
        }
    }
}
