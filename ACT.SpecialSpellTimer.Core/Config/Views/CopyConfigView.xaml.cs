using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ACT.SpecialSpellTimer.Config.Models;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Common;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// TagView.xaml の相互作用ロジック
    /// </summary>
    public partial class CopyConfigView :
        Window,
        ILocalizable,
        INotifyPropertyChanged
    {
        public CopyConfigView(
            ITreeItem source)
        {
            this.SourceConfig = source;

            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);

            // ウィンドウのスタート位置を決める
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.MouseLeftButtonDown += (x, y) => this.DragMove();

            this.PreviewKeyUp += (x, y) =>
            {
                if (y.Key == Key.Escape)
                {
                    this.Close();
                }
            };

            this.CloseButton.Click += (x, y) =>
            {
                this.Close();
            };

            this.ApplyButton.Click += this.ApplyButton_Click;
        }

        private async void ApplyButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            void ifdo(bool b, Action a)
            {
                if (b)
                {
                    a.Invoke();
                }
            }

            void copySpell(Spell d, Spell s)
            {
                ifdo(this.CopyFont, () => d.Font = s.Font.Clone() as FontInfo);
                ifdo(this.CopyFontFill, () => d.FontColor = s.FontColor);
                ifdo(this.CopyFontStrokke, () => d.FontOutlineColor = s.FontOutlineColor);
                ifdo(this.CopyFontWarningFill, () => d.WarningFontColor = s.WarningFontColor);
                ifdo(this.CopyFontWarningStroke, () => d.WarningFontOutlineColor = s.WarningFontOutlineColor);
                ifdo(this.CopyProgressBarSize, () => d.BarHeight = s.BarHeight);
                ifdo(this.CopyProgressBarSize, () => d.BarWidth = s.BarWidth);
                ifdo(this.CopyProgressBarFill, () => d.BarColor = s.BarColor);
                ifdo(this.CopyProgressBarStroke, () => d.BarOutlineColor = s.BarOutlineColor);
                ifdo(this.CopyBackground, () => d.BackgroundColor = s.BackgroundColor);
                ifdo(this.CopyBackground, () => d.BackgroundAlpha = s.BackgroundAlpha);
                ifdo(this.CopyIconSize, () => d.SpellIconSize = s.SpellIconSize);
                ifdo(this.CopyIconOverlapRecastTime, () => d.OverlapRecastTime = s.OverlapRecastTime);
                ifdo(this.CopyIconToDarkness, () => d.ReduceIconBrightness = s.ReduceIconBrightness);
                ifdo(this.CopyIconHideSpellName, () => d.HideSpellName = s.HideSpellName);
            }

            void copyTicker(Ticker d, Ticker s)
            {
                ifdo(this.CopyFont, () => d.Font = s.Font.Clone() as FontInfo);
                ifdo(this.CopyFontFill, () => d.FontColor = s.FontColor);
                ifdo(this.CopyFontStrokke, () => d.FontOutlineColor = s.FontOutlineColor);
                ifdo(this.CopyBackground, () => d.BackgroundColor = s.BackgroundColor);
                ifdo(this.CopyBackground, () => d.BackgroundAlpha = s.BackgroundAlpha);
                ifdo(this.CopyX, () => d.Left = s.Left);
                ifdo(this.CopyY, () => d.Top = s.Top);
            }

            await Task.Run(() =>
            {
                foreach (var dest in this.Dests.Where(x => x.ToCopy))
                {
                    if (this.IsSpell)
                    {
                        var src = this.SourceConfig as Spell;
                        var d = dest.Item as Spell;
                        copySpell(d, src);
                    }

                    if (this.IsTicker)
                    {
                        var src = this.SourceConfig as Ticker;
                        var d = dest.Item as Ticker;
                        copyTicker(d, src);
                    }
                }
            });

            ModernMessageBox.ShowDialog(
                "Done!",
                "ACT.Hojoring");

            this.Close();
        }

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);

        public ITreeItem SourceConfig { get; private set; } = null;

        public bool IsSpell => this.SourceConfig is Spell;

        public bool IsTicker => this.SourceConfig is Ticker;

        public string SourceName
        {
            get
            {
                var text = string.Empty;

                switch (this.SourceConfig)
                {
                    case Spell s:
                        text = s.Panel?.PanelName + " - " + s.SpellTitle;
                        break;

                    case Ticker t:
                        text = t.Title;
                        break;
                }

                return text;
            }
        }

        public Dest[] Dests
        {
            get
            {
                var dests = new List<Dest>();

                if (this.IsSpell)
                {
                    dests.AddRange(
                        from x in SpellTable.Instance.Table
                        orderby
                        x.Panel?.PanelName,
                        x.DisplayNo,
                        x.ID
                        select new Dest()
                        {
                            Item = x
                        });
                }

                if (this.IsTicker)
                {
                    dests.AddRange(
                        from x in TickerTable.Instance.Table
                        orderby
                        x.Title,
                        x.ID
                        select new Dest()
                        {
                            Item = x
                        });
                }

                return dests.ToArray();
            }
        }

        #region Copy設定

        public bool CopyFont { get; set; }
        public bool CopyFontFill { get; set; }
        public bool CopyFontStrokke { get; set; }
        public bool CopyFontWarningFill { get; set; }
        public bool CopyFontWarningStroke { get; set; }
        public bool CopyProgressBarSize { get; set; }
        public bool CopyProgressBarFill { get; set; }
        public bool CopyProgressBarStroke { get; set; }
        public bool CopyBackground { get; set; }
        public bool CopyIconSize { get; set; }
        public bool CopyIconOverlapRecastTime { get; set; }
        public bool CopyIconToDarkness { get; set; }
        public bool CopyIconHideSpellName { get; set; }
        public bool CopyX { get; set; }
        public bool CopyY { get; set; }

        #endregion Copy設定

        #region INotifyPropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(
            [CallerMemberName]string propertyName = null)
        {
            this.PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            this.PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));

            return true;
        }

        #endregion INotifyPropertyChanged

        public class Dest
        {
            public bool ToCopy { get; set; }

            public string Text
            {
                get
                {
                    var text = string.Empty;

                    switch (this.Item)
                    {
                        case Spell s:
                            text = s.Panel?.PanelName + " - " + s.SpellTitle;
                            break;

                        case Ticker t:
                            text = t.Title;
                            break;
                    }

                    return text;
                }
            }

            public ITreeItem Item { get; set; }

            public override string ToString() => this.Text;
        }
    }
}
