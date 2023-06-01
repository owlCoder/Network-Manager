using NetworkService.Helpers;

namespace NetworkService.Model
{
    public class Server : BindableBase
    {
        private int id;
        private string naziv;
        private string ip;
        private string slika;
        private int zauzece;
        private bool boja;
        private string klasa;
        private int canvas_pozicija;

        public Server()
        {
            // prazan konstruktor
        }

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string Naziv
        {
            get
            {
                return naziv;
            }

            set
            {
                if (naziv != value)
                {
                    naziv = value;
                    OnPropertyChanged("Naziv");
                }
            }
        }

        public string IP
        {
            get
            {
                return ip;
            }

            set
            {
                if (ip != value)
                {
                    ip = value;
                    OnPropertyChanged("IP");
                }
            }
        }

        public string Slika
        {
            get
            {
                return slika;
            }

            set
            {
                if (slika != value)
                {
                    slika = value;
                    OnPropertyChanged("Slika");
                }
            }
        }

        public int Zauzece
        {
            get
            {
                return zauzece;
            }

            set
            {
                if (zauzece != value)
                {
                    zauzece = value;
                    OnPropertyChanged("Zauzece");
                }

                if (zauzece < 45 || zauzece > 75)
                {
                    Boja = true;
                    Slika = "/Assets/uredjaj_error.png";
                }
                else
                {
                    Boja = false;
                    Slika = "/Assets/uredjaj.png";
                }

                OnPropertyChanged("Boja");
                OnPropertyChanged("Slika");
            }
        }

        public bool Boja
        {
            get
            {
                return boja;
            }

            set
            {
                if (boja != value)
                {
                    boja = value;
                    OnPropertyChanged("Boja");
                }
            }
        }

        public string Klasa
        {
            get
            {
                return klasa;
            }

            set
            {
                if (klasa != value)
                {
                    klasa = value;
                    OnPropertyChanged("Klasa");
                }
            }
        }

        public int Canvas_pozicija
        {
            get
            {
                return canvas_pozicija;
            }

            set
            {
                if (canvas_pozicija != value)
                {
                    canvas_pozicija = value;
                    OnPropertyChanged("Canvas_pozicija");
                }
            }
        }

        public override string ToString()
        {
            return Naziv + " (" + IP + ")";
        }
    }
}
