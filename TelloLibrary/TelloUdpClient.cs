using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TelloLibrary
{
    internal class TelloUdpClient
    {

        private UdpClient _udpClient;
        private IPEndPoint _droneEndpoint;
        

        public string Host => _droneEndpoint.Address.ToString();


        private TelloUdpClient(IPEndPoint droneEndpoint)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, (int)Tello.WellKnownPort.RecvPort);
            
            _udpClient = new UdpClient(localEndPoint);
            _udpClient.Connect(droneEndpoint);
            _droneEndpoint = droneEndpoint;
        }

        /// <summary>
        /// Create a UDPClient to use with Tello
        /// </summary>
        /// <param name="droneIP">The IPAddress of the Drone</param>
        public TelloUdpClient(string droneIP) : this(new IPEndPoint(IPAddress.Parse(droneIP), (int)Tello.WellKnownPort.SendPort))
        { }


        internal void SendMessage(TelloAction action)
        {
            if (_udpClient == null)
            {
                throw new Exception("UdpClient is Null");
            }
            //
            var data = Encoding.UTF8.GetBytes(action.Command.ToLower());
            _udpClient.Send(data, data.Length);
            //
        }

        internal string RecvMessage()
        {
            IPEndPoint remoteIpEndPoint = null;
            var receiveBytes = _udpClient.Receive(ref remoteIpEndPoint);
            var serverReponse = Encoding.UTF8.GetString(receiveBytes);

            return serverReponse;
        }

        public void Close()
        {
            _udpClient.Close();
            _udpClient.Dispose();
        }

    }
}
