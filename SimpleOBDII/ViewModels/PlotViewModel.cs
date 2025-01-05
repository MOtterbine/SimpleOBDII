using OS.OBDII.ViewModels;
using OS.OBDII.Models;
using OS.OBDII.Interfaces;
using System.Windows.Input;
using Constants = OS.OBDII.Constants;

namespace OS.OBDII.ViewModels
{
    public class PlotViewModel : BaseViewModel_AdSupport, IViewModel
    {
        public event ViewModelEvent ModelEvent;
        public event RequestPopup NeedYesNoPopup;

        public PlotViewModel(IOBDIICommonUI appShell) : base(appShell)
        {

            Title = "Data Plots";

        }


        #region NavigateHomeCommand

        public ICommand NavigateHomeCommand => new Command(() => {

            if (this.ModelEvent != null)
            {
                this.IsBusy = true;
                using (ViewModelEventArgs evt = new ViewModelEventArgs())
                {
                    evt.EventType = ViewModelEventEventTypes.NavigateTo;
                    evt.dataObject = "HomePage";
                    this.ModelEvent(this, evt);
                }
            }
        });

        #endregion NavigateHomeCommand





        /// <summary>
        /// This method will call the first queued command which should result in a bluetooth device event
        /// where the next queued command (if one exists) can be called
        /// </summary>



        public void Start()
        {
            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_INITIAL_VIEW))
            {
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_INITIAL_VIEW);
            }

            this.IsBusy = false;
        }
        public void Initialize()
        {
            this.IsBusy = false;
        }

        // to satisfy IViewModel
        public void CloseCommService()
        {

            _appShellModel.SendHapticFeedback();

        }
        public void Stop()
        {
        }

    }
}
