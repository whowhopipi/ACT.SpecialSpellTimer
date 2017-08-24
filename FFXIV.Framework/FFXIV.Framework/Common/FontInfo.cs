using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace FFXIV.Framework.Common
{
    [Serializable]
    [DataContract]
    public class FontInfo
    {
        private static readonly object locker = new object();

        [XmlIgnore]
        public static readonly FontInfo DefaultFont = new FontInfo(
            new FontFamily("Arial"),
            11,
            FontStyles.Normal,
            FontWeights.Normal,
            FontStretches.Normal);

        [XmlIgnore]
        private static readonly Dictionary<string, FontFamily> fontFamilyDictionary = new Dictionary<string, FontFamily>();

        [XmlIgnore]
        private static FontStretchConverter stretchConverter = new FontStretchConverter();

        [XmlIgnore]
        private static FontStyleConverter styleConverter = new FontStyleConverter();

        [XmlIgnore]
        private static FontWeightConverter weightConverter = new FontWeightConverter();

        public FontInfo()
        {
        }

        public FontInfo(
            string family)
        {
            this.Family = GetFontFamily(family);
        }

        public FontInfo(
            string family,
            double size)
        {
            this.Family = GetFontFamily(family);
            this.Size = size;
        }

        public FontInfo(
            string family,
            double size,
            string style,
            string weight,
            string stretch)
        {
            this.Family = GetFontFamily(family);
            this.Size = size;
            this.StyleString = style;
            this.WeightString = weight;
            this.StretchString = stretch;
        }

        public FontInfo(
            FontFamily family,
            double size,
            FontStyle style,
            FontWeight weight,
            FontStretch stretch)
        {
            this.Family = family;
            this.Size = size;
            this.Style = style;
            this.Weight = weight;
            this.Stretch = stretch;
        }

        [XmlIgnore]
        public FontFamily Family { get; set; } = new FontFamily("Arial");

        [XmlAttribute("FontFamily")]
        [DataMember(Name = "FontFamily")]
        public string FamilyName
        {
            get => this.Family != null ?
                this.Family.Source ?? string.Empty :
                string.Empty;
            set => this.Family = FontInfo.GetFontFamily(value);
        }

        [XmlAttribute("Size")]
        [DataMember(Name = "Size")]
        public double Size { get; set; } = 11.0d;

        [XmlIgnore]
        public FontStretch Stretch { get; set; } = FontStretches.Normal;

        [XmlAttribute("Stretch")]
        [DataMember(Name = "Stretch")]
        public string StretchString
        {
            get => FontInfo.stretchConverter.ConvertToString(this.Stretch);
            set => this.Stretch = (FontStretch)FontInfo.stretchConverter.ConvertFromString(value);
        }

        [XmlIgnore]
        public FontStyle Style { get; set; } = FontStyles.Normal;

        [XmlAttribute("Style")]
        [DataMember(Name = "Style")]
        public string StyleString
        {
            get => FontInfo.styleConverter.ConvertToString(this.Style);
            set => this.Style = (FontStyle)FontInfo.styleConverter.ConvertFromString(value);
        }

        [XmlIgnore]
        public FamilyTypeface Typeface
        {
            get => new FamilyTypeface()
            {
                Stretch = this.Stretch,
                Weight = this.Weight,
                Style = this.Style,
            };
        }

        [XmlIgnore]
        public FontWeight Weight { get; set; } = FontWeights.Normal;

        [XmlAttribute("Weight")]
        [DataMember(Name = "Weight")]
        public string WeightString
        {
            get => FontInfo.weightConverter.ConvertToString(this.Weight);
            set => this.Weight = (FontWeight)FontInfo.weightConverter.ConvertFromString(value);
        }

        public static double TextOutlineThicknessGain { get; set; } = 1.0d;

        /// <summary>
        /// アウトラインの太さ
        /// </summary>
        [XmlIgnore]
        public double OutlineThickness
        {
            get
            {
                // 基準の太さ
                var thickness = 1.0d;

                // フォントサイズを基準に補正をかける
                thickness *=
                    this.Size / 11.0d;

                // ウェイトによる補正をかける
                thickness *=
                    this.Weight.ToOpenTypeWeight() /
                    FontWeights.Normal.ToOpenTypeWeight();

                // 設定によって増幅させる
                thickness *= TextOutlineThicknessGain;

                return thickness;
            }
        }

        public static FontInfo FromString(
            string json)
        {
            var obj = default(FontInfo);

            var serializer = new DataContractJsonSerializer(typeof(FontInfo));
            var data = Encoding.UTF8.GetBytes(json);
            using (var ms = new MemoryStream(data))
            {
                obj = (FontInfo)serializer.ReadObject(ms);
            }

            return obj;
        }

        public override string ToString()
        {
            var json = string.Empty;

            var serializer = new DataContractJsonSerializer(typeof(FontInfo));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, this);
                json = Encoding.UTF8.GetString(ms.ToArray());
            }

            return json;
        }

        private static FontFamily GetFontFamily(
            string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return new FontFamily();
            }

            lock (locker)
            {
                if (!fontFamilyDictionary.ContainsKey(source))
                {
                    fontFamilyDictionary[source] = new FontFamily(source);
                }

                return fontFamilyDictionary[source];
            }
        }

        public System.Drawing.Font ToFontForWindowsForm()
        {
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;

            if (this.Style == FontStyles.Italic ||
                this.Style == FontStyles.Oblique)
            {
                style |= System.Drawing.FontStyle.Italic;
            }

            if (this.Weight > FontWeights.Normal)
            {
                style |= System.Drawing.FontStyle.Bold;
            }

            System.Drawing.Font f = new System.Drawing.Font(
                this.FamilyName,
                (float)this.Size,
                style);

            return f;
        }
    }
}
