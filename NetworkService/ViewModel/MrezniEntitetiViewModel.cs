using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace NetworkService.ViewModel
{
    public class MrezniEntitetiViewModel : BindableBase
    {
        #region KOMANDE
        // Komanda za filtriranje
        public MyICommand FiltrirajKomanda { get; set; }

        // Komanda za dodavanje
        public MyICommand DodajKomanda { get; set; }

        // Komanda za brisanje
        public MyICommand ObrisiKomanda { get; set; }
        #endregion

        #region POLJA KLASE MrezniEntitetiViewModel
        private static readonly ObservableCollection<string> adresneKlase = new ObservableCollection<string>
                                                                   { "Adrese Klase A", "Adrese Klase B", "Adrese Klase C",
                                                                     "Adrese Klase D", "Adrese Klase E"};

        private enum KLASE { A, B, C, D, E };

        private int odabranaKlasaIndeks;

        private bool veceCekirano;
        private bool manjeCekirano;
        private bool jednakoCekirano;

        private int trenutniId;

        public static ObservableCollection<Filter> IstorijaFiltera { get; set; }

        private int odabraniIndeksIstorijeFiltera;

        private int odabraniIndeksDodavanjeEntiteta;

        private bool moguceBrisanje;

        private Entitet odabraniEntitet;

        private Filter odabraniFilter = new Filter();


        // Ako nije primenjen nijedan filter - prikazuje se originalna lista entiteta
        // U suprotnom prikazuju se filtrirani entiteti iz Liste FiltriraniEntiteti
        private static ObservableCollection<Entitet> listaEntiteta { get; set; }
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
            OdabraniIndeksIstorijeFiltera = 0;
            odabraniIndeksDodavanjeEntiteta = 0;
            FiltrirajKomanda = new MyICommand(OnFilterPress);
            ObrisiKomanda = new MyICommand(OnBrisanjePress);
            DodajKomanda = new MyICommand(OnDodajPress);
            listaEntiteta = MainWindowViewModel.Entiteti; // na prvi prikaz prikazuju se svi entiteti
            MoguceBrisanje = false;
            OdabraniEntitet = null; // nije odabran nijedan entitet

           // Simulator = 
        }

        #endregion

        #region PROPERTY KLASE MrezniEntitetiViewModel
        public static ObservableCollection<string> AdresneKlase
        {
            get
            {
                return adresneKlase;
            }
        }

        public ObservableCollection<Entitet> ListaEntiteta
        {
            get
            {
                return listaEntiteta;
            }

            set
            {
                if (listaEntiteta != null)
                {
                    listaEntiteta = value;
                    OnPropertyChanged("ListaEntiteta");
                }
            }
        }

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
                    OnPropertyChanged("MoguceBrisanje");
                }
            }
        }

        public bool MoguceBrisanje
        {
            get
            {
                return OdabraniEntitet != null;
            }

            set
            {
                if (moguceBrisanje != value)
                {
                    moguceBrisanje = value;
                    OnPropertyChanged("MoguceBrisanje");
                    OnPropertyChanged("Background");
                }
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
                if (odabranaKlasaIndeks != value)
                {
                    odabranaKlasaIndeks = value;
                    OnPropertyChanged("OdabranaKlasaIndeks");
                }
            }
        }
        public int OdabraniIndeksDodavanjeEntiteta
        {
            get
            {
                return odabraniIndeksDodavanjeEntiteta;
            }

            set
            {
                if (odabraniIndeksDodavanjeEntiteta != value)
                {
                    odabraniIndeksDodavanjeEntiteta = value;
                    OnPropertyChanged("OdabraniIndeksDodavanjeEntiteta");
                }
            }
        }

        public int OdabraniIndeksIstorijeFiltera
        {
            get
            {
                return odabraniIndeksIstorijeFiltera;
            }

            set
            {
                if (odabraniIndeksIstorijeFiltera != value)
                {
                    odabraniIndeksIstorijeFiltera = value;
                    OnPropertyChanged("OdabraniIndeksIstorijeFiltera");
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
                if (manjeCekirano != value)
                {
                    manjeCekirano = value;

                    if (manjeCekirano)
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
                if (jednakoCekirano != value)
                {
                    jednakoCekirano = value;

                    if (jednakoCekirano)
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
                if (odabraniFilter != value)
                {
                    odabraniFilter = value;

                    // primeniti filter na polja
                    OdabranaKlasaIndeks = odabraniFilter.IndeksUListiAdresneKlase;
                    VeceCekirano = odabraniFilter.VeceCekirano;
                    ManjeCekirano = odabraniFilter.ManjeCekirano;
                    JednakoCekirano = odabraniFilter.JednakoCekirano;
                    TrenutniId = odabraniFilter.TrazeniId;

                    OnPropertyChanged("OdabraniFilter");
                    OnPropertyChanged("OdabranaKlasaIndeks");
                    OnPropertyChanged("VeceCekirano");
                    OnPropertyChanged("ManjeCekirano");
                    OnPropertyChanged("JednakoCekirano");
                    OnPropertyChanged("TrenutniId");

                    // primeni filter
                    OnFilterPress();
                }
            }
        }
        #endregion

        #region Implementacija komadni za dodavanje i brisanje entiteta
        public void OnDodajPress()
        {
            OdabraniEntitet = null;
            ListaEntiteta = MainWindowViewModel.Entiteti;

            // CG1 - Na osnovu odabrane adresne klase kreira random entitet

            // novi id je trenutni najveci id + 1
            int max_id = ListaEntiteta.Count != 0 ? ListaEntiteta.Max(x => x.Id) + 1 : 1;

            int odabrana_adresna_klasa = OdabraniIndeksDodavanjeEntiteta;
            const int ip_min = 0, ip_max = 255;
            int ip_prvi_oktet, ip_drugi_oktet, ip_treci_oktet, ip_cetvrti_oktet;
            string ip, klasa;

            // generisanje na osnovu odabrane adresne klase
            switch (odabrana_adresna_klasa)
            {
                case 0: ip_prvi_oktet = new Random().Next(1, 127); klasa = "A"; break;
                case 1: ip_prvi_oktet = new Random().Next(128, 191); klasa = "B"; break;
                case 2: ip_prvi_oktet = new Random().Next(192, 223); klasa = "C"; break;
                case 3: ip_prvi_oktet = new Random().Next(224, 239); klasa = "D"; break;
                case 4: ip_prvi_oktet = new Random().Next(240, 255); klasa = "E"; break;
                default: ip_prvi_oktet = 0; klasa = "A"; break;
            }

            ip_drugi_oktet = new Random().Next(ip_min, ip_max);
            Thread.Sleep(50);
            ip_treci_oktet = new Random().Next(ip_min, ip_max);
            Thread.Sleep(50);
            ip_cetvrti_oktet = new Random().Next(ip_min, ip_max);

            ip = ip_prvi_oktet + "." + ip_drugi_oktet + "." + ip_treci_oktet + "." + ip_cetvrti_oktet;

            Messenger.Default.Send(
                new Entitet()
                {
                    Id = max_id,
                    Naziv = "Entitet " + (max_id < 10 ? ("0" + max_id).ToString() : max_id.ToString()),
                    IP = ip,
                    Slika = "/Assets/uredjaj.png",
                    Zauzece = new Random().Next(0, 100),
                    Klasa = klasa,
                    Canvas_pozicija = -1,
                    Povezan_sa_entitet_id = -1
                });

            MainWindowViewModel.rasporedMrezeViewModel.Preraspodela();
        }

        public void OnBrisanjePress()
        {
            Messenger.Default.Send(ListaEntiteta.IndexOf(OdabraniEntitet));
            OdabraniEntitet = null;
            Messenger.Default.Send(ListaEntiteta);
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
            foreach (Filter tmp in IstorijaFiltera)
            {
                if (tmp.Equals(istorija))
                {
                    postoji = true;
                    break;
                }
            }

            if (!postoji)
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
            //Messenger.Default.Send(svi);

            // Nije odabran nijedan entitet
            OdabraniEntitet = null;

            if (svi != null && svi.Count > 0)
            {
                foreach (Entitet t in svi)
                {
                    int klasa = ProveriAdresnuKlasu(t.IP);

                    // Pripada odabranoj klasi
                    if (OdabranaKlasaIndeks == klasa)
                    {
                        // Proveri vece, manje, jednako
                        if (PrimeniFilter(t))
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

            if (prvi_oktet_ip >= 1 && prvi_oktet_ip <= 127) pripada = 0;
            if (prvi_oktet_ip >= 128 && prvi_oktet_ip <= 191) pripada = 1;
            if (prvi_oktet_ip >= 192 && prvi_oktet_ip <= 223) pripada = 2;
            if (prvi_oktet_ip >= 224 && prvi_oktet_ip <= 239) pripada = 3;
            if (prvi_oktet_ip >= 240 && prvi_oktet_ip <= 255) pripada = 4;

            return pripada;
        }
        #endregion

        #region FILTRIRANJE PODATAKA
        bool PrimeniFilter(Entitet trenutni)
        {
            if (VeceCekirano && (trenutni.Id > TrenutniId))
            {
                return true;
            }
            else if (ManjeCekirano && (trenutni.Id < TrenutniId))
            {
                return true;
            }
            else if (JednakoCekirano && (trenutni.Id == TrenutniId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
