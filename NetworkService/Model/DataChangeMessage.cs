using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.Model
{
    public class DataChangeMessage
    {
        public Visibility Visibility_Uspesno { get; set; }
        public Visibility Visibility_Greska { get; set; }
        public string Poruka { get; set; }

        public DataChangeMessage() 
        {
            // Prazan konstruktor
        }
    }
}
