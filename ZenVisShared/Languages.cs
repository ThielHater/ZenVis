namespace ZenVis.Shared
{
    public static class Languages
    {
        public static Language Czech;

        public static Language Polish;

        public static Language Romanian;

        public static Language Russian;

        public static Language English;

        public static Language German;

        public static Language Italian;

        public static Language Spanish;

        public static Language Undefined;

        static Languages()
        {
            Languages.Czech = new Language("CS", 1250);
            Languages.Polish = new Language("PL", 1250);
            Languages.Romanian = new Language("RO", 1250);
            Languages.Russian = new Language("RU", 1251);
            Languages.English = new Language("EN", 1252);
            Languages.German = new Language("DE", 1252);
            Languages.Italian = new Language("IT", 1252);
            Languages.Spanish = new Language("ES", 1252);
            Languages.Undefined = new Language("XX", 65001);
        }
    }
}