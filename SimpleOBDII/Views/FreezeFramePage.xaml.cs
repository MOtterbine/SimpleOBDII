using System;
using System.Collections.Generic;
using System.ComponentModel;
using OS.OBDII.ViewModels;
using OS.OBDII.Interfaces;
using OS.OBDII.Models;
using OS.OBDII.ViewModels;

namespace OS.OBDII.Views
{
    
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class FreezeFramePage : ContentPage
    {
        private IViewModel viewModel = null;
        public FreezeFramePage()
        {
            InitializeComponent();
            this.BindingContext = new FreezeFrameViewModel(AppShellModel.Instance);
            viewModel = (this.BindingContext as IViewModel);
        }

        private void LiveScrollView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                switch (e.PropertyName)
                {
                    case "IsVisible":
                       // this.LiveScrollView.ScrollToAsync(0, 0, true);
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        protected override bool OnBackButtonPressed()
        {
            viewModel.Back();
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.ModelEvent += this.OnViewModelEvent;
            viewModel?.Start();
        }

        protected override void OnDisappearing()
        {
            this.viewModel.ModelEvent -= this.OnViewModelEvent;
            viewModel.IsBusy = true;
            base.OnDisappearing();
        }

        private void OnViewModelEvent(object sender, ViewModelEventArgs e)
        {
            switch (e.EventType)
            {
                case ViewModelEventEventTypes.NavigateTo:

                    switch ((e.dataObject as string))
                    {
                        case "CodesPage":
                            Navigation.PopAsync(false);
                            break;
                    }
                    break;

                case ViewModelEventEventTypes.ScrollTo:
                    break;
            }
        }

    }
}