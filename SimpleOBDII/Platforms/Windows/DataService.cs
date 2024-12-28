using Newtonsoft.Json;
using OS.OBDII.Interfaces;
using OS.OBDII.Models;
using OS.OBDII;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace OS.OBDII.PartialClasses;

public partial class DataService : IPIDManager, IDataService
{

    private string pidsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.USER_PIDS_JSON_FILE);
    private string testsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.USER_TESTS_JSON_FILE);

    public T Load<T>(string fileName) where T : new()
    {
        bool doesExist = File.Exists(fileName);
        if (File.Exists(fileName))
        {
            string jsonData = File.ReadAllText(fileName);
            var f = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
            return f;
        }
        return new T();
    }

    public void Save<T>(string fileName, T data)
    {
        try
        {
            var str = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(fileName, str, Encoding.UTF8);
        }
        catch (Exception e)
        {
            var p = e.Message;
        }
    }

    T IDataService.Load<T>(string fileName)
    {
        // was not able to run this method as async, therefore this hack
        evt.Reset();
        LoadJsonDataFromFile(fileName);
        evt.WaitOne();
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
    }
    ManualResetEvent evt = new ManualResetEvent(true);
    string jsonData = "";
    private async Task LoadJsonDataFromFile(string fileName)
    {
        jsonData = await LoadMauiAsset(fileName);
        evt.Set();

    }

    /// <summary>
    /// From the Resources\Raw folder
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private async Task<string> LoadMauiAsset(string fileName)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    public IList<UserPID> LoadUserPIDs()
    {
        return Load<List<UserPID>>(pidsFileName);
    }
    public void SaveUserPIDs(IList<UserPID> pids)
    {
        Save(pidsFileName, pids);
    }

    public IList<ActiveTest> LoadActiveTests()
    {
        return Load<List<ActiveTest>>(testsFileName);
    }

    public void SaveActiveTests(IList<ActiveTest> tests)
    {
        Save(testsFileName, tests);
    }

}