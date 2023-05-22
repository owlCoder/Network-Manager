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

        #region LINIJA ZA POVEZIVANJE NA CANVASU
        private static Visibility c1e1, c1e2, c1e3, c1e4, c1e5, c1e6, c1e7, c1e8, c1e9, c1e10, c1e11, c1e12;
        private static Visibility c2e1, c2e2, c2e3, c2e4, c2e5, c2e6, c2e7, c2e8, c2e9, c2e10, c2e11, c2e12;
        private static Visibility c3e1, c3e2, c3e3, c3e4, c3e5, c3e6, c3e7, c3e8, c3e9, c3e10, c3e11, c3e12;
        private static Visibility c4e1, c4e2, c4e3, c4e4, c4e5, c4e6, c4e7, c4e8, c4e9, c4e10, c4e11, c4e12;
        private static Visibility c5e1, c5e2, c5e3, c5e4, c5e5, c5e6, c5e7, c5e8, c5e9, c5e10, c5e11, c5e12;
        private static Visibility c6e1, c6e2, c6e3, c6e4, c6e5, c6e6, c6e7, c6e8, c6e9, c6e10, c6e11, c6e12;
        private static Visibility c7e1, c7e2, c7e3, c7e4, c7e5, c7e6, c7e7, c7e8, c7e9, c7e10, c7e11, c7e12;
        private static Visibility c8e1, c8e2, c8e3, c8e4, c8e5, c8e6, c8e7, c8e8, c8e9, c8e10, c8e11, c8e12;
        private static Visibility c9e1, c9e2, c9e3, c9e4, c9e5, c9e6, c9e7, c9e8, c9e9, c9e10, c9e11, c9e12;
        private static Visibility c10e1, c10e2, c10e3, c10e4, c10e5, c10e6, c10e7, c10e8, c10e9, c10e10, c10e11, c10e12;
        private static Visibility c11e1, c11e2, c11e3, c11e4, c11e5, c11e6, c11e7, c11e8, c11e9, c11e10, c11e11, c11e12;
        private static Visibility c12e1, c12e2, c12e3, c12e4, c12e5, c12e6, c12e7, c12e8, c12e9, c12e10, c12e11, c12e12;

        private Canvas src = null;
        private Canvas dst = null;

        #endregion
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

        #region ZA LINIJE
        public Visibility C1e1
        {
            get
            {
                return c1e1;
            }

            set
            {
                if (c1e1 != value)
                {
                    c1e1 = value;
                    OnPropertyChanged("C1e1");
                }

            }
        }

        public Visibility C1e2
        {
            get
            {
                return c1e2;
            }

            set
            {
                if (c1e2 != value)
                {
                    c1e2 = value;
                    OnPropertyChanged("C1e2");
                }

            }
        }

        public Visibility C1e3
        {
            get
            {
                return c1e3;
            }

            set
            {
                if (c1e3 != value)
                {
                    c1e3 = value;
                    OnPropertyChanged("C1e3");
                }

            }
        }

        public Visibility C1e4
        {
            get
            {
                return c1e4;
            }

            set
            {
                if (c1e4 != value)
                {
                    c1e4 = value;
                    OnPropertyChanged("C1e4");
                }

            }
        }

        public Visibility C1e5
        {
            get
            {
                return c1e5;
            }

            set
            {
                if (c1e5 != value)
                {
                    c1e5 = value;
                    OnPropertyChanged("C1e5");
                }

            }
        }

        public Visibility C1e6
        {
            get
            {
                return c1e6;
            }

            set
            {
                if (c1e6 != value)
                {
                    c1e6 = value;
                    OnPropertyChanged("C1e6");
                }

            }
        }

        public Visibility C1e7
        {
            get
            {
                return c1e7;
            }

            set
            {
                if (c1e7 != value)
                {
                    c1e7 = value;
                    OnPropertyChanged("C1e7");
                }

            }
        }

        public Visibility C1e8
        {
            get
            {
                return c1e8;
            }

            set
            {
                if (c1e8 != value)
                {
                    c1e8 = value;
                    OnPropertyChanged("C1e8");
                }

            }
        }

        public Visibility C1e9
        {
            get
            {
                return c1e9;
            }

            set
            {
                if (c1e9 != value)
                {
                    c1e9 = value;
                    OnPropertyChanged("C1e9");
                }

            }
        }

        public Visibility C1e10
        {
            get
            {
                return c1e10;
            }

            set
            {
                if (c1e10 != value)
                {
                    c1e10 = value;
                    OnPropertyChanged("C1e10");
                }

            }
        }

        public Visibility C1e11
        {
            get
            {
                return c1e11;
            }

            set
            {
                if (c1e11 != value)
                {
                    c1e11 = value;
                    OnPropertyChanged("C1e11");
                }

            }
        }

        public Visibility C1e12
        {
            get
            {
                return c1e12;
            }

            set
            {
                if (c1e12 != value)
                {
                    c1e12 = value;
                    OnPropertyChanged("C1e12");
                }

            }
        }

        public Visibility C2e1
        {
            get
            {
                return c2e1;
            }

            set
            {
                if (c2e1 != value)
                {
                    c2e1 = value;
                    OnPropertyChanged("C2e1");
                }

            }
        }

        public Visibility C2e2
        {
            get
            {
                return c2e2;
            }

            set
            {
                if (c2e2 != value)
                {
                    c2e2 = value;
                    OnPropertyChanged("C2e2");
                }

            }
        }

        public Visibility C2e3
        {
            get
            {
                return c2e3;
            }

            set
            {
                if (c2e3 != value)
                {
                    c2e3 = value;
                    OnPropertyChanged("C2e3");
                }

            }
        }

        public Visibility C2e4
        {
            get
            {
                return c2e4;
            }

            set
            {
                if (c2e4 != value)
                {
                    c2e4 = value;
                    OnPropertyChanged("C2e4");
                }

            }
        }

        public Visibility C2e5
        {
            get
            {
                return c2e5;
            }

            set
            {
                if (c2e5 != value)
                {
                    c2e5 = value;
                    OnPropertyChanged("C2e5");
                }

            }
        }

        public Visibility C2e6
        {
            get
            {
                return c2e6;
            }

            set
            {
                if (c2e6 != value)
                {
                    c2e6 = value;
                    OnPropertyChanged("C2e6");
                }

            }
        }

        public Visibility C2e7
        {
            get
            {
                return c2e7;
            }

            set
            {
                if (c2e7 != value)
                {
                    c2e7 = value;
                    OnPropertyChanged("C2e7");
                }

            }
        }

        public Visibility C2e8
        {
            get
            {
                return c2e8;
            }

            set
            {
                if (c2e8 != value)
                {
                    c2e8 = value;
                    OnPropertyChanged("C2e8");
                }

            }
        }

        public Visibility C2e9
        {
            get
            {
                return c2e9;
            }

            set
            {
                if (c2e9 != value)
                {
                    c2e9 = value;
                    OnPropertyChanged("C2e9");
                }

            }
        }

        public Visibility C2e10
        {
            get
            {
                return c2e10;
            }

            set
            {
                if (c2e10 != value)
                {
                    c2e10 = value;
                    OnPropertyChanged("C2e10");
                }

            }
        }

        public Visibility C2e11
        {
            get
            {
                return c2e11;
            }

            set
            {
                if (c2e11 != value)
                {
                    c2e11 = value;
                    OnPropertyChanged("C2e11");
                }

            }
        }

        public Visibility C2e12
        {
            get
            {
                return c2e12;
            }

            set
            {
                if (c2e12 != value)
                {
                    c2e12 = value;
                    OnPropertyChanged("C2e12");
                }

            }
        }

        public Visibility C3e1
        {
            get
            {
                return c3e1;
            }

            set
            {
                if (c3e1 != value)
                {
                    c3e1 = value;
                    OnPropertyChanged("C3e1");
                }

            }
        }

        public Visibility C3e2
        {
            get
            {
                return c3e2;
            }

            set
            {
                if (c3e2 != value)
                {
                    c3e2 = value;
                    OnPropertyChanged("C3e2");
                }

            }
        }

        public Visibility C3e3
        {
            get
            {
                return c3e3;
            }

            set
            {
                if (c3e3 != value)
                {
                    c3e3 = value;
                    OnPropertyChanged("C3e3");
                }

            }
        }

        public Visibility C3e4
        {
            get
            {
                return c3e4;
            }

            set
            {
                if (c3e4 != value)
                {
                    c3e4 = value;
                    OnPropertyChanged("C3e4");
                }

            }
        }

        public Visibility C3e5
        {
            get
            {
                return c3e5;
            }

            set
            {
                if (c3e5 != value)
                {
                    c3e5 = value;
                    OnPropertyChanged("C3e5");
                }

            }
        }

        public Visibility C3e6
        {
            get
            {
                return c3e6;
            }

            set
            {
                if (c3e6 != value)
                {
                    c3e6 = value;
                    OnPropertyChanged("C3e6");
                }

            }
        }

        public Visibility C3e7
        {
            get
            {
                return c3e7;
            }

            set
            {
                if (c3e7 != value)
                {
                    c3e7 = value;
                    OnPropertyChanged("C3e7");
                }

            }
        }

        public Visibility C3e8
        {
            get
            {
                return c3e8;
            }

            set
            {
                if (c3e8 != value)
                {
                    c3e8 = value;
                    OnPropertyChanged("C3e8");
                }

            }
        }

        public Visibility C3e9
        {
            get
            {
                return c3e9;
            }

            set
            {
                if (c3e9 != value)
                {
                    c3e9 = value;
                    OnPropertyChanged("C3e9");
                }

            }
        }

        public Visibility C3e10
        {
            get
            {
                return c3e10;
            }

            set
            {
                if (c3e10 != value)
                {
                    c3e10 = value;
                    OnPropertyChanged("C3e10");
                }

            }
        }

        public Visibility C3e11
        {
            get
            {
                return c3e11;
            }

            set
            {
                if (c3e11 != value)
                {
                    c3e11 = value;
                    OnPropertyChanged("C3e11");
                }

            }
        }

        public Visibility C3e12
        {
            get
            {
                return c3e12;
            }

            set
            {
                if (c3e12 != value)
                {
                    c3e12 = value;
                    OnPropertyChanged("C3e12");
                }

            }
        }

        public Visibility C4e1
        {
            get
            {
                return c4e1;
            }

            set
            {
                if (c4e1 != value)
                {
                    c4e1 = value;
                    OnPropertyChanged("C4e1");
                }

            }
        }

        public Visibility C4e2
        {
            get
            {
                return c4e2;
            }

            set
            {
                if (c4e2 != value)
                {
                    c4e2 = value;
                    OnPropertyChanged("C4e2");
                }

            }
        }

        public Visibility C4e3
        {
            get
            {
                return c4e3;
            }

            set
            {
                if (c4e3 != value)
                {
                    c4e3 = value;
                    OnPropertyChanged("C4e3");
                }

            }
        }

        public Visibility C4e4
        {
            get
            {
                return c4e4;
            }

            set
            {
                if (c4e4 != value)
                {
                    c4e4 = value;
                    OnPropertyChanged("C4e4");
                }

            }
        }

        public Visibility C4e5
        {
            get
            {
                return c4e5;
            }

            set
            {
                if (c4e5 != value)
                {
                    c4e5 = value;
                    OnPropertyChanged("C4e5");
                }

            }
        }

        public Visibility C4e6
        {
            get
            {
                return c4e6;
            }

            set
            {
                if (c4e6 != value)
                {
                    c4e6 = value;
                    OnPropertyChanged("C4e6");
                }

            }
        }

        public Visibility C4e7
        {
            get
            {
                return c4e7;
            }

            set
            {
                if (c4e7 != value)
                {
                    c4e7 = value;
                    OnPropertyChanged("C4e7");
                }

            }
        }

        public Visibility C4e8
        {
            get
            {
                return c4e8;
            }

            set
            {
                if (c4e8 != value)
                {
                    c4e8 = value;
                    OnPropertyChanged("C4e8");
                }

            }
        }

        public Visibility C4e9
        {
            get
            {
                return c4e9;
            }

            set
            {
                if (c4e9 != value)
                {
                    c4e9 = value;
                    OnPropertyChanged("C4e9");
                }

            }
        }

        public Visibility C4e10
        {
            get
            {
                return c4e10;
            }

            set
            {
                if (c4e10 != value)
                {
                    c4e10 = value;
                    OnPropertyChanged("C4e10");
                }

            }
        }

        public Visibility C4e11
        {
            get
            {
                return c4e11;
            }

            set
            {
                if (c4e11 != value)
                {
                    c4e11 = value;
                    OnPropertyChanged("C4e11");
                }

            }
        }

        public Visibility C4e12
        {
            get
            {
                return c4e12;
            }

            set
            {
                if (c4e12 != value)
                {
                    c4e12 = value;
                    OnPropertyChanged("C4e12");
                }

            }
        }

        public Visibility C5e1
        {
            get
            {
                return c5e1;
            }

            set
            {
                if (c5e1 != value)
                {
                    c5e1 = value;
                    OnPropertyChanged("C5e1");
                }

            }
        }

        public Visibility C5e2
        {
            get
            {
                return c5e2;
            }

            set
            {
                if (c5e2 != value)
                {
                    c5e2 = value;
                    OnPropertyChanged("C5e2");
                }

            }
        }

        public Visibility C5e3
        {
            get
            {
                return c5e3;
            }

            set
            {
                if (c5e3 != value)
                {
                    c5e3 = value;
                    OnPropertyChanged("C5e3");
                }

            }
        }

        public Visibility C5e4
        {
            get
            {
                return c5e4;
            }

            set
            {
                if (c5e4 != value)
                {
                    c5e4 = value;
                    OnPropertyChanged("C5e4");
                }

            }
        }

        public Visibility C5e5
        {
            get
            {
                return c5e5;
            }

            set
            {
                if (c5e5 != value)
                {
                    c5e5 = value;
                    OnPropertyChanged("C5e5");
                }

            }
        }

        public Visibility C5e6
        {
            get
            {
                return c5e6;
            }

            set
            {
                if (c5e6 != value)
                {
                    c5e6 = value;
                    OnPropertyChanged("C5e6");
                }

            }
        }

        public Visibility C5e7
        {
            get
            {
                return c5e7;
            }

            set
            {
                if (c5e7 != value)
                {
                    c5e7 = value;
                    OnPropertyChanged("C5e7");
                }

            }
        }

        public Visibility C5e8
        {
            get
            {
                return c5e8;
            }

            set
            {
                if (c5e8 != value)
                {
                    c5e8 = value;
                    OnPropertyChanged("C5e8");
                }

            }
        }

        public Visibility C5e9
        {
            get
            {
                return c5e9;
            }

            set
            {
                if (c5e9 != value)
                {
                    c5e9 = value;
                    OnPropertyChanged("C5e9");
                }

            }
        }

        public Visibility C5e10
        {
            get
            {
                return c5e10;
            }

            set
            {
                if (c5e10 != value)
                {
                    c5e10 = value;
                    OnPropertyChanged("C5e10");
                }

            }
        }

        public Visibility C5e11
        {
            get
            {
                return c5e11;
            }

            set
            {
                if (c5e11 != value)
                {
                    c5e11 = value;
                    OnPropertyChanged("C5e11");
                }

            }
        }

        public Visibility C5e12
        {
            get
            {
                return c5e12;
            }

            set
            {
                if (c5e12 != value)
                {
                    c5e12 = value;
                    OnPropertyChanged("C5e12");
                }

            }
        }

        public Visibility C6e1
        {
            get
            {
                return c6e1;
            }

            set
            {
                if (c6e1 != value)
                {
                    c6e1 = value;
                    OnPropertyChanged("C6e1");
                }

            }
        }

        public Visibility C6e2
        {
            get
            {
                return c6e2;
            }

            set
            {
                if (c6e2 != value)
                {
                    c6e2 = value;
                    OnPropertyChanged("C6e2");
                }

            }
        }

        public Visibility C6e3
        {
            get
            {
                return c6e3;
            }

            set
            {
                if (c6e3 != value)
                {
                    c6e3 = value;
                    OnPropertyChanged("C6e3");
                }

            }
        }

        public Visibility C6e4
        {
            get
            {
                return c6e4;
            }

            set
            {
                if (c6e4 != value)
                {
                    c6e4 = value;
                    OnPropertyChanged("C6e4");
                }

            }
        }

        public Visibility C6e5
        {
            get
            {
                return c6e5;
            }

            set
            {
                if (c6e5 != value)
                {
                    c6e5 = value;
                    OnPropertyChanged("C6e5");
                }

            }
        }

        public Visibility C6e6
        {
            get
            {
                return c6e6;
            }

            set
            {
                if (c6e6 != value)
                {
                    c6e6 = value;
                    OnPropertyChanged("C6e6");
                }

            }
        }

        public Visibility C6e7
        {
            get
            {
                return c6e7;
            }

            set
            {
                if (c6e7 != value)
                {
                    c6e7 = value;
                    OnPropertyChanged("C6e7");
                }

            }
        }

        public Visibility C6e8
        {
            get
            {
                return c6e8;
            }

            set
            {
                if (c6e8 != value)
                {
                    c6e8 = value;
                    OnPropertyChanged("C6e8");
                }

            }
        }

        public Visibility C6e9
        {
            get
            {
                return c6e9;
            }

            set
            {
                if (c6e9 != value)
                {
                    c6e9 = value;
                    OnPropertyChanged("C6e9");
                }

            }
        }

        public Visibility C6e10
        {
            get
            {
                return c6e10;
            }

            set
            {
                if (c6e10 != value)
                {
                    c6e10 = value;
                    OnPropertyChanged("C6e10");
                }

            }
        }

        public Visibility C6e11
        {
            get
            {
                return c6e11;
            }

            set
            {
                if (c6e11 != value)
                {
                    c6e11 = value;
                    OnPropertyChanged("C6e11");
                }

            }
        }

        public Visibility C6e12
        {
            get
            {
                return c6e12;
            }

            set
            {
                if (c6e12 != value)
                {
                    c6e12 = value;
                    OnPropertyChanged("C6e12");
                }

            }
        }

        public Visibility C7e1
        {
            get
            {
                return c7e1;
            }

            set
            {
                if (c7e1 != value)
                {
                    c7e1 = value;
                    OnPropertyChanged("C7e1");
                }

            }
        }

        public Visibility C7e2
        {
            get
            {
                return c7e2;
            }

            set
            {
                if (c7e2 != value)
                {
                    c7e2 = value;
                    OnPropertyChanged("C7e2");
                }

            }
        }

        public Visibility C7e3
        {
            get
            {
                return c7e3;
            }

            set
            {
                if (c7e3 != value)
                {
                    c7e3 = value;
                    OnPropertyChanged("C7e3");
                }

            }
        }

        public Visibility C7e4
        {
            get
            {
                return c7e4;
            }

            set
            {
                if (c7e4 != value)
                {
                    c7e4 = value;
                    OnPropertyChanged("C7e4");
                }

            }
        }

        public Visibility C7e5
        {
            get
            {
                return c7e5;
            }

            set
            {
                if (c7e5 != value)
                {
                    c7e5 = value;
                    OnPropertyChanged("C7e5");
                }

            }
        }

        public Visibility C7e6
        {
            get
            {
                return c7e6;
            }

            set
            {
                if (c7e6 != value)
                {
                    c7e6 = value;
                    OnPropertyChanged("C7e6");
                }

            }
        }

        public Visibility C7e7
        {
            get
            {
                return c7e7;
            }

            set
            {
                if (c7e7 != value)
                {
                    c7e7 = value;
                    OnPropertyChanged("C7e7");
                }

            }
        }

        public Visibility C7e8
        {
            get
            {
                return c7e8;
            }

            set
            {
                if (c7e8 != value)
                {
                    c7e8 = value;
                    OnPropertyChanged("C7e8");
                }

            }
        }

        public Visibility C7e9
        {
            get
            {
                return c7e9;
            }

            set
            {
                if (c7e9 != value)
                {
                    c7e9 = value;
                    OnPropertyChanged("C7e9");
                }

            }
        }

        public Visibility C7e10
        {
            get
            {
                return c7e10;
            }

            set
            {
                if (c7e10 != value)
                {
                    c7e10 = value;
                    OnPropertyChanged("C7e10");
                }

            }
        }

        public Visibility C7e11
        {
            get
            {
                return c7e11;
            }

            set
            {
                if (c7e11 != value)
                {
                    c7e11 = value;
                    OnPropertyChanged("C7e11");
                }

            }
        }

        public Visibility C7e12
        {
            get
            {
                return c7e12;
            }

            set
            {
                if (c7e12 != value)
                {
                    c7e12 = value;
                    OnPropertyChanged("C7e12");
                }

            }
        }

        public Visibility C8e1
        {
            get
            {
                return c8e1;
            }

            set
            {
                if (c8e1 != value)
                {
                    c8e1 = value;
                    OnPropertyChanged("C8e1");
                }

            }
        }

        public Visibility C8e2
        {
            get
            {
                return c8e2;
            }

            set
            {
                if (c8e2 != value)
                {
                    c8e2 = value;
                    OnPropertyChanged("C8e2");
                }

            }
        }

        public Visibility C8e3
        {
            get
            {
                return c8e3;
            }

            set
            {
                if (c8e3 != value)
                {
                    c8e3 = value;
                    OnPropertyChanged("C8e3");
                }

            }
        }

        public Visibility C8e4
        {
            get
            {
                return c8e4;
            }

            set
            {
                if (c8e4 != value)
                {
                    c8e4 = value;
                    OnPropertyChanged("C8e4");
                }

            }
        }

        public Visibility C8e5
        {
            get
            {
                return c8e5;
            }

            set
            {
                if (c8e5 != value)
                {
                    c8e5 = value;
                    OnPropertyChanged("C8e5");
                }

            }
        }

        public Visibility C8e6
        {
            get
            {
                return c8e6;
            }

            set
            {
                if (c8e6 != value)
                {
                    c8e6 = value;
                    OnPropertyChanged("C8e6");
                }

            }
        }

        public Visibility C8e7
        {
            get
            {
                return c8e7;
            }

            set
            {
                if (c8e7 != value)
                {
                    c8e7 = value;
                    OnPropertyChanged("C8e7");
                }

            }
        }

        public Visibility C8e8
        {
            get
            {
                return c8e8;
            }

            set
            {
                if (c8e8 != value)
                {
                    c8e8 = value;
                    OnPropertyChanged("C8e8");
                }

            }
        }

        public Visibility C8e9
        {
            get
            {
                return c8e9;
            }

            set
            {
                if (c8e9 != value)
                {
                    c8e9 = value;
                    OnPropertyChanged("C8e9");
                }

            }
        }

        public Visibility C8e10
        {
            get
            {
                return c8e10;
            }

            set
            {
                if (c8e10 != value)
                {
                    c8e10 = value;
                    OnPropertyChanged("C8e10");
                }

            }
        }

        public Visibility C8e11
        {
            get
            {
                return c8e11;
            }

            set
            {
                if (c8e11 != value)
                {
                    c8e11 = value;
                    OnPropertyChanged("C8e11");
                }

            }
        }

        public Visibility C8e12
        {
            get
            {
                return c8e12;
            }

            set
            {
                if (c8e12 != value)
                {
                    c8e12 = value;
                    OnPropertyChanged("C8e12");
                }

            }
        }

        public Visibility C9e1
        {
            get
            {
                return c9e1;
            }

            set
            {
                if (c9e1 != value)
                {
                    c9e1 = value;
                    OnPropertyChanged("C9e1");
                }

            }
        }

        public Visibility C9e2
        {
            get
            {
                return c9e2;
            }

            set
            {
                if (c9e2 != value)
                {
                    c9e2 = value;
                    OnPropertyChanged("C9e2");
                }

            }
        }

        public Visibility C9e3
        {
            get
            {
                return c9e3;
            }

            set
            {
                if (c9e3 != value)
                {
                    c9e3 = value;
                    OnPropertyChanged("C9e3");
                }

            }
        }

        public Visibility C9e4
        {
            get
            {
                return c9e4;
            }

            set
            {
                if (c9e4 != value)
                {
                    c9e4 = value;
                    OnPropertyChanged("C9e4");
                }

            }
        }

        public Visibility C9e5
        {
            get
            {
                return c9e5;
            }

            set
            {
                if (c9e5 != value)
                {
                    c9e5 = value;
                    OnPropertyChanged("C9e5");
                }

            }
        }

        public Visibility C9e6
        {
            get
            {
                return c9e6;
            }

            set
            {
                if (c9e6 != value)
                {
                    c9e6 = value;
                    OnPropertyChanged("C9e6");
                }

            }
        }

        public Visibility C9e7
        {
            get
            {
                return c9e7;
            }

            set
            {
                if (c9e7 != value)
                {
                    c9e7 = value;
                    OnPropertyChanged("C9e7");
                }

            }
        }

        public Visibility C9e8
        {
            get
            {
                return c9e8;
            }

            set
            {
                if (c9e8 != value)
                {
                    c9e8 = value;
                    OnPropertyChanged("C9e8");
                }

            }
        }

        public Visibility C9e9
        {
            get
            {
                return c9e9;
            }

            set
            {
                if (c9e9 != value)
                {
                    c9e9 = value;
                    OnPropertyChanged("C9e9");
                }

            }
        }

        public Visibility C9e10
        {
            get
            {
                return c9e10;
            }

            set
            {
                if (c9e10 != value)
                {
                    c9e10 = value;
                    OnPropertyChanged("C9e10");
                }

            }
        }

        public Visibility C9e11
        {
            get
            {
                return c9e11;
            }

            set
            {
                if (c9e11 != value)
                {
                    c9e11 = value;
                    OnPropertyChanged("C9e11");
                }

            }
        }

        public Visibility C9e12
        {
            get
            {
                return c9e12;
            }

            set
            {
                if (c9e12 != value)
                {
                    c9e12 = value;
                    OnPropertyChanged("C9e12");
                }

            }
        }

        public Visibility C10e1
        {
            get
            {
                return c10e1;
            }

            set
            {
                if (c10e1 != value)
                {
                    c10e1 = value;
                    OnPropertyChanged("C10e1");
                }

            }
        }

        public Visibility C10e2
        {
            get
            {
                return c10e2;
            }

            set
            {
                if (c10e2 != value)
                {
                    c10e2 = value;
                    OnPropertyChanged("C10e2");
                }

            }
        }

        public Visibility C10e3
        {
            get
            {
                return c10e3;
            }

            set
            {
                if (c10e3 != value)
                {
                    c10e3 = value;
                    OnPropertyChanged("C10e3");
                }

            }
        }

        public Visibility C10e4
        {
            get
            {
                return c10e4;
            }

            set
            {
                if (c10e4 != value)
                {
                    c10e4 = value;
                    OnPropertyChanged("C10e4");
                }

            }
        }

        public Visibility C10e5
        {
            get
            {
                return c10e5;
            }

            set
            {
                if (c10e5 != value)
                {
                    c10e5 = value;
                    OnPropertyChanged("C10e5");
                }

            }
        }

        public Visibility C10e6
        {
            get
            {
                return c10e6;
            }

            set
            {
                if (c10e6 != value)
                {
                    c10e6 = value;
                    OnPropertyChanged("C10e6");
                }

            }
        }

        public Visibility C10e7
        {
            get
            {
                return c10e7;
            }

            set
            {
                if (c10e7 != value)
                {
                    c10e7 = value;
                    OnPropertyChanged("C10e7");
                }

            }
        }

        public Visibility C10e8
        {
            get
            {
                return c10e8;
            }

            set
            {
                if (c10e8 != value)
                {
                    c10e8 = value;
                    OnPropertyChanged("C10e8");
                }

            }
        }

        public Visibility C10e9
        {
            get
            {
                return c10e9;
            }

            set
            {
                if (c10e9 != value)
                {
                    c10e9 = value;
                    OnPropertyChanged("C10e9");
                }

            }
        }

        public Visibility C10e10
        {
            get
            {
                return c10e10;
            }

            set
            {
                if (c10e10 != value)
                {
                    c10e10 = value;
                    OnPropertyChanged("C10e10");
                }

            }
        }

        public Visibility C10e11
        {
            get
            {
                return c10e11;
            }

            set
            {
                if (c10e11 != value)
                {
                    c10e11 = value;
                    OnPropertyChanged("C10e11");
                }

            }
        }

        public Visibility C10e12
        {
            get
            {
                return c10e12;
            }

            set
            {
                if (c10e12 != value)
                {
                    c10e12 = value;
                    OnPropertyChanged("C10e12");
                }

            }
        }

        public Visibility C11e1
        {
            get
            {
                return c11e1;
            }

            set
            {
                if (c11e1 != value)
                {
                    c11e1 = value;
                    OnPropertyChanged("C11e1");
                }

            }
        }

        public Visibility C11e2
        {
            get
            {
                return c11e2;
            }

            set
            {
                if (c11e2 != value)
                {
                    c11e2 = value;
                    OnPropertyChanged("C11e2");
                }

            }
        }

        public Visibility C11e3
        {
            get
            {
                return c11e3;
            }

            set
            {
                if (c11e3 != value)
                {
                    c11e3 = value;
                    OnPropertyChanged("C11e3");
                }

            }
        }

        public Visibility C11e4
        {
            get
            {
                return c11e4;
            }

            set
            {
                if (c11e4 != value)
                {
                    c11e4 = value;
                    OnPropertyChanged("C11e4");
                }

            }
        }

        public Visibility C11e5
        {
            get
            {
                return c11e5;
            }

            set
            {
                if (c11e5 != value)
                {
                    c11e5 = value;
                    OnPropertyChanged("C11e5");
                }

            }
        }

        public Visibility C11e6
        {
            get
            {
                return c11e6;
            }

            set
            {
                if (c11e6 != value)
                {
                    c11e6 = value;
                    OnPropertyChanged("C11e6");
                }

            }
        }

        public Visibility C11e7
        {
            get
            {
                return c11e7;
            }

            set
            {
                if (c11e7 != value)
                {
                    c11e7 = value;
                    OnPropertyChanged("C11e7");
                }

            }
        }

        public Visibility C11e8
        {
            get
            {
                return c11e8;
            }

            set
            {
                if (c11e8 != value)
                {
                    c11e8 = value;
                    OnPropertyChanged("C11e8");
                }

            }
        }

        public Visibility C11e9
        {
            get
            {
                return c11e9;
            }

            set
            {
                if (c11e9 != value)
                {
                    c11e9 = value;
                    OnPropertyChanged("C11e9");
                }

            }
        }

        public Visibility C11e10
        {
            get
            {
                return c11e10;
            }

            set
            {
                if (c11e10 != value)
                {
                    c11e10 = value;
                    OnPropertyChanged("C11e10");
                }

            }
        }

        public Visibility C11e11
        {
            get
            {
                return c11e11;
            }

            set
            {
                if (c11e11 != value)
                {
                    c11e11 = value;
                    OnPropertyChanged("C11e11");
                }

            }
        }

        public Visibility C11e12
        {
            get
            {
                return c11e12;
            }

            set
            {
                if (c11e12 != value)
                {
                    c11e12 = value;
                    OnPropertyChanged("C11e12");
                }

            }
        }

        public Visibility C12e1
        {
            get
            {
                return c12e1;
            }

            set
            {
                if (c12e1 != value)
                {
                    c12e1 = value;
                    OnPropertyChanged("C12e1");
                }

            }
        }

        public Visibility C12e2
        {
            get
            {
                return c12e2;
            }

            set
            {
                if (c12e2 != value)
                {
                    c12e2 = value;
                    OnPropertyChanged("C12e2");
                }

            }
        }

        public Visibility C12e3
        {
            get
            {
                return c12e3;
            }

            set
            {
                if (c12e3 != value)
                {
                    c12e3 = value;
                    OnPropertyChanged("C12e3");
                }

            }
        }

        public Visibility C12e4
        {
            get
            {
                return c12e4;
            }

            set
            {
                if (c12e4 != value)
                {
                    c12e4 = value;
                    OnPropertyChanged("C12e4");
                }

            }
        }

        public Visibility C12e5
        {
            get
            {
                return c12e5;
            }

            set
            {
                if (c12e5 != value)
                {
                    c12e5 = value;
                    OnPropertyChanged("C12e5");
                }

            }
        }

        public Visibility C12e6
        {
            get
            {
                return c12e6;
            }

            set
            {
                if (c12e6 != value)
                {
                    c12e6 = value;
                    OnPropertyChanged("C12e6");
                }

            }
        }

        public Visibility C12e7
        {
            get
            {
                return c12e7;
            }

            set
            {
                if (c12e7 != value)
                {
                    c12e7 = value;
                    OnPropertyChanged("C12e7");
                }

            }
        }

        public Visibility C12e8
        {
            get
            {
                return c12e8;
            }

            set
            {
                if (c12e8 != value)
                {
                    c12e8 = value;
                    OnPropertyChanged("C12e8");
                }

            }
        }

        public Visibility C12e9
        {
            get
            {
                return c12e9;
            }

            set
            {
                if (c12e9 != value)
                {
                    c12e9 = value;
                    OnPropertyChanged("C12e9");
                }

            }
        }

        public Visibility C12e10
        {
            get
            {
                return c12e10;
            }

            set
            {
                if (c12e10 != value)
                {
                    c12e10 = value;
                    OnPropertyChanged("C12e10");
                }

            }
        }

        public Visibility C12e11
        {
            get
            {
                return c12e11;
            }

            set
            {
                if (c12e11 != value)
                {
                    c12e11 = value;
                    OnPropertyChanged("C12e11");
                }

            }
        }

        public Visibility C12e12
        {
            get
            {
                return c12e12;
            }

            set
            {
                if (c12e12 != value)
                {
                    c12e12 = value;
                    OnPropertyChanged("C12e12");
                }

            }
        }
        #endregion
    }
}
