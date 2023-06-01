using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static BindingList<KlasifikovaniEntiteti> LevaLista { get; set; }
        public static BindingList<KlasifikovaniEntiteti> DesnaLista { get; set; }

        // komande za drag & drop
        public MyICommand<Canvas> DragOverKomanda { get; private set; }
        public MyICommand<Canvas> DropKomanda { get; private set; }
        public MyICommand MouseLevoDugme { get; private set; }
        public MyICommand<TreeView> TreeViewOdabran { get; private set; }
        public MyICommand<Canvas> OslobodiKomanda { get; private set; }
        public MyICommand<Grid> NasumicnoRasporedi { get; private set; }

        public MyICommand<Canvas> PreviewMouseUpKomanda { get; private set; }
        public MyICommand<Canvas> PreviewMouseMoveKomanda { get; private set; }
        public MyICommand<Canvas> PreviewMouseDownKomanda { get; private set; }

        // za drag & drop
        private Server trenutniServer = null;
        private bool prevlacenjeUToku = false;
        private int selected;
        private Canvas pocetni = null;
        public static Grid desni;

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

            // liste za entitete za tree view i canvas
            Klasifikacija();

            // za prijem novih entiteta
            Messenger.Default.Register<ServerTreeView>(this, DodajUTreeViewListu);

            // za uklanjanje entiteta ako se ukloni iz liste svih
            Messenger.Default.Register<BrisanjeServera>(this, UkloniElementCanvasTreeView);
        }

        // Kako bi se pamtilo stanje na canvasu - potrebno je koristiti konstruktor sa parametrima
        public RasporedMrezeViewModel(Grid desniGridCanvas)
        {
            desni = desniGridCanvas;

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

            // restauracija canvasa
            List<Canvas> kanvasi = new List<Canvas>();

            for (int i = 1; i < 13; i++)
            {
                DockPanel panel = (DockPanel)(desniGridCanvas.Children[i]);
                Canvas canvas = (Canvas)(panel.Children[1]);
                kanvasi.Add(canvas);
            }

            foreach (KlasifikovaniEntiteti ke in LevaLista.ToList())
            {
                foreach (Server e in ke.ListaEntiteta.ToList())
                {
                    if (e.Canvas_pozicija != -1)
                    {
                        trenutniServer = MainWindowViewModel.Serveri.FirstOrDefault(p => p.Id == e.Id);
                        Canvas kanvas = kanvasi[e.Canvas_pozicija - 1];

                        TextBlock ispis = ((TextBlock)(kanvas).Children[0]);

                        if (trenutniServer != null)
                        {
                            if (kanvas.Resources["taken"] == null)
                            {
                                BitmapImage img = new BitmapImage();
                                img.BeginInit();
                                string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                                img.UriSource = new Uri(putanja, UriKind.Absolute);
                                img.EndInit();
                                kanvas.Background = new ImageBrush(img);
                                ispis.Text = trenutniServer.Naziv;
                                ispis.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                                trenutniServer.Canvas_pozicija = GetCanvasId(kanvas.Name);
                                kanvas.Resources.Add("taken", true);
                            }
                            trenutniServer = null;
                            prevlacenjeUToku = false;
                        }
                    }
                }
            }

            Messenger.Default.Register<int>(this, UkloniAkoJeNaCanvasu);
        }

        // metoda koje povezuje brisanje iz MV i Canvas
        private void UkloniAkoJeNaCanvasu(int id_canvasa)
        {
            // ako jos uvek nije inicijalizovan view, return
            if (desni == null)
                return;

            // indeksiranje
            // dock paneli krecu od indeksa 1
            // indeks 1 u dock panelu je canvas
            List<Canvas> kanvasi = new List<Canvas>();

            for (int i = 1; i < 13; i++)
            {
                DockPanel panel = (DockPanel)(desni.Children[i]);
                Canvas canvas = (Canvas)(panel.Children[1]);
                kanvasi.Add(canvas);
            }
        }

        private void PreviewMouseDown(Canvas canvas)
        {
            // prvo pronadjemo koji je to element na canvasu
            string naziv_entiteta = ((TextBlock)canvas.Children[0]).Text;
            Server ent = null;

            foreach (KlasifikovaniEntiteti ke in LevaLista)
            {
                foreach (Server e in ke.ListaEntiteta)
                {
                    if (e.Naziv.Equals(naziv_entiteta))
                    {
                        ent = e;
                        goto Izlaz;
                    }
                }
            }

        Izlaz:
            trenutniServer = ent;
            pocetni = canvas;
        }

        private void PreviewMouseMove(Canvas canvas)
        {
            if (trenutniServer == null)
                return;
        }

        private void PreviewMouseUp(Canvas canvas)
        {
            // Samo ako imamo pocetni canvas i odabrani element i canvas na koji prebacujemo nije vec zauzet
            if (trenutniServer != null && pocetni != null && canvas.Resources["taken"] == null)
            {
                // prebaci na novi canvas
                TextBlock ispis = ((TextBlock)(canvas).Children[0]);

                if (trenutniServer != null)
                {
                    if (canvas.Resources["taken"] == null)
                    {
                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                        img.UriSource = new Uri(putanja, UriKind.Absolute);
                        img.EndInit();
                        canvas.Background = new ImageBrush(img);
                        ispis.Text = trenutniServer.Naziv;
                        ispis.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                        trenutniServer.Canvas_pozicija = GetCanvasId(canvas.Name);
                        canvas.Resources.Add("taken", true);
                    }
                    trenutniServer = null;
                    prevlacenjeUToku = false;
                }

                // oslobodi pocetni canvas
                if (pocetni.Resources["taken"] != null)
                {
                    pocetni.Background = Brushes.White;
                    ((TextBlock)pocetni.Children[0]).Text = string.Empty;
                    pocetni.Resources.Remove("taken");
                }

                trenutniServer = null;
                pocetni = null;
            }
        }

        private void UkloniElementCanvasTreeView(BrisanjeServera pd)
        {
            Server za_brisanje = pd.Entitet;
            int klasa = 0;

            if (za_brisanje.Klasa.Equals("A")) klasa = 0;
            if (za_brisanje.Klasa.Equals("B")) klasa = 1;
            if (za_brisanje.Klasa.Equals("C")) klasa = 2;
            if (za_brisanje.Klasa.Equals("D")) klasa = 3;
            if (za_brisanje.Klasa.Equals("E")) klasa = 4;

            // ako se element nalazi na canvasu, ukloniti ga iz liste i sa canvasa
            if (LevaLista[klasa].ListaEntiteta.Contains(za_brisanje))
            {
                LevaLista[klasa].ListaEntiteta.Remove(za_brisanje);
            }

            // ako se nalazi u tree view - ukloniti ga
            if (DesnaLista[klasa].ListaEntiteta.Contains(za_brisanje))
            {
                DesnaLista[klasa].ListaEntiteta.Remove(za_brisanje);
            }
        }

        private void DodajUTreeViewListu(ServerTreeView pf)
        {
            Server novi = pf.Entitet;
            int klasa = 0;

            if (novi.Klasa.Equals("A")) klasa = 0;
            if (novi.Klasa.Equals("B")) klasa = 1;
            if (novi.Klasa.Equals("C")) klasa = 2;
            if (novi.Klasa.Equals("D")) klasa = 3;
            if (novi.Klasa.Equals("E")) klasa = 4;

            DesnaLista[klasa].ListaEntiteta.Add(novi);
        }

        private void Oslobodi_Dugme(Canvas kanvas)
        {
            if (kanvas.Resources["taken"] != null)
            {
                VratiElement(kanvas);

                kanvas.Background = Brushes.White;
                ((TextBlock)kanvas.Children[0]).Text = string.Empty;
                kanvas.Resources.Remove("taken");
            }
        }

        private void VratiElement(Canvas kanvas)
        {
            string naziv_entiteta = ((TextBlock)kanvas.Children[0]).Text;

            Server item = null;
            KlasifikovaniEntiteti pripada_tipu_adrese = null;
            int klasa = 0;

            // prolazimo kroz sve adresne klase
            foreach (KlasifikovaniEntiteti ke in LevaLista)
            {
                // i trazimo u listi entiteta odredjene klase onaj entitet koji je na canvasu
                foreach (Server e in ke.ListaEntiteta)
                {
                    if (e.Naziv.Equals(naziv_entiteta))
                    {
                        // pronasli smo entitet, zapamti
                        pripada_tipu_adrese = ke;
                        item = e;
                        goto Izlaz;
                    }
                }

                klasa += 1; // prelazimo u sledecu adresnu klasu (A = 0, B = 1, ..., E = 4)
            }

        Izlaz:
            if (item == null || pripada_tipu_adrese == null || klasa > 4)
            {
                return;
            }

            // u pronadjenoj klasi, za pronadjeni entitet - ukloniti referencu
            item.Canvas_pozicija = -1;

            pripada_tipu_adrese.ListaEntiteta.Remove(item);

            // dodajemo u tree view u odredjenu klasu adresa kojoj entitet i pripada
            DesnaLista[klasa].ListaEntiteta.Add(item);
        }

        private void DropMetoda(Canvas kanvas)
        {
            TextBlock ispis = ((TextBlock)(kanvas).Children[0]);

            if (trenutniServer != null)
            {
                if (kanvas.Resources["taken"] == null)
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                    img.UriSource = new Uri(putanja, UriKind.Absolute);
                    img.EndInit();
                    kanvas.Background = new ImageBrush(img);
                    ispis.Text = trenutniServer.Naziv;
                    ispis.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                    trenutniServer.Canvas_pozicija = GetCanvasId(kanvas.Name);
                    kanvas.Resources.Add("taken", true);
                    UkloniElement(trenutniServer);
                }
                trenutniServer = null;
                prevlacenjeUToku = false;
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

            if (!prevlacenjeUToku && tv != null && tv.SelectedItem != null && tv.SelectedItem.GetType() == typeof(Server))
            {
                prevlacenjeUToku = true;
                trenutniServer = (Server)tv.SelectedItem;
                selected = PronadjiElement(trenutniServer);
                DragDrop.DoDragDrop(prozor, trenutniServer, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

        private void TreeView_MouseLeftButtonUp()
        {
            prevlacenjeUToku = false;
            trenutniServer = null;
        }

        private int PronadjiElement(Server draggedItem)
        {
            int index = 0;

            if (draggedItem.Klasa.Equals("A"))
                index = DesnaLista[0].ListaEntiteta.IndexOf(draggedItem);
            else if (draggedItem.Klasa.Equals("B"))
                index = DesnaLista[1].ListaEntiteta.IndexOf(draggedItem);
            else if (draggedItem.Klasa.Equals("C"))
                index = DesnaLista[2].ListaEntiteta.IndexOf(draggedItem);
            else if (draggedItem.Klasa.Equals("D"))
                index = DesnaLista[3].ListaEntiteta.IndexOf(draggedItem);
            else if (draggedItem.Klasa.Equals("E"))
                index = DesnaLista[4].ListaEntiteta.IndexOf(draggedItem);

            return index;
        }

        private void UkloniElement(Server draggedItem)
        {
            switch (draggedItem.Klasa)
            {
                case "A":
                    DesnaLista[0].ListaEntiteta.RemoveAt(selected);
                    LevaLista[0].ListaEntiteta.Add(draggedItem);
                    break;
                case "B":
                    DesnaLista[1].ListaEntiteta.RemoveAt(selected);
                    LevaLista[1].ListaEntiteta.Add(draggedItem);
                    break;
                case "C":
                    DesnaLista[2].ListaEntiteta.RemoveAt(selected);
                    LevaLista[2].ListaEntiteta.Add(draggedItem);
                    break;
                case "D":
                    DesnaLista[3].ListaEntiteta.RemoveAt(selected);
                    LevaLista[3].ListaEntiteta.Add(draggedItem);
                    break;
                case "E":
                    DesnaLista[4].ListaEntiteta.RemoveAt(selected);
                    LevaLista[4].ListaEntiteta.Add(draggedItem);
                    break;
            }
        }

        private void DragOverMetoda(Canvas kanvas)
        {
            // TO DO
        }

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
                    if (DesnaLista[0].ListaEntiteta.Count > 0)
                    {
                        trenutniServer = DesnaLista[0].ListaEntiteta[0];
                        DesnaLista[0].ListaEntiteta.RemoveAt(0);
                        LevaLista[0].ListaEntiteta.Add(trenutniServer);
                    }
                    else if (DesnaLista[1].ListaEntiteta.Count > 0)
                    {
                        trenutniServer = DesnaLista[1].ListaEntiteta[0];
                        DesnaLista[1].ListaEntiteta.RemoveAt(0);
                        LevaLista[1].ListaEntiteta.Add(trenutniServer);
                    }
                    else if (DesnaLista[2].ListaEntiteta.Count > 0)
                    {
                        trenutniServer = DesnaLista[2].ListaEntiteta[0];
                        DesnaLista[2].ListaEntiteta.RemoveAt(0);
                        LevaLista[2].ListaEntiteta.Add(trenutniServer);
                    }
                    else if (DesnaLista[3].ListaEntiteta.Count > 0)
                    {
                        trenutniServer = DesnaLista[3].ListaEntiteta[0];
                        DesnaLista[3].ListaEntiteta.RemoveAt(0);
                        LevaLista[3].ListaEntiteta.Add(trenutniServer);
                    }
                    else if (DesnaLista[4].ListaEntiteta.Count > 0)
                    {
                        trenutniServer = DesnaLista[4].ListaEntiteta[0];
                        DesnaLista[4].ListaEntiteta.RemoveAt(0);
                        LevaLista[4].ListaEntiteta.Add(trenutniServer);
                    }

                    if (trenutniServer != null)
                    {
                        trenutniServer.Canvas_pozicija = i; // pozicija na canvasu

                        if (kanvas.Resources["taken"] == null)
                        {
                            BitmapImage img = new BitmapImage();
                            img.BeginInit();
                            string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                            img.UriSource = new Uri(putanja, UriKind.Absolute);
                            img.EndInit();
                            kanvas.Background = new ImageBrush(img);
                            trenutni.Text = trenutniServer.Naziv;
                            trenutni.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                            trenutniServer.Canvas_pozicija = GetCanvasId(kanvas.Name);
                            kanvas.Resources.Add("taken", true);
                        }
                        trenutniServer = null;
                        prevlacenjeUToku = false;
                    }
                }
            }
        }

        public void Klasifikacija()
        {
            LevaLista = new BindingList<KlasifikovaniEntiteti>()
            {
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa A" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa B" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa C" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa D" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa E" }
            };

            DesnaLista = new BindingList<KlasifikovaniEntiteti>()
            {
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa A" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa B" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa C" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa D" },
                new KlasifikovaniEntiteti() { AdresnaKlasa = "IP Tipa E" }
            };
        }
    }
}