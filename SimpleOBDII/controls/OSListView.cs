using OS.OBDII.Interfaces;
using OS.OBDII.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace OS.OBDII.Controls
{
    public class OSListView : ListView, ICustomButtonController
    {



        public ObservableCollection<IPid> Items
        {
            get { return (ObservableCollection<IPid>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        //public static readonly BindableProperty ItemsProperty =
        //  BindableProperty.Create("Items", typeof(IEnumerable<IPid>), typeof(OSListView), new List<IPid>());

#if ANDROID || IOS




        public event EventHandler<SelectedItemChangedEventArgs> ItemLongClicked;
        public void NotifyItemLongClicked(object selectedItem, int selectedItemIndex)
        {
            if (ItemLongClicked != null)
            {
                ItemLongClicked(this, new SelectedItemChangedEventArgs(selectedItem, selectedItemIndex));
            }
        }


        public event EventHandler<SelectedItemChangedEventArgs> ItemClicked;

        public void NotifyItemClicked(object selectedItem, int selectedItemIndex)
        {
            if (ItemClicked != null)
            {
                ItemClicked(this, new SelectedItemChangedEventArgs(selectedItem, selectedItemIndex));
            }
        }

        // Hide the base version of this event, we can only call it from the class it was created in
        public new event EventHandler<ItemTappedEventArgs> ItemTapped;
        public void NotifyItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (ItemTapped != null)
            {

                ItemTapped(this, e);
            }
        }

#endif 

        // Hide the base version of this event, we can only call it from the class it was created in
        public new event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public void NotifyItemSelected(object selectedItem, int selectedItemIndex)
        {
            if (ItemSelected != null)
            {
                ItemSelected(this, new SelectedItemChangedEventArgs(selectedItem, selectedItemIndex));
            }
        }



        public event EventHandler Clicked;
        void ICustomButtonController.SendClicked()
        {
            
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Touched;
        void ICustomButtonController.SendTouched()
        {
            Touched?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler LongClicked;
        void ICustomButtonController.SendLongClicked()
        {
            LongClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Released;
        void ICustomButtonController.SendReleased()
        {
            Released?.Invoke(this, EventArgs.Empty);
        }

    }
}
