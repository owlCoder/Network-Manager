using NetworkService.Helpers;
using NetworkService.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NetworkService.ViewModel
{
    public class RasporedMrezeViewModel : BindableBase
    {
        public MyICommand NasumicnoRasporedi { get; private set; }
        public static ObservableCollection<Entitet> Entiteti { get; set; }

        public BindingList<KlasifikovaniEntiteti> Klasifikovani { get; set; }

        // komande za drag & drop
        // to do treba ih inicijalizovati!
        public MyICommand DragOverKomanda { get; private set; }
        public MyICommand DropKomanda { get; private set; }
        public MyICommand MouseLevoDugme { get; private set; }
        public MyICommand SelectedItemPromena { get; private set; }
        public MyICommand OslobodiKomanda { get; private set; }

        public RasporedMrezeViewModel()
        {
            Entiteti = MainWindowViewModel.Entiteti;
            NasumicnoRasporedi = new MyICommand(Rasporedi);
            Preraspodela();
        }

        #region PROPERTY KLASE RasporedMrezeViewModel
        private void Rasporedi()
        {

        }
        #endregion

        #region METODA PRERASPODELE
        public void Preraspodela()
        {
            Klasifikovani = new BindingList<KlasifikovaniEntiteti>();
            Klasifikovani.Clear();

            KlasifikovaniEntiteti klasa_a = new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa A" };
            KlasifikovaniEntiteti klasa_b = new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa B" };
            KlasifikovaniEntiteti klasa_c = new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa C" };
            KlasifikovaniEntiteti klasa_d = new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa D" };
            KlasifikovaniEntiteti klasa_e = new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa E" };

            foreach (var item in Entiteti)
            {
                if (item.Klasa.Equals("A") && item.Canvas_pozicija == -1) // dodajemo samo entitete koji nisu vec na canvasu
                {
                    klasa_a.ListaEntiteta.Add(item);
                }
                else if (item.Klasa.Equals("B") && item.Canvas_pozicija == -1) // dodajemo samo entitete koji nisu vec na canvasu
                {
                    klasa_b.ListaEntiteta.Add(item);
                }
                else if (item.Klasa.Equals("C") && item.Canvas_pozicija == -1) // dodajemo samo entitete koji nisu vec na canvasu
                {
                    klasa_c.ListaEntiteta.Add(item);
                }
                else if (item.Klasa.Equals("D") && item.Canvas_pozicija == -1) // dodajemo samo entitete koji nisu vec na canvasu
                {
                    klasa_d.ListaEntiteta.Add(item);
                }
                else if (item.Klasa.Equals("E") && item.Canvas_pozicija == -1) // dodajemo samo entitete koji nisu vec na canvasu
                {
                    klasa_e.ListaEntiteta.Add(item);
                }
            }

            Klasifikovani.Add(klasa_a);
            Klasifikovani.Add(klasa_b);
            Klasifikovani.Add(klasa_c);
            Klasifikovani.Add(klasa_d);
            Klasifikovani.Add(klasa_e);
        }
        #endregion
    }
}
