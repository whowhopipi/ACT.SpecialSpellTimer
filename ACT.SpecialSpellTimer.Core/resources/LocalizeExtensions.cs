using System;
using System.Linq;
using System.Windows;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.resources
{
    public static class LocalizeExtensions
    {
        public static void ReloadLocaleDictionary<T>(
            this T element,
            Locales locale) where T : FrameworkElement, ILocalizable
        {
            var dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(
                $@"ACT.SpecialSpellTimer.Core;component/resources/strings/Strings.{locale.ToText()}.xaml",
                UriKind.Relative);

            // 旧文字列を削除する
            var removeItems = element.Resources.MergedDictionaries
                .Where(x => x.Source.ToString().Contains("Strings"))
                .ToArray();
            foreach (var item in removeItems)
            {
                element.Resources.MergedDictionaries.Remove(item);
            }

            // 新しい文字列辞書を登録する
            element.Resources.MergedDictionaries.Add(dictionary);
        }
    }
}
