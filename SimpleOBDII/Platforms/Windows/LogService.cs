using Microsoft.Extensions.Logging;
using OS.OBDII.Interfaces;


namespace OS.OBDII.PartialClasses;

public partial class LogService : ILogService
{

    public LogService()
    {
        this.logFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.LOG_FILE_PATH);
    }


    public void AppendLog(LogLevel level, string logText)
    {
        try
        {

            AppendFile($"[{DateTime.Now.ToString("MM/dd/yyyy hh:mmtt")}] {level}: {logText}");
            //AppendFile($"[{DateTime.Now.ToString("MM/dd/yyyy hh:mmtt")}] {level}: {logText}{Environment.NewLine}");

        }
        catch (Exception e)
        {
            // File.AppendAllText(logFileName, $"[{DateTime.Now.ToString("MM/dd/yyyy hh:mmtt")}] Error: {e.Message}{Environment.NewLine}", Encoding.UTF8);
            //Android.Util.Log.Error(Constants.STRING_LOG_TAG, $"LogService.AppendLog(...) - {e.Message}");
        }
    }




}