using Microsoft.Extensions.Logging;
using OS.OBDII.Interfaces;


namespace OS.OBDII.PartialClasses;
using Constants = OS.OBDII.Constants;

public partial class LogService : ILogService
{

    ILogger logger;// = factory.CreateLogger("Program");
    public LogService()
    {
      //  this.logFileName = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath, Constants.LOG_FILE_PATH);
    }

    public void AppendLog(LogLevel level, string logText)
    {
        try
        {
            switch (level)
            {
                case LogLevel.Information:
                    Android.Util.Log.Info(Constants.STRING_LOG_TAG, logText);
                    break;
                case LogLevel.Debug:
                    Android.Util.Log.Debug(Constants.STRING_LOG_TAG, logText);
                    break;
                case LogLevel.Warning:
                    Android.Util.Log.Warn(Constants.STRING_LOG_TAG, logText);
                    break;
                case LogLevel.Error:
                    Android.Util.Log.Error(Constants.STRING_LOG_TAG, logText);
                    break;
                default:
                    Android.Util.Log.Info(Constants.STRING_LOG_TAG, logText);
                    break;

            }

           // AppendFile($"[{DateTime.Now.ToString("MM/dd/yyyy hh:mmtt")}] {level}: {logText}");

        }
        catch (Exception e)
        {
            // File.AppendAllText(logFileName, $"[{DateTime.Now.ToString("MM/dd/yyyy hh:mmtt")}] Error: {e.Message}{Environment.NewLine}", Encoding.UTF8);
            Android.Util.Log.Error(Constants.STRING_LOG_TAG, $"LogService.AppendLog(...) - {e.Message}");
        }
    }



}