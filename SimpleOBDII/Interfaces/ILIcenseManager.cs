using System;
using System.Threading.Tasks;

namespace OS.OBDII.Interfaces
{
    public interface ILicenseManager
    {
        void SaveAppInstallCode(string code);
        string GetAppId();
    }

}
