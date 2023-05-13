using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class MrezniEntitetiViewModel : BindableBase
    {
        private static readonly ObservableCollection<string> adresneKlase = new ObservableCollection<string> 
                                                                   { "Adrese Klase A", "Adrese Klase B", "Adrese Klase C",
                                                                     "Adrese Klase D", "Adrese Klase E"};

        private int odabranaKlasaIndeks;

        private bool veceCekirano;
        private bool manjeCekirano;
        private bool jednakoCekirano;

        private int trenutniId;

        public MrezniEntitetiViewModel() 
        {
            odabranaKlasaIndeks = 0;
            veceCekirano = true;
            manjeCekirano = false;
            jednakoCekirano = false;
            trenutniId = 0;
        }

        #region PROPERTY KLASE MrezniEntitetiViewModel
        public ObservableCollection<string> AdresneKlase
        {
            get
            {
                return adresneKlase;
            }
        }

        public int OdabranaKlasaIndeks
        {
            get
            {
                return odabranaKlasaIndeks;
            }

            set
            {
                if(odabranaKlasaIndeks  != value)
                {
                    odabranaKlasaIndeks = value;
                    OnPropertyChanged("OdabranaKlasaIndeks");
                }
            }
        }
        #endregion

        #region PROPERTY ZA RADIO DUGMADI
        public bool VeceCekirano
        {
            get
            {
                return veceCekirano;
            }

            set
            {
                if (veceCekirano != value)
                {
                    veceCekirano = value;

                    if (veceCekirano)
                    {
                        ManjeCekirano = false;
                        JednakoCekirano = false;

                        OnPropertyChanged("ManjeCekirano");
                        OnPropertyChanged("JednakoCekirano");
                    }

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
                if(manjeCekirano != value)
                {
                    manjeCekirano = value;

                    if(manjeCekirano)
                    {
                        VeceCekirano = false;
                        JednakoCekirano = false;

                        OnPropertyChanged("VeceCekirano");
                        OnPropertyChanged("JednakoCekirano");
                    }

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
                if(jednakoCekirano != value)
                {
                    jednakoCekirano = value;

                    if(jednakoCekirano)
                    {
                        VeceCekirano = false;
                        ManjeCekirano = false;

                        OnPropertyChanged("ManjeCekirano");
                        OnPropertyChanged("VeceCekirano");
                    }

                    OnPropertyChanged("JednakoCekirano");
                }
            }
        }
        #endregion

        #region PROPERTY ZA Trenutni uneti Id
        public int TrenutniId
        {
            get
            {
                return trenutniId;
            }

            set
            {
                if (trenutniId != value)
                {
                    trenutniId = value;
                    OnPropertyChanged("TrenutniId");
                }
            }
        }
        #endregion
    }
}
