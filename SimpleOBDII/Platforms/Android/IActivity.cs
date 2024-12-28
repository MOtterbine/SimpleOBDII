using System;
using System.Threading.Tasks;
using static Microsoft.Maui.LifecycleEvents.AndroidLifecycle;

namespace OS.OBDII.PartialClasses
{
    public delegate void OnConfigurationChanged(Android.Content.Res.Configuration config);

    public interface IActivity
    {
        event EventHandler Paused;
        event EventHandler Resumed;
        event OS.OBDII.PartialClasses.OnConfigurationChanged ConfigurationChanged;

    }
}
