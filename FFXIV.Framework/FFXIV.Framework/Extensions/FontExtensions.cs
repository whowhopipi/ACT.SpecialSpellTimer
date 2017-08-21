using System.Windows.Controls;
using FFXIV.Framework.Common;

namespace FFXIV.Framework.Extensions
{
    public static class FontExtensions
    {
        public static FontInfo GetFontInfo(
            this Control control)
        {
            return new FontInfo(
                control.FontFamily,
                control.FontSize,
                control.FontStyle,
                control.FontWeight,
                control.FontStretch);
        }

        public static void SetFontInfo(
            this Control control,
            FontInfo fontInfo)
        {
            if (control.GetFontInfo().ToString() != fontInfo.ToString())
            {
                control.FontFamily = fontInfo.Family;
                control.FontSize = fontInfo.Size;
                control.FontStyle = fontInfo.Style;
                control.FontWeight = fontInfo.Weight;
                control.FontStretch = fontInfo.Stretch;
            }
        }

        public static FontInfo GetFontInfo(
            this TextBlock control)
        {
            return new FontInfo(
                control.FontFamily,
                control.FontSize,
                control.FontStyle,
                control.FontWeight,
                control.FontStretch);
        }

        public static void SetFontInfo(
            this TextBlock control,
            FontInfo fontInfo)
        {
            if (control.GetFontInfo().ToString() != fontInfo.ToString())
            {
                control.FontFamily = fontInfo.Family;
                control.FontSize = fontInfo.Size;
                control.FontStyle = fontInfo.Style;
                control.FontWeight = fontInfo.Weight;
                control.FontStretch = fontInfo.Stretch;
            }
        }
    }
}
