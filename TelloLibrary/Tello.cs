using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelloLibrary;

namespace TelloLibrary
{
    /// <summary>
    /// A Class to use the DJI Tello.
    /// </summary>
    public class Tello : IDisposable
    {
        private Queue<string> _messageQueue;
        private AutoResetEvent _messageEvent;

        public enum Response
        {
            OK,
            FAIL
        }

        public enum WellKnownPort
        {
            SendPort = 8889,
            RecvPort = 8890
        }

        public enum TimeOut
        {
            Standard = 4500,
            Pulse = 6000
        }

        private TelloUdpClient _client;
        private bool _commandMode = false;
        private Exception _lastException = null;
        private CancellationTokenSource _cancelRecvSource;
        private CancellationToken _cancelRecv;
        private Task _messagePump;
        private CancellationTokenSource _cancelPulseSource;
        private CancellationToken _cancelPulse;
        private ManualResetEvent _pulseEvent;
        private Task _pulseTello;
        private TelloState _state;

        /// <summary>
        /// Indicate if Command mode is active
        /// </summary>
        public bool CommandModeEnabled => _commandMode;
        /// <summary>
        /// Access the latest exception
        /// </summary>
        public Exception LastException => _lastException;
        /// <summary>
        /// Return the current TelloState
        /// </summary>
        public TelloState State => _state;

        /// <summary>
        /// Create a Tello Object
        /// </summary>
        /// <param name="droneIP">The IP Address of the Tello Drone</param>
        public Tello(string droneIP)
        {
            _state = new TelloState("");
            _messageQueue = new Queue<string>();
            _messageEvent = new AutoResetEvent(false);
            //
            _client = new TelloUdpClient(droneIP);
            // The task that will receive the messages coming from the Drone
            _cancelRecvSource = new CancellationTokenSource();
            _cancelRecv = _cancelRecvSource.Token;
            _messagePump = new Task(this.RecvTask, _cancelRecv);
            _messagePump.Start();
            // Pulse task to keep connection alive when flying
            _pulseEvent = new ManualResetEvent(false);

        }

        /// <summary>
        /// Running task that will receive messages from the Drone
        /// </summary>
        private void RecvTask()
        {
            while (true)
            {
                try
                {
                    // Wait for a message
                    string response = _client.RecvMessage();
                    if (response.StartsWith("mid"))
                    {
                        // Update Drone State
                        lock (_state)
                        {
                            _state.State = response;
                        }
                        continue;
                    }
                    // Put the message in Queue
                    lock (_messageQueue)
                    {
                        _messageQueue.Enqueue(response);
                    }
                    // And raise the flag, indicating we have a message
                    _messageEvent.Set();
                }
                catch (Exception e)
                {
                    // Mmmm Something went wrong
                    Console.WriteLine(e.Message);
                    break;
                }
                // Should we stop ?
                if (_cancelRecv.IsCancellationRequested)
                {
                    _cancelRecv.ThrowIfCancellationRequested();
                }
            }
        }

        #region Command Set
        /// <summary>
        /// Command response : Wake up the Tello
        /// </summary>
        /// <returns></returns>
        public Response Command()
        {
            TelloAction action = new TelloAction(this, "Command", "command", TelloAction.ActionTypes.Command);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        /// Auto TakeOff
        /// </summary>
        /// <returns></returns>
        public Response TakeOff()
        {
            TelloAction action = new TelloAction(this, "Auto takeoff", "takeoff", TelloAction.ActionTypes.Control);
            action.SendCommand(10000);
            if (action.Response == Response.OK)
                StartPulse();
            return action.Response;
        }

        /// <summary>
        /// Land drone
        /// </summary>
        /// <returns></returns>
        public Response Land()
        {
            TelloAction action = new TelloAction(this, "Auto land", "land", TelloAction.ActionTypes.Control);
            action.SendCommand();
            StopPulse();
            return action.Response;
        }

        public String Battery()
        {
            TelloAction action = new TelloAction(this, "Get current battery percentage", "battery?", TelloAction.ActionTypes.Read);
            action.SendCommand();
            return action.ServerResponse.Trim('\r', '\n');
        }

        /// <summary>
        /// Turn On Video Stream
        /// </summary>
        /// <returns></returns>
        public Response StreamOn()
        {
            TelloAction action = new TelloAction(this, "Stream On", "streamon", TelloAction.ActionTypes.Command);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        /// Turn Off Video Stream
        /// </summary>
        /// <returns></returns>
        public Response StreamOff()
        {
            TelloAction action = new TelloAction(this, "Stream Off", "streamoff", TelloAction.ActionTypes.Command);
            action.SendCommand();
            return action.Response;
        }
        /// <summary>
        /// Turn clockwise.
        /// </summary>
        /// <param name="degrees">Angle in degrees 1-360°</param>
        /// <returns></returns>
        public Response TurnClockwise( int degrees)
        {
            RotateClockwise action = new RotateClockwise(this, "Turn Clockwise", degrees);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        /// Turn Counter clockwise.
        /// </summary>
        /// <param name="degrees">Angle in degrees 1-360°</param>
        /// <returns></returns>
        public Response TurnCounterClockwise( int degrees )
        {
            RotateCounterClockwise action = new RotateCounterClockwise(this, "Turn CounterClockwise", degrees);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        /// Move Up
        /// </summary>
        /// <param name="distance">cm</param>
        /// <returns></returns>
        public Response MoveUp(int distance)
        {
            MoveUp action = new MoveUp(this, "Move Up", distance);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        /// Move Down
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Response MoveDown(int distance)
        {
            MoveDown action = new MoveDown(this, "Move Down", distance);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        /// Move Left
        /// </summary>
        /// <param name="distance">cm</param>
        /// <returns></returns>
        public Response MoveLeft(int distance)
        {
            MoveLeft action = new MoveLeft(this, "Move Left", distance);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        /// Move Right
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Response MoveRight(int distance)
        {
            MoveRight action = new MoveRight(this, "Move Right", distance);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        /// Move Forward
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Response MoveForward(int distance)
        {
            MoveForward action = new MoveForward(this, "Move Forward", distance);
            action.SendCommand();
            return action.Response;
        }

        /// <summary>
        ///  Move Backward
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Response MoveBackward(int distance)
        {
            MoveBackward action = new MoveBackward(this, "Move Backward", distance);
            action.SendCommand();
            return action.Response;
        }

        #endregion

        public string SendCommand(TelloAction action, Tello.TimeOut waitTime)
        {
            return this.SendCommand(action, (int)waitTime);
        }
        public string SendCommand(TelloAction action, int waitTime)
        {
            lock (this)
            {
                string response = "";
                try
                {
                    if (_client == null)
                    {
                        throw new Exception("Client is null");
                    }
                    if (_messageQueue.Count != 0)
                    {
                        lock (_messageQueue)
                        {
                            _messageQueue.Clear();
                        }
                    }
                    // We MUST be in Command mode to send command, except the Command command ;)
                    if ((action.Type != TelloAction.ActionTypes.Command) && !CommandModeEnabled)
                    {
                        // First, go to Command mode
                        var rsp = this.Command();
                        if (rsp != Tello.Response.OK)
                            return Tello.Response.FAIL.ToString();
                        this._commandMode = true;
                    }
                    _client.SendMessage(action);
                    // Indicate that a message has been sent, no need for pulse
                    _pulseEvent.Set();
                    // Wait for a reply
                    response = "";
                    if (_messageEvent.WaitOne(waitTime))
                    {
                        lock (_messageQueue)
                        {
                            response = _messageQueue.Dequeue();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (action.Type == TelloAction.ActionTypes.Command)
                    {
                        //drone is probably already in command mode. Continue
                        return Tello.Response.OK.ToString();
                    }
                    //_lastException = ex;
                    return Tello.Response.FAIL.ToString(); ;
                }
                return response;
            }
        }

        /// <summary>
        /// Dispose the Tello object
        /// </summary>
        public void Dispose()
        {
            _cancelRecvSource.Cancel();
            _client.Close();
            StopPulse();
        }

        #region Pulse Task
        /// <summary>
        /// Pulse Task
        /// Send empty Command to the drone, every Tello.TimeOut.Pulse ms if, in the meantime, no message has been sent.
        /// </summary>
        private void PulseTask()
        {
            while (true)
            {
                try
                {
                    //
                    bool hasBeenSignaled = _pulseEvent.WaitOne((int)Tello.TimeOut.Pulse);
                    if (!hasBeenSignaled)
                    {
                        // Send a Command to Keep Alive.
                        this.Command();
                    }
                    _pulseEvent.Reset();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
                //
                if (_cancelPulse.IsCancellationRequested)
                {
                    _cancelPulse.ThrowIfCancellationRequested();
                }
            }
        }

        /// <summary>
        /// Start the Pulse Task, called when the TakeOff command is sent
        /// </summary>
        internal void StartPulse()
        {
            if ((_pulseTello == null) || (_pulseTello.Status != TaskStatus.Running))
            {
                _cancelPulseSource = new CancellationTokenSource();
                _cancelPulse = _cancelPulseSource.Token;
                //
                _pulseTello = new Task(this.PulseTask, _cancelPulse);
                _pulseTello.Start();
            }
        }

        /// <summary>
        /// Stop the Pulse Task, called when Land command is sent
        /// </summary>
        internal void StopPulse()
        {
            if ((_pulseTello != null) && (_pulseTello.Status == TaskStatus.Running))
            {
                _cancelPulseSource.Cancel();
            }
        }
        #endregion
    }
}
