using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using OS.OBDII.Interfaces;
using System.Text;

namespace OS.OBDII.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class VehicleInfoPage : ContentPage
    {
        private IViewModel viewModel = null;

        public VehicleInfoPage()
        {
            InitializeComponent();
            this.BindingContext = new VehicleInfoViewModel(AppShellModel.Instance);
            viewModel = (this.BindingContext as IViewModel);

        }

        protected override void OnAppearing()
        {

            this.viewModel.ModelEvent += OnViewModelEvent;
            viewModel.Start();
            viewModel.IsBusy = false;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
           // this.AbortAnimation(Constants.STRING_COMM_BLINKING_VISUAL_ELEMENT);
            // Update the model when we come back to this page
            viewModel.IsBusy = true;
            this.viewModel.ModelEvent -= OnViewModelEvent;
            this.viewModel?.Stop();
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            viewModel.Stop();
            return base.OnBackButtonPressed();
        }


        private void OnViewModelEvent(object sender, ViewModelEventArgs e)
        {
            var curApp = ((App)Application.Current);
            switch (e.EventType)
            {
                case ViewModelEventEventTypes.NavigateTo:

                    switch ((e.dataObject as string))
                    {
                        case "HomePage":
                            Dispatcher.Dispatch(()=> Navigation.PopAsync(false));
                            break;
                    }
                    break;

                case OBDII.Models.ViewModelEventEventTypes.ScrollTo:
                    break;
            }
        }

    }
}