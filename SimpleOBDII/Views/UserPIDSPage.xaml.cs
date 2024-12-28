
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using OS.OBDII.Views;
using OS.OBDII.Controls;
using System.ComponentModel;
using OS.OBDII.Interfaces;
using OS.OBDII;


namespace OS.OBDII.Views
{
    
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class UserPIDSPage : ContentPage
    {
        private IEditableViewModel viewModel = null;

        public UserPIDSPage()
        {
            InitializeComponent();
            this.SelectionGrid.RequestToEditItem += OnItemEditRequest;

            this.BindingContext = new UserPIDSViewModel(AppShellModel.Instance);
            viewModel = (this.BindingContext as IEditableViewModel);
        }

        private void ContentView_ChildRemoved(object sender, ElementEventArgs e)
        {
            (e.Element as IDisposable)?.Dispose();
        }


        private void OnItemEditRequest(object sender, SelectedItemChangedEventArgs e)
        {
            //                       SelectedItem="{Binding Mode=OneWayToSource, Source={RelativeSource Mode=FindAncestor,
            //                       AncestorType={x:Type local:Views.Controls.LiveDataSelectionGrid} }, Path=SelectedItem}"
            // this.SelectionGrid.RemoveBinding(LiveDataSelectionGrid.SelectedItemProperty);
            //Binding b = new Binding("SelectedItem");
            //b.Source = this.SelectionGrid.SelectedItem;
            //b.Mode= BindingMode.OneWay;
            //this.SelectionGrid.SetBinding(LiveDataSelectionGrid.SelectedItemProperty, b );




            var bbbb = e.SelectedItem;
            viewModel.Edit(bbbb);
        }

        private void OnViewModelEvent(object sender, ViewModelEventArgs e)
        {

            switch (e.EventType)
            {
                case ViewModelEventEventTypes.NavigateTo:

                    switch ((e.dataObject as string))
                    {
                        case "LiveHomePage":
                            Navigation.PopAsync(false);
                            break;
                        case "PIDDetailsPage":
                            var curApp = ((App)Application.Current);
                            if (curApp.PIDDetailsPage == null) curApp.PIDDetailsPage = new PIDDetailsPage();
                            Navigation.PushAsync(curApp.PIDDetailsPage, false);
                            break;
                    }
                    break;
                case ViewModelEventEventTypes.ScrollTo:
                    if (!(e.dataObject is IPid)) break;
                    // which control?
                    switch (e.Data[0])
                    {
                        case 0:
                            this.SelectionGrid.ScrollTo(e.dataObject, ScrollToPosition.MakeVisible, true);
                            break;
                        case 1:
                            //this.SelectionGrid.SelectedItem = e.dataObject as IPid;
                                this.SelectionGrid.ScrollTo(e.dataObject, ScrollToPosition.MakeVisible, true);
                            break;
                        case 0xFF:
                                this.SelectionGrid.ScrollTo(e.dataObject, ScrollToPosition.MakeVisible, true);
                            break;
                    }
                    break;
                case ViewModelEventEventTypes.RequestYesNoPopup:
                    break;
            }

        }
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
        }



        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
       }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.NeedYesNoPopup += OnNeedYesNoPopup;
            this.viewModel.ModelEvent += OnViewModelEvent;

            viewModel?.Start();
       }
        protected override void OnDisappearing()
        {
            this.viewModel.NeedYesNoPopup -= OnNeedYesNoPopup;
            this.viewModel.ModelEvent -= OnViewModelEvent;

            viewModel.IsBusy = true;
           // viewModel.Stop();
            base.OnDisappearing();
        }

        private async Task<bool> OnNeedYesNoPopup(string title, string prompt, bool isYesNo = true, string okText = "Yes", string cancelText = "No")
        {
            return await AppShellModel.Instance.ShowPopupAsync(new PopupInfo(title, prompt, isYesNo, okText, cancelText));
        }
        protected override bool OnBackButtonPressed()
        {
            viewModel.Stop();
            return true; // It's been handled
        }

        private void LivePlotsGrid_BindingContextChanged(object sender, EventArgs e)
        {
            var s = (sender as LivePlotsGrid);
         //   s.ColumnSpacing = 3;
            //s.WidthRequest = 500;
            //s.WidthRequest = 400;
           

           // s.HeightRequest = 200;
        }
    }
}