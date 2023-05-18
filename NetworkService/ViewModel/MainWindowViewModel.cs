﻿using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
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
        #region NAVIGACIONE KOMANDE
        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> CloseWindowCommand { get; private set; }
        #endregion

        #region VIEW MODEL INSTANCE
        public PocetnaViewModel pocetnaViewModel;
        public MrezniEntitetiViewModel mrezniEntitetiViewModel;
        public StatistikaMrezeViewModel statistikaMrezeViewModel;
        public static RasporedMrezeViewModel rasporedMrezeViewModel;
        #endregion

        #region TRENUTNO PRIKAZAN VIEW MODEL
        private BindableBase currentViewModel;
        #endregion

        #region LISTA I BROJ MREZNIH ENTITETA
        public static ObservableCollection<Entitet> Entiteti { get; set; }
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
            rasporedMrezeViewModel = new RasporedMrezeViewModel();
            CurrentViewModel = pocetnaViewModel;

            Messenger.Default.Register<Entitet>(this, AddToList);
            Messenger.Default.Register<int>(this, RemoveFromList);
            Messenger.Default.Register<ObservableCollection<Entitet>>(this, GetList);

            #region TEST ENTITETI 10 PRIMERAKA
            for (int i = 0; i < 5; i++)
            {
                mrezniEntitetiViewModel.OnDodajPress();
            }
            #endregion

            statistikaMrezeViewModel = new StatistikaMrezeViewModel();

            // brisanje starog log fajla ako postoji
            if (File.Exists("log.txt"))
            {
                File.Delete("log.txt");
            }

            statistikaMrezeViewModel.AzuriranjeMerenja();
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

                            // if(za_izmenu != null && zauzece >= 45 && zauzece <= 75)
                            //{
                            za_izmenu.Zauzece = zauzece;
                            // }

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
            Entiteti.Add(novi);
        }

        private void RemoveFromList(int index)
        {
            Entiteti.RemoveAt(index);
        }

        private void GetList(ObservableCollection<Entitet> e)
        {
            e = Entiteti;
        }

        #region delimiter datoteke
        void Delimit_File(string str)
        {
            PocetnaViewModel.DELIMITER_CONST.Add(str);
            int old_id = statistikaMrezeViewModel.OdabraniId;
            statistikaMrezeViewModel.OdabraniId = 1;
            statistikaMrezeViewModel.OdabraniId = old_id;
        }
        #endregion

        private void CloseWindow(Window MainWindow)
        {
            MainWindow.Close();
        }
    }
}
