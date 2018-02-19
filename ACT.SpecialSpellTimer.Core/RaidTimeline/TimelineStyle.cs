using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Config.Views;
using ACT.SpecialSpellTimer.Image;
using FFXIV.Framework.Common;
using FFXIV.Framework.Dialog;
using FFXIV.Framework.Extensions;
using Prism.Commands;
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

        public TimelineStyle Clone()
        {
            var clone = this.MemberwiseClone() as TimelineStyle;
            clone.Font = this.Font.Clone() as FontInfo;
            return clone;
        }

        private ICommand changeDefaultCommand;

        public ICommand ChangeDefaultCommand =>
            this.changeDefaultCommand ?? (this.changeDefaultCommand = new DelegateCommand(() =>
            {
                if (this.IsDefault)
                {
                    foreach (var style in TimelineSettings.Instance.Styles)
                    {
                        if (style != this)
                        {
                            style.IsDefault = false;
                        }
                    }
                }
                else
                {
                    if (!TimelineSettings.Instance.Styles.Any(x => x.IsDefault))
                    {
                        TimelineSettings.Instance.Styles.FirstOrDefault().IsDefault = true;
                    }
                }
            }));

        #region Change Font

        private ICommand CreateChangeFontCommand(
            Func<FontInfo> getCurrentoFont,
            Action<FontInfo> changeFont)
            => new DelegateCommand(() =>
            {
                var result = FontDialogWrapper.ShowDialog(getCurrentoFont());
                if (result.Result)
                {
                    changeFont.Invoke(result.Font);
                }
            });

        private ICommand changeFontCommand;

        public ICommand ChangeFontCommand =>
            this.changeFontCommand ?? (this.changeFontCommand = this.CreateChangeFontCommand(
                () => this.Font,
                (font) =>
                {
                    this.Font.FontFamily = font.FontFamily;
                    this.Font.Size = font.Size;
                    this.Font.Style = font.Style;
                    this.Font.Weight = font.Weight;
                    this.Font.Stretch = font.Stretch;
                    this.Font.RaisePropertyChanged(nameof(this.Font.DisplayText));
                }));

        #endregion Change Font

        #region Change Colors

        private ICommand CreateChangeColorCommand(
            Func<Color> getCurrentColor,
            Action<Color> changeColorAction)
            => new DelegateCommand(() =>
            {
                var result = ColorDialogWrapper.ShowDialog(getCurrentColor(), true);
                if (result.Result)
                {
                    changeColorAction.Invoke(result.Color);
                }
            });

        private ICommand changeFontColorCommand;

        public ICommand ChangeFontColorCommand =>
            this.changeFontColorCommand ?? (this.changeFontColorCommand = this.CreateChangeColorCommand(
                () => this.Color,
                (color) => this.Color = color));

        private ICommand changeFontOutlineColorCommand;

        public ICommand ChangeFontOutlineColorCommand =>
            this.changeFontOutlineColorCommand ?? (this.changeFontOutlineColorCommand = this.CreateChangeColorCommand(
                () => this.OutlineColor,
                (color) => this.OutlineColor = color));

        private ICommand changeBarColorCommand;

        public ICommand ChangeBarColorCommand =>
            this.changeBarColorCommand ?? (this.changeBarColorCommand = this.CreateChangeColorCommand(
                () => this.BarColor,
                (color) => this.BarColor = color));

        #endregion Change Colors

        #region Change Icon

        private ICommand selectIconCommand;

        public ICommand SelectIconCommand =>
            this.selectIconCommand ?? (this.selectIconCommand = new DelegateCommand(() =>
            {
                var view = new IconBrowserView();

                view.SelectedIconName = this.Icon;

                if (view.ShowDialog() ?? false)
                {
                    this.Icon = view.SelectedIconName;
                }
            }));

        #endregion Change Icon

        #region Default Style

        private static TimelineStyle defaultStyle;

        public static TimelineStyle DefaultStyle =>
            defaultStyle ?? (defaultStyle = new TimelineStyle()
            {
                Name = "Default",
                Font = new FontInfo("Arial", 18, "Normal", "Bold", "Normal"),
            });

        #endregion Default Style
    }
}
