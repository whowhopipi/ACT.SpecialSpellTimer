using System.Collections.Concurrent;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;

namespace ACT.SpecialSpellTimer.Models
{
    public static class PCNameDictionaryExtensions
    {
        public static string ChangeNameStyle(
            this string name,
            NameStyles style)
        {
            return PCNameDictionary.Instance.ChangeStyle(name, style);
        }

        public static string ChangeNameStyle(
            this string name)
        {
            return PCNameDictionaryExtensions.ChangeNameStyle(name, Settings.Default.PCNameInitialOnLogStyle);
        }

        public static string ReplaceName(
            this string name,
            NameStyles style)
        {
            return PCNameDictionary.Instance.Replace(name, style);
        }

        public static string ReplaceName(
            this string name)
        {
            return PCNameDictionaryExtensions.ReplaceName(name, Settings.Default.PCNameInitialOnDisplayStyle);
        }
    }

    public class PCNameDictionary
    {
        #region Singleton

        private static PCNameDictionary instance = new PCNameDictionary();

        private PCNameDictionary()
        {
        }

        public static PCNameDictionary Instance => instance;

        #endregion Singleton

        /// <summary>Naoki Y.をキーにした辞書</summary>
        private ConcurrentDictionary<string, PCName> namesByFI = new ConcurrentDictionary<string, PCName>();

        /// <summary>フルネームをキーにした辞書</summary>
        private ConcurrentDictionary<string, PCName> namesByFull = new ConcurrentDictionary<string, PCName>();

        /// <summary>N. Yoshidaをキーにした辞書</summary>
        private ConcurrentDictionary<string, PCName> namesByIF = new ConcurrentDictionary<string, PCName>();

        /// <summary>N. Y.をキーにした辞書</summary>
        private ConcurrentDictionary<string, PCName> namesByII = new ConcurrentDictionary<string, PCName>();

        public void Add(
            Combatant combatant)
        {
            if (combatant.MobType != MobType.Player)
            {
                return;
            }

            var pcName = new PCName()
            {
                Name = combatant.Name,
                NameFI = combatant.NameFI,
                NameIF = combatant.NameIF,
                NameII = combatant.NameII
            };

            this.namesByFull.TryAdd(pcName.Name, pcName);
            this.namesByFI.TryAdd(pcName.NameFI, pcName);
            this.namesByIF.TryAdd(pcName.NameIF, pcName);
            this.namesByII.TryAdd(pcName.NameII, pcName);
        }

        public string ChangeStyle(
            string name,
            NameStyles style)
        {
            PCName pcName = null;

            if (!this.namesByFull.TryGetValue(name, out pcName) &&
                !this.namesByFI.TryGetValue(name, out pcName) &&
                !this.namesByIF.TryGetValue(name, out pcName) &&
                !this.namesByII.TryGetValue(name, out pcName))
            {
                return name;
            }

            switch (style)
            {
                case NameStyles.FullName:
                    return pcName.Name;

                case NameStyles.FullInitial:
                    return pcName.NameFI;

                case NameStyles.InitialFull:
                    return pcName.NameIF;

                case NameStyles.InitialInitial:
                    return pcName.NameII;

                default:
                    return name;
            }
        }

        public void Clear()
        {
            this.namesByFull.Clear();
            this.namesByFI.Clear();
            this.namesByIF.Clear();
            this.namesByII.Clear();
        }

        public string Replace(
            string name,
            NameStyles style)
        {
            string getReplacement(
                PCName pcName,
                NameStyles replaceStyle)
            {
                switch (replaceStyle)
                {
                    case NameStyles.FullName:
                        return pcName.Name;

                    case NameStyles.FullInitial:
                        return pcName.NameFI;

                    case NameStyles.InitialFull:
                        return pcName.NameIF;

                    case NameStyles.InitialInitial:
                        return pcName.NameII;

                    default:
                        return name;
                }
            }

            var replacedName = name;

            foreach (var entry in this.namesByFull)
            {
                replacedName = replacedName.Replace(entry.Key, getReplacement(entry.Value, style));
            }

            if (replacedName != name)
            {
                return replacedName;
            }

            foreach (var entry in this.namesByFI)
            {
                replacedName = replacedName.Replace(entry.Key, getReplacement(entry.Value, style));
            }

            if (replacedName != name)
            {
                return replacedName;
            }

            foreach (var entry in this.namesByIF)
            {
                replacedName = replacedName.Replace(entry.Key, getReplacement(entry.Value, style));
            }

            if (replacedName != name)
            {
                return replacedName;
            }

            foreach (var entry in this.namesByII)
            {
                replacedName = replacedName.Replace(entry.Key, getReplacement(entry.Value, style));
            }

            if (replacedName != name)
            {
                return replacedName;
            }

            return name;
        }
    }
}
