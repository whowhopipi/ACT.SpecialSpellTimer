namespace XIVDBDownloader.Constants
{
    public enum Language
    {
        EN = 0,
        JA,
        FR,
        DE
    }

    public static class LanguageExtensions
    {
        public static string ToLocale(
            this Language language)
        {
            switch (language)
            {
                case Language.EN: return "en-US";
                case Language.JA: return "ja-JP";
                case Language.FR: return "fr-FR";
                case Language.DE: return "de-DE";
                default: return "en-US";
            }
        }
    }
}
