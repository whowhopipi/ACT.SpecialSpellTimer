using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.SpecialSpellTimer.Utility
{
    class Language
    {
        public string FriendlyName { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return FriendlyName;
        }

        public static Language[] GetLanguageList()
        {
            return new Language[] {
                new Language { FriendlyName = "English", Value = "EN" },
                new Language { FriendlyName = "日本語", Value = "JP" },
                new Language { FriendlyName = "한국어", Value = "KR" },
            };
        }
    }
}
