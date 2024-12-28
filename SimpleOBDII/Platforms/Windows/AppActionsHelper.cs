using System.Text;

namespace Microsoft.Maui.ApplicationModel;

static class AppActionsHelper
{
    const string AppActionPrefix = "XE_APP_ACTIONS-";

    public static string? GetAppActionId(string? arguments)
    {
        if (arguments?.StartsWith(AppActionPrefix) ?? false)
            return Encoding.Default.GetString(Convert.FromBase64String(arguments.Substring(AppActionPrefix.Length)));

        return default;
    }
}