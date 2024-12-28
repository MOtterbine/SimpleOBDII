
using System.Reflection;


namespace OS.OBDII
{
    public static class VersionInfo
    {
        public static string AppName
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().Name;
            }
        }
        public static string AppVersion
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().Version.ToString();
            }
        }

        static public System.Reflection.AssemblyName AssemblyVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName();
            }
        }





    }
}
