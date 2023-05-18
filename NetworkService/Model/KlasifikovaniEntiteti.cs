using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
