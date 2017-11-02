using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Image;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Sound;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;
using FFXIV.Framework.WPF.Controls;

namespace ACT.SpecialSpellTimer.Views
{
    /// <summary>
    /// SpellTimerControl
    /// </summary>
    public partial class SpellTimerControl : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpellTimerControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 設定モードか？
        /// </summary>
        public bool IsSettingMode { get; set; } = false;

        #region Colors

        /// <summary>
        /// バーの色
        /// </summary>
        public string BarColor { get; set; }

        /// <summary>
        /// バーOutlineの色
        /// </summary>
        public string BarOutlineColor { get; set; }

        /// <summary>
        /// Fontの色
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// FontOutlineの色
        /// </summary>
        public string FontOutlineColor { get; set; }

        /// <summary>
        /// WarningFontの色
        /// </summary>
        public string WarningFontColor { get; set; }

        /// <summary>
        /// WarningFontOutlineの色
        /// </summary>
        public string WarningFontOutlineColor { get; set; }

        /// <summary>バーのBrush</summary>
        private SolidColorBrush BarBrush { get; set; }

        /// <summary>バーのアウトラインのBrush</summary>
        private SolidColorBrush BarOutlineBrush { get; set; }

        /// <summary>フォントのBrush</summary>
        private SolidColorBrush FontBrush { get; set; }

        /// <summary>フォントのアウトラインBrush</summary>
        private SolidColorBrush FontOutlineBrush { get; set; }

        /// <summary>フォントのBrush</summary>
        private SolidColorBrush WarningFontBrush { get; set; }

        /// <summary>フォントのアウトラインBrush</summary>
        private SolidColorBrush WarningFontOutlineBrush { get; set; }

        /// <summary>
        /// Should font color change when warning?
        /// </summary>
        public bool ChangeFontColorsWhenWarning { get; set; }

        /// <summary>
        /// リキャスト中にアイコンの明度を下げるか？
        /// </summary>
        public bool ReduceIconBrightness { get; set; }

        #endregion Colors

        #region Sizes

        /// <summary>
        /// バーの高さ
        /// </summary>
        public int BarHeight { get; set; }

        /// <summary>
        /// バーの幅
        /// </summary>
        public int BarWidth { get; set; }

        /// <summary>
        /// スペルのIcon
        /// </summary>
        public string SpellIcon { get; set; }

        /// <summary>
        /// スペルIconサイズ
        /// </summary>
        public int SpellIconSize { get; set; }

        /// <summary>
        /// スペル表示領域の幅
        /// </summary>
        public int SpellWidth =>
            this.BarWidth > this.SpellIconSize ?
            this.BarWidth :
            this.SpellIconSize;

        #endregion Sizes

        #region Times

        /// <summary>
        /// リキャストの進捗率
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// リキャストタイムを重ねて表示するか？
        /// </summary>
        public bool OverlapRecastTime { get; set; }

        /// <summary>
        /// 残りリキャストTime(秒数)
        /// </summary>
        public double RecastTime { get; set; }

        /// <summary>
        /// Time left warning in seconds
        /// </summary>
        public double WarningTime { get; set; }

        /// <summary>
        /// Time left blink in seconds
        /// </summary>
        public double BlinkTime { get; set; }

        /// <summary>
        /// リキャスト秒数の書式
        /// </summary>
        private static string RecastTimeFormat =>
            Settings.Default.EnabledSpellTimerNoDecimal ? "N0" : "N1";

        #endregion Times

        /// <summary>
        /// フォント
        /// </summary>
        public FontInfo FontInfo { get; set; }

        /// <summary>
        /// スペル名を非表示とするか？
        /// </summary>
        public bool HideSpellName { get; set; }

        /// <summary>
        /// プログレスバーを逆にするか？
        /// </summary>
        public bool IsReverse { get; set; }

        /// <summary>
        /// スペルのTitle
        /// </summary>
        public string SpellTitle { get; set; }

        public SpellTimer Spell { get; set; }

        /// <summary>
        /// 描画を更新する
        /// </summary>
        public void Refresh()
        {
            // 点滅を判定する
            if (!this.StartBlink())
            {
                // アイコンの不透明度を設定する
                var opacity = 1.0;
                if (this.ReduceIconBrightness)
                {
                    if (this.RecastTime > 0)
                    {
                        opacity = this.IsReverse ?
                            1.0 :
                            ((double)Settings.Default.ReduceIconBrightness / 100d);
                    }
                    else
                    {
                        opacity = this.IsReverse ?
                            ((double)Settings.Default.ReduceIconBrightness / 100d) :
                            1.0;
                    }
                }

                if (this.SpellIconImage.Opacity != opacity)
                {
                    this.SpellIconImage.Opacity = opacity;
                }
            }

            // リキャスト時間を描画する
            var tb = this.RecastTimeTextBlock;
            var recast = this.RecastTime > 0 ?
                this.RecastTime.ToString(RecastTimeFormat) :
                this.IsReverse ? Settings.Default.OverText : Settings.Default.ReadyText;

            if (tb.Text != recast) tb.Text = recast;
            tb.SetFontInfo(this.FontInfo);
            tb.SetAutoStrokeThickness();

            var fill = this.FontBrush;
            var stroke = this.FontOutlineBrush;

            if (this.ChangeFontColorsWhenWarning &&
                this.RecastTime < this.WarningTime)
            {
                fill = this.WarningFontBrush;
                stroke = this.WarningFontOutlineBrush;
            }

            if (tb.Fill != fill) tb.Fill = fill;
            if (tb.Stroke != stroke) tb.Stroke = stroke;
        }

        /// <summary>
        /// 描画設定を更新する
        /// </summary>
        public void Update()
        {
            this.Width = this.SpellWidth;

            // Brushを生成する
            var fontColor = string.IsNullOrWhiteSpace(this.FontColor) ?
                Settings.Default.FontColor.ToWPF() :
                this.FontColor.FromHTMLWPF();
            var fontOutlineColor = string.IsNullOrWhiteSpace(this.FontOutlineColor) ?
                Settings.Default.FontOutlineColor.ToWPF() :
                this.FontOutlineColor.FromHTMLWPF();
            var warningFontColor = string.IsNullOrWhiteSpace(this.WarningFontColor) ?
                Settings.Default.WarningFontColor.ToWPF() :
                this.WarningFontColor.FromHTMLWPF();
            var warningFontOutlineColor = string.IsNullOrWhiteSpace(this.WarningFontOutlineColor) ?
                Settings.Default.WarningFontOutlineColor.ToWPF() :
                this.WarningFontOutlineColor.FromHTMLWPF();

            var barColor = string.IsNullOrWhiteSpace(this.BarColor) ?
                Settings.Default.ProgressBarColor.ToWPF() :
                this.BarColor.FromHTMLWPF();
            var barOutlineColor = string.IsNullOrWhiteSpace(this.BarOutlineColor) ?
                Settings.Default.ProgressBarOutlineColor.ToWPF() :
                this.BarOutlineColor.FromHTMLWPF();

            this.FontBrush = this.GetBrush(fontColor);
            this.FontOutlineBrush = this.GetBrush(fontOutlineColor);
            this.WarningFontBrush = this.GetBrush(warningFontColor);
            this.WarningFontOutlineBrush = this.GetBrush(warningFontOutlineColor);
            this.BarBrush = this.GetBrush(barColor);
            this.BarOutlineBrush = this.GetBrush(barOutlineColor);

            var tb = default(OutlineTextBlock);
            var font = this.FontInfo;

            // アイコンを描画する
            var image = this.SpellIconImage;
            var iconFile = IconController.Instance.GetIconFile(this.SpellIcon);
            if (image.Source == null &&
                iconFile != null)
            {
                var bitmap = new BitmapImage(new Uri(iconFile.FullPath));
                image.Source = bitmap;
                image.Height = this.SpellIconSize;
                image.Width = this.SpellIconSize;

                this.SpellIconPanel.OpacityMask = new ImageBrush(bitmap);
            }

            // Titleを描画する
            tb = this.SpellTitleTextBlock;
            var title = string.IsNullOrWhiteSpace(this.SpellTitle) ? "　" : this.SpellTitle;
            title = title.Replace(",", Environment.NewLine);

            if (tb.Text != title) tb.Text = title;
            if (tb.Fill != this.FontBrush) tb.Fill = this.FontBrush;
            if (tb.Stroke != this.FontOutlineBrush) tb.Stroke = this.FontOutlineBrush;
            tb.SetFontInfo(font);
            tb.SetAutoStrokeThickness();

            if (this.HideSpellName)
            {
                tb.Visibility = Visibility.Collapsed;
            }

            if (this.OverlapRecastTime)
            {
                this.RecastTimePanel.SetValue(Grid.ColumnProperty, 0);
                this.RecastTimePanel.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                this.RecastTimePanel.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                this.RecastTimePanel.Width = this.SpellIconSize >= 6 ? this.SpellIconSize - 6 : double.NaN;
                this.RecastTimePanel.Height = this.RecastTimePanel.Width;
            }
            else
            {
                this.RecastTimePanel.Width = double.NaN;
                this.RecastTimePanel.Height = double.NaN;
            }

            // ProgressBarを描画する
            var foreRect = this.BarRectangle;
            if (foreRect.Fill != this.BarBrush) foreRect.Fill = this.BarBrush;
            if (foreRect.Width != this.BarWidth) foreRect.Width = this.BarWidth;
            if (foreRect.Height != this.BarHeight) foreRect.Height = this.BarHeight;

            var backRect = this.BarBackRectangle;
            if (backRect.Width != this.BarWidth) backRect.Width = this.BarWidth;

            var outlineRect = this.BarOutlineRectangle;
            if (outlineRect.Stroke != this.BarOutlineBrush) outlineRect.Stroke = this.BarOutlineBrush;

            // バーのエフェクトカラーも手動で設定する
            // Bindingだとアニメーションでエラーが発生するため
            var effectColor = this.BarBrush.Color.ChangeBrightness(1.1);
            if (this.BarEffect.Color != effectColor) this.BarEffect.Color = effectColor;
        }

        #region Bar Animations

        /// <summary>バーのアニメーション用DoubleAnimation</summary>
        private DoubleAnimation BarAnimation { get; set; }

        /// <summary>
        /// バーのアニメーションを開始する
        /// </summary>
        public void StartBarAnimation()
        {
            if (this.BarWidth == 0)
            {
                return;
            }

            if (this.BarAnimation == null)
            {
                this.BarAnimation = new DoubleAnimation();
                this.BarAnimation.AutoReverse = false;
            }

            var fps = (int)Math.Ceiling(this.BarWidth / this.RecastTime);
            if (fps <= 0 || fps > Settings.Default.MaxFPS)
            {
                fps = Settings.Default.MaxFPS;
            }

            Timeline.SetDesiredFrameRate(this.BarAnimation, fps);

            var currentWidth = this.IsReverse ?
                (double)(this.BarWidth * (1.0d - this.Progress)) :
                (double)(this.BarWidth * this.Progress);
            if (this.IsReverse)
            {
                this.BarAnimation.From = currentWidth / this.BarWidth;
                this.BarAnimation.To = 0;
            }
            else
            {
                this.BarAnimation.From = currentWidth / this.BarWidth;
                this.BarAnimation.To = 1.0;
            }

            this.BarAnimation.Duration = new Duration(TimeSpan.FromSeconds(this.RecastTime));

            this.BarScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            this.BarScale.BeginAnimation(ScaleTransform.ScaleXProperty, this.BarAnimation);
        }

        #endregion Bar Animations

        #region Blink Animations

        /// <summary>
        /// アイコンの暗い状態の値
        /// </summary>
        /// <remarks>
        /// 暗さ設定の80%とする。点滅の際にはよりコントラストが必要なため</remarks>
        private static readonly double IconDarkValue =
            ((double)Settings.Default.ReduceIconBrightness / 100d) *
            Settings.Default.BlinkBrightnessDark;

        /// <summary>
        /// アイコンの明るい状態の値
        /// </summary>
        private static readonly double IconLightValue = 1.0;

        /// <summary>
        /// ブリンク状態か？
        /// </summary>
        private volatile bool isBlinking = false;

        #region Icon

        private DiscreteDoubleKeyFrame IconKeyframe1 => (DiscreteDoubleKeyFrame)this.iconBlinkAnimation.KeyFrames[0];
        private LinearDoubleKeyFrame IconKeyframe2 => (LinearDoubleKeyFrame)this.iconBlinkAnimation.KeyFrames[1];

        private DoubleAnimationUsingKeyFrames iconBlinkAnimation = new DoubleAnimationUsingKeyFrames()
        {
            KeyFrames = new DoubleKeyFrameCollection()
            {
                new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))),
                new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3)))
            }
        };

        #endregion Icon

        #region Bar

        private DiscreteDoubleKeyFrame BarKeyframe1 => (DiscreteDoubleKeyFrame)this.barBlinkAnimation.KeyFrames[0];
        private LinearDoubleKeyFrame BarKeyframe2 => (LinearDoubleKeyFrame)this.barBlinkAnimation.KeyFrames[1];

        private DoubleAnimationUsingKeyFrames barBlinkAnimation = new DoubleAnimationUsingKeyFrames()
        {
            KeyFrames = new DoubleKeyFrameCollection()
            {
                new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))),
                new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3)))
            }
        };

        #endregion Bar

        public bool StartBlink()
        {
            this.InitializeBlinkAnimation();

            if (this.BlinkTime == 0 ||
                this.RecastTime == 0 ||
                this.RecastTime > this.BlinkTime)
            {
                if (this.isBlinking)
                {
                    this.isBlinking = false;

                    this.SpellIconImage.BeginAnimation(
                        System.Windows.Controls.Image.OpacityProperty,
                        null);
                    this.BarRectangle.BeginAnimation(
                        Rectangle.OpacityProperty,
                        null);
                    this.BarEffect.BeginAnimation(
                        DropShadowEffect.OpacityProperty,
                        null);
                }

                return false;
            }

            if (!this.isBlinking)
            {
                this.isBlinking = true;

                if (this.Spell.BlinkIcon)
                {
                    Timeline.SetDesiredFrameRate(this.iconBlinkAnimation, Settings.Default.MaxFPS);

                    this.SpellIconImage.BeginAnimation(
                        System.Windows.Controls.Image.OpacityProperty,
                        this.iconBlinkAnimation);
                }

                if (this.Spell.BlinkBar)
                {
                    Timeline.SetDesiredFrameRate(this.barBlinkAnimation, Settings.Default.MaxFPS);

                    this.BarRectangle.BeginAnimation(
                        Rectangle.OpacityProperty,
                        this.barBlinkAnimation);

                    this.BarEffect.BeginAnimation(
                        DropShadowEffect.OpacityProperty,
                        this.barBlinkAnimation);
                }
            }

            return true;
        }

        private void InitializeBlinkAnimation()
        {
            // アイコンのアニメを設定する
            if (this.SpellIconSize > 0 &&
                this.Spell.BlinkIcon)
            {
                var value1 = !this.IsReverse ? SpellTimerControl.IconDarkValue : SpellTimerControl.IconLightValue;
                var value2 = !this.IsReverse ? SpellTimerControl.IconLightValue : SpellTimerControl.IconDarkValue;

                this.IconKeyframe1.Value = value2;
                this.IconKeyframe2.Value = value1;

                this.iconBlinkAnimation.AutoReverse = true;
                this.iconBlinkAnimation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(this.BlinkTime));
            }

            // バーのアニメを設定する
            if ((this.BarWidth > 0 || this.BarHeight > 0) &&
                this.Spell.BlinkBar)
            {
                // バーのエフェクト強度を設定する
                var weekEffect = 0.0;
                var strongEffect = 1.0;

                var effect1 = !this.IsReverse ? weekEffect : strongEffect;
                var effect2 = !this.IsReverse ? strongEffect : weekEffect;

                this.BarKeyframe1.Value = effect2;
                this.BarKeyframe2.Value = effect1;

                this.barBlinkAnimation.AutoReverse = true;
                this.barBlinkAnimation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(this.BlinkTime));
            }
        }

        #endregion Blink Animations

        private void TestMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.Spell != null)
            {
                // 擬似的にマッチ状態にする
                var now = DateTime.Now;
                this.Spell.MatchDateTime = now;
                this.Spell.CompleteScheduledTime = now.AddSeconds(this.Spell.RecastTime);

                this.Spell.UpdateDone = false;
                this.Spell.OverDone = false;
                this.Spell.BeforeDone = false;
                this.Spell.TimeupDone = false;

                // マッチ時点のサウンドを再生する
                SoundController.Instance.Play(this.Spell.MatchSound);
                SoundController.Instance.Play(this.Spell.MatchTextToSpeak);

                // 遅延サウンドタイマを開始する
                this.Spell.StartOverSoundTimer();
                this.Spell.StartBeforeSoundTimer();
                this.Spell.StartTimeupSoundTimer();
            }
        }
    }
}
