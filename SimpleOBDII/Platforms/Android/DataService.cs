using Newtonsoft.Json;
using OS.OBDII.Interfaces;
using OS.Communication;
using OS.OBDII.Models;
using System.Text;
using static Android.Provider.Settings;


namespace OS.OBDII.PartialClasses;

public partial class DataService : IPIDManager, IDataService
{

    private string pidsFileName = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath, Constants.USER_PIDS_JSON_FILE);
    private string testsFileName = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath, Constants.USER_TESTS_JSON_FILE);

    public T Load<T>(string fileName) where T : new()
    {
        // File.Delete(fileName);
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