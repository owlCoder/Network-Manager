using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand<string> Navigacija { get; private set; }
        public MyICommand<Window> Izlaz { get; private set; }
        public MrezniEntitetiViewModel mreza;
        public StatistikaMrezeViewModel statistika;
        public RasporedMrezeViewModel raspored;
        public static List<string> System_Delimiter = new List<string>();
        private BindableBase trenutniView;
        public static ObservableCollection<Server> Serveri { get; set; }

        public MainWindowViewModel()
        {
            CreateListener(); // Povezivanje sa serverskom aplikacijom

            Navigacija = new MyICommand<string>(OnNav);
            Izlaz = new MyICommand<Window>(CloseWindow);
            Serveri = new ObservableCollection<Server>();
            mreza = new MrezniEntitetiViewModel();
            raspored = new RasporedMrezeViewModel();
            TrenutniView = mreza;

            Messenger.Default.Register<Server>(this, DodajServer);
            Messenger.Default.Register<int>(this, UkloniServer);
            Messenger.Default.Register<ObservableCollection<Server>>(this, ListaSvihServera);

            // test podaci pri pokretanju aplikacije
            for (int i = 0; i < 3; i++) mreza.OnDodajPress();
            mreza.OdabraniIndeksDodavanjeEntiteta = 0;

            statistika = new StatistikaMrezeViewModel();
            statistika.AzuriranjeMerenja();
        }

        private void CreateListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25565);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = Encoding.ASCII.GetBytes(Serveri.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            // Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            int len = incomming.Length;
                            string substring = incomming.Substring(8, len - 8);
                            string[] splitovano = substring.Split(':');
                            int id = int.Parse(splitovano[0]); // id entiteta koji se menja
                            int zauzece = int.Parse(splitovano[1]); // zauzece koje se menja

                            Server za_izmenu = Serveri.FirstOrDefault(p => p.Id == id + 1);

                            if (za_izmenu != null) // obrisan objekat a simulator se jos nije restartovao - odbaciti
                            {
                                int staro = za_izmenu.Zauzece;
                                za_izmenu.Zauzece = zauzece;
                            }

                            // upis merenja u txt datoteku
                            string za_upis = ((id + 1) + "-" + zauzece).ToString() + "-" + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            File.AppendAllText("log.txt", za_upis + "\n");
                            Delimit_File(za_upis);
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        public BindableBase TrenutniView
        {
            get { return trenutniView; }
            set { SetProperty(ref trenutniView, value); }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "mrezni_entiteti":
                    TrenutniView = mreza;
                    break;
                case "statistika_mreze":
                    TrenutniView = statistika;
                    break;
                case "raspored_mreze":
                    TrenutniView = raspored;
                    break;
            }
        }

        private void DodajServer(Server novi)
        {
            Serveri.Add(novi);
            Messenger.Default.Send(new ServerTreeView() { Entitet = novi });
        }

        private void UkloniServer(int index)
        {
            int idx = Serveri[index].Canvas_pozicija;
            Serveri.RemoveAt(index);
        }

        private void ListaSvihServera(ObservableCollection<Server> e)
        {
            e = Serveri;
        }

        void Delimit_File(string str)
        {
            System_Delimiter.Add(str);
            int old_id = statistika.OdabraniId;
            statistika.OdabraniId = 1;
            statistika.OdabraniId = old_id;
        }

        private void CloseWindow(Window MainWindow)
        {
            MainWindow.Close();
        }
    }
}
