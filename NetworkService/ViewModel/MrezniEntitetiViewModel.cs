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
    public class MrezniEntitetiViewModel : BindableBase
    {
        #region POLJA KLASE MrezniEntitetiViewModel
        private static readonly ObservableCollection<string> adresneKlase = new ObservableCollection<string> 
                                                                   { "Adrese Klase A", "Adrese Klase B", "Adrese Klase C",
                                                                     "Adrese Klase D", "Adrese Klase E"};

        private enum KLASE { A, B, C, D, E};

        private int odabranaKlasaIndeks;

        private bool veceCekirano;
        private bool manjeCekirano;
        private bool jednakoCekirano;

        private int trenutniId;

        public static ObservableCollection<Filter> IstorijaFiltera { get; set; }

        private Filter odabraniFilter = new Filter();

        // Ako nije primenjen nijedan filter - prikazuje se originalna lista entiteta
        // U suprotnom prikazuju se filtrirani entiteti iz Liste FiltriraniEntiteti
        public static ObservableCollection<Entitet> ListaEntiteta { get; set; }
        #endregion

        #region KONSTRUKTOR KLASE MrezniEntitetiViewModel
        public MrezniEntitetiViewModel() 
        {
            odabranaKlasaIndeks = 0;
            veceCekirano = true;
            manjeCekirano = false;
            jednakoCekirano = false;
            trenutniId = 0;
            IstorijaFiltera = new ObservableCollection<Filter>();
        }
        #endregion

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
                    trenutniId = Math.Abs(value);
                    OnPropertyChanged("TrenutniId");
                }
            }
        }
        #endregion

        #region PROPERTY ZA TRENUTNO ODABRANI FILTER
        public Filter OdabraniFilter
        {
            get
            {
                return odabraniFilter;
            }

            set
            {
                if(odabraniFilter != value)
                {
                    odabraniFilter = value;
                    OnPropertyChanged("OdabraniFilter");
                }
            }
        }
        #endregion

        #region METODE ZA FILTRIRANJE PODATAKA
        public void OnFilterPress()
        {
            // Sacuvaj u istoriju filtera
            Filter istorija = new Filter
            {
                IndeksUListiAdresneKlase = odabranaKlasaIndeks,
                VeceCekirano = VeceCekirano,
                ManjeCekirano = ManjeCekirano,
                JednakoCekirano = JednakoCekirano,
                TrazeniId = TrenutniId
            };

            // Ako filter vec postoji u listi filtera - ne dodaje ste
            bool postoji = false;
            foreach(Filter tmp in IstorijaFiltera) 
            { 
                if(tmp.Equals(istorija))
                {
                    postoji = true;
                    break;
                }
            }

            if(!postoji)
            {
                IstorijaFiltera.Add(istorija);
            }

            // Primena filtera
            ListaEntiteta = Filtracija();
        }
        #endregion

        #region METODA ZA FILTRIRANJE ENTITETA
        ObservableCollection<Entitet> Filtracija()
        {
            ObservableCollection<Entitet> filtrirani = new ObservableCollection<Entitet>();
            ObservableCollection<Entitet> svi = MainWindowViewModel.Entiteti;

            if (svi != null && svi.Count > 0)
            {
                foreach(Entitet t in svi)
                {
                    int klasa = ProveriAdresnuKlasu(t.IP);

                    // Pripada odabranoj klasi
                    if(OdabranaKlasaIndeks == klasa)
                    {
                        // Proveri vece, manje, jednako
                        if(PrimeniFilter(t))
                        {
                            filtrirani.Add(t);
                        }
                    }
                }                
            }

            return filtrirani;
        }
        #endregion

        #region PROVERA ADRESNE KLASE
        int ProveriAdresnuKlasu(string ip)
        {
            int pripada = 0;
            int prvi_oktet_ip = int.Parse(ip.Split('.')[0]);

            if(prvi_oktet_ip >= 1   && prvi_oktet_ip <= 127) pripada = 0;
            if(prvi_oktet_ip >= 128 && prvi_oktet_ip <= 191) pripada = 1;
            if(prvi_oktet_ip >= 192 && prvi_oktet_ip <= 223) pripada = 2;
            if(prvi_oktet_ip >= 224 && prvi_oktet_ip <= 239) pripada = 3;
            if(prvi_oktet_ip >= 240 && prvi_oktet_ip <= 255) pripada = 4;

            return pripada;
        }
        #endregion
        
        bool PrimeniFilter(Entitet trenutni)
        {
            if(VeceCekirano && (trenutni.Id > TrenutniId))
            {
                return true;
            }
            else if(ManjeCekirano && (trenutni.Id < TrenutniId))
            {
                return true;
            }
            else if(JednakoCekirano && (trenutni.Id == TrenutniId))
            {
                return true;
            }    
            else
            {
                return false;
            }
        }
    }
}
