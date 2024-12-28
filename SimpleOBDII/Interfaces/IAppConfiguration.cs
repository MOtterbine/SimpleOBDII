
namespace OS.OBDII.Interfaces;

public interface IAppConfiguration
{
    event EventHandler ConfigurationChanged;
    void NotifyConfigurationChanged();
}
