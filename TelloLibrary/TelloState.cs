using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelloLibrary
{
    /// <summary>
    /// State of the Tello
    /// </summary>
    public class TelloState
    {

        private string state;
        private Dictionary<string, string> states;

        public TelloState(string msgState)
        {
            states = new Dictionary<string, string>();
            State = msgState;
        }

        /// <summary>
        /// A String indicating the current state of the Tello
        /// </summary>
        public string State
        {
            get
            {
                return state;
            }

            internal set
            {
                // The State string
                state = value;
                // Remove trailing \r\n
                state = state.Trim('\r', '\n');
                // Split each elements
                string[] elements = state.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                // For each element, retrieve the Keyp/Value pairs
                foreach (var item in elements)
                {
                    string[] itemValue = item.Split(new char[] { ':' });
                    //if the key doesn't exist add-it, or update
                    if (states.ContainsKey(itemValue[0]))
                    {
                        states[itemValue[0]] = itemValue[1];
                    }
                    else
                        states.Add(itemValue[0], itemValue[1]);
                }
            }
        }


        #region Key/Value Helpers
        private int RetrieveIntValue( string key )
        {
            string itemValue;
            if (states.TryGetValue(key, out itemValue))
            {
                int itemInt;
                if (int.TryParse(itemValue, out itemInt))
                    return itemInt;
            }
            return 0;
        }

        private double RetrieveDoubleValue(string key)
        {
            string itemValue;
            if (states.TryGetValue(key, out itemValue))
            {
                double itemDouble;
                if (double.TryParse(itemValue, out itemDouble))
                    return itemDouble;
            }
            return 0.0;
        }
        #endregion

        /// <summary>
        /// The ID of the mission Pad
        /// </summary>
        public int mid
        {
            get
            {
                return RetrieveIntValue("mid");
            }
        }

        /// <summary>
        /// The "x" coordinate detected on the Mission Pad.
        /// </summary>
        public int x
        {
            get
            {
                return RetrieveIntValue("x");
            }
        }

        /// <summary>
        /// The "y" coordinate detected on the Mission Pad.
        /// </summary>
        public int y
        {
            get
            {
                return RetrieveIntValue("y");
            }
        }

        /// <summary>
        /// The "z" coordinate detected on the Mission Pad.
        /// </summary>
        public int z
        {
            get
            {
                return RetrieveIntValue("z");
            }
        }



        /// <summary>
        /// The degree of the attitude pitch
        /// </summary>
        public int pitch
        {
            get
            {
                return RetrieveIntValue("pitch");
            }
        }

        /// <summary>
        /// The degree of the attitude roll
        /// </summary>
        public int roll
        {
            get
            {
                return RetrieveIntValue("roll");
            }
        }

        /// <summary>
        /// The degree of the attitude yaw
        /// </summary>
        public int yaw
        {
            get
            {
                return RetrieveIntValue("yaw");
            }
        }

        /// <summary>
        /// The speed of "x" axis
        /// </summary>
        public int vgx
        {
            get
            {
                return RetrieveIntValue("vgx");
            }
        }

        /// <summary>
        /// The speed of "y" axis
        /// </summary>
        public int vgy
        {
            get
            {
                return RetrieveIntValue("vgy");
            }
        }

        /// <summary>
        /// The speed of "z" axis
        /// </summary>
        public int vgz
        {
            get
            {
                return RetrieveIntValue("vgz");
            }
        }

        /// <summary>
        /// The lowest temperature in degree Celsius
        /// </summary>
        public int templ
        {
            get
            {
                return RetrieveIntValue("templ");
            }
        }

        /// <summary>
        /// The highest temperature in degree Celsius
        /// </summary>
        public int temph
        {
            get
            {
                return RetrieveIntValue("temph");
            }
        }

        /// <summary>
        /// The time of flight distance in cm
        /// </summary>
        public int tof
        {
            get
            {
                return RetrieveIntValue("tof");
            }
        }

        /// <summary>
        /// The height in cm
        /// </summary>
        public int height
        {
            get
            {
                return RetrieveIntValue("h");
            }
        }

        /// <summary>
        /// The percentage of the current battery level
        /// </summary>
        public int bat
        {
            get
            {
                return RetrieveIntValue("bat");
            }
        }

        /// <summary>
        /// The barometer measurement in cm
        /// </summary>
        public double baro
        {
            get
            {
                return RetrieveDoubleValue("baro");
            }
        }
        /// <summary>
        /// The amount of time the motor has been used
        /// </summary>
        public int time
        {
            get
            {
                return RetrieveIntValue("time");
            }
        }
        /// <summary>
        /// The acceleration of the "x" axis
        /// </summary>
        public double agx
        {
            get
            {
                return RetrieveDoubleValue("agx");
            }
        }
        /// <summary>
        /// The acceleration of the "x" axis
        /// </summary>
        public double agy
        {
            get
            {
                return RetrieveDoubleValue("agy");
            }
        }
        /// <summary>
        /// The acceleration of the "x" axis
        /// </summary>
        public double agz
        {
            get
            {
                return RetrieveDoubleValue("agz");
            }
        }
    }
}
