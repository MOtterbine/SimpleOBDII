using System;
using System.Collections.Generic;
using System.Text;


namespace OS.OBDII.Interfaces
{

    public interface ICustomButtonController : IViewController
    {
        void SendTouched();

        void SendLongClicked();
        void SendClicked();

        void SendReleased();
    }
}