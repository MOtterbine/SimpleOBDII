using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.OBDII.Communication
{
    public interface ICommunicationDevice
    {

        #region Events

        event DeviceEvent CommunicationEvent;

        #endregion Events

        #region Properties and Fields

        //        string MessageString { get; set; }
        bool IsConnected { get; }

        /// <summary>
        /// General description
        /// </summary>
        string Description
        {
            get;
        }
        /// <summary>
        /// Device Name or Model...
        /// </summary>
        string DeviceName
        {
            get;
            set;
        }

        #endregion  Properties and Fields

        #region  Methods

        bool Initialize();
        bool Open();
        Task<bool> Send(string text);
        Task<bool> Send(byte[] buffer, int offset, int count);
        /// <summary>
        /// Method is used to close the entire channel (client or listener)
        /// </summary>
        /// <returns></returns>
        bool Close();

        #endregion  Methods

    }
}
