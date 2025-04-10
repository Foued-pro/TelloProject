using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TelloFake
{
    class Program
    {
        private const int listenPort = 8889;
        private static int speed = 0;
        private static int battery = 100;
        public static int Main()
        {

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, listenPort);
            string command;
            byte[] rawData;
            try
            {
                bool fini = false;
                while (!fini)
                {
                    Console.WriteLine();
                    Console.WriteLine("En attente de commande");
                    rawData = listener.Receive(ref clientEP);
                    Console.WriteLine("--> Paquet de {0}", clientEP.ToString());
                    //
                    command = Encoding.ASCII.GetString(rawData, 0, rawData.Length);
                    Console.WriteLine("--> Command : {0}", command);
                    //
                    if (command.IndexOf("?") > -1)
                    {
                        var resp = processReadCommand(command);
                        SendMessage(listener, resp, clientEP);
                    }
                    else
                    {
                        processCommand(command);
                        SendMessage(listener, "OK", clientEP);
                    }
                }
            }
            catch (Exception e)
            {
                WriteError(e.ToString());
                SendMessage(listener, e.Message, clientEP);
            }
            listener.Close();
            return 0;
        }

        private static void SendMessage(UdpClient client, string message, IPEndPoint endpoint)
        {
            var msgData = Encoding.ASCII.GetBytes(message);
            client.Connect(endpoint);
            client.Send(msgData, msgData.Length);
        }

        private static string processReadCommand(string command)
        {
            switch (command)
            {
                case "speed?":
                    return speed.ToString();
                case "battery?":
                    return battery.ToString();
                case "time?":
                    return "10";
            }
            return string.Empty;
        }


        private static void processCommand(string command)
        {
            // On récupère les infos
            var cmdPart = command.Split(' ');
            //
            Console.WriteLine("Commande reçue : " + command);
            switch (cmdPart[0])
            {
                case "command":
                    Console.WriteLine("Mode Command activé");
                    break;
                case "takeoff":
                    Console.WriteLine("Auto take off");
                    break;
                case "land":
                    Console.WriteLine("Auto landing");
                    break;
                case "up":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Up manquant.");
                        break;
                    }
                    Console.WriteLine( "Mouvement : up " + cmdPart[1] );
                    break;
                case "down":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Down manquant.");
                        break;
                    }
                    Console.WriteLine("Mouvement : down " + cmdPart[1]);
                    break;
                case "left":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Left manquant.");
                        break;
                    }
                    Console.WriteLine("Mouvement : left " + cmdPart[1]);
                    break;
                case "right":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Right manquant.");
                        break;
                    }
                    Console.WriteLine("Mouvement : right " + cmdPart[1]);
                    break;
                case "forward":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Forward manquant.");
                        break;
                    }
                    Console.WriteLine("Mouvement : forward " + cmdPart[1]);
                    break;
                case "back":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Back manquant.");
                        break;
                    }
                    Console.WriteLine("Mouvement : back " + cmdPart[1]);
                    break;
                case "cw":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre ClockWise manquant.");
                        break;
                    }
                    Console.WriteLine("Mouvement : cw " + cmdPart[1]);
                    break;
                case "ccw":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre CounterClockWise manquant.");
                        break;
                    }
                    Console.WriteLine("Mouvement : ccw " + cmdPart[1]);
                    break;
                case "flip":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Flip Impossible, direction non fournie.");
                        break;
                    }
                    Console.WriteLine("Mouvement : flip " + cmdPart[1]);
                    break;
                case "speed":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Speed Impossible, valeur non fournie.");
                        break;
                    }
                    speed = Convert.ToInt32(cmdPart[1]);
                    Console.WriteLine("Vitesse de {0} cm/s", cmdPart[1]);
                    break;

            }
        }

        private static void WriteError(string message)
        {
            var oldColor = Console.ForegroundColor;
            var oldBg = Console.BackgroundColor;
            //
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            //
            Console.WriteLine("Error: " + message);
            //
            Console.BackgroundColor = oldBg;
            Console.ForegroundColor = oldColor;
        }
    }
}



