using System.ComponentModel;

namespace NetworkService.Model
{
    public class KlasifikovaniEntiteti
    {
        public BindingList<Entitet> ListaEntiteta { get; set; }

        public string AdresnaKlasa { get; set; }

        public KlasifikovaniEntiteti()
        {
            ListaEntiteta = new BindingList<Entitet>();
        }
    }
}
