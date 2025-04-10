using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TellokFakeGUI
{
    public partial class MainWindow : Form
    {
        private CancellationTokenSource _cancelMainTaskSource;
        private CancellationToken _cancelMainTask;
        private Task _taskTello;

        private Tello leTello;
        private CancellationTokenSource _cancelListenSource;
        private CancellationToken _cancelListen;

        public MainWindow()
        {
            InitializeComponent();
            leTello = new Tello();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Visible = false;
            buttonStop.Visible = true;
            //
            StartTello();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStart.Visible = true;
            buttonStop.Visible = false;
        }

        internal void StartTello()
        {
            if ((_taskTello == null) || (_taskTello.Status != TaskStatus.Running))
            {
                _cancelMainTaskSource = new CancellationTokenSource();
                _cancelMainTask = _cancelMainTaskSource.Token;
                //
                _taskTello = new Task(this.TelloTask, _cancelMainTask);
                _taskTello.Start();
            }
        }

        async private void TelloTask()
        {
            UdpClient listener = new UdpClient(leTello.ListeningPort);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, leTello.ListeningPort);
            string command;
            byte[] rawData;
            try
            {
                bool fini = false;
                while (!fini)
                {
                    _cancelListenSource = new CancellationTokenSource();
                    _cancelListen = _cancelMainTaskSource.Token;
                    var listenTask = listener.ReceiveAsync();
                    listenTask.Wait(_cancelListen);
                    if (!_cancelListen.IsCancellationRequested)
                    {
                        clientEP = listenTask.Result.RemoteEndPoint;
                        //WriteTextMessage(String.Format("--> Paquet de {0}", clientEP.ToString()));
                        //
                        rawData = listenTask.Result.Buffer;
                        command = Encoding.ASCII.GetString(rawData, 0, rawData.Length);
                        //WriteTextMessage(String.Format("--> Command : {0}", command));
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
                    else
                    {
                        fini = true;
                    }
                }
            }
            catch (Exception e)
            {
                WriteError(e.ToString());
                SendMessage(listener, e.Message, clientEP);
            }
            listener.Close();
        }

        private string processReadCommand(string command)
        {
            switch (command)
            {
                case "speed?":
                    return leTello.Speed.ToString();
                case "battery?":
                    return leTello.Battery.ToString();
                case "time?":
                    return "10";
            }
            return string.Empty;
        }


        private void processCommand(string command)
        {
            // On récupère les infos
            var cmdPart = command.Split(' ');
            //
            WriteTextMessage("Commande reçue : " + command);
            switch (cmdPart[0])
            {
                case "command":
                    WriteTextMessage("Mode Command activé");
                    break;
                case "takeoff":
                    WriteTextMessage("Auto take off");
                    leTello.IsFlying = true;
                    leTello.Altitude = 50;
                    break;
                case "land":
                    WriteTextMessage("Auto landing");
                    leTello.IsFlying = false;
                    leTello.Altitude = 0;
                    break;
                case "up":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Up manquant.");
                        break;
                    }
                    WriteTextMessage("Mouvement : up " + cmdPart[1]);
                    leTello.Altitude += int.Parse(cmdPart[1]);
                    break;
                case "down":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Down manquant.");
                        break;
                    }
                    WriteTextMessage("Mouvement : down " + cmdPart[1]);
                    leTello.Altitude -= int.Parse(cmdPart[1]);
                    break;
                case "left":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Left manquant.");
                        break;
                    }
                    WriteTextMessage("Mouvement : left " + cmdPart[1]);
                    leTello.XPos -= int.Parse(cmdPart[1]);
                    break;
                case "right":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Right manquant.");
                        break;
                    }
                    WriteTextMessage("Mouvement : right " + cmdPart[1]);
                    leTello.XPos += int.Parse(cmdPart[1]);
                    break;
                case "forward":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Forward manquant.");
                        break;
                    }
                    WriteTextMessage("Mouvement : forward " + cmdPart[1]);
                    leTello.YPos += int.Parse(cmdPart[1]);
                    break;
                case "back":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre Back manquant.");
                        break;
                    }
                    WriteTextMessage("Mouvement : back " + cmdPart[1]);
                    leTello.YPos -= int.Parse(cmdPart[1]);
                    break;
                case "cw":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre ClockWise manquant.");
                        break;
                    }
                    WriteTextMessage("Mouvement : cw " + cmdPart[1]);
                    break;
                case "ccw":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Parametre CounterClockWise manquant.");
                        break;
                    }
                    WriteTextMessage("Mouvement : ccw " + cmdPart[1]);
                    break;
                case "flip":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Flip Impossible, direction non fournie.");
                        break;
                    }
                    WriteTextMessage("Mouvement : flip " + cmdPart[1]);
                    break;
                case "speed":
                    if (cmdPart.Length == 1)
                    {
                        WriteError("Speed Impossible, valeur non fournie.");
                        break;
                    }
                    leTello.Speed = Convert.ToInt32(cmdPart[1]);
                    WriteTextMessage( String.Format("Vitesse de {0} cm/s", cmdPart[1]));
                    break;
            }
            UpdateTello();
        }

        private void SendMessage(UdpClient client, string message, IPEndPoint endpoint)
        {
            var msgData = Encoding.ASCII.GetBytes(message);
            client.Connect(endpoint);
            client.Send(msgData, msgData.Length);
        }

        private void WriteError(string v)
        {
            Action safeWrite = delegate { WriteErrorSafe(v); };
            this.Invoke(safeWrite);
        }

        private void WriteTextMessage(string v)
        {
            Action safeWrite = delegate { WriteTextSafe(v); };
            this.Invoke(safeWrite);
        }

        private void WriteErrorSafe(string v)
        {
            statusLabel.ForeColor = Color.Red;
            statusLabel.Text = v;
        }

        private void WriteTextSafe(string v)
        {
            textMessages.AppendText( v + Environment.NewLine );
        }

        private void UpdateTello()
        {
            Action safeWrite = delegate { UpdateTelloSafe(); };
            this.Invoke(safeWrite);
        }

        private void UpdateTelloSafe()
        {
            if ( leTello.IsFlying )
            {
                pictureAlt.Location = new Point(17, 0);
            }
            else
            {
                pictureAlt.Location = new Point(17, panel2.Height - pictureAlt.Height - 5);
            }
            //
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Etat du Tello");
            sb.AppendLine(" Alt. : " + leTello.Altitude);
            sb.AppendLine(" Speed : " + leTello.Speed);
            sb.AppendLine("Position");
            sb.AppendLine(" X : " + leTello.XPos);
            sb.AppendLine(" Y : " + leTello.YPos);
            //
            labelInfo.Text = sb.ToString();
        }
    }
}
