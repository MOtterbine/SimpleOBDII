using OS.OBDII.Interfaces;
using OS.OBDII.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OS.OBDII.Models
{
    public class SystemReport : BaseViewModel
    {
        private IPIDManager _PIDManager = null;

        public SystemReport(IPIDManager pidManager)
        {
            // handles platform-specific file handling
            this._PIDManager = pidManager;
            // must be called only after pidmanager is instantiated.
            //this.LoadUserPIDs();

        }


        public ObservableCollection<IPid> UserPIDs
        {
            get => this.userPIDs;
            set => SetProperty(ref userPIDs, value);
        }
        private ObservableCollection<IPid> userPIDs = new ObservableCollection<IPid>();

        public bool LoadUserPIDs(uint index = 0, int count = 0)
        {
            if (count < 0) return false;
            try
            {
                if (userPIDs == null) this.userPIDs = new ObservableCollection<IPid>();
                // assign index values to 'Code' field..
                uint curIdx = index;
                uint stIdx = index;
                this.UserPIDs.Clear();

                var rawPids = ((List<UserPID>)this._PIDManager.LoadUserPIDs()).ToArray();

                if (curIdx > rawPids.Length - 1) return false;
                if (count > 0)
                {
                    // while the index is in bounds and until the count is fulfilled
                    for (; (curIdx < rawPids.Length) && (curIdx - stIdx < count); curIdx++)
                    {
                        rawPids[curIdx].Code = curIdx - stIdx;
                        this.UserPIDs.Add(rawPids[curIdx]);
                    }
                    return true;
                }

                // while the index is in bounds - all the available data
                for (; curIdx < rawPids.Length ; curIdx++)
                {
                    rawPids[curIdx].Code = curIdx - stIdx;
                    this.UserPIDs.Add(rawPids[curIdx]);
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        public void SaveUserPIDs(IList<UserPID> pids)
        {
            this._PIDManager.SaveUserPIDs(pids);
        }
        //public ObservableCollection<IActiveTestCommand> UserTests
        //{
        //    get => this.userTests;
        //    set => SetProperty(ref userTests, value);
        //}
        //private ObservableCollection<IActiveTestCommand> userTests = new ObservableCollection<IActiveTestCommand>();

        public ObservableCollection<ActiveTest> ActiveTests// = new ObservableCollection<ActiveTest>();
        {
            get => this.activeTests;
            set => SetProperty(ref activeTests, value);
        }
        private ObservableCollection<ActiveTest> activeTests = new ObservableCollection<ActiveTest>();

        public bool LoadActiveTests()//uint index = 0, int count = 0)
        {
           // if (count < 0) return false;
            try
            {
               // if (userTests == null) this.userTests = new ObservableCollection<IActiveTestCommand>();
                // assign index values to 'Code' field..
              //  uint curIdx = index;
              //  uint stIdx = index;

                var testobjects = ((List<ActiveTest>)this._PIDManager.LoadActiveTests());
                if (testobjects == null || testobjects.Count < 1)
                {
                    return false;
                }
                //      if (testobjects[0].Tests == null) return false;

                //   this.userTests.Clear();
                try
                {
                    this.ActiveTests.Clear();
                }
                catch(Exception)
                { }
                testobjects.ForEach((test) => {
                    this.ActiveTests.Add(test);
                });
                OnPropertyChanged("ActiveTests");
                
                //var objectList = ActiveTests[0].Tests.ToArray();

                //if (curIdx > objectList.Length - 1) return false;
                //if (count > 0)
                //{
                //    // while the index is in bounds and until the count is fulfilled
                //    for (; (curIdx < objectList.Length) && (curIdx - stIdx < count); curIdx++)
                //    {
                //        objectList[curIdx].Code = curIdx - stIdx;
                //        this.UserTests.Add(objectList[curIdx]);
                //    }
                //    return true;
                //}

                //// while the index is in bounds - all the available data
                //for (; curIdx < objectList.Length; curIdx++)
                //{
                //    objectList[curIdx].Code = curIdx - stIdx;
                //    this.UserTests.Add(objectList[curIdx]);
                //}
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public void SaveActiveTests(IList<ActiveTest> test)
        {
            this._PIDManager.SaveActiveTests(test);
        }






        public bool HasBank1
        {
            get => this.hasBank1;
            set => SetProperty(ref hasBank1, value);
        }
        private bool hasBank1 = false;

        public bool HasBank2
        {
            get => this.hasBank2;
            set => SetProperty(ref hasBank2, value);
        }
        private bool hasBank2 = false;

        public bool HasBank3
        {
            get => this.hasBank3;
            set => SetProperty(ref hasBank3, value);
        }
        private bool hasBank3 = false;

        public bool HasBank4
        {
            get => this.hasBank4;
            set => SetProperty(ref hasBank4, value);
        }
        private bool hasBank4 = false;

        public List<FuelSystemStatus> FuelSystemsStatus = new List<FuelSystemStatus>();

        public uint FreezeFrameDTC
        {
            get => this.freezeFrameDTC;
            set => SetProperty(ref freezeFrameDTC, value);
        }
        private uint freezeFrameDTC = 0;
        public int DTCCount
        {
            get => this.dTCCount;
            set => SetProperty(ref dTCCount, value);
        }
        private int dTCCount = 0;

        public string VIN
        {
            get => this.vIN;
            set => SetProperty(ref vIN, value);
        }
        private string vIN = string.Empty;

        public List<DTCTransportData> DTCList
        {
            get => this.dTCList;
            set => SetProperty(ref dTCList, value);
        }
        private List<DTCTransportData> dTCList = new List<DTCTransportData>();

        public String ModelName
        {
            get => this.modelName;
            set => SetProperty(ref modelName, value);
        }
        private String modelName = Constants.STRING_NOT_SET;

        public string SystemProtocolDescription
        {
            get { return systemProtocolDescription; }
            set { SetProperty(ref systemProtocolDescription, value); }
        }
        private string systemProtocolDescription = Constants.STRING_NO_PROTOCOL;


        public IVehicleModel SelectedManufacturer
        {
            get { return selectedManufacturer; }
            set { SetProperty(ref selectedManufacturer, value); }
        }
        private IVehicleModel selectedManufacturer = null;// Preferences.Default.Get(Constants.PREFS_KEY_MANUFACTURER, _manufacturerList.Items[0]);;


        //public static List<IManufacturer> ManufacturerList { get; } = new Manufacturers.Manufacturers().Items;
        //public static Manufacturers.Manufacturers ManufacturerList { get; };

        public void Clear(bool clearVIN = true)
        {
            if(clearVIN) this.VIN = "00000000000000000000";
            //this.VIN = string.Empty;
            //this.DTCCount = 0;
            //this.DTCCodes.ToList().ForEach(l => l.Clear());

            this.freezeFrameDTC = 0x0000;
            this.dTCCount = 0;
            this.dTCList.Clear();
        }
    }
}
