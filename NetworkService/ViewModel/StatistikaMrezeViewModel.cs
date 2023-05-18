using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.lO;

namespace NetworkService.ViewModel
{
    public class StatistikaMrezeViewModel : BindableBase
    {
        #region POLJA KLASE StatistikaMrezeViewModel
        public static ObservableCollection<Entitet> Entiteti { get; set; }

        private Entitet odabraniEntitet;

        private int odabraniId;

        Merenje merenje_1, merenje_2, merenje_3, merenje_4, merenje_5;
        #endregion

        #region KONSTRUKTOR KLASE StatistikaMrezeViewModel
        public StatistikaMrezeViewModel()
        {
            Entiteti = MainWindowViewModel.Entiteti; // svi entiteti se modeluju
            OdabraniEntitet = Entiteti[0];
            OnPropertyChanged("OdabraniEntitet");

            // poslednjih 5 merenja
            Merenje_1 = new Merenje() { Izmereno = OdabraniEntitet.Zauzece };
            Merenje_2 = new Merenje() { Izmereno = 0, VanOpsega = true };
            Merenje_3 = new Merenje() { Izmereno = 0, VanOpsega = true };
            Merenje_4 = new Merenje() { Izmereno = 0, VanOpsega = true };
            Merenje_5 = new Merenje() { Izmereno = 0, VanOpsega = true };
        }
        #endregion

        #region PROPERTY KLASE
        public Entitet OdabraniEntitet
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
        #endregion


        #region POZADINSKA NIT KOJA CITA IZ FAJLA POSLEDNJIH 5 MERENJA

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

                    switch (izmereno)
                    {
                        case 1: Merenje_1.Izmereno = merenje_log; OnPropertyChanged("Merenje_1"); break;
                        case 2: Merenje_2.Izmereno = merenje_log; OnPropertyChanged("Merenje_2"); break;
                        case 3: Merenje_3.Izmereno = merenje_log; OnPropertyChanged("Merenje_3"); break;
                        case 4: Merenje_4.Izmereno = merenje_log; OnPropertyChanged("Merenje_4"); break;
                        case 5: Merenje_5.Izmereno = merenje_log; OnPropertyChanged("Merenje_5"); break;
                        default: Merenje_1.Izmereno = merenje_log; OnPropertyChanged("Merenje_1"); break;
                    }

                    izmereno++;
                }
            }
        }
        #endregion
    }
}
