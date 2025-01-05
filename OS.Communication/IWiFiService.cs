
namespace OS.Communication
{
    public interface IWiFiService 
    {
        bool IsEnabled { get; }
        List<String> SSIDList { get; }
    }
}
