using System.Collections.Generic;
using System.Globalization;
using ZenVisShared.Properties;

namespace ZenVis.Shared
{
    public class Localization
    {
        public static Language CurrentLanguage = (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "de" ? Languages.German : Languages.English);

        private static Localization _instance;

        private static volatile object _lock;

        private Dictionary<string, Dictionary<Language, string>> _translations;

        public static Localization Instance
        {
            get
            {
                Localization localization;
                lock (Localization._lock)
                {
                    Localization localization1 = Localization._instance;
                    if (localization1 == null)
                    {
                        localization1 = new Localization();
                        Localization._instance = localization1;
                    }
                    localization = localization1;
                }
                return localization;
            }
        }

        static Localization()
        {
            Localization._lock = new object();
        }

        private Localization()
        {
            this._translations = Helper.DeserializeFromByteArray<Dictionary<string, Dictionary<Language, string>>>(Resources.Localization);
        }

        public string GetTranslation(string key, Language lang = null)
        {
            if (lang == null)
            {
                lang = Localization.CurrentLanguage;
            }
            if (!this._translations.ContainsKey(key) || string.IsNullOrEmpty(this._translations[key][lang]))
            {
                return "Translation missing!";
            }
            return this._translations[key][lang];
        }
    }
}