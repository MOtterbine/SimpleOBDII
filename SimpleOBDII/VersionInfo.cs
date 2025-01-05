
using System.Reflection;


namespace OS.OBDII
{
    public static class VersionInfo
    {

        public static string AppName
        {
            get
            {
                return AppInfo.Current.Name;
            }
        }

        public static string AppVersion
        {
            get
            {
               // return $"{AppInfo.Current.Version.Major}.{AppInfo.Current.Version.MajorRevision}.{AppInfo.Current.Version.Minor}.{AppInfo.Current.Version.MinorRevision}";
                return AppInfo.Current.VersionString;
            }
        }

        public static string AssemblyName
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

    }
}
