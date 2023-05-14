using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Filter : BindableBase
    {
        #region POLJA KLASE Filter
        private int indeksUListiAdresneKlase;
        private bool veceCekirano;
        private bool manjeCekirano;
        private bool jednakoCekirano;
        private int trazeniId;
        #endregion

        #region KONSTRUKTOR KLASE Filter
        public Filter() 
        {
            // Prazan konstruktor
        }
        #endregion

        #region PROPERTY KLASE Filter
        public int IndeksUListiAdresneKlase
        {
            get
            {
                return indeksUListiAdresneKlase;
            }

            set
            {
                if(indeksUListiAdresneKlase != value)
                {
                    indeksUListiAdresneKlase = value;
                    OnPropertyChanged("IndeksUListiAdresneKlase");
                }
            }
        }

        public bool VeceCekirano
        {
            get
            {
                return veceCekirano;
            }

            set
            {
                if(veceCekirano != value)
                {
                    veceCekirano = value;
                    OnPropertyChanged("VeceCekirano");
                }
            }
        }

        public bool ManjeCekirano
        {
            get
            {
                return manjeCekirano;
            }

          set
            {
                if (manjeCekirano != value)
                {
                    manjeCekirano = value;
                    OnPropertyChanged("ManjeCekirano");
                }
            }
        }

        public bool JednakoCekirano
        {
            get
            {
                return jednakoCekirano;
            }

            set
            {
                if (jednakoCekirano != value)
                {
                    jednakoCekirano = value;
                    OnPropertyChanged("JednakoCekirano");
                }
            }
        }

        public int TrazeniId
        {
            get
            {
                return trazeniId;
            }

            set
            {
                if(trazeniId  != value)
                {
                    trazeniId = value;
                    OnPropertyChanged("TrazeniId");
                }
            }
        }
        #endregion
    }
}
