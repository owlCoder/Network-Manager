using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class RasporedMrezeViewModel : BindableBase
    {
        public static BindingList<KlasifikovaniEntiteti> EntitetiCanvas { get; set; }
        public static BindingList<KlasifikovaniEntiteti> EntitetiTreeView { get; set; }

        #region KOMANDE ZA DRAG & DROP
        // komande za drag & drop
        public MyICommand<Canvas> DragOverKomanda { get; private set; }
        public MyICommand<Canvas> DropKomanda { get; private set; }
        public MyICommand MouseLevoDugme { get; private set; }
        public MyICommand<TreeView> TreeViewOdabran { get; private set; }
        public MyICommand<Canvas> OslobodiKomanda { get; private set; }
        public MyICommand<Grid> NasumicnoRasporedi { get; private set; }
        #endregion

        #region KOMANDE ZA D&D SA CANVASA NA CANVAS
        public MyICommand<Canvas> PreviewMouseUpKomanda { get; private set; }
        public MyICommand<Canvas> PreviewMouseMoveKomanda { get; private set; }
        public MyICommand<Canvas> PreviewMouseDownKomanda { get; private set; }
        #endregion

        // za drag & drop
        private Entitet draggedItem = null;
        private bool dragging = false;
        private int selected;
        private Canvas pocetni = null;

        private Visibility uspesno, greska, informacija;
        private string poruka;

        public RasporedMrezeViewModel()
        {
            NasumicnoRasporedi = new MyICommand<Grid>(Rasporedi);

            // komande
            DragOverKomanda = new MyICommand<Canvas>(DragOverMetoda);
            DropKomanda = new MyICommand<Canvas>(DropMetoda);
            MouseLevoDugme = new MyICommand(TreeView_MouseLeftButtonUp);
            TreeViewOdabran = new MyICommand<TreeView>(Promena_SelectedItemChanged);
            OslobodiKomanda = new MyICommand<Canvas>(Oslobodi_Dugme);

            // komande za d&d
            PreviewMouseUpKomanda = new MyICommand<Canvas>(PreviewMouseUp);
            PreviewMouseMoveKomanda = new MyICommand<Canvas>(PreviewMouseMove);
            PreviewMouseDownKomanda = new MyICommand<Canvas>(PreviewMouseDown);

            Uspesno = Greska = Visibility.Hidden;
            Informacija = Visibility.Visible;
            Poruka = "ℹ Dobrodošli, @Dispečer 3244! Možete započeti sa Vašim radom u aplikaciji za upravljanje serversko-mrežnim entitetima.";

            // liste za entitete za tree view i canvas
            InicijalizacijaListi();

            // za prijem novih entiteta
            Messenger.Default.Register<PassForwardDummy>(this, DodajUTreeViewListu);
            
            // za uklanjanje entiteta ako se ukloni iz liste svih
            Messenger.Default.Register<PassDeleteDummy>(this, UkloniElementCanvasTreeView);

            Messenger.Default.Register<DataChangeMessage>(this, Notifikacija);
        }

        private void Notifikacija(DataChangeMessage message)
        {
            Greska = message.Visibility_Greska;
            Uspesno = message.Visibility_Uspesno;
            Poruka = message.Poruka;
        }

        // Kako bi se pamtilo stanje na canvasu - potrebno je koristiti konstruktor sa parametrima
        public RasporedMrezeViewModel(Grid desniGridCanvas)
        {
            #region PODESAVANJE KOMANDI
            NasumicnoRasporedi = new MyICommand<Grid>(Rasporedi);

            // komande
            DragOverKomanda = new MyICommand<Canvas>(DragOverMetoda);
            DropKomanda = new MyICommand<Canvas>(DropMetoda);
            MouseLevoDugme = new MyICommand(TreeView_MouseLeftButtonUp);
            TreeViewOdabran = new MyICommand<TreeView>(Promena_SelectedItemChanged);
            OslobodiKomanda = new MyICommand<Canvas>(Oslobodi_Dugme);

            // komande za d&d
            PreviewMouseUpKomanda = new MyICommand<Canvas>(PreviewMouseUp);
            PreviewMouseMoveKomanda = new MyICommand<Canvas>(PreviewMouseMove);
            PreviewMouseDownKomanda = new MyICommand<Canvas>(PreviewMouseDown);
            #endregion
            
            Uspesno = Greska = Visibility.Hidden;
            Informacija = Visibility.Visible;
            Poruka = "ℹ Dobrodošli, @Dispečer 3244! Možete započeti sa Vašim radom u aplikaciji.";

            // restauracija canvasa
            // indeksiranje
            // dock paneli krecu od indeksa 1
            // indeks 1 u dock panelu je canvas
            List<Canvas> kanvasi = new List<Canvas>();

            for(int i = 1; i < 13; i++)
            {
                DockPanel panel = (DockPanel)(desniGridCanvas.Children[i]);
                Canvas canvas = (Canvas)(panel.Children[1]);
                kanvasi.Add(canvas);
            }

            foreach(KlasifikovaniEntiteti ke in EntitetiCanvas.ToList())
            {
                foreach(Entitet e in ke.ListaEntiteta.ToList())
                {
                    if(e.Canvas_pozicija != -1)
                    {
                        draggedItem = MainWindowViewModel.Entiteti.FirstOrDefault(p => p.Id == e.Id);
                        Canvas kanvas = kanvasi[e.Canvas_pozicija - 1];

                        TextBlock ispis = ((TextBlock)(kanvas).Children[0]);

                        if (draggedItem != null)
                        {
                            if (kanvas.Resources["taken"] == null)
                            {
                                BitmapImage img = new BitmapImage();
                                img.BeginInit();
                                string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                                img.UriSource = new Uri(putanja, UriKind.Absolute);
                                img.EndInit();
                                kanvas.Background = new ImageBrush(img);
                                ispis.Text = draggedItem.Naziv;
                                ispis.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                                draggedItem.Canvas_pozicija = GetCanvasId(kanvas.Name);
                                kanvas.Resources.Add("taken", true);
                            }
                            draggedItem = null;
                            dragging = false;
                        }
                    }
                }
            }

        }

        private void PreviewMouseDown(Canvas canvas)
        {
            // prvo pronadjemo koji je to element na canvasu
            string naziv_entiteta = ((TextBlock)canvas.Children[0]).Text;
            Entitet ent = null;

            foreach(KlasifikovaniEntiteti ke in EntitetiCanvas)
            {
                foreach(Entitet e in ke.ListaEntiteta)
                {
                    if(e.Naziv.Equals(naziv_entiteta))
                    {
                        ent = e;
                        goto Izlaz;
                    }
                }
            }

        Izlaz:
            draggedItem = ent;
            pocetni = canvas;
        }

        private void PreviewMouseMove(Canvas canvas)
        {
            if (draggedItem == null)
                return;
        }

        private void PreviewMouseUp(Canvas canvas)
        {
            // Samo ako imamo pocetni canvas i odabrani element i canvas na koji prebacujemo nije vec zauzet
            if (draggedItem != null && pocetni != null && canvas.Resources["taken"] == null)
            {
                // prebaci na novi canvas
                TextBlock ispis = ((TextBlock)(canvas).Children[0]);

                if (draggedItem != null)
                {
                    if (canvas.Resources["taken"] == null)
                    {
                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                        img.UriSource = new Uri(putanja, UriKind.Absolute);
                        img.EndInit();
                        canvas.Background = new ImageBrush(img);
                        ispis.Text = draggedItem.Naziv;
                        ispis.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                        draggedItem.Canvas_pozicija = GetCanvasId(canvas.Name);
                        canvas.Resources.Add("taken", true);
                    }
                    draggedItem = null;
                    dragging = false;
                }

                // oslobodi pocetni canvas
                if (pocetni.Resources["taken"] != null)
                {
                    pocetni.Background = Brushes.White;
                    ((TextBlock)pocetni.Children[0]).Text = string.Empty;
                    pocetni.Resources.Remove("taken");
                }

                draggedItem = null;
                pocetni = null;
            }
        }

        private void UkloniElementCanvasTreeView(PassDeleteDummy pd)
        {
            Entitet za_brisanje = pd.Entitet;
            int klasa = 0;

            if (za_brisanje.Klasa.Equals("A")) klasa = 0;
            if (za_brisanje.Klasa.Equals("B")) klasa = 1;
            if (za_brisanje.Klasa.Equals("C")) klasa = 2;
            if (za_brisanje.Klasa.Equals("D")) klasa = 3;
            if (za_brisanje.Klasa.Equals("E")) klasa = 4;

            // ako se element nalazi na canvasu, ukloniti ga iz liste i sa canvasa
            if (EntitetiCanvas[klasa].ListaEntiteta.Contains(za_brisanje))
            {
                EntitetiCanvas[klasa].ListaEntiteta.Remove(za_brisanje);

                // TO DO - UKLONITI SA CANVASA


                // Ponisti poziciju na canvasu


                // Ukloniti sve linije sa canvasa
                // vrv lista.clear ili tako nesto
            }

            // ako se nalazi u tree view - ukloniti ga
            if (EntitetiTreeView[klasa].ListaEntiteta.Contains(za_brisanje))
            {
                EntitetiTreeView[klasa].ListaEntiteta.Remove(za_brisanje);
            }
        }

        private void DodajUTreeViewListu(PassForwardDummy pf)
        {
            Entitet novi = pf.Entitet;
            int klasa = 0;

            if (novi.Klasa.Equals("A")) klasa = 0;
            if (novi.Klasa.Equals("B")) klasa = 1;
            if (novi.Klasa.Equals("C")) klasa = 2;
            if (novi.Klasa.Equals("D")) klasa = 3;
            if (novi.Klasa.Equals("E")) klasa = 4;

            EntitetiTreeView[klasa].ListaEntiteta.Add(novi);
        }

        private void Oslobodi_Dugme(Canvas kanvasRoditelj)
        {
            if (kanvasRoditelj.Resources["taken"] != null)
            {
                VratiElement(kanvasRoditelj);

                kanvasRoditelj.Background = Brushes.White;
                ((TextBlock)kanvasRoditelj.Children[0]).Text = string.Empty;
                kanvasRoditelj.Resources.Remove("taken");
            }
        }

        private void VratiElement(Canvas kanvasRoditelj)
        {
            string naziv_entiteta = ((TextBlock)kanvasRoditelj.Children[0]).Text;

            Entitet item = null;
            KlasifikovaniEntiteti klasa_u_kojoj_se_nalazi = null;
            int brojac_klase = 0;

            // prolazimo kroz sve adresne klase
            foreach(KlasifikovaniEntiteti ke in EntitetiCanvas)
            {
                // i trazimo u listi entiteta odredjene klase onaj entitet koji je na canvasu
                foreach(Entitet e in ke.ListaEntiteta)
                {
                    if(e.Naziv.Equals(naziv_entiteta))
                    {
                        // pronasli smo entitet, zapamti
                        klasa_u_kojoj_se_nalazi = ke;
                        item = e;
                        goto Izlaz;
                    }
                }

                brojac_klase += 1; // prelazimo u sledecu adresnu klasu (A = 0, B = 1, ..., E = 4)
            }

        Izlaz:
            if (item == null || klasa_u_kojoj_se_nalazi == null || brojac_klase > 4)
            {
                return;
            }

            // u pronadjenoj klasi, za pronadjeni entitet - ukloniti referencu
            item.Canvas_pozicija = -1;

            // TO DO: ponisti i sve koordinate!

            klasa_u_kojoj_se_nalazi.ListaEntiteta.Remove(item);

            // dodajemo u tree view u odredjenu klasu adresa kojoj entitet i pripada
            EntitetiTreeView[brojac_klase].ListaEntiteta.Add(item);

            // ukloniti sve linije kao i sve linije sa kojima je entitet povezan na canvasu
            // TO DO
        }

        private void DropMetoda(Canvas kanvas)
        {
            TextBlock ispis = ((TextBlock)(kanvas).Children[0]);

            if (draggedItem != null)
            {
                if (kanvas.Resources["taken"] == null)
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                    img.UriSource = new Uri(putanja, UriKind.Absolute);
                    img.EndInit();
                    kanvas.Background = new ImageBrush(img);
                    ispis.Text = draggedItem.Naziv;
                    ispis.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                    draggedItem.Canvas_pozicija = GetCanvasId(kanvas.Name);
                    kanvas.Resources.Add("taken", true);
                    UkloniElement(draggedItem);
                }
                draggedItem = null;
                dragging = false;
            }
        }

        private int GetCanvasId(string name)
        {
            int id = 1;

            if (name.Equals("c1")) id = 1;
            if (name.Equals("c2")) id = 2;
            if (name.Equals("c3")) id = 3;
            if (name.Equals("c4")) id = 4;
            if (name.Equals("c5")) id = 5;
            if (name.Equals("c6")) id = 6;
            if (name.Equals("c7")) id = 7;
            if (name.Equals("c8")) id = 8;
            if (name.Equals("c9")) id = 9;
            if (name.Equals("c10")) id = 10;
            if (name.Equals("c11")) id = 11;
            if (name.Equals("c12")) id = 12;

            return id;
        }

        private void Promena_SelectedItemChanged(TreeView tv)
        {
            var prozor = RasporedMrezeView.UserControl;

            if (!dragging && tv != null && tv.SelectedItem != null && tv.SelectedItem.GetType() == typeof(Entitet))
            {
                dragging = true;
                draggedItem = (Entitet)tv.SelectedItem;
                selected = PronadjiElement(draggedItem);
                DragDrop.DoDragDrop(prozor, draggedItem, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }


        private void TreeView_MouseLeftButtonUp()
        {
            dragging = false;
            draggedItem = null;
        }

        private int PronadjiElement(Entitet draggedItem)
        {
            int index = 0;
            if (draggedItem.Klasa.Equals("A"))
            {
                index = EntitetiTreeView[0].ListaEntiteta.IndexOf(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("B"))
            {
                index = EntitetiTreeView[1].ListaEntiteta.IndexOf(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("C"))
            {
                index = EntitetiTreeView[2].ListaEntiteta.IndexOf(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("D"))
            {
                index = EntitetiTreeView[3].ListaEntiteta.IndexOf(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("E"))
            {
                index = EntitetiTreeView[4].ListaEntiteta.IndexOf(draggedItem);
            }

            return index;
        }

        private void UkloniElement(Entitet draggedItem)
        {
            if (draggedItem.Klasa.Equals("A"))
            {
                EntitetiTreeView[0].ListaEntiteta.RemoveAt(selected);
                EntitetiCanvas[0].ListaEntiteta.Add(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("B"))
            {
                EntitetiTreeView[1].ListaEntiteta.RemoveAt(selected);
                EntitetiCanvas[1].ListaEntiteta.Add(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("C"))
            {
                EntitetiTreeView[2].ListaEntiteta.RemoveAt(selected);
                EntitetiCanvas[2].ListaEntiteta.Add(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("D"))
            {
                EntitetiTreeView[3].ListaEntiteta.RemoveAt(selected);
                EntitetiCanvas[3].ListaEntiteta.Add(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("E"))
            {
                EntitetiTreeView[4].ListaEntiteta.RemoveAt(selected);
                EntitetiCanvas[4].ListaEntiteta.Add(draggedItem);
            }
        }

        private void DragOverMetoda(Canvas kanvas)
        {
            // TO DO
        }

        #region PROPERTY KLASE RasporedMrezeViewModel
        private void Rasporedi(Grid desni_grid_canvas)
        {
            // Rasporedi na preostala slobodna mesta
            for (int i = 1; i <= 12; i++)
            {
                // uzmemo canvas
                Canvas kanvas = ((Canvas)((DockPanel)(desni_grid_canvas.Children[i])).Children[1]);
                TextBlock trenutni = (TextBlock)((kanvas).Children[0]);
                string naziv_entiteta = trenutni.Text.Trim();

                if (naziv_entiteta.Equals(""))
                {
                    // prazan je canvas
                    if (EntitetiTreeView[0].ListaEntiteta.Count > 0)
                    {
                        draggedItem = EntitetiTreeView[0].ListaEntiteta[0];
                        EntitetiTreeView[0].ListaEntiteta.RemoveAt(0);
                        EntitetiCanvas[0].ListaEntiteta.Add(draggedItem);
                    }
                    else if (EntitetiTreeView[1].ListaEntiteta.Count > 0)
                    {
                        draggedItem = EntitetiTreeView[1].ListaEntiteta[0];
                        EntitetiTreeView[1].ListaEntiteta.RemoveAt(0);
                        EntitetiCanvas[1].ListaEntiteta.Add(draggedItem);
                    }
                    else if (EntitetiTreeView[2].ListaEntiteta.Count > 0)
                    {
                        draggedItem = EntitetiTreeView[2].ListaEntiteta[0];
                        EntitetiTreeView[2].ListaEntiteta.RemoveAt(0);
                        EntitetiCanvas[2].ListaEntiteta.Add(draggedItem);
                    }
                    else if (EntitetiTreeView[3].ListaEntiteta.Count > 0)
                    {
                        draggedItem = EntitetiTreeView[3].ListaEntiteta[0];
                        EntitetiTreeView[3].ListaEntiteta.RemoveAt(0);
                        EntitetiCanvas[3].ListaEntiteta.Add(draggedItem);
                    }
                    else if (EntitetiTreeView[4].ListaEntiteta.Count > 0)
                    {
                        draggedItem = EntitetiTreeView[4].ListaEntiteta[0];
                        EntitetiTreeView[4].ListaEntiteta.RemoveAt(0);
                        EntitetiCanvas[4].ListaEntiteta.Add(draggedItem);
                    }

                    if (draggedItem != null)
                    {
                        draggedItem.Canvas_pozicija = i; // pozicija na canvasu

                        if (kanvas.Resources["taken"] == null)
                        {
                            BitmapImage img = new BitmapImage();
                            img.BeginInit();
                            string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                            img.UriSource = new Uri(putanja, UriKind.Absolute);
                            img.EndInit();
                            kanvas.Background = new ImageBrush(img);
                            trenutni.Text = draggedItem.Naziv;
                            trenutni.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                            draggedItem.Canvas_pozicija = GetCanvasId(kanvas.Name);
                            kanvas.Resources.Add("taken", true);
                            //UkloniElement(draggedItem);
                        }
                        draggedItem = null;
                        dragging = false;
                    }
                }
            }
        }
        #endregion

        #region METODA INICIJALIZACIJE
        public void InicijalizacijaListi()
        {
            EntitetiCanvas = new BindingList<KlasifikovaniEntiteti>()
            {
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa A" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa B" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa C" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa D" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa E" }
            };

            EntitetiTreeView = new BindingList<KlasifikovaniEntiteti>()
            {
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa A" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa B" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa C" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa D" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "Adresna Klasa E" }
            };
        }
        #endregion

        #region PROPERTY ZA PORUKE
        public string Poruka
        {
            get
            {
                return poruka;
            }

            set
            {
                if (poruka != value)
                {
                    poruka = value;
                    OnPropertyChanged("Poruka");
                }
            }
        }
        public Visibility Uspesno
        {
            get
            {
                return uspesno;
            }

            set
            {
                if (uspesno != value)
                {
                    uspesno = value;

                    if (uspesno == Visibility.Visible)
                    {
                        Greska = Informacija = Visibility.Hidden;
                        OnPropertyChanged("Greska");
                        OnPropertyChanged("Informacija");
                    }

                    OnPropertyChanged("Uspesno");
                }
            }
        }

        public Visibility Greska
        {
            get
            {
                return greska;
            }

            set
            {
                if (greska != value)
                {
                    greska = value;

                    if (greska == Visibility.Visible)
                    {
                        uspesno = informacija = Visibility.Hidden;
                        OnPropertyChanged("Uspesno");
                        OnPropertyChanged("Informacija");
                    }

                    OnPropertyChanged("Greska");
                }
            }
        }

        public Visibility Informacija
        {
            get
            {
                return informacija;
            }

            set
            {
                if (informacija != value)
                {
                    informacija = value;

                    if (informacija == Visibility.Visible)
                    {
                        greska = uspesno = Visibility.Hidden;
                        OnPropertyChanged("Greska");
                        OnPropertyChanged("Uspesno");
                    }

                    OnPropertyChanged("Informacija");
                }
            }
        }
        #endregion
    }
}
