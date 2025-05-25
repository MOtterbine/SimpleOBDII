using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Localization
{
    public class LocalizationResourceManager : INotifyPropertyChanged
    {
        private LocalizationResourceManager()
        {
            Resources.Languages.Culture = CultureInfo.CurrentCulture;
        }

        public CultureInfo Culture
        {
            get => CultureInfo.CurrentUICulture;
            set
            {
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public static LocalizationResourceManager Instance { get; } = new();

        public object this[string resourceKey]
            => Resources.Languages.ResourceManager.GetObject(resourceKey, Culture) ?? Array.Empty<byte>();

        public event PropertyChangedEventHandler PropertyChanged;

    }

    /// <summary>
    /// xaml example:    <Label Text="{lang:LocText OBDII_PIDS}"></Label> where OBDII_PIDS is a resource name
    /// </summary>
    [ContentProperty(nameof(Key))]
    public class LocTextExtension : IMarkupExtension<BindingBase>
    {
        public string Key { get; set; } = string.Empty;

        public BindingBase ProvideValue(IServiceProvider serviceProvider) =>
            new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Key}]",
                Source = LocalizationResourceManager.Instance,
            };

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }

}
