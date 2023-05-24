using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MeteringSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static double value = -1;
        private static int objectNum = 0, prev_obj_count = 0, timeout = 1;
        private int numObjects = -1;
        private Random r = new Random();
        private bool prviPut = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(5000);

            //Proveri broj objekata pod monitoringom
            askForCount();
            //Pocni prijavljivanje novih vrednosti za objekte
            startReporting();
        }

        private void askForCount()
        {
            try
            {
                //Pita koliko aplikacija ima objekata
                //Request
                Int32 port = 25565;
                TcpClient client = new TcpClient("localhost", port);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes("Need object count");
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                //Obrada odgovora
                //Response
                Byte[] responseData = new Byte[1024];
                string response = "";
                Int32 bytess = stream.Read(responseData, 0, responseData.Length);
                response = System.Text.Encoding.ASCII.GetString(responseData, 0, bytess);

                //Parsiranje odgovora u int vrednost
                numObjects = Int32.Parse(response);

                // ako se promeni broj entiteta restartuj simulator
                if (!prviPut && numObjects != prev_obj_count)
                {
                    RestartButton_Click(null, null);
                }
                else
                {
                    prviPut = false;
                    prev_obj_count = numObjects;
                }

                //Zatvaranje konekcije
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

        private void startReporting()
        {
            //Na radnom vreme posalji izmenu vrednosti nekog random objekta i nastavi da to radis u rekurziji
            int waitTime = r.Next(1000, 5000);

            // svaki peti put se pita za broj objekata
            if (timeout == 5)
            {
                timeout = 0;
                askForCount(); // proveri da li se broj objekata promenio

            }

            timeout += 1;

            Task.Delay(waitTime).ContinueWith(_ =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    //Slanje izmene stanja nekog objekta
                    sendReport();
                    //Upis u text box, radi lakse provere
                    textBox.Text = "Entity_" + (objectNum + 1) + " changed state to: " + value.ToString() + "\n" + textBox.Text;
                    //Pocni proces ispocetka
                    startReporting();
                });
            });
        }

        private void sendReport()
        {
            try
            {
                //Slanje nove vrednosti objekta
                //Request
                Int32 port = 25565;
                TcpClient client = new TcpClient("localhost", port);
                int rInt = r.Next(0, numObjects); //Brojimo od nule, maxValue nije ukljucen u range
                objectNum = rInt;
                value = r.Next(0, 100); //Uzete su nasumicne i realne vrednosti
                Byte[] data = System.Text.Encoding.ASCII.GetBytes("Entitet_" + rInt + ":" + value);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                //Zatvaranje konekcije
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception: " + e.Message);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        //public static void Reset()
        //{
        //    string str = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "");
        //    Process.Start(str + "MeteringSimulator.exe");
        //    Application.Current.Shutdown();
        //}
    }
}
