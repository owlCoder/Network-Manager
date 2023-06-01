using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace NetworkService.ViewModel
{
    public class MrezniEntitetiViewModel : BindableBase
    {
        // Komanda za filtriranje
        public MyICommand FiltrirajKomanda { get; set; }

        // Komanda za dodavanje
        public MyICommand DodajKomanda { get; set; }

        // Komanda za brisanje
        public MyICommand ObrisiKomanda { get; set; }

        private static readonly ObservableCollection<string> adresneKlase = new ObservableCollection<string>
                                                                   { "IP Tipa A", "IP Tipa B", "IP Tipa C",
                                                                     "IP Tipa D", "IP Tipa E"};

        private enum KLASE { A, B, C, D, E };

        private int indeksKlase;

        private bool vece;
        private bool manje;
        private bool jednako;

        private int trenutniId;

        public static ObservableCollection<Filter> Istorija { get; set; }
        private int indeksFiltera;
        private int indeksDodavanja;
        private bool moguceBrisanje;
        private Server odabraniEntitet;
        private Filter odabraniFilter = new Filter();

        // Ako nije primenjen nijedan filter - prikazuje se originalna lista entiteta
        // U suprotnom prikazuju se filtrirani entiteti iz Liste FiltriraniEntiteti
        private static ObservableCollection<Server> listaEntiteta { get; set; }

        public MrezniEntitetiViewModel()
        {
            indeksKlase = 0;
            vece = true;
            manje = false;
            jednako = false;
            trenutniId = 0;
            Istorija = new ObservableCollection<Filter>();
            OdabraniIndeksIstorijeFiltera = 0;
            indeksDodavanja = 0;
            FiltrirajKomanda = new MyICommand(OnFilterPress);
            ObrisiKomanda = new MyICommand(OnBrisanjePress);
            DodajKomanda = new MyICommand(OnDodajPress);
            listaEntiteta = MainWindowViewModel.Serveri; // na prvi prikaz prikazuju se svi entiteti
            MoguceBrisanje = false;
            OdabraniEntitet = null; // nije odabran nijedan entitet
        }

        public static ObservableCollection<string> AdresneKlase
        {
            get
            {
                return adresneKlase;
            }
        }

        public ObservableCollection<Server> ListaEntiteta
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
                return indeksKlase;
            }

            set
            {
                if (indeksKlase != value)
                {
                    indeksKlase = value;
                    OnPropertyChanged("OdabranaKlasaIndeks");
                }
            }
        }
        public int OdabraniIndeksDodavanjeEntiteta
        {
            get
            {
                return indeksDodavanja;
            }

            set
            {
                if (indeksDodavanja != value)
                {
                    indeksDodavanja = value;
                    OnPropertyChanged("OdabraniIndeksDodavanjeEntiteta");
                }
            }
        }

        public int OdabraniIndeksIstorijeFiltera
        {
            get
            {
                return indeksFiltera;
            }

            set
            {
                if (indeksFiltera != value)
                {
                    indeksFiltera = value;
                    OnPropertyChanged("OdabraniIndeksIstorijeFiltera");
                }
            }
        }

        public bool VeceCekirano
        {
            get
            {
                return vece;
            }

            set
            {
                if (vece != value)
                {
                    vece = value;

                    if (vece)
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
                return manje;
            }

            set
            {
                if (manje != value)
                {
                    manje = value;

                    if (manje)
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
                return jednako;
            }

            set
            {
                if (jednako != value)
                {
                    jednako = value;

                    if (jednako)
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

        public void OnDodajPress()
        {
            OdabraniEntitet = null;
            ListaEntiteta = MainWindowViewModel.Serveri;

            // CG1 - Na osnovu odabrane adresne klase kreira random entitet

            // novi id je trenutni najveci id + 1
            int max_id = ListaEntiteta.Count != 0 ? ListaEntiteta.Max(x => x.Id) + 1 : 1;

            int odabrana_adresna_klasa = OdabraniIndeksDodavanjeEntiteta;
            const int ip_min = 0, ip_max = 255;
            int ip_prvi_oktet, ip_drugi_oktet, ip_treci_oktet, ip_cetvrti_oktet;
            string ip, klasa;

            // generisanje na osnovu odabrane adresne klase
            if (odabrana_adresna_klasa == 0)
            {
                ip_prvi_oktet = new Random().Next(1, 127);
                klasa = "A";
            }
            else if (odabrana_adresna_klasa == 1)
            {
                ip_prvi_oktet = new Random().Next(128, 191);
                klasa = "B";
            }
            else if (odabrana_adresna_klasa == 2)
            {
                ip_prvi_oktet = new Random().Next(192, 223);
                klasa = "C";
            }
            else if (odabrana_adresna_klasa == 3)
            {
                ip_prvi_oktet = new Random().Next(224, 239);
                klasa = "D";
            }
            else if (odabrana_adresna_klasa == 4)
            {
                ip_prvi_oktet = new Random().Next(240, 255);
                klasa = "E";
            }
            else
            {
                ip_prvi_oktet = 0;
                klasa = "A";
            }

            ip_drugi_oktet = new Random().Next(ip_min, ip_max);
            Thread.Sleep(50);
            ip_treci_oktet = new Random().Next(ip_min, ip_max);
            Thread.Sleep(50);
            ip_cetvrti_oktet = new Random().Next(ip_min, ip_max);

            ip = ip_prvi_oktet + "." + ip_drugi_oktet + "." + ip_treci_oktet + "." + ip_cetvrti_oktet;
            string naziv = "Server " + (max_id < 10 ? ("0" + max_id).ToString() : max_id.ToString());

            Messenger.Default.Send(
                new Server()
                {
                    Id = max_id,
                    Naziv = naziv,
                    IP = ip,
                    Slika = "/Assets/uredjaj.png",
                    Zauzece = new Random().Next(0, 100),
                    Klasa = klasa,
                    Canvas_pozicija = -1,
                });

        }

        public void OnBrisanjePress()
        {
            Messenger.Default.Send(ListaEntiteta.IndexOf(odabraniEntitet));
            OdabraniEntitet = null;
            Messenger.Default.Send(ListaEntiteta);
        }

        public void OnFilterPress()
        {
            // Sacuvaj u istoriju filtera
            Filter istorija = new Filter
            {
                IndeksUListiAdresneKlase = indeksKlase,
                VeceCekirano = VeceCekirano,
                ManjeCekirano = ManjeCekirano,
                JednakoCekirano = JednakoCekirano,
                TrazeniId = TrenutniId
            };

            // Ako filter vec postoji u listi filtera - ne dodaje ste
            bool postoji = false;
            foreach (Filter tmp in Istorija)
            {
                if (tmp.Equals(istorija))
                {
                    postoji = true;
                    break;
                }
            }

            if (!postoji)
            {
                Istorija.Add(istorija);
            }

            // Primena filtera
            ListaEntiteta = Filtracija();
        }

        ObservableCollection<Server> Filtracija()
        {
            ObservableCollection<Server> filtrirani = new ObservableCollection<Server>();
            ObservableCollection<Server> svi = MainWindowViewModel.Serveri;

            // Nije odabran nijedan entitet
            OdabraniEntitet = null;

            if (svi != null && svi.Count > 0)
            {
                foreach (Server t in svi)
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

        bool PrimeniFilter(Server trenutni)
        {
            if (VeceCekirano && (trenutni.Id > TrenutniId)) return true;
            else if (ManjeCekirano && (trenutni.Id < TrenutniId)) return true;
            else if (JednakoCekirano && (trenutni.Id == TrenutniId)) return true;
            else return false;
        }
    }
}
