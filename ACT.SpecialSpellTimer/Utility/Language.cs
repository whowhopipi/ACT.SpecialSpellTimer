namespace ACT.SpecialSpellTimer.Utility
{
    internal class Language
    {
        public string FriendlyName { get; set; }
        public string Value { get; set; }

        public static Language[] GetLanguageList()
        {
            return new Language[]
            {
                new Language { FriendlyName = "English", Value = "EN" },
                new Language { FriendlyName = "日本語", Value = "JP" },
                new Language { FriendlyName = "한국어", Value = "KR" },
            };
        }

        public override string ToString()
        {
            return FriendlyName;
        }
    }
}