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
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        #region NAVIGACIONE KOMANDE
        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> CloseWindowCommand { get; private set; }
        #endregion

        #region VIEW MODEL INSTANCE
        public PocetnaViewModel pocetnaViewModel;
        public MrezniEntitetiViewModel mrezniEntitetiViewModel;
        public StatistikaMrezeViewModel statistikaMrezeViewModel;
        public RasporedMrezeViewModel rasporedMrezeViewModel;
        #endregion

        #region TRENUTNO PRIKAZAN VIEW MODEL
        private BindableBase currentViewModel;
        #endregion
     
        #region LISTA I BROJ MREZNIH ENTITETA
        public static ObservableCollection<Entitet> Entiteti { get; set; }

        private int count = 1; // Inicijalna vrednost broja objekata u sistemu
                               // ######### ZAMENITI stvarnim brojem elemenata
                               //           zavisno od broja entiteta u listi
        #endregion

        #region KONSTRUKTOR KLASE 
        public MainWindowViewModel()
        {
            createListener(); // Povezivanje sa serverskom aplikacijom

            NavCommand = new MyICommand<string>(OnNav);
            CloseWindowCommand = new MyICommand<Window>(CloseWindow);

            Entiteti = new ObservableCollection<Entitet>();

            pocetnaViewModel = new PocetnaViewModel();
            mrezniEntitetiViewModel = new MrezniEntitetiViewModel();
            pocetnaViewModel = new PocetnaViewModel();
            rasporedMrezeViewModel = new RasporedMrezeViewModel();
            CurrentViewModel = pocetnaViewModel;

            Messenger.Default.Register<Entitet>(this, AddToList);
            Messenger.Default.Register<int>(this, RemoveFromList);
            Messenger.Default.Register<ObservableCollection<Entitet>>(this, GetList);

            // test entiteti
            Entiteti.Add(new Entitet()
            {
                Id = 1,
                Naziv = "Entitet 1",
                IP = "192.168.0.1",
                Slika = "/Assets/uredjaj.png",
                Zauzece = 15
            }) ;

            Entiteti.Add(new Entitet()
            {
                Id = 2,
                Naziv = "Entitet 2",
                IP = "233.168.0.1",
                Slika = "/Assets/uredjaj.png",
                Zauzece = 15
            });

            Entiteti.Add(new Entitet()
            {
                Id = 3,
                Naziv = "Entitet 3",
                IP = "14.168.0.1",
                Slika = "/Assets/uredjaj.png",
                Zauzece = 15
            });
        }
        #endregion

        #region TCP MREZNA KONEKCIJA SA SERVEROM
        private void createListener()
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
                            Byte[] data = Encoding.ASCII.GetBytes(Entiteti.Count.ToString());
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

                            Entitet za_izmenu = Entiteti.FirstOrDefault(p => p.Id == id + 1);

                            if(za_izmenu != null && zauzece >= 45 && zauzece <= 75)
                            {
                                // zauzece je u opsegu pa se upisuje u entitet
                                za_izmenu.Zauzece = zauzece;
                            }

                            // upis merenja u txt datoteku
                            File.AppendAllText("log.txt", ((id + 1) + "-" + zauzece).ToString() + "-" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n");
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
        #endregion

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "pocetna":
                    CurrentViewModel = pocetnaViewModel;
                    break;
                case "mrezni_entiteti":
                    CurrentViewModel = mrezniEntitetiViewModel;
                    break;
                case "statistika_mreze":
                    CurrentViewModel = statistikaMrezeViewModel;
                    break;
                case "raspored_mreze":
                    CurrentViewModel = rasporedMrezeViewModel;
                    break;
            }
        }

        private void AddToList(Entitet novi)
        {
            Entiteti.Insert(0, novi);
        }

        private void RemoveFromList(int index) 
        {
            Entiteti.RemoveAt(index); 
        }

        private void GetList(ObservableCollection<Entitet> e)
        {
            e = Entiteti;
        }

        private void CloseWindow(Window MainWindow)
        {
            MainWindow.Close();
        }
    }
}
