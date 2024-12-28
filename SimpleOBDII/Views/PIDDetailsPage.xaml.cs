
using System.ComponentModel;
using OS.OBDII.Models;
using OS.OBDII.Views;
using OS.OBDII.ViewModels;
using OS.OBDII.Views.Styles;
using OS.OBDII.Interfaces;
using Constants = OS.OBDII.Constants;

namespace OS.OBDII.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class PIDDetailsPage : ContentPage
    {
        //private PIDDetailViewModel viewModel = null;
        private IViewModel viewModel = null;
        private ResourceDictionary customStyles = null;

        public PIDDetailsPage()
        {
            AssignStyles();
            InitializeComponent();
            this.BindingContext = new PIDDetailViewModel(AppShellModel.Instance);
            viewModel = (this.BindingContext as IViewModel);
        }
        

        void AssignStyles()
        {
            bool isSmall = AppShellModel.Instance.DeviceScreenDPI > Constants.STYLES_BREAKPOINT_0;

            if (isSmall)
            {
                customStyles = new Styles_sm();
                if (!Resources.ContainsKey((typeof(Styles_sm)).Name))
                {
                    Resources.Add(customStyles);
                }
            }
            else
            {
                customStyles = new Styles_lg();
                if (!Resources.ContainsKey((typeof(Styles_lg)).Name))
                {
                    Resources.Add(customStyles);
                }
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
            this.viewModel.NeedYesNoPopup += OnNeedYesNoPopup;
            this.viewModel.ModelEvent += this.OnViewModelEvent;
            // Update the model when we come back to this page
            viewModel?.Start();
        }

        protected override void OnDisappearing()
        {
            this.viewModel.ModelEvent -= this.OnViewModelEvent;
            this.viewModel.NeedYesNoPopup -= OnNeedYesNoPopup;
            // Update the model when we come back to this page
            this.viewModel.IsBusy = true;
            base.OnDisappearing();
        }



        private async Task<bool> ShowPopup(PopupInfo popupInfo)
        {
            return await AppShellModel.Instance.ShowPopupAsync(popupInfo);
        }

        private async Task<bool> OnNeedYesNoPopup(string title, string prompt, bool isYesNo = true, string okText = "Ok", string cancelText = "Cancel")
        {
            return await AppShellModel.Instance.ShowPopupAsync(new PopupInfo(title, prompt, isYesNo, okText, cancelText));
        }




        private void OnViewModelEvent(object sender, ViewModelEventArgs e)
        {
            switch (e.EventType)
            {
                case ViewModelEventEventTypes.NavigateTo:

                    switch ((e.dataObject as string))
                    {
                        case "UserPIDSPage":
                            Application.Current.Dispatcher.Dispatch(()=>Navigation.PopAsync(false));
                            break;
                    }
                    break;

                case ViewModelEventEventTypes.ScrollTo:

                    break;
            }
        }

    }
}