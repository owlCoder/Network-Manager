using NetworkService.Helpers;

namespace NetworkService.Model
{
    public class Entitet : ValidationBase
    {
        #region POLJA KLASE Entitet
        private int id;
        private string naziv;
        private string ip;
        private string slika;
        private int zauzece;
        private bool boja;
        private string klasa;
        private int canvas_pozicija;
        private int povezan_sa_entitet_id;
        #endregion

        #region KONSTRUKTOR KLASE Entitet
        public Entitet()
        {
            // prazan konstruktor
        }
        #endregion

        #region PROPERTY KLASE Entitet
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
                }
                else
                {
                    Boja = false;
                }

                OnPropertyChanged("Boja");
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

        public int Povezan_sa_entitet_id
        {
            get
            {
                return povezan_sa_entitet_id;
            }

            set
            {
                if (povezan_sa_entitet_id != value)
                {
                    povezan_sa_entitet_id = value;
                    OnPropertyChanged("Povezan_sa_entitet_id");
                }
            }
        }
        #endregion

        #region METODA ZA MODELOVANI ISPIS ENTITETA
        public override string ToString()
        {
            return "Entitet " + Id + ": " + Naziv + " (" + IP + ")";
        }
        #endregion
    }
}
