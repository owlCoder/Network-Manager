using NetworkService.Helpers;
using NetworkService.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace NetworkService.ViewModel
{
    public class RasporedMrezeViewModel : BindableBase
    {
        public static ObservableCollection<Entitet> Entiteti { get; set; }

        public BindingList<KlasifikovaniEntiteti> Klasifikovani { get; set; }

        public RasporedMrezeViewModel()
        {
            Entiteti = MainWindowViewModel.Entiteti;

            Preraspodela();
        }

        #region PROPERTY KLASE RasporedMrezeViewModel
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
                if (item.Klasa.Equals("A"))
                {
                    klasa_a.ListaEntiteta.Add(item);
                }
                else if (item.Klasa.Equals("B"))
                {
                    klasa_b.ListaEntiteta.Add(item);
                }
                else if (item.Klasa.Equals("C"))
                {
                    klasa_c.ListaEntiteta.Add(item);
                }
                else if (item.Klasa.Equals("D"))
                {
                    klasa_d.ListaEntiteta.Add(item);
                }
                else
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
