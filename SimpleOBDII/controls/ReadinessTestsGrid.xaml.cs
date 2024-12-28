using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using OS.OBDII.Models;
using OS.OBDII.Interfaces;
using OS.OBDII.ViewModels;
using System.Collections.ObjectModel;

namespace OS.OBDII.Controls
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ReadinessTestsGrid : Grid
    {

        public ReadinessTestsGrid()
        {
            InitializeComponent();

            this.BindingContextChanged += OnContextChanged;
            

        }

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.SourceIsEmpty = this.ItemsSource.Count < 1;
        }

        private void OnContextChanged(object sender, EventArgs e)
        {
            this.ItemsSource.CollectionChanged -= OnCollectionChanged;
            this.SourceIsEmpty = this.ItemsSource.Count < 1;
            this.ItemsSource.CollectionChanged += OnCollectionChanged;
        }


        public String VIN
        {
            get { return (String)GetValue(VINProperty); }
            set { SetValue(VINProperty, value); }
        }

        public static readonly BindableProperty VINProperty =
            BindableProperty.Create("VIN", typeof(String), typeof(ReadinessTestsGrid), "");


        public int DTCCount
        {
            get { return (int)GetValue(DTCCountProperty); }
            set { SetValue(DTCCountProperty, value); }
        }

        public static readonly BindableProperty DTCCountProperty =
            BindableProperty.Create("DTCCount", typeof(int), typeof(ReadinessTestsGrid), 0);

        public string EmptyGridString
        {
            get { return (string)GetValue(EmptyGridStringProperty); }
            set
            {
                SetValue(EmptyGridStringProperty, value);
            }
        }

        public static readonly BindableProperty EmptyGridStringProperty =
            BindableProperty.Create("EmptyGridString", typeof(string), typeof(ReadinessTestsGrid), Constants.STRING_NO_DATA);

        public string MonitorTitle
        {
            get { return (string)GetValue(MonitorTitleProperty); }
            set
            {
                SetValue(MonitorTitleProperty, value);
            }
        }

        public static readonly BindableProperty MonitorTitleProperty =
            BindableProperty.Create("MonitorTitle", typeof(string), typeof(ReadinessTestsGrid), "Tests");

        public ObservableCollection<ReadinessMonitor> ItemsSource
        {
            get { return (ObservableCollection<ReadinessMonitor>)GetValue(ItemsSourceProperty); }
            set 
            {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(ObservableCollection<ReadinessMonitor>), typeof(ReadinessTestsGrid), new ObservableCollection<ReadinessMonitor>());

        public bool SourceIsEmpty
        {
            get { return (bool)GetValue(SourceIsEmptyProperty); }
            set { SetValue(SourceIsEmptyProperty, value); }
        }
        public static readonly BindableProperty SourceIsEmptyProperty =
            BindableProperty.Create("SourceIsEmpty", typeof(bool), typeof(ReadinessTestsGrid), true);


        public bool AllTestsComplete
        {
            get { return (bool)GetValue(AllTestsCompleteProperty); }
            set { SetValue(AllTestsCompleteProperty, value); }
        }
        public static readonly BindableProperty AllTestsCompleteProperty =
            BindableProperty.Create("AllTestsComplete", typeof(bool), typeof(ReadinessTestsGrid), false);


        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public static readonly BindableProperty ShowHeaderProperty =
            BindableProperty.Create("ShowHeader", typeof(bool), typeof(ReadinessTestsGrid), true);

    }
}