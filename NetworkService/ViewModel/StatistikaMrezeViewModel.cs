using MeteringSimulator;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.lO;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class StatistikaMrezeViewModel : BindableBase
    {
        public static ObservableCollection<Server> Entiteti { get; set; }

        private Server odabraniEntitet;
        private int odabraniId;
        Merenje merenje_1, merenje_2, merenje_3, merenje_4, merenje_5;

        public StatistikaMrezeViewModel()
        {
            Entiteti = MainWindowViewModel.Serveri; // svi entiteti se modeluju
            OdabraniEntitet = Entiteti[0];
            OnPropertyChanged("OdabraniEntitet");

            // poslednjih 5 merenja
            Merenje_1 = new Merenje() { Izmereno = OdabraniEntitet.Zauzece };
            Merenje_2 = new Merenje() { Izmereno = 0, VanOpsega = true };
            Merenje_3 = new Merenje() { Izmereno = 0, VanOpsega = true };
            Merenje_4 = new Merenje() { Izmereno = 0, VanOpsega = true };
            Merenje_5 = new Merenje() { Izmereno = 0, VanOpsega = true };
        }

        public Server OdabraniEntitet
        {
            get
            {
                return odabraniEntitet;
            }

            set
            {
                if (odabraniEntitet != value)
                {
                    odabraniEntitet = value;
                    OnPropertyChanged("OdabraniEntitet");
                }

                OdabraniId = OdabraniEntitet.Id;
                OnPropertyChanged("OdabraniId");
            }
        }

        public int OdabraniId
        {
            get
            {
                return odabraniId;
            }

            set
            {
                if (odabraniId != value)
                {
                    odabraniId = value;
                    OnPropertyChanged("OdabraniId");
                }

                if (Merenje_1 != null)
                {
                    Merenje_1.Izmereno = 0;
                    Merenje_2.Izmereno = 0;
                    Merenje_3.Izmereno = 0;
                    Merenje_4.Izmereno = 0;
                    Merenje_5.Izmereno = 0;

                    AzuriranjeMerenja();

                    OnPropertyChanged("Merenje_1");
                    OnPropertyChanged("Merenje_2");
                    OnPropertyChanged("Merenje_3");
                    OnPropertyChanged("Merenje_4");
                    OnPropertyChanged("Merenje_5");
                }
            }
        }

        public Merenje Merenje_1
        {
            get
            {
                return merenje_1;
            }

            set
            {
                if (merenje_1 != value)
                {
                    merenje_1 = value;
                    OnPropertyChanged("Merenje_1");
                }
            }
        }

        public Merenje Merenje_2
        {
            get
            {
                return merenje_2;
            }

            set
            {
                if (merenje_2 != value)
                {
                    merenje_2 = value;
                    OnPropertyChanged("Merenje_2");
                }
            }
        }

        public Merenje Merenje_3
        {
            get
            {
                return merenje_3;
            }

            set
            {
                if (merenje_3 != value)
                {
                    merenje_3 = value;
                    OnPropertyChanged("Merenje_3");
                }
            }
        }

        public Merenje Merenje_4
        {
            get
            {
                return merenje_4;
            }

            set
            {
                if (merenje_4 != value)
                {
                    merenje_4 = value;
                    OnPropertyChanged("Merenje_4");
                }
            }
        }

        public Merenje Merenje_5
        {
            get
            {
                return merenje_5;
            }

            set
            {
                if (merenje_5 != value)
                {
                    merenje_5 = value;
                    OnPropertyChanged("Merenje_5");
                }
            }
        }


        public void AzuriranjeMerenja()
        {
            // na osnovu trenutnog id citati iz fajla dok se ne nadje merenje
            if (!File.Exists("log.txt"))
                return;

            string[] procitano = File.ReadAllLines("log.txt");
            Array.Reverse(procitano); // citamo unazad log datoteku
            int izmereno = 1;

            foreach (string red in procitano)
            {
                if (izmereno > 5) // simulacija steka
                    izmereno = 0;

                string[] kolona = red.Split('-');

                if (int.Parse(kolona[0]) == OdabraniId)
                {
                    int merenje_log = int.Parse(kolona[1]); // izmerena vrednost

                    if (izmereno == 1)
                    {
                        Merenje_1.Izmereno = merenje_log;
                        OnPropertyChanged("Merenje_1");
                    }
                    else if (izmereno == 2)
                    {
                        Merenje_2.Izmereno = merenje_log;
                        OnPropertyChanged("Merenje_2");
                    }
                    else if (izmereno == 3)
                    {
                        Merenje_3.Izmereno = merenje_log;
                        OnPropertyChanged("Merenje_3");
                    }
                    else if (izmereno == 4)
                    {
                        Merenje_4.Izmereno = merenje_log;
                        OnPropertyChanged("Merenje_4");
                    }
                    else if (izmereno == 5)
                    {
                        Merenje_5.Izmereno = merenje_log;
                        OnPropertyChanged("Merenje_5");
                    }
                    else
                    {
                        Merenje_1.Izmereno = merenje_log;
                        OnPropertyChanged("Merenje_1");
                    }

                    izmereno++;
                }
            } 

            if(merenje_1.Izmereno == 0 && merenje_2.Izmereno == 0) merenje_1.Vidljivo = Visibility.Hidden; else merenje_1.Vidljivo= Visibility.Visible;
            if(merenje_2.Izmereno == 0 && merenje_3.Izmereno == 0) merenje_2.Vidljivo = Visibility.Hidden; else merenje_2.Vidljivo = Visibility.Visible;
            if (merenje_3.Izmereno == 0 && merenje_4.Izmereno == 0) merenje_3.Vidljivo = Visibility.Hidden; else merenje_3.Vidljivo = Visibility.Visible;
            if (merenje_4.Izmereno == 0 && merenje_5.Izmereno == 0) merenje_4.Vidljivo = Visibility.Hidden; else merenje_4.Vidljivo = Visibility.Visible;
            OnPropertyChanged("Merenje_1"); OnPropertyChanged("Merenje_2"); OnPropertyChanged("Merenje_3"); OnPropertyChanged("Merenje_4"); OnPropertyChanged("Merenje_5");
        }
    }
}
