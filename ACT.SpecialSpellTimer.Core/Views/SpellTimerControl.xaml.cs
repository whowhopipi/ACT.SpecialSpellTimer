using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Image;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Sound;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;

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

        /// <summary>
        /// バーの色
        /// </summary>
        public string BarColor { get; set; }

        /// <summary>
        /// バーの高さ
        /// </summary>
        public int BarHeight { get; set; }

        /// <summary>
        /// バーOutlineの色
        /// </summary>
        public string BarOutlineColor { get; set; }

        /// <summary>
        /// バーの幅
        /// </summary>
        public int BarWidth { get; set; }

        /// <summary>
        /// Should font color change when warning?
        /// </summary>
        public bool ChangeFontColorsWhenWarning { get; set; }

        /// <summary>
        /// Fontの色
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// フォント
        /// </summary>
        public FontInfo FontInfo { get; set; }

        /// <summary>
        /// FontOutlineの色
        /// </summary>
        public string FontOutlineColor { get; set; }

        /// <summary>
        /// スペル名を非表示とするか？
        /// </summary>
        public bool HideSpellName { get; set; }

        /// <summary>
        /// プログレスバーを逆にするか？
        /// </summary>
        public bool IsReverse { get; set; }

        /// <summary>
        /// リキャストタイムを重ねて表示するか？
        /// </summary>
        public bool OverlapRecastTime { get; set; }

        /// <summary>
        /// リキャストの進捗率
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// 残りリキャストTime(秒数)
        /// </summary>
        public double RecastTime { get; set; }

        /// <summary>
        /// リキャスト中にアイコンの明度を下げるか？
        /// </summary>
        public bool ReduceIconBrightness { get; set; }

        /// <summary>
        /// スペルのIcon
        /// </summary>
        public string SpellIcon { get; set; }

        /// <summary>
        /// スペルIconサイズ
        /// </summary>
        public int SpellIconSize { get; set; }

        /// <summary>
        /// スペルのTitle
        /// </summary>
        public string SpellTitle { get; set; }

        /// <summary>
        /// スペル表示領域の幅
        /// </summary>
        public int SpellWidth =>
            this.BarWidth > this.SpellIconSize ?
            this.BarWidth :
            this.SpellIconSize;

        /// <summary>
        /// WarningFontの色
        /// </summary>
        public string WarningFontColor { get; set; }

        /// <summary>
        /// WarningFontOutlineの色
        /// </summary>
        public string WarningFontOutlineColor { get; set; }

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

        /// <summary>バーのアニメーション用DoubleAnimation</summary>
        private DoubleAnimation BarAnimation { get; set; }

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
        }

        #region Blink Animations

        /// <summary>
        /// 点滅のサイクル
        /// </summary>
        private const double BlinkDuration = 0.6;

        /// <summary>
        /// 点滅のピーク状態のホールド時間
        /// </summary>
        /// <remarks>
        /// ピーク状態を多少ホールドしないと点滅が目立たないため</remarks>
        private const double BlinkHoldDuration = BlinkDuration * 0.2;

        /// <summary>
        /// アイコンの暗い状態の値
        /// </summary>
        /// <remarks>
        /// 暗さ設定の80%とする。点滅の際にはよりコントラストが必要なため</remarks>
        private static readonly double IconDarkValue = ((double)Settings.Default.ReduceIconBrightness / 100d) * 0.8;

        /// <summary>
        /// アイコンの明るい状態の値
        /// </summary>
        private static readonly double IconLightValue = 1.0;

        /// <summary>
        /// ブリンク状態か？
        /// </summary>
        private volatile bool isBlinking = false;

        #region Icon

        private DiscreteDoubleKeyFrame iconKeyframe1 = new DiscreteDoubleKeyFrame(0, TimeSpan.FromSeconds(0));
        private DiscreteDoubleKeyFrame iconKeyframe2 = new DiscreteDoubleKeyFrame(0, TimeSpan.FromSeconds(BlinkHoldDuration));
        private LinearDoubleKeyFrame iconKeyframe3 = new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(BlinkDuration));

        private DoubleAnimationUsingKeyFrames iconBlinkAnimation;

        private DoubleAnimationUsingKeyFrames IconBlinkAnimation =>
            (this.iconBlinkAnimation ?? (this.iconBlinkAnimation = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = new DoubleKeyFrameCollection()
                {
                    this.iconKeyframe1,
                    this.iconKeyframe2,
                    this.iconKeyframe3
                }
            }));

        #endregion Icon

        #region Bar

        private DiscreteColorKeyFrame barKeyframe1 = new DiscreteColorKeyFrame(Colors.Transparent, TimeSpan.FromSeconds(0));
        private DiscreteColorKeyFrame barKeyframe2 = new DiscreteColorKeyFrame(Colors.Transparent, TimeSpan.FromSeconds(BlinkHoldDuration));
        private LinearColorKeyFrame barKeyframe3 = new LinearColorKeyFrame(Colors.Transparent, TimeSpan.FromSeconds(BlinkDuration));

        private ColorAnimationUsingKeyFrames barBlinkAnimation;

        private ColorAnimationUsingKeyFrames BarBlinkAnimation =>
            (this.barBlinkAnimation ?? (this.barBlinkAnimation = new ColorAnimationUsingKeyFrames()
            {
                KeyFrames = new ColorKeyFrameCollection()
                {
                    this.barKeyframe1,
                    this.barKeyframe2,
                    this.barKeyframe3
                }
            }));

        private DiscreteDoubleKeyFrame barEffectKeyframe1 = new DiscreteDoubleKeyFrame(0, TimeSpan.FromSeconds(0));
        private DiscreteDoubleKeyFrame barEffectKeyframe2 = new DiscreteDoubleKeyFrame(0, TimeSpan.FromSeconds(BlinkHoldDuration));
        private LinearDoubleKeyFrame barEffectKeyframe3 = new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(BlinkDuration));

        private DoubleAnimationUsingKeyFrames barEffectBlinkAnimation;

        private DoubleAnimationUsingKeyFrames BarEffectBlinkAnimation =>
            (this.barEffectBlinkAnimation ?? (this.barEffectBlinkAnimation = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = new DoubleKeyFrameCollection()
                {
                    this.barEffectKeyframe1,
                    this.barEffectKeyframe2,
                    this.barEffectKeyframe3
                }
            }));

        #endregion Bar

        private Storyboard blinkStoryboard;

        private Storyboard BlinkStoryboard
        {
            get
            {
                lock (this)
                {
                    if (this.blinkStoryboard == null)
                    {
                        var story = this.blinkStoryboard = new Storyboard();

                        // アイコンのアニメを設定する
                        if (this.SpellIconSize > 0 &&
                            this.Spell.BlinkIcon)
                        {
                            story.Children.Add(this.IconBlinkAnimation);

                            var value1 = !this.IsReverse ? IconDarkValue : IconLightValue;
                            var vakue2 = !this.IsReverse ? IconLightValue : IconDarkValue;

                            this.iconKeyframe1.Value = value1;
                            this.iconKeyframe2.Value = value1;
                            this.iconKeyframe3.Value = vakue2;
                        }

                        // バーのアニメを設定する
                        if ((this.BarWidth > 0 || this.BarHeight > 0) &&
                            this.Spell.BlinkBar)
                        {
                            // バーの色を設定する
                            story.Children.Add(this.BarBlinkAnimation);
                            var darkColor = this.BarBrush.Color.ChangeBrightness(0.5);
                            var lightColor = this.BarBrush.Color.ChangeBrightness(1.1);

                            var value1 = !this.IsReverse ? darkColor : lightColor;
                            var vakue2 = !this.IsReverse ? lightColor : darkColor;

                            this.barKeyframe1.Value = value1;
                            this.barKeyframe2.Value = value1;
                            this.barKeyframe3.Value = vakue2;

                            // バーのエフェクト強度を設定する
                            story.Children.Add(this.BarEffectBlinkAnimation);
                            var weekEffect = this.BarEffect.BlurRadius * 0.5;
                            var strongEffect = this.BarEffect.BlurRadius * 1.5;

                            var effect1 = !this.IsReverse ? weekEffect : strongEffect;
                            var effect2 = !this.IsReverse ? strongEffect : weekEffect;

                            this.barEffectKeyframe1.Value = effect1;
                            this.barEffectKeyframe2.Value = effect1;
                            this.barEffectKeyframe3.Value = effect2;
                        }

                        story.AutoReverse = true;
                        story.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(this.BlinkTime));

                        // FPSを制限する。しないと負荷が高くなる
                        Timeline.SetDesiredFrameRate(story, 30);
                    }

                    return this.blinkStoryboard;
                }
            }
        }

        public bool StartBlink()
        {
            if (this.BlinkTime == 0 ||
                this.RecastTime == 0 ||
                this.RecastTime > this.BlinkTime)
            {
                if (this.isBlinking)
                {
                    this.isBlinking = false;
                    this.BlinkStoryboard.Stop();
                }

                return false;
            }

            if (!this.isBlinking)
            {
                Storyboard.SetTarget(this.IconBlinkAnimation, this.SpellIconImage);
                Storyboard.SetTargetProperty(this.IconBlinkAnimation, new PropertyPath("Opacity"));
                Storyboard.SetTarget(this.BarBlinkAnimation, this.BarRectangle);
                Storyboard.SetTargetProperty(this.BarBlinkAnimation, new PropertyPath("Fill.Color"));
                Storyboard.SetTarget(this.BarEffectBlinkAnimation, this.BarEffect);
                Storyboard.SetTargetProperty(this.BarEffectBlinkAnimation, new PropertyPath("BlurRadius"));

                this.isBlinking = true;
                this.BlinkStoryboard.Begin();
            }

            return true;
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
