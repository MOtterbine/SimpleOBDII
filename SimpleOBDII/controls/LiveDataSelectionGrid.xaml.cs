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
    public partial class LiveDataSelectionGrid : Grid
    {
        public LiveDataSelectionGrid()
        {
            InitializeComponent();
#if WINDOWS
            this.PIDListView.ItemSelected += PIDListView_ItemSelected;

            this.PIDListView.ItemTapped += OnItemTapped;

#elif ANDROID || IOS
            // To edit...
            this.PIDListView.ItemLongClicked += onSelectItemForEdit;
            this.PIDListView.ItemClicked += onSelectItemForEdit;
#endif
            this.BindingContextChanged += OnContextChanged;

        }


        bool selectionPressed = false;
        private ResourceDictionary customStyles = null;


        private void PIDListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            _itemSelected = true;
        }
        private bool _itemSelected = false;

        private System.Threading.Timer timer = new System.Threading.Timer((state) => {


        });

        public event EventHandler<SelectedItemChangedEventArgs> RequestToEditItem;
        int selectedIndex = -1;
        private void onSelectItemForEdit(object sender, SelectedItemChangedEventArgs e)
        {
            if(RequestToEditItem!=null)
            {
                this.RequestToEditItem(this, e);


                //if (this.SelectedItem == null)
                //{
                   // this.SelectedItem = (UserPID)e.SelectedItem;

               // }


            }
        }
        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (selectedIndex == e.ItemIndex)
            {
                
                this.SelectedItem = null;
                selectedIndex = -1;
                return;
            }
            selectedIndex = e.ItemIndex;
            var s = e.Item as IPid;
            this.SelectedItem = this.ItemsSource.Where(i=> i.Code == s.Code).FirstOrDefault();

            if(_itemSelected)
            {

                if (RequestToEditItem != null)
                {
                    this.RequestToEditItem(this, new SelectedItemChangedEventArgs(e.Item, e.ItemIndex));
                }

               _itemSelected = false;
            }
           // this.SelectedItem = new UserPID(s.Description, s.UnitDescriptor, s.DecimalPlaces, s.QueryBytes, s.CalcExpression);
           // this.SelectedItem.CalcExpression = s.CalcExpression;
        }


        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.SourceIsEmpty = this.ItemsSource.Count < 1;
        }

        private void OnContextChanged(object sender, EventArgs e)
        {
            // if (this.ItemsSource == null) return;
            this.ItemsSource.CollectionChanged -= OnCollectionChanged;
            this.SourceIsEmpty = this.ItemsSource.Count < 1;
            this.ItemsSource.CollectionChanged += OnCollectionChanged;
        }


        public void ScrollTo(object item, ScrollToPosition position, bool animated)
        {
            this.PIDListView.ScrollTo(item, position, animated);
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
            BindableProperty.Create("EmptyGridString", typeof(string), typeof(LiveDataSelectionGrid), Constants.STRING_NO_DATA);

        public string MonitorTitle
        {
            get { return (string)GetValue(MonitorTitleProperty); }
            set
            {
                SetValue(MonitorTitleProperty, value);
            }
        }

        public static readonly BindableProperty MonitorTitleProperty =
            BindableProperty.Create("MonitorTitle", typeof(string), typeof(LiveDataSelectionGrid), "Tests");

        public ObservableCollection<IPid> ItemsSource
        {
            get { return (ObservableCollection<IPid>)GetValue(ItemsSourceProperty); }
            set 
            {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(ObservableCollection<IPid>), typeof(LiveDataSelectionGrid), new ObservableCollection<IPid>());


        public IPid SelectedItem
        {
            get
            {
                return (IPid)GetValue(SelectedItemProperty);
            }
            set
            {
                this.selectedIndex = -1;
                SetValue(SelectedItemProperty, value);
                OnPropertyChanged("ItemsSource");
            }
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem", typeof(UserPID), typeof(LiveDataSelectionGrid), null);

        public ListViewSelectionMode SelectionMode
        {
            get
            {
                return (ListViewSelectionMode)GetValue(SelectionModeProperty);
            }
            set
            {
                SetValue(SelectionModeProperty, value);
            }
        }

        public static readonly BindableProperty SelectionModeProperty =
            BindableProperty.Create("SelectionMode", typeof(ListViewSelectionMode), typeof(LiveDataSelectionGrid), ListViewSelectionMode.None);

        //Xamarin.Forms.ListView l = new ListView();
        

        public bool SourceIsEmpty
        {
            get { return (bool)GetValue(SourceIsEmptyProperty); }
            set { SetValue(SourceIsEmptyProperty, value); }
        }

        public static readonly BindableProperty SourceIsEmptyProperty =
            BindableProperty.Create("SourceIsEmpty", typeof(bool), typeof(LiveDataSelectionGrid), true);

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public static readonly BindableProperty ShowHeaderProperty =
            BindableProperty.Create("ShowHeader", typeof(bool), typeof(LiveDataSelectionGrid), true);

        private void ViewCell_Appearing(object sender, EventArgs e)
        {
            var s = sender as ViewCell;
            if (s == null) return;
            var userPID = s.BindingContext as UserPID;
        }

        private void ViewCell_Disappearing(object sender, EventArgs e)
        {
            var s = sender as ViewCell;
            if (s == null) return;
            var userPID = s.BindingContext as UserPID;

        }

        private void PIDListView_Scrolled(object sender, ScrolledEventArgs e)
        {
        }





    }
}