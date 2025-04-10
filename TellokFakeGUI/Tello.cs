using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellokFakeGUI
{
    public class Tello
    {
        int battery;
        int altitude;
        int speed;
        int listeningPort;

        public int Battery { get => battery; set => battery = value; }
        public int Altitude { get => altitude; set => altitude = value; }
        public int ListeningPort { get => listeningPort; set => listeningPort = value; }
        public int Speed { get => speed; set => speed = value; }
        public bool IsFlying { get; internal set; }
        public int YPos { get; internal set; }
        public int XPos { get; internal set; }

        public Tello()
        {
            Battery = 100;
            Speed = 0;
            Altitude = 0;
            ListeningPort = 8889;
        }
    }
}
