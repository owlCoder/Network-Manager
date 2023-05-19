using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using NetworkService.Views;
using System.IO;

namespace NetworkService.ViewModel
{
    public class RasporedMrezeViewModel : BindableBase
    {
        public MyICommand NasumicnoRasporedi { get; private set; }
        public static ObservableCollection<Entitet> Entiteti { get; set; }

        public BindingList<KlasifikovaniEntiteti> Klasifikovani { get; set; }

        // komande za drag & drop
        // to do treba ih inicijalizovati!
        public MyICommand<Canvas> DragOverKomanda { get; private set; }
        public MyICommand<Canvas> DropKomanda { get; private set; }
        public MyICommand MouseLevoDugme { get; private set; }
        public MyICommand<TreeView> TreeViewOdabran { get; private set; }
        public MyICommand OslobodiKomanda { get; private set; }

        // za drag&drop
        private Entitet draggedItem = null;
        private bool dragging = false;
        private int selected;

        public RasporedMrezeViewModel()
        {
            Entiteti = MainWindowViewModel.Entiteti;
            NasumicnoRasporedi = new MyICommand(Rasporedi);
            Preraspodela();

            // komande
            DragOverKomanda = new MyICommand<Canvas>(DragOverMetoda);
            DropKomanda = new MyICommand<Canvas>(DropMetoda);
            MouseLevoDugme = new MyICommand(TreeView_MouseLeftButtonUp);
            TreeViewOdabran = new MyICommand<TreeView>(Promena_SelectedItemChanged);
        }

        private void DropMetoda(Canvas kanvas)
        {
            TextBlock ispis = ((TextBlock)((Canvas)kanvas).Children[0]);
            // base.OnDrop(e);
            if (draggedItem != null)
            {
                if (kanvas.Resources["taken"] == null)
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    string putanja = Directory.GetCurrentDirectory() + draggedItem.Slika;
                    img.UriSource = new Uri(putanja, UriKind.Absolute);
                    img.EndInit();
                    kanvas.Background = new ImageBrush(img);
                    ispis.Text = draggedItem.Naziv;
                    //ispis.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF1338BE"));
                    draggedItem.Canvas_pozicija = GetCanvasId(kanvas.Name);
                    kanvas.Resources.Add("taken", true);
                    ukloniElement(draggedItem);
                }
                draggedItem = null;
                dragging = false;
            }
            // e.Handled = true;
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

            if (!dragging && tv.SelectedItem.GetType() == typeof(Entitet))
            {
                dragging = true;
                draggedItem = (Entitet)tv.SelectedItem;
                selected = pronadjiElement(draggedItem);
                DragDrop.DoDragDrop(prozor, draggedItem, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }


        private void TreeView_MouseLeftButtonUp()
        {
            dragging = false;
            draggedItem = null;
        }

        private int pronadjiElement(Entitet draggedItem)
        {
            int index = 0;
            if (draggedItem.Klasa.Equals("A"))
            {
                index = Klasifikovani[0].ListaEntiteta.IndexOf(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("B"))
            {
                index = Klasifikovani[1].ListaEntiteta.IndexOf(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("C"))
            {
                index = Klasifikovani[2].ListaEntiteta.IndexOf(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("D"))
            {
                index = Klasifikovani[3].ListaEntiteta.IndexOf(draggedItem);
            }
            else if (draggedItem.Klasa.Equals("E"))
            {
                index = Klasifikovani[4].ListaEntiteta.IndexOf(draggedItem);
            }

            return index;
        }

        private void ukloniElement(Entitet draggedItem)
        {
            if (draggedItem.Klasa.Equals("A"))
            {
                Klasifikovani[0].ListaEntiteta.RemoveAt(selected);
            }
            else if (draggedItem.Klasa.Equals("B"))
            {
                Klasifikovani[1].ListaEntiteta.RemoveAt(selected);
            }
            else if (draggedItem.Klasa.Equals("C"))
            {
                Klasifikovani[2].ListaEntiteta.RemoveAt(selected);
            }
            else if (draggedItem.Klasa.Equals("D"))
            {
                Klasifikovani[3].ListaEntiteta.RemoveAt(selected);
            }
            else if (draggedItem.Klasa.Equals("E"))
            {
                Klasifikovani[4].ListaEntiteta.RemoveAt(selected);
            }
        }

        private void DragOverMetoda(Canvas kanvas)
        {
            // base.OnDragOver(e);
            //if (kanvas.Resources["taken"] != null)
            //{
            //    DragDropEff = DragDropEffects.None;
            //}
            //else
            //{
            //    e.Effects = DragDropEffects.Copy;
            //}
            //e.Handled = true;
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
