using System;
using System.ComponentModel;
using System.Drawing;

namespace ACT.SpecialSpellTimer.Config
{
    [TypeConverter(typeof(FontConverter))]
    public static class FontSerializationHelper
    {
        private const char FontSerializationDelimiter = ',';

        public static Font FromString(
            string value)
        {
            var parts = value.Split(FontSerializationDelimiter);
            return new Font(
                parts[0],
                float.Parse(parts[1]),
                (FontStyle)Enum.Parse(typeof(FontStyle), parts[2]),
                (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), parts[3]),
                byte.Parse(parts[4]),
                bool.Parse(parts[5])
            );
        }

        public static string ToString(
            Font font)
        {
            return string.Join(
                FontSerializationDelimiter.ToString(),
                new string[]
                {
                    font.FontFamily.Name,
                    font.Size.ToString(),
                    font.Style.ToString(),
                    font.Unit.ToString(),
                    font.GdiCharSet.ToString(),
                    font.GdiVerticalFont.ToString()
                });
        }
    }
}