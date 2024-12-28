using System;
using System.Collections.Generic;
using System.ComponentModel;
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using OS.OBDII.Views;
using OS.OBDII.ViewModels;
using OS.OBDII.Interfaces;
using OS.OBDII;

namespace OS.OBDII.Views;

// Learn more about making custom code visible in the Xamarin.Forms previewer
// by visiting https://aka.ms/xamarinforms-previewer
[DesignTimeVisible(false)]
public partial class CodesPage : ContentPage
{
    private IViewModel viewModel = null;
    private ResourceDictionary customStyles = null;

    public CodesPage()
    {
        AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "CodesPage..ctor");
        InitializeComponent();
        AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "InitializeComponent() called...");

        this.BindingContext = new CodesViewModel(AppShellModel.Instance);
        viewModel = (this.BindingContext as IViewModel);
    }

    /// <summary>
    /// For resetting the scrolled item view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DTCGrid_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        try 
        { 
            switch (e.PropertyName)
            {
                case "DTCCount":
                    //this.DTCScrollView.ScrollToAsync(0, 0, true);
                    break;
            }
        }
        catch(Exception)
        {

        }
    }

    private async Task<bool> DisplayAlert(string title, string prompt)
    {
        await DisplayAlert(title, prompt, "OK"); 
        return true;
    }

    protected override void OnAppearing()
    {
        AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "CodesPage.OnAppearing()");
        this.viewModel.NeedYesNoPopup += OnNeedYesNoPopup;
        this.viewModel.ModelEvent += this.OnViewModelEvent;
        //this.DTCGrid.PropertyChanged += DTCGrid_PropertyChanged;
        // Update the model when we come back to this page
        viewModel?.Start();
        base.OnAppearing();
    }

    private async Task<bool> OnNeedYesNoPopup(string title, string prompt, bool isYesNo = true, string okText = "Clear DTCs", string cancelText = "Cancel")
    {
        return await AppShellModel.Instance.ShowPopupAsync(new PopupInfo(title, prompt, isYesNo, okText, cancelText));
    }

    protected override void OnDisappearing()
    {
        AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "CodesPage.OnDisappearing()");
        this.viewModel.IsBusy = true;
      //  viewModel.Stop();
        this.viewModel.ModelEvent -= this.OnViewModelEvent;
        this.viewModel.NeedYesNoPopup -= OnNeedYesNoPopup;
        //this.DTCGrid.PropertyChanged -= DTCGrid_PropertyChanged;
        // Update the model when we come back to this page
        base.OnDisappearing();
    }

    protected override bool OnBackButtonPressed()
    {
        viewModel.Stop();
        return base.OnBackButtonPressed();
    }

    private void OnViewModelEvent(object sender, ViewModelEventArgs e)
    {
        switch (e.EventType)
        {
            case ViewModelEventEventTypes.NavigateTo:

                switch ((e.dataObject as string))
                {
                    case "HomePage":
                        Dispatcher.Dispatch(()=>Navigation.PopAsync(false));
                        break;
                    case "FreezeFramePage":
                        var curApp = ((App)Application.Current);
                        if (curApp.FreezeFramePage == null) curApp.FreezeFramePage = new FreezeFramePage();
                        Dispatcher.Dispatch(() => Navigation.PushAsync(curApp.FreezeFramePage, false));
                        break;
                }
                break;

            case ViewModelEventEventTypes.ScrollTo:
                break;
        }
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
       // (this.AdCtrl as IDisposable).Dispose();
    }
}