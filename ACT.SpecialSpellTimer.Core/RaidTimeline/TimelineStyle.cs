using System;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Image;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [XmlType(TypeName = "Style")]
    [Serializable]
    public class TimelineStyle :
        BindableBase
    {
        private string name = string.Empty;

        /// <summary>
        /// スタイルの名前
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        private bool isDefault;

        /// <summary>
        /// 規定のスタイル？
        /// </summary>
        public bool IsDefault
        {
            get => this.isDefault;
            set => this.SetProperty(ref this.isDefault, value);
        }

        private FontInfo font = new FontInfo();

        /// <summary>
        /// フォント
        /// </summary>
        public FontInfo Font
        {
            get => this.font;
            set
            {
                if (!Equals(this.font, value))
                {
                    this.font = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private Color color = Colors.White;

        /// <summary>
        /// カラー
        /// </summary>
        [XmlIgnore]
        public Color Color
        {
            get => this.color;
            set => this.SetProperty(ref this.color, value);
        }

        /// <summary>
        /// カラー
        /// </summary>
        [XmlElement(ElementName = "Color")]
        public string ColorText
        {
            get => this.Color.ToString();
            set => this.Color = this.Color.FromString(value);
        }

        private Color outlineColor = Colors.Navy;

        /// <summary>
        /// アウトラインのカラー
        /// </summary>
        [XmlIgnore]
        public Color OutlineColor
        {
            get => this.outlineColor;
            set => this.SetProperty(ref this.outlineColor, value);
        }

        /// <summary>
        /// アウトラインのカラー
        /// </summary>
        [XmlElement(ElementName = "OutlineColor")]
        public string OutlineColorText
        {
            get => this.OutlineColor.ToString();
            set => this.OutlineColor = this.OutlineColor.FromString(value);
        }

        private Color barColor = Colors.OrangeRed;

        /// <summary>
        /// バーのカラー
        /// </summary>
        [XmlIgnore]
        public Color BarColor
        {
            get => this.barColor;
            set => this.SetProperty(ref this.barColor, value);
        }

        /// <summary>
        /// バーのカラー
        /// </summary>
        [XmlElement(ElementName = "BarColor")]
        public string BarColorText
        {
            get => this.BarColor.ToString();
            set => this.BarColor = this.BarColor.FromString(value);
        }

        private double barHeight = 3;

        /// <summary>
        /// バーの高さ
        /// </summary>
        public double BarHeight
        {
            get => this.barHeight;
            set => this.SetProperty(ref this.barHeight, value);
        }

        public bool isCircleStyle = false;

        /// <summary>
        /// プログレスCircleを使用する？
        /// </summary>
        public bool IsCircleStyle
        {
            get => this.isCircleStyle;
            set => this.SetProperty(ref this.isCircleStyle, value);
        }

        private string icon = string.Empty;

        /// <summary>
        /// ICON
        /// </summary>
        public string Icon
        {
            get => this.icon;
            set
            {
                if (this.SetProperty(ref this.icon, value))
                {
                    this.RaisePropertyChanged(nameof(this.IconFile));
                }
            }
        }

        [XmlIgnore]
        public string IconFile =>
            string.IsNullOrEmpty(this.Icon) ?
            string.Empty :
            IconController.Instance.GetIconFile(this.Icon)?.FullPath;

        private double iconSize = 24;

        public double IconSize
        {
            get => this.iconSize;
            set => this.SetProperty(ref this.iconSize, value);
        }

        #region Default Style

        private static TimelineStyle defaultStyle;

        public static TimelineStyle DefaultStyle =>
            defaultStyle ?? (defaultStyle = new TimelineStyle()
            {
                Font = new FontInfo("Arial", 18, "Normal", "Bold", "Normal"),
            });

        #endregion Default Style
    }
}
