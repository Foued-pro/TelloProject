using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TelloLibrary;

namespace Tello1
{
    class Program
    {
        static CancellationTokenSource _cancelVideoSource;
        static CancellationToken _cancelVideo;
        static Task _videoTask;
        static VideoCapture capture;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting " + DateTime.Now.ToString() + " session.");
            Console.WriteLine("Initialize OpenCV.....");
            capture = new VideoCapture();
            //
            Console.WriteLine("Open connection to Tello.....");
            Console.Write("IP Address : (empty for default) ");
            string ipAddress = Console.ReadLine();
            if (String.IsNullOrEmpty(ipAddress))
                ipAddress = "192.168.10.1";
            Tello drone = new Tello(ipAddress);
            int cmdCode;
            int modePage = 0;
            //
            do
            {
                cmdCode = MenuPage(modePage);
                //
                switch (cmdCode)
                {
                    #region Menu 1
                    case 1:
                        if (drone.Command() == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 2:
                        Console.WriteLine(drone.Battery());
                        break;
                    case 3:
                        if (drone.TakeOff() == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 4:
                        if (drone.Land() == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 5:
                        if (drone.TurnClockwise(360) == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 6:
                        Console.WriteLine("Time Of Flight :" + drone.State.tof);
                        break;

                    case 7:
                        if (drone.StreamOn() == Tello.Response.OK)
                        {
                            _cancelVideoSource = new CancellationTokenSource();
                            _cancelVideo = _cancelVideoSource.Token;
                            _videoTask = new Task(Program.VideoThread, _cancelVideo);
                            _videoTask.Start();
                            Console.WriteLine("Ok");
                        }
                        break;
                    case 8:
                        if (drone.StreamOff() == Tello.Response.OK)
                        {
                            _cancelVideoSource.Cancel();
                            Console.WriteLine("Ok");
                        }
                        break;
                    case 9:
                    case 19:
                        modePage++;
                        if (modePage > 1)
                            modePage = 0;
                        break;
                    #endregion

                    #region Menu 2
                    case 11:
                        if (drone.MoveUp(20) == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 12:
                        if (drone.MoveDown(20) == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 13:
                        if (drone.MoveLeft(20) == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 14:
                        if (drone.MoveRight(20) == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 15:
                        if (drone.MoveForward(20) == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 16:
                        if (drone.MoveBackward(20) == Tello.Response.OK)
                            Console.WriteLine("Ok");
                        break;
                    case 17:
                        Console.Write("Command :");
                        var str = Console.ReadLine();
                        var action = new TelloAction(drone, "Texte", str, TelloAction.ActionTypes.Read);
                        var reponse = drone.SendCommand(action, Tello.TimeOut.Standard);
                        Console.WriteLine(reponse);
                        break;
                    #endregion
                    default:
                        break;
                }
            } while (cmdCode != 0);
            Console.WriteLine("Appuyez sur Return pour sortir.");
            Console.ReadLine();
            drone.Dispose();
        }

        private static int MenuPage(int page)
        {
            if (page == 0)
            {
                Console.WriteLine("0. Quit");
                Console.WriteLine("1. Command");
                Console.WriteLine("2. Battery");
                Console.WriteLine("3. TakeOff");
                Console.WriteLine("4. Land");
                Console.WriteLine("5. Turn Clockwise");
                Console.WriteLine("6. State");
                Console.WriteLine("7. Stream On");
                Console.WriteLine("8. Stream Off");
                Console.WriteLine("9. Autre Page");
            }
            else
            {
                Console.WriteLine("0. Quit");
                Console.WriteLine("1. up 20");
                Console.WriteLine("2. down 20");
                Console.WriteLine("3. left 20");
                Console.WriteLine("4. right 20");
                Console.WriteLine("5. forward 20");
                Console.WriteLine("6. backward 20");
                Console.WriteLine("7. commande Texte");
                Console.WriteLine("9. Autre Page");
            }
            //
            string cmd = Console.ReadLine();
            int cmdCode;
            if (int.TryParse(cmd, out cmdCode))
            {
                //
                if (cmdCode != 0)
                    cmdCode = page * 10 + cmdCode;
            }
            else
                cmdCode = -1;
            //
            return cmdCode;
        }

        static void VideoThread()
        {
            VideoWriter encoder = null;
            String fileName = GetUniqueFileName("tello.avi");
            if (capture.Open("udp://0.0.0.0:11111"))
            {
                var sz = new Size(capture.FrameWidth, capture.FrameHeight);
                encoder = new VideoWriter(fileName, FourCC.MJPG, 15, sz);
                int seq = 0;
                using (Mat frame = new Mat())
                {
                    while (true)
                    {
                        try
                        {
                            if (capture.Read(frame))
                            {
                                // Ne fonctionne QUE dans le thread d'UI !!!!!
                                //Cv2.ImShow("Tello", frame);
                                //
                                //frame.SaveImage("img" + seq.ToString()+".jpg");
                                encoder.Write(frame);
                                seq++;
                            }
                            //
                            if (_cancelVideo.IsCancellationRequested)
                            {
                                _cancelVideo.ThrowIfCancellationRequested();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Erreur " + e.Message);
                            break;
                        }

                    }
                }

            }
            encoder?.Release();
            capture.Release();
        }

        private static string GetUniqueFileName(string v)
        {
            String fileName = Path.GetFileNameWithoutExtension(v);
            String ext = Path.GetExtension(v);
            int max = 0;
            //
            var files = Directory.EnumerateFiles(System.AppContext.BaseDirectory, fileName + "*" + ext);
            //
            foreach (var file in files)
            {
                String fileInfo = Path.GetFileNameWithoutExtension(file);
                fileInfo = fileInfo.Substring(fileName.Length);
                int number = 0;
                if (int.TryParse(fileInfo, out number))
                    max = Math.Max(max, number);
            }
            max++;
            return fileName + max.ToString("D4") + ext;
        }
    }
}
