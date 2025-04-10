using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelloLibrary
{
    class Program
    {
        static string lastResponse = "";

        static bool IsNewReponseAvailable = false;
        static string LastResponse
        {
            get
            {
                IsNewReponseAvailable = false;
                return lastResponse;
            }

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Tello RAW Test");
            //
            IPEndPoint droneEndpoint = new IPEndPoint(IPAddress.Parse("192.168.10.1"), 8889);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8890);
            UdpClient client = new UdpClient(localEndPoint);
            client.Connect(droneEndpoint);
            Thread th = new Thread(Program.RecvThread);
            th.Start(client);
            //
            while (true)
            {
                Console.Write("Command : ");
                string cmd = Console.ReadLine();
                if (String.IsNullOrEmpty(cmd))
                    break;
                //
                byte[] cmdData = Encoding.UTF8.GetBytes(cmd);
                //
                //client.Send(cmdData, cmdData.Length,droneEndpoint);
                client.Send(cmdData, cmdData.Length);
                Thread.Sleep(0);
                if (!th.IsAlive)
                    th.Start(client);
                Thread.Sleep(0);
                //
                while (!Program.IsNewReponseAvailable) ;
                Console.WriteLine(Program.LastResponse);
                //
            }
            client.Close();

            //
            Console.WriteLine("Press <return> to Close...");
            Console.ReadLine();
        }

        public static void RecvThread( object param )
        {
            UdpClient client = (UdpClient)param;
            while (true)
            {
                try
                {
                    IPEndPoint remoteEndPoint = null;
                    byte[] responseData = client.Receive(ref remoteEndPoint);
                    //
                    string response = Encoding.UTF8.GetString(responseData);
                    if (response.StartsWith("mid"))
                        continue;
                    Program.IsNewReponseAvailable = true;
                    Program.lastResponse = response;
                }
                catch ( Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }
            Console.WriteLine("-->>...");
        }
    }
}
