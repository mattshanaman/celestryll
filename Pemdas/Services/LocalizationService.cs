using System.Globalization;

namespace Pemdas.Services
{
    public interface ILocalizationService
    {
        CultureInfo CurrentCulture { get; }
        void SetCulture(CultureInfo culture);
        List<CultureInfo> GetSupportedCultures();
    }

    public class LocalizationService : ILocalizationService
    {
        private readonly List<CultureInfo> _supportedCultures = new()
        {
            new CultureInfo("en"),    // English (default)
            new CultureInfo("es"),    // Spanish
            new CultureInfo("fr"),    // French
            new CultureInfo("de"),    // German
            new CultureInfo("it"),    // Italian
            new CultureInfo("pt"),    // Portuguese
            new CultureInfo("ja"),    // Japanese
            new CultureInfo("zh-Hans"), // Chinese Simplified
            new CultureInfo("ko"),    // Korean
            new CultureInfo("ar"),    // Arabic
            new CultureInfo("ru"),    // Russian
            new CultureInfo("hi"),    // Hindi
        };

        public CultureInfo CurrentCulture => CultureInfo.CurrentUICulture;

        public void SetCulture(CultureInfo culture)
        {
            if (culture == null)
                return;

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

#if ANDROID || IOS || MACCATALYST
            // Update resource manager for runtime culture switching
            Resources.Localization.AppResources.Culture = culture;
#endif
        }

        public List<CultureInfo> GetSupportedCultures()
        {
            return _supportedCultures;
        }
    }
}
