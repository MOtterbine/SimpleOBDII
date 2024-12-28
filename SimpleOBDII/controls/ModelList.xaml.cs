using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using OS.OBDII.Interfaces;
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using System.Collections.ObjectModel;
using OS.OBDII.Manufacturers;

namespace OS.OBDII.Controls
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
   // [DesignTimeVisible(false)]
    public partial class ModelList : Grid, IDisposable
    {

        public ModelList()
        {
            InitializeComponent();

            this.SourceIsEmpty = false;

        }


        private void OnContextChanged(object sender, EventArgs e)
        {
          //  this.ItemsSource.CollectionChanged -= OnCollectionChanged;
          //  this.SourceIsEmpty = this.ItemsSource.Count < 1;
          //  this.ItemsSource.CollectionChanged += OnCollectionChanged;
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
            BindableProperty.Create("EmptyGridString", typeof(string), typeof(ModelList), Constants.STRING_NO_DATA);

        public string MonitorTitle
        {
            get { return (string)GetValue(MonitorTitleProperty); }
            set
            {
                SetValue(MonitorTitleProperty, value);
            }
        }

        public static readonly BindableProperty MonitorTitleProperty =
            BindableProperty.Create("MonitorTitle", typeof(string), typeof(ModelList), "Tests");

        public ObservableCollection<IVehicleModel> ItemsSource
        {
            get { return (ObservableCollection<IVehicleModel>)GetValue(ItemsSourceProperty); }
            set 
            {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(ObservableCollection<IVehicleModel>), typeof(ModelList), new ObservableCollection<IVehicleModel>());

        public bool SourceIsEmpty
        {
            get { return (bool)GetValue(SourceIsEmptyProperty); }
            set { SetValue(SourceIsEmptyProperty, value); }
        }

        public static readonly BindableProperty SourceIsEmptyProperty =
            BindableProperty.Create("SourceIsEmpty", typeof(bool), typeof(ModelList), false);

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public static readonly BindableProperty ShowHeaderProperty =
            BindableProperty.Create("ShowHeader", typeof(bool), typeof(ModelList), true);

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
               //     this.ItemsSource.CollectionChanged -= OnCollectionChanged;
                    this.BindingContextChanged -= OnContextChanged;
                    // Dispose managed resources.
                }
                // Unmanaged resources are disposed in any case

                //	CloseHandle(handle);
                //	handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }
        ~ModelList()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

    }
}