using System.Text;

namespace OS.OBDII.PartialClasses;

public partial class LogService
{
    protected string logFileName = string.Empty;

    public void AppendFile(string logText)
    {
        try
        {
            var t = new FileInfo(logFileName);
            if (t.Exists && t.Length > OS.OBDII.Constants.MAX_LOG_FILE_LENGTH)
            {
                var lines = File.ReadAllLines(logFileName);
                File.WriteAllLines(logFileName, lines.Take(new Range(1, lines.Length - 1)).ToArray());
            }
            File.AppendAllText(logFileName, $"{logText}{Environment.NewLine}", Encoding.UTF8);
        }
        catch (Exception e)
        {
            File.AppendAllText(logFileName, $"[{DateTime.Now.ToString("MM/dd/yyyy hh:mmtt")}] Error: {e.Message}{Environment.NewLine}", Encoding.UTF8);
            var p = e.Message;
        }
    }

}