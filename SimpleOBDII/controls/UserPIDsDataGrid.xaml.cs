using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using OS.OBDII.Interfaces;
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using System.Collections.ObjectModel;

namespace OS.OBDII.Controls
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class UserPIDsDataGrid : Grid
    {

        public UserPIDsDataGrid()
        {
            InitializeComponent();
            //this.PIDListView.ItemTapped += OnItemTapped;
            //// To edit...
            //this.PIDListView.ItemLongClicked += onSelectItemForEdit;
            //this.PIDListView.ItemClicked += onSelectItemForEdit;


        }

        private void onItemTapped(object sender, ItemTappedEventArgs e)
        {
            var d = e.Item as IPid;


        }

        public void ScrollTo(object item, ScrollToPosition position, bool animated)
        {
            this.mainlist.ScrollTo(item, position, animated);
        }

        public string EmptyGridString
        {
            get { return (string)GetValue(EmptyGridStringProperty); }
            set
            {
                SetValue(EmptyGridStringProperty, value);
                
            }
        }
        public static readonly BindableProperty EmptyGridStringProperty =
            BindableProperty.Create("EmptyGridString", typeof(string), typeof(UserPIDsDataGrid), Constants.STRING_NO_DATA);

        public string MonitorTitle
        {
            get { return (string)GetValue(MonitorTitleProperty); }
            set
            {
                SetValue(MonitorTitleProperty, value);
            }
        }

        public static readonly BindableProperty MonitorTitleProperty =
            BindableProperty.Create("MonitorTitle", typeof(string), typeof(UserPIDsDataGrid), "Tests");

        public ObservableCollection<IPid> ItemsSource
        {
            get { return (ObservableCollection<IPid>)GetValue(ItemsSourceProperty); }
            set 
            {
                SetValue(ItemsSourceProperty, value);
                this.SourceIsEmpty = this.ItemsSource.Count < 1;
            }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(ObservableCollection<IPid>), typeof(UserPIDsDataGrid), new ObservableCollection<IPid>());

        public bool SourceIsEmpty
        {
            get { return (bool)GetValue(SourceIsEmptyProperty); }
            set { SetValue(SourceIsEmptyProperty, value); }
        }

        public static readonly BindableProperty SourceIsEmptyProperty =
            BindableProperty.Create("SourceIsEmpty", typeof(bool), typeof(UserPIDsDataGrid), false);

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public static readonly BindableProperty ShowHeaderProperty =
            BindableProperty.Create("ShowHeader", typeof(bool), typeof(UserPIDsDataGrid), true);

    }
}