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
        #endregion

        #region KONSTRUKTOR KLASE StatistikaMrezeViewModel
        public StatistikaMrezeViewModel()
        {
            Entiteti = MainWindowViewModel.Entiteti; // svi entiteti se modeluju
            OdabraniEntitet = Entiteti[0];
            OnPropertyChanged("OdabraniEntitet");
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
            }
        }
        #endregion
    }
}
