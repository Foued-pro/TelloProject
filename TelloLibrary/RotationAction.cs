using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelloLibrary
{
    /// <summary>
    /// Create a Rotation command
    /// </summary>
    public class RotateClockwise : TelloAction
    {
        public RotateClockwise(Tello drone, string name, int degrees) : base(drone, name, "", TelloAction.ActionTypes.Control)
        {
            if (degrees < 0 || degrees > 3600)
            {
                throw new ArgumentException("Invalid degrees value", nameof(degrees));
            }
            this._actionCommand = "cw " + degrees.ToString();
        }

    }

    public class RotateCounterClockwise : TelloAction
    {
        public RotateCounterClockwise(Tello drone, string name, int degrees) : base(drone, name, "", TelloAction.ActionTypes.Control)
        {
            if (degrees < 0 || degrees > 3600)
            {
                throw new ArgumentException("Invalid degrees value", nameof(degrees));
            }
            this._actionCommand = "ccw " + degrees.ToString();
        }

    }
}
