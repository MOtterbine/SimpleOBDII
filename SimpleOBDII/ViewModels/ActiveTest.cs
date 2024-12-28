using Newtonsoft.Json;
using OS.OBDII.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace OS.OBDII.Models
{

    public class ActiveTest : BaseViewModel
    {

        public ActiveTest(IList<ActiveTestCommand> commands)
        {
            foreach(ActiveTestCommand cmd in commands)
            {
                this.Tests.Add(cmd);
            }
        }
        public ActiveTest()
        {
            
        }

        public override string ToString()
        {
            return $"{Name} ECU:{ECUID}";
        }
       // public ObservableCollection<ActiveTestCommand> Tests { get; set; }
        public ObservableCollection<ActiveTestCommand> Tests
        {
            get { return tests; }
            set { SetProperty(ref tests, value); }
        }
        private ObservableCollection<ActiveTestCommand> tests = new ObservableCollection<ActiveTestCommand>();

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        private string name = string.Empty;
        public string ECUID
        {
            get { return eCUID; }
            set { SetProperty(ref eCUID, value); }
        }
        private string eCUID = string.Empty;

        public string ToolPresentMsg
        {
            get { return toolPresentMsg; }
            set { SetProperty(ref toolPresentMsg, value); }
        }
        private string toolPresentMsg = string.Empty;

        public bool UseToolPresentMsg
        {
            get { return useToolPresentMsg; }
            set { SetProperty(ref useToolPresentMsg, value); }
        }
        private bool useToolPresentMsg = true;


        public string QueryStateMsg
        {
            get { return queryStateMsg; }
            set { SetProperty(ref queryStateMsg, value); }
        }
        private string queryStateMsg = string.Empty;

        public bool UseQueryStateMsg
        {
            get { return useQueryStateMsg; }
            set { SetProperty(ref useQueryStateMsg, value); }
        }
        private bool useQueryStateMsg = true;



    }
}




