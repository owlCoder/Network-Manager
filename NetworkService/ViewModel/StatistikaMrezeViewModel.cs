using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Merenje_1 = new Merenje() { Izmereno = 0, VanOpsega = true };
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
                if(odabraniEntitet != value) 
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
                if(merenje_1 != value)
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
    }
}
