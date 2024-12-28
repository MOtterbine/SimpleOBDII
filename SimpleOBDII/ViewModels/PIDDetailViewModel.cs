using System.Text;
using System.Windows.Input;
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using OS.OBDII.Interfaces;
using System.Collections.ObjectModel;
using Constants = OS.OBDII.Constants;

namespace OS.OBDII.ViewModels
{
    [QueryProperty("Add", "add")]
    [QueryProperty("IsEditing", "edit")]
    public class PIDDetailViewModel : BaseViewModel_AdSupport, IViewModel
    {
        public event ViewModelEvent ModelEvent;
        public event RequestPopup NeedYesNoPopup;

        private UserPID OriginalValues = null;

        private String OriginalCalculation = string.Empty;
        // Passed in via the route, happens after constructor 
        public bool Add
        {
            get => this.adding;
            set
            {
                adding = value;
                if (adding == true)
                {

                    if (AppShellModel.Instance.tempIPid == null)
                    {
                        this.TargetPID = new UserPID(0);
                    }
                    else
                    {
                        this.TargetPID = new UserPID(AppShellModel.Instance.tempIPid.Code);
                    }
                    AppShellModel.Instance.tempIPid = null; // throw away temp data
                    this.OriginalCalculation = this.CalcExpression = "A";
                    this.Description = this.Name = this.GenerateNewPIDName("New PID");
                    OnPropertyChanged("PIDString");
                    OnPropertyChanged("UnitDescriptor");
                    OnPropertyChanged("DecimalPlaces");
                    OnPropertyChanged("ResponseByteCount");
                }
            }
        }
        private bool adding = false;

        public PIDDetailViewModel(IOBDIICommonUI appShell) : base(appShell)
        {
            this.Add = false;

            if(Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_ADD))
            {
                this.Add = Preferences.Get(Constants.APP_PROPERTY_KEY_ADD, false);// = true;
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_ADD);
            }
            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_EDIT))
            {
                this.IsEditing = Preferences.Get(Constants.APP_PROPERTY_KEY_EDIT, false);
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_EDIT);
            }


            this.Init();
        }

        public ObservableCollection<IPid> UserPIDs
        {
            get => OBD2Device.SystemReport.UserPIDs;
        }

        //private void LoadUserPIDs()
        //{
        //    this.UserPids = new ObservableCollection<UserPID>(AppShellModel.Instance.LoadUserPIDs().ToList());
        //    // assign index values to 'Code' field..
        //    uint i = 0;
        //    foreach (UserPID pid in UserPids)
        //    {
        //        pid.Code = i++;
        //    }

        //}

        private void Init()
        {
            Start();

            //this.LoadUserPIDs();
 //           OBD2Device.SystemReport.LoadUserPIDs();

        }




        public override void Back()
        {
            this.Cancel();
            //Task.Run(this.Cancel);
        }


      //  private ObservableCollection<UserPID> UserPids = null;

        public string Description
        {
            get => this.TargetPID.Description;
            set
            {
                this.TargetPID.Description = value;
                OnPropertyChanged("Description");
            }
        }
        private string description = string.Empty;
        public string Name
        {
            get => this.TargetPID.Name;
            set
            {
                this.TargetPID.Name = value;
                OnPropertyChanged("Name");
                this.Description = value;
            }
        }

        public string UnitDescriptor
        {
            get => this.TargetPID.UnitDescriptor;
            set
            {
                // User PIDS don't care about which system is being used - set both to the user setting
                this.TargetPID.UnitDescriptor = value;
                this.TargetPID.EnglishUnitDescriptor = value;
                this.TargetPID.MetricUnitDescriptor = value;
                OnPropertyChanged("UnitDescriptor");
            }
        }
        private string unitDescriptor = string.Empty;
        public string PIDString
        {
            get => Encoding.ASCII.GetString(this.TargetPID.QueryBytes??new byte[] { });
            set
            {
                this.TargetPID.CommandString = value;
                OnPropertyChanged("PIDString");
            }
        }

        public string CalcExpression
        {
            get => calcExpression;
            set { SetProperty(ref calcExpression, value); }
        }
        private string calcExpression = string.Empty;

        public string CANID
        {
            get => cANID;
            set { SetProperty(ref cANID, value); }
        }
        private string cANID = Constants.DEFAULT_DIAG_FUNCADDR_CAN_ID_11.ToString("X");
        //public bool IsBroadcast
        //{
        //    get => isBroadcast;
        //    set { SetProperty(ref isBroadcast, value); }
        //}
        //private bool isBroadcast = false;

        public bool IsBroadcast
        {
            get => this.TargetPID.IsBroadcast;
            set
            {
                this.TargetPID.IsBroadcast = value;
                OnPropertyChanged("IsBroadcast");
            }
        }
        public bool CanCancel => !IsEditing;//this.adding;// && IsEditing;


        public int ResponseByteCount
        {
            get => this.TargetPID.ResponseByteCount;
            set
            {
                this.TargetPID.ResponseByteCount = value;
                OnPropertyChanged("ResponseByteCount");
            }
        }

        public int DecimalPlaces
        {
            get => this.TargetPID.DecimalPlaces;
            set 
            { 
                this.TargetPID.DecimalPlaces = value;
                OnPropertyChanged("DecimalPlaces");
            }
        }

        //public string StatusMessage
        //{
        //    get { return statusMessage; }
        //    set { SetProperty(ref statusMessage, value); }
        //}
        //private string statusMessage = string.Empty;

        protected SynchronizationContext syncContext;

        public ICommand CancelCommand => new Command(async () => {
            await Cancel();
        });//, ()=>CanCancel);

        private async Task Cancel()
        {
            if (this.IsEditing)
            {
                this.IsEditing = false;
                
                if (!this.Add)
                {
                    // An EDIT, so reload the last-saved pid values:
                    LoadTargetPID(this.TargetPID.Code);

                    return;
                }


                //if (this.ModelEvent != null)
                //{
                //    this.IsBusy = true;
                //    using (ViewModelEventArgs evt = new ViewModelEventArgs())
                //    {
                //        evt.EventType = ViewModelEventEventTypes.NavigateTo;
                //        evt.dataObject = "UserPIDSPage";
                //        this.ModelEvent(this, evt);
                //    }
                //}


                //// Just go back to the User PIDs list
                //var q = $"//Home/CustomLive";
                ////var q = $"{Shell.Current.CurrentState.Location}/CustomLive";
                //await Shell.Current.GoToAsync(q, false);
          //      return;
            }

            if (this.ModelEvent != null)
            {
                this.IsBusy = true;
                using (ViewModelEventArgs evt = new ViewModelEventArgs())
                {
                    evt.EventType = ViewModelEventEventTypes.NavigateTo;
                    evt.dataObject = "UserPIDSPage";
                    this.ModelEvent(this, evt);
                }
            }

            //var qu = $"//Home/CustomLive";
            ////var qu = $"//Home/CustomLive?new={this.Add}&code={this.TargetPID.Code}"; // this isn't used, but kept here for future reference
            //await Shell.Current.GoToAsync(qu, false);

        }


        public string EditSaveButtonText
        {
            get { return saveCancelButtonText; }
            set { SetProperty(ref saveCancelButtonText, value); }
        }
        private string saveCancelButtonText = "Edit";

        public string CancelButtonText => "Back";

        public bool CanCopy => !this.IsEditing && !this.Add;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                this.EditSaveButtonText = value ? "Done" : "Edit";
                SetProperty(ref isEditing, value);
                OnPropertyChanged("CanCopy");
                OnPropertyChanged("CanCancel");
                this.OriginalCalculation = this.TargetPID.CalcExpression;
            }
        }
        private bool isEditing = false;


        private bool canSend = false;
        public bool CanSend
        {
            get { return canSend; }
            set { SetProperty(ref canSend, value); }
        }

        private int newItemIndex = 0;
        public ICommand CopyToNewCommand => new Command(async () => {


            if (Constants.FORCE_LIMITS)
            {
                if (this.UserPIDs.Count >= Constants.APP_LIMIT_MAX_USER_PID_ROWS)
                {

                    if (this.NeedYesNoPopup == null)
                    {
                        this.StatusMessage = "Max User Pids Reached";
                        return;
                    }
                    await NeedYesNoPopup("Copy to New PID", $"This version is limited to {Constants.APP_LIMIT_MAX_USER_PID_ROWS} user-defined pids", false);

                    return;
                }
            }


            string tmpName = this.GenerateCopiedPIDName(this.TargetPID.Name);

            uint newCode = 1;
            if(this.UserPIDs?.Count > 0) newCode = this.UserPIDs.Max(p => p.Code) + 1;

            var copiedPID = new UserPID(newCode, this.TargetPID) { Name = tmpName, Description = tmpName };
            this.TargetPID = copiedPID;
            this.LoadTargetPID();
            this.adding = true;
            this.IsEditing = true;
           
            //this.UserPIDs.Add(copiedPID);
            //OBD2Device.SystemReport.SaveUserPIDs(this.UserPIDs.ToList().ConvertAll(p=>p as UserPID));
            //this.LoadTargetPID(newCode);

            //await Task.Delay(0);

        });

        private void LoadTargetPID()
        {
            if (this.TargetPID == null) return;
            this.CalcExpression = this.TargetPID.CalcExpression;
            this.Title = this.TargetPID.Name;
            this.CANID = this.TargetPID.CANID;
            this.IsBroadcast = this.TargetPID.IsBroadcast;
            OnPropertyChanged("Name");
            OnPropertyChanged("PIDString");
            OnPropertyChanged("UnitDescriptor");
            OnPropertyChanged("DecimalPlaces");
            OnPropertyChanged("ResponseByteCount");
            OnPropertyChanged("CanCancel");
        }


        private void LoadTargetPID(uint code)
        {

            this.TargetPID = this.UserPIDs.FirstOrDefault(p => p.Code == code) as UserPID;
            if (this.TargetPID == null) return;
            this.CalcExpression = this.TargetPID.CalcExpression;
            this.Title = this.TargetPID.Name;
            this.CANID = this.TargetPID.CANID;
            this.IsBroadcast = this.TargetPID.IsBroadcast;
            OnPropertyChanged("Name");
            OnPropertyChanged("PIDString");
            OnPropertyChanged("UnitDescriptor");
            OnPropertyChanged("DecimalPlaces");
            OnPropertyChanged("ResponseByteCount");
            OnPropertyChanged("CanCancel");

        }
        private string GenerateCopiedPIDName(string baseName)
        {
            int nIdx = 0;
            string startName = $"{baseName} Copy";
            string tmpName = string.Empty;
            do
            {
                tmpName = $"{startName} {(nIdx > 0 ? nIdx.ToString() : string.Empty)}";
                nIdx++;
            }
            while (this.UserPIDs.Any(p => string.Compare(p.Name, tmpName) == 0));
            return tmpName;
        }

        private string GenerateNewPIDName(string baseName)
        {
            string tmpName = string.Empty;
            do
            {
                tmpName = $"{baseName} {(newItemIndex > 0 ? newItemIndex.ToString() : string.Empty)}";
                newItemIndex++;
            }
            while (this.UserPIDs.Any(p => string.Compare(p.Name, tmpName) == 0));
            return tmpName;
        }

        /// <summary>
        /// This method will call the first queued command which should result in a bluetooth device event
        /// where the next queued command (if one exists) can be called
        /// </summary>
        public ICommand EditSaveCommand => new Command(async () => {
            if (this.IsEditing)
            {
                if (TargetPID == null) TargetPID = new UserPID((uint)this.UserPIDs.Count);
                uint pidCode = TargetPID.Code;
                try
                {
                    this.TargetPID.CalcExpression = this.calcExpression;
                    this.TargetPID.CANID = this.CANID;
                }
                catch (Exception)
                {

                    if (this.NeedYesNoPopup == null) return;
                    this.StatusMessage = "The calculation expression is invalid.";
                    var exit = await NeedYesNoPopup("Calculation Error", $"{this.StatusMessage}{Environment.NewLine}Reset Calculation?");
                    if (!exit)
                    {
                        return;
                    }
                //    if (this.Add)
                //    {
                        this.CalcExpression = OriginalCalculation;
                        //OnPropertyChanged("CalcExpression");
                //    }
                //    else
                //    {
                     //   this.LoadTargetPID(pidCode);
                //    }

                    return;

                }
                if (this.Add)
                {
                    this.UserPIDs.Add(this.TargetPID as UserPID);

                    OBD2Device.SystemReport.SaveUserPIDs(this.UserPIDs.ToList().ConvertAll(p => p as UserPID));

                    Preferences.Set(Constants.APP_PROPERTY_KEY_ADD, this.Add);
                    //Preferences.Set(Constants.USER_PIDS_VM_CODE, this.TargetPID.Code);


                    if (this.ModelEvent != null)
                    {
                        this.IsBusy = true;
                        using (ViewModelEventArgs evt = new ViewModelEventArgs())
                        {
                            evt.EventType = ViewModelEventEventTypes.NavigateTo;
                            evt.dataObject = "UserPIDSPage";
                            this.ModelEvent(this, evt);
                        }
                    }



                    //var qu = $"//Home/CustomLive?new={this.Add}&code={this.TargetPID.Code}"; // this isn't used, but kept here for future reference
                    ////                                                                         //var qu = $"{Shell.Current.CurrentState.Location}/CustomLive?code={this.TargetPID.Code}";
                    //Shell.Current.GoToAsync(qu, false);

                    return;

                }
                OBD2Device.SystemReport.SaveUserPIDs(this.UserPIDs.ToList().ConvertAll(p => p as UserPID));

                this.LoadTargetPID(pidCode);
                this.IsEditing = false;

            }
            else
            {
                this.IsEditing = true;
            }
        });


      //  private UserPID TargetPID = new UserPID(string.Empty, string.Empty, 1, string.Empty, null) { ResponseByteCount = 1 };
        private UserPID TargetPID = new UserPID() { ResponseByteCount = 1 };
        public void Start()
        {

            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_ADD))
            {
                this.Add = Preferences.Get(Constants.APP_PROPERTY_KEY_ADD, false);// = true;
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_ADD);
            }
            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_EDIT))
            {
                this.IsEditing = Preferences.Get(Constants.APP_PROPERTY_KEY_EDIT, false);
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_EDIT);
            }

            // View/edit an existing pid, so we need to load it
            if (AppShellModel.Instance.tempIPid != null && !this.Add)
            {
                LoadTargetPID(AppShellModel.Instance.tempIPid.Code);

                // Remove the temp pid object
                AppShellModel.Instance.tempIPid = null;
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

            AppShellModel.Instance.SendHapticFeedback();

        }
        public void Stop()
        {
            this.CloseCommService();
        }

    }
}



