using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelloLibrary
{
    public class TelloAction
    {
        protected Tello _drone;
        private string _response;

        protected string _actionName;
        protected string _actionCommand;
        private ActionTypes _actionType;
        

        public enum ActionTypes
        {
            Command,
            Control,
            Set,
            Read
        }

        /// <summary>
        /// Description of the command. Used for loggin (not implemented now), maybe used in Log File ??
        /// </summary>
        public string Name => _actionName;
        /// <summary>
        /// The command string sent to the Tello
        /// </summary>
        public string Command => _actionCommand;
        /// <summary>
        /// Waht kind of Command is it ?
        /// </summary>
        public ActionTypes Type => _actionType;
        /// <summary>
        /// The Tello response to the Command (the original string)
        /// </summary>
        public string ServerResponse => _response;
        /// <summary>
        /// The Tello response to the Command (OK/FAIL)
        /// </summary>
        public Tello.Response Response => _response.ToUpper() == "OK" ? Tello.Response.OK : Tello.Response.FAIL;
        /// <summary>
        /// Create a Tello Action
        /// </summary>
        /// <param name="drone">The Tello object that will send the command, and give a response</param>
        /// <param name="name">The description of the Command</param>
        /// <param name="command">The Command sent to the Tello</param>
        /// <param name="type">The type of this Action</param>
        public TelloAction( Tello drone, string name, string command, ActionTypes type )
        {
            _actionCommand = command;
            _actionName = name;
            _actionType = type;
            _drone = drone;
        }

        /// <summary>
        /// Send the Command
        /// </summary>
        /// <param name="waitTime">The TimeOut of the Response</param>
        /// <returns></returns>
        public String SendCommand(int waitTime )
        {
            _response = _drone.SendCommand(this, waitTime);
            return _response;
        }
        /// <summary>
        /// Send the Command 
        /// </summary>
        /// <param name="waitTime">The TimeOut of the Response, Tello.TimeOut.Standard per default</param>
        /// <returns></returns>
        public String SendCommand(Tello.TimeOut waitTime = Tello.TimeOut.Standard )
        {
            return _response = this.SendCommand((int)waitTime);
        }
    }
}
