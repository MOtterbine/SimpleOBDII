using System;
using System.Collections.Generic;
using System.Text;
using OS.OBDII.Models;

namespace OS.OBDII.Interfaces
{
    public interface IPIDManager
    {
        void SaveUserPIDs(IList<UserPID> pids);
        IList<UserPID> LoadUserPIDs();
        void SaveActiveTests(IList<ActiveTest> tests);
        IList<ActiveTest> LoadActiveTests();

    }
}
