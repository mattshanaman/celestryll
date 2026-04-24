using System.ComponentModel;
using System.Globalization;

namespace Pemdas.Resources.Localization
{
    public class LocalizationResourceManager : INotifyPropertyChanged
    {
        private LocalizationResourceManager()
        {
            AppResources.Culture = CultureInfo.CurrentCulture;
        }

        public static LocalizationResourceManager Instance { get; } = new();

        public string this[string resourceKey] =>
            AppResources.ResourceManager.GetObject(resourceKey, AppResources.Culture)?.ToString() ?? string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void SetCulture(CultureInfo culture)
        {
            AppResources.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
