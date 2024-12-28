using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using OS.OBDII.Interfaces;
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using System.Collections.ObjectModel;
//using Microcharts;
//using SkiaSharp;

namespace OS.OBDII.Controls
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class LivePlotsGrid : Grid, IDisposable
    {

        public LivePlotsGrid()
        {
            InitializeComponent();


            this.SourceIsEmpty = false;

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

        public void ScrollTo(object item, ScrollToPosition position, bool animated)
        {
            this.PlotListView.ScrollTo(item, position, animated);
        }

        public int PlotHeight
        {
            get { return (int)GetValue(PlotHeightProperty); }
            set
            {
                SetValue(PlotHeightProperty, value);
            }
        }
        public static readonly BindableProperty PlotHeightProperty =
            BindableProperty.Create("PlotHeight", typeof(int), typeof(LivePlotsGrid), 150);

        public string EmptyGridString
        {
            get { return (string)GetValue(EmptyGridStringProperty); }
            set
            {
                SetValue(EmptyGridStringProperty, value);
            }
        }
        public static readonly BindableProperty EmptyGridStringProperty =
            BindableProperty.Create("EmptyGridString", typeof(string), typeof(LivePlotsGrid), Constants.STRING_NO_DATA);

        public string MonitorTitle
        {
            get { return (string)GetValue(MonitorTitleProperty); }
            set
            {
                SetValue(MonitorTitleProperty, value);
            }
        }

        public static readonly BindableProperty MonitorTitleProperty =
            BindableProperty.Create("MonitorTitle", typeof(string), typeof(LivePlotsGrid), "Tests");

        public bool SourceIsEmpty
        {
            get { return (bool)GetValue(SourceIsEmptyProperty); }
            set { SetValue(SourceIsEmptyProperty, value); }
        }

        public static readonly BindableProperty SourceIsEmptyProperty =
            BindableProperty.Create("SourceIsEmpty", typeof(bool), typeof(LivePlotsGrid), true);


        public ObservableCollection<DataPlotModel> ItemsSource
        {
            get { return (ObservableCollection<DataPlotModel>)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(ObservableCollection<DataPlotModel>), typeof(LivePlotsGrid), new ObservableCollection<DataPlotModel>());


        public DataPlotModel SelectedItem
        {
            get
            {
                return (DataPlotModel)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
                //this.selectedIndex = -1;
            }
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem", typeof(DataPlotModel), typeof(LivePlotsGrid), null);


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
            BindableProperty.Create("SelectionMode", typeof(ListViewSelectionMode), typeof(LivePlotsGrid), ListViewSelectionMode.None);




        #region IDisposable

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.

        // Track whether Dispose has been called.
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        // Dispose(bool disposing) executes in two distinct scenarios.
        // 1) If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // 2) If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this.disposed == false)
            {
                // If disposing equals true managed resources can be disposed
                if (disposing == true)
                {
                    // Dispose managed resources.
                    //	component.Dispose();
                }
                // Unmanaged resources are disposed in any case

                //	CloseHandle(handle);
                //	handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }
        ~LivePlotsGrid()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion











    }
}