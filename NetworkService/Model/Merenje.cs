using NetworkService.Helpers;
using System.Windows;

namespace NetworkService.Model
{
    public class Merenje : BindableBase
    {
        private int izmereno, duplirano, pozicija;

        private bool vanOpsega;
        Visibility vidljivo;

        public Merenje()
        {
            // skaliranje pozicija na kanvasu
            if (izmereno >= 0 && izmereno <= 13) Pozicija = 340;
            if (izmereno >= 14 && izmereno <= 32) Pozicija = 300;
            if (izmereno >= 33 && izmereno <= 52) Pozicija = 260;
            if (izmereno >= 53 && izmereno <= 72) Pozicija = 220;
            if (izmereno >= 73 && izmereno <= 92) Pozicija = 180;
            if (izmereno >= 93 && izmereno <= 100) Pozicija = 140;

            // ako je merenje 0, ni ni treba da bude vidljiva linija
            if (izmereno == 0)
            {
                Vidljivo = Visibility.Hidden;
            }
            else
            {
                Vidljivo = Visibility.Visible;
            }

            // obavesti UI jer je doslo do promene
            OnPropertyChanged("Pozicija");
        }

        public int Izmereno
        {
            get
            {
                return izmereno;
            }
            set
            {
                if (izmereno != value)
                {
                    izmereno = value;
                    Duplirano = izmereno * 2;
                    OnPropertyChanged("Duplirano");
                    OnPropertyChanged("Izmereno");

                    // koja je y1 kordinata tacke

                    // skaliranje pozicija na kanvasu
                    if (izmereno >= 0 && izmereno <= 13) Pozicija = 340;
                    if (izmereno >= 14 && izmereno <= 32) Pozicija = 300;
                    if (izmereno >= 33 && izmereno <= 52) Pozicija = 260;
                    if (izmereno >= 53 && izmereno <= 72) Pozicija = 220;
                    if (izmereno >= 73 && izmereno <= 92) Pozicija = 180;
                    if (izmereno >= 93 && izmereno <= 100) Pozicija = 140;

                    // ako je merenje 0, ni ni treba da bude vidljiva linija
                    if(izmereno == 0)
                    {
                        Vidljivo = Visibility.Hidden;
                    }
                    else
                    {
                        Vidljivo = Visibility.Visible;
                    }

                    // obavesti UI jer je doslo do promene
                    OnPropertyChanged("Pozicija");
                }

                if (izmereno < 45 || izmereno > 75)
                {
                    vanOpsega = true;
                    OnPropertyChanged("VanOpsega");
                }
                else
                {
                    vanOpsega = false;
                    OnPropertyChanged("VanOpsega");
                }
            }
        }

        public int Pozicija
        {
            get { return pozicija; }
            set
            {
                if(value != pozicija)
                {
                    pozicija = value;
                    OnPropertyChanged("Pozicija");
                }
            }
        }

        public Visibility Vidljivo
        {
            get { return vidljivo; }
            set
            {
                if(vidljivo != value)
                {
                    vidljivo = value;
                    OnPropertyChanged("Vidljivo");
                }
            }
        }
        public int Duplirano
        {
            get
            { return duplirano;}
            set
            { if (duplirano != value)
                {
                    duplirano = value;
                    OnPropertyChanged("Duplirano");
                } }
        }

        public bool VanOpsega
        {
            get
            {
                return vanOpsega;
            }

            set
            {
                if (vanOpsega != value)
                {
                    vanOpsega = value;
                    OnPropertyChanged("VanOpsega");
                }
            }
        }
    }
}
