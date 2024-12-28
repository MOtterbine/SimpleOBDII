using System.ComponentModel;
using System.Threading.Tasks;

using OS.OBDII.Models;
using OS.OBDII.Interfaces;
using System.Runtime.CompilerServices;
using OS.OBDII.ViewModels;

namespace OS.OBDII.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SettingsPage : ContentPage
    {
        private IViewModel viewModel = null;
        public SettingsPage()
        {
            InitializeComponent();

            this.BindingContext = new SettingsViewModel(AppShellModel.Instance);
            this.viewModel = this.BindingContext as IViewModel;

        }

        private void ContentView_ChildRemoved(object sender, ElementEventArgs e)
        {
            (e.Element as IDisposable)?.Dispose();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.ModelEvent += this.OnViewModelEvent;
            // Update the model when we come back to this page
            viewModel?.Start();
        }
        protected override void OnDisappearing()
        {
            this.viewModel.IsBusy = true;
            viewModel?.Stop();
            viewModel.ModelEvent -= this.OnViewModelEvent;
            // Update the model when we come back to this page
            base.OnDisappearing();
        }

        private void OnViewModelEvent(object sender, ViewModelEventArgs e)
        {
            switch (e.EventType)
            {
                case ViewModelEventEventTypes.NavigateTo:

                    switch ((e.dataObject as string))
                    {
                        case "HomePage":
                            Dispatcher.Dispatch(() => Navigation.PopAsync(false));
                            break;
                    }
                    break;

                case ViewModelEventEventTypes.ScrollTo:
                    break;
            }
        }


    }
}