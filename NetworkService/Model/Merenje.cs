﻿using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Merenje : BindableBase
    {
        #region POLJA KLASE Merenje
        private int izmereno;

        private bool vanOpsega;
        #endregion

        public Merenje() 
        { 
            // Prazan konstruktor
        }

        #region PROPERTY KLASE Merenje
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
                    OnPropertyChanged("Izmereno");
                }

                if(izmereno < 45 || izmereno > 75)
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

        public bool VanOpsega
        {
            get
            {
                return vanOpsega;
            }

            set
            {
                if(vanOpsega != value)
                {
                    vanOpsega = value;
                    OnPropertyChanged("VanOpsega");
                }
            }
        }
        #endregion
    }
}
