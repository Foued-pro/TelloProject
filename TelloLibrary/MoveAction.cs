using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelloLibrary
{
    public class MoveAction : TelloAction
    {
        public MoveAction(Tello drone, string name, string cmd, int distance) : base(drone, name, "", TelloAction.ActionTypes.Control)
        {
            if (distance < 20 || distance > 500)
            {
                throw new ArgumentException("Invalid distance value", nameof(distance));
            }
            if (string.IsNullOrEmpty(cmd))
            {
                throw new ArgumentException("Invalid command string", nameof(cmd));
            }
            this._actionCommand = cmd + " "+ distance.ToString();
        }
    }

    public class MoveUp : MoveAction
    {
        public MoveUp(Tello drone, string name, int distance) : base(drone, name, "up", distance)
        {}
    }

    public class MoveDown : MoveAction
    {
        public MoveDown(Tello drone, string name, int distance) : base(drone, name, "down", distance)
        { }
    }

    public class MoveLeft : MoveAction
    {
        public MoveLeft(Tello drone, string name, int distance) : base(drone, name, "left", distance)
        { }
    }

    public class MoveRight : MoveAction
    {
        public MoveRight(Tello drone, string name, int distance) : base(drone, name, "right", distance)
        { }
    }

    public class MoveForward : MoveAction
    {
        public MoveForward(Tello drone, string name, int distance) : base(drone, name, "forward", distance)
        { }
    }

    public class MoveBackward : MoveAction
    {
        public MoveBackward(Tello drone, string name, int distance) : base(drone, name, "back", distance)
        { }
    }
}
