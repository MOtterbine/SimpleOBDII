using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using OS.OBDII.Models;

namespace OS.OBDII.Interfaces
{

    public interface IDataService
    {
        T Load<T>(string fileName) where T : new();
        void Save<T>(string fileName, T data);

    }

}
