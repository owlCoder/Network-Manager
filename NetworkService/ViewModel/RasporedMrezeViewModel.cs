using NetworkService.Helpers;
using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace NetworkService.ViewModel
{
    public class RasporedMrezeViewModel : BindableBase
    {
        public static UserControl prozor { get; set; }
        public MyICommand NasumicnoRasporedi { get; private set; }

        public Canvas CanvasElement { get; set; }

        public static ObservableCollection<Entitet> Entiteti { get; set; }

        public BindingList<KlasifikovaniEntiteti> Klasifikovani { get; set; }

        // komande za drag & drop
        // to do treba ih inicijalizovati!
        public MyICommand<Canvas> DragOverKomanda { get; private set; }
        public MyICommand<Canvas> DropKomanda { get; private set; }
        public MyICommand MouseLevoDugme { get; private set; }
        public MyICommand<Entitet> SelectedItemPromena { get; private set; }
        public MyICommand OslobodiKomanda { get; private set; }

        // za drag&drop
        private Entitet draggedItem = null;
        private bool dragging = false;
        private int selected;

        // tree view pristup odabranog item-u
        private bool isSelected;

        // za drag&drop - again...
        // This is public get-only here but you could implement a public setter which
        // also selects the item.
        // Also this should be moved to an instance property on a VM for the whole tree, 
        // otherwise there will be conflicts for more than one tree.
        private static object selectedItem = null;

        private Entitet odabranZaPrelacenje;

        public RasporedMrezeViewModel()
        {
            prozor = RasporedMrezeView.UserControl;

            // komande za drag&drop
            MouseLevoDugme = new MyICommand(TreeView_MouseLeftButtonUp);
            SelectedItemPromena = new MyICommand<Entitet>(Promena_SelectedItemChanged);
            DragOverKomanda = new MyICommand<Canvas>(PrevuciNaCanvas);
            DropKomanda = new MyICommand<Canvas>(UkloniSaCanvas);

            Entiteti = MainWindowViewModel.Entiteti;
            NasumicnoRasporedi = new MyICommand(Rasporedi);
            Preraspodela();
        }

        private void UkloniSaCanvas(Canvas obj)
        {
            throw new NotImplementedException();
        }

        private void PrevuciNaCanvas(Canvas obj)
        {
            Canvas kanvas = ((Canvas)obj);
            // RasporedMrezeView.UserControl.OnDragOver(e);

            if (kanvas.Resources["taken"] != null)
            {
                //e.Effects = DragDropEffects.None;
            }
            else
            {
                //e.Effects = DragDropEffects.Copy;
            }
           // e.Handled = true;
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

        private void Promena_SelectedItemChanged(Entitet odabran)
        {
            prozor = RasporedMrezeView.UserControl;
            SelectedItem = OdabranZaPrevlacenje = odabran;
            OnPropertyChanged("SelectedItem");
            OnPropertyChanged("OdabranZaPrevlacenje");

            if (!dragging && SelectedItem.GetType() == typeof(Entitet))
            {
                dragging = true;
                draggedItem = OdabranZaPrevlacenje;
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
            else if(draggedItem.Klasa.Equals("B"))
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

        #endregion

        #region PROPERTY KLASE
        public object SelectedItem
        {
            get 
            { 
                return selectedItem; 
            }

            private set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnSelectedItemChanged();
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        private void OnSelectedItemChanged()
        {
            // Raise event / do other things
            OdabranZaPrevlacenje = (Entitet) SelectedItem;
            OnPropertyChanged("SelectedItem");
        }
  
        public Entitet OdabranZaPrevlacenje
        {
            get
            {
                return odabranZaPrelacenje;
            }

            set
            {
                if(odabranZaPrelacenje != value)
                {
                    odabranZaPrelacenje = value;
                    OnPropertyChanged("OdabranZaPrelacenje");
                }
            }
        }

        public bool IsSelected
        {
            get 
            {
                return isSelected;
            }

            set
            {
                if(isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");

                    if(isSelected)
                    {
                        SelectedItem = this;
                        OnPropertyChanged("SelectedItem");
                    }
                }
            }
        }
        #endregion
    }
}
