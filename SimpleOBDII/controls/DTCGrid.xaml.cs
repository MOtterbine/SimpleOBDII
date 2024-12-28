using System;
using System.Collections.Generic;
using System.ComponentModel;
using OS.OBDII.Models;
using OS.OBDII.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using OS.OBDII.ViewModels;

namespace OS.OBDII.Controls
{

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class DTCGrid : Grid
    {

        public String VIN
        {
            get { return (String)GetValue(VINProperty); }
            set { SetValue(VINProperty, value); }
        }
        public static readonly BindableProperty VINProperty =
            BindableProperty.Create("VIN", typeof(String), typeof(DTCGrid), string.Empty);

        public int DTCCount
        {
            get { return (int)GetValue(DTCCountProperty); }
            set { SetValue(DTCCountProperty, value); }
        }
        public static readonly BindableProperty DTCCountProperty =
            BindableProperty.Create("DTCCount", typeof(int), typeof(DTCGrid), 0);

        public string NoDataString
        {
            get => (string)GetValue(NoDataStringProperty); 
            set => SetValue(NoDataStringProperty, value);
        }
        public static readonly BindableProperty NoDataStringProperty =
            BindableProperty.Create("NoDataString", typeof(string), typeof(DTCGrid), Constants.STRING_NO_DATA);

        public bool SourceIsEmpty
        {
            get { return (bool)GetValue(SourceIsEmptyProperty); }
            set { SetValue(SourceIsEmptyProperty, value); }
        }
        public static readonly BindableProperty SourceIsEmptyProperty =
            BindableProperty.Create("SourceIsEmpty", typeof(bool), typeof(DTCGrid), true);

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }
        public static readonly BindableProperty ShowHeaderProperty =
            BindableProperty.Create("ShowHeader", typeof(bool), typeof(DTCGrid), true);

        public ObservableCollection<DTCGroup> ItemsSource
        {
            get => (ObservableCollection<DTCGroup>)GetValue(ItemsSourceProperty); 
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(ObservableCollection<DTCGroup>), typeof(DTCGrid), new ObservableCollection<DTCGroup>());


        public ICommand GridCommand
        {
            get => (ICommand)GetValue(GridCommandProperty);
            set => SetValue(GridCommandProperty, value);
        }
        public static readonly BindableProperty GridCommandProperty =
            BindableProperty.Create("GridCommand", typeof(ICommand), typeof(DTCGrid), null);


        public ICommand FreezeFrameClicked => new Command(() => {
            this.GridCommand.Execute(new GridEventArgs() {EventType = GridEventTypes.RowButtonClicked, dataObject="FF Clicked" });
        });

        public CollectionView ItemsView { get; set; } = null;

        public DTCGrid()
        {
            InitializeComponent();

            // Watches the collection for changes/modifications
            this.ItemsView = new CollectionView
            {
                ItemsSource = this.ItemsSource
            };

            this.BindingContextChanged -= OnContextChanged;
            this.BindingContextChanged += OnContextChanged;

        }

        #region Events

        private void FireRowButtonEvent(string description)
        {
            using (GridEventArgs evt = new GridEventArgs())
            {
                evt.EventType = GridEventTypes.RowButtonClicked;
                evt.Description = description;
                this.FireGridEvent(evt);
            }
        }
        private byte foundStatus = 0x00;

        private void FireAdapterEvent(GridEventTypes eventType, string message = null)
        {
            using (GridEventArgs evt = new GridEventArgs())
            {
                evt.EventType = eventType;
                evt.Description = message;
                this.FireGridEvent(evt);
            }
        }

        protected void FireGridEvent(GridEventArgs e)
        {
            //if (this.GridEvent != null)
            //{
            //    GridEvent();// this, e);
            //}
        }

        #endregion Events

        private ObservableCollection<DTCGroup> dTCCodes = new ObservableCollection<DTCGroup>() {
            new DTCGroup(Constants.DTC_CODES_GROUP_CONFIRMED),
            new DTCGroup(Constants.DTC_CODES_GROUP_PENDING),
            new DTCGroup(Constants.DTC_CODES_GROUP_PERMANENT),
            new DTCGroup(Constants.DTC_CODES_GROUP_STORED)
        };

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var group = sender as DTCGroup;
            if (group == null) return;

            switch(group.Name)
            {
                case Constants.DTC_CODES_GROUP_CONFIRMED:
                    this.foundStatus |= 0x01;
                    break;
                case Constants.DTC_CODES_GROUP_PENDING:
                    this.foundStatus |= 0x02;
                    break;
                case Constants.DTC_CODES_GROUP_PERMANENT:
                    this.foundStatus |= 0x04;
                    break;
                case Constants.DTC_CODES_GROUP_STORED:
                    this.foundStatus |= 0x08;
                    break;
            }

            this.SourceIsEmpty = IsSourceEmpty();
        }

        private void OnContextChanged(object sender, EventArgs e)
        {

            // Confirmed, pending and permanent collections...

            if (this.ItemsSource == null) return;

            this.ItemsSource.ToList().ForEach(dtcList => { 
                dtcList.CollectionChanged -= OnCollectionChanged;
            });

            this.SourceIsEmpty = IsSourceEmpty();

            this.ItemsSource.ToList().ForEach(dtcList => { 
                dtcList.CollectionChanged += OnCollectionChanged;
            });

        }

        private bool IsSourceEmpty()
        {
            foreach(DTCGroup grp in this.ItemsSource)
            {
                if (grp.Count > 0) return false;
            }
            return true;
        }

    }
}