namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using ACT.SpecialSpellTimer.Image;
    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;
    using System.Windows.Media.Animation;

    /// <summary>
    /// SpellTimerControl
    /// </summary>
    public partial class SpellTimerControl : UserControl
    {
        /// <summary>
        /// リキャスト秒数の書式
        /// </summary>
        private static string recastTimeFormat =
            Settings.Default.EnabledSpellTimerNoDecimal ? "N0" : "N1";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpellTimerControl()
        {
#if DEBUG
            Debug.WriteLine("Spell");
#endif
            this.InitializeComponent();
        }

        /// <summary>
        /// スペルのTitle
        /// </summary>
        public string SpellTitle { get; set; }

        /// <summary>
        /// スペルのIcon
        /// </summary>
        public string SpellIcon { get; set; }

        /// <summary>
        /// スペルIconサイズ
        /// </summary>
        public int SpellIconSize { get; set; }

        /// <summary>
        /// 残りリキャストTime(秒数)
        /// </summary>
        public double RecastTime { get; set; }

        /// <summary>
        /// リキャストの進捗率
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// プログレスバーを逆にするか？
        /// </summary>
        public bool IsReverse { get; set; }

        /// <summary>
        /// スペル名を非表示とするか？
        /// </summary>
        public bool HideSpellName { get; set; }

        /// <summary>
        /// リキャストタイムを重ねて表示するか？
        /// </summary>
        public bool OverlapRecastTime { get; set; }

        /// <summary>
        /// リキャスト中にアイコンの明度を下げるか？
        /// </summary>
        public bool ReduceIconBrightness { get; set; }

        /// <summary>
        /// バーの色
        /// </summary>
        public string BarColor { get; set; }

        /// <summary>
        /// バーOutlineの色
        /// </summary>
        public string BarOutlineColor { get; set; }

        /// <summary>
        /// バーの幅
        /// </summary>
        public int BarWidth { get; set; }
        /// <summary>
        /// バーの高さ
        /// </summary>
        public int BarHeight { get; set; }

        /// <summary>
        /// スペル表示領域の幅
        /// </summary>
        public int SpellWidth
        {
            get
            {
                return BarWidth > SpellIconSize ? BarWidth : SpellIconSize;
            }
        }

        /// <summary>
        /// フォント
        /// </summary>
        public FontInfo FontInfo { get; set; }

        /// <summary>
        /// Fontの色
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// FontOutlineの色
        /// </summary>
        public string FontOutlineColor { get; set; }

        /// <summary>フォントのBrush</summary>
        private SolidColorBrush FontBrush { get; set; }

        /// <summary>フォントのアウトラインBrush</summary>
        private SolidColorBrush FontOutlineBrush { get; set; }

        /// <summary>バーのBrush</summary>
        private SolidColorBrush BarBrush { get; set; }

        /// <summary>バーの背景のBrush</summary>
        private SolidColorBrush BarBackBrush { get; set; }

        /// <summary>バーのアウトラインのBrush</summary>
        private SolidColorBrush BarOutlineBrush { get; set; }

        /// <summary>バーのアニメーション用DoubleAnimation</summary>
        private DoubleAnimation BarAnimation { get; set; }

        /// <summary>
        /// 描画を更新する
        /// </summary>
        public void Refresh()
        {
            // アイコンの不透明度を設定する
            var image = this.SpellIconImage;
            if (this.ReduceIconBrightness)
            {
                if (this.RecastTime > 0)
                {
                    image.Opacity = this.IsReverse ? 
                        1.0 : 
                        (Settings.Default.ReduceIconBrightness / 100);
                }
                else
                {
                    image.Opacity = this.IsReverse ? 
                        (Settings.Default.ReduceIconBrightness / 100) : 
                        1.0;
                }
            }
            else
            {
                image.Opacity = 1.0;
            }

            // リキャスト時間を描画する
            var tb = this.RecastTimeTextBlock;
            var recast = this.RecastTime > 0 ?
                this.RecastTime.ToString(recastTimeFormat) :
                this.IsReverse ? Settings.Default.OverText : Settings.Default.ReadyText;
            if (tb.Text != recast)
            {
                tb.Text = recast;
                tb.SetFontInfo(this.FontInfo);
                tb.Fill = this.FontBrush;
                tb.Stroke = this.FontOutlineBrush;
                tb.StrokeThickness = 0.5d * tb.FontSize / 13.0d;
            }
        }

        /// <summary>
        /// 描画設定を更新する
        /// </summary>
        public void Update()
        {
#if false
            var sw = Stopwatch.StartNew();
#endif
            this.Width = this.SpellWidth;

            // Brushを生成する
            var fontColor = string.IsNullOrWhiteSpace(this.FontColor) ?
                Settings.Default.FontColor.ToWPF() :
                this.FontColor.FromHTMLWPF();
            var fontOutlineColor = string.IsNullOrWhiteSpace(this.FontOutlineColor) ?
                Settings.Default.FontOutlineColor.ToWPF() :
                this.FontOutlineColor.FromHTMLWPF();
            var barColor = string.IsNullOrWhiteSpace(this.BarColor) ?
                Settings.Default.ProgressBarColor.ToWPF() :
                this.BarColor.FromHTMLWPF();
            var barBackColor = barColor.ChangeBrightness(0.4d);
            var barOutlineColor = string.IsNullOrWhiteSpace(this.BarOutlineColor) ?
                Settings.Default.ProgressBarOutlineColor.ToWPF() :
                this.BarOutlineColor.FromHTMLWPF();

            this.FontBrush = this.GetBrush(fontColor);
            this.FontOutlineBrush = this.GetBrush(fontOutlineColor);
            this.BarBrush = this.GetBrush(barColor);
            this.BarBackBrush = this.GetBrush(barBackColor);
            this.BarOutlineBrush = this.GetBrush(barOutlineColor);

            var tb = default(OutlineTextBlock);
            var font = this.FontInfo;

            // アイコンを描画する
            var image = this.SpellIconImage;
            var iconFile = IconController.Default.getIconFile(this.SpellIcon);
            if (image.Source == null && iconFile != null)
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

            if (tb.Text != title)
            {
                tb.Text = title;
                tb.SetFontInfo(font);
                tb.Fill = this.FontBrush;
                tb.Stroke = this.FontOutlineBrush;
                tb.StrokeThickness = 0.65d *
                    this.FontSize *
                    (this.FontWeight.ToOpenTypeWeight() / FontWeights.Normal.ToOpenTypeWeight()) /
                    13.0d;

            }

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
            this.BarCanvas.Width = this.BarWidth;

            var foreRect = this.BarRectangle;
            foreRect.Stroke = this.BarBrush;
            foreRect.Fill = this.BarBrush;
            foreRect.Width = this.BarWidth;
            foreRect.Height = this.BarHeight;
            foreRect.RadiusX = 2.0d;
            foreRect.RadiusY = 2.0d;
            Canvas.SetLeft(foreRect, 0);
            Canvas.SetTop(foreRect, 0);

            var backRect = this.BarBackRectangle;
            backRect.Stroke = this.BarBackBrush;
            backRect.Fill = this.BarBackBrush;
            backRect.Width = this.BarWidth;
            backRect.Height = this.BarHeight;
            backRect.RadiusX = 2.0d;
            backRect.RadiusY = 2.0d;
            Canvas.SetLeft(backRect, 0);
            Canvas.SetTop(backRect, 0);

            var outlineRect = this.BarOutlineRectangle;
            outlineRect.Stroke = this.BarOutlineBrush;
            outlineRect.StrokeThickness = 1.0d;
            outlineRect.Width = this.BarWidth;
            outlineRect.Height = this.BarHeight;
            outlineRect.RadiusX = 2.0d;
            outlineRect.RadiusY = 2.0d;
            Canvas.SetLeft(outlineRect, 0);
            Canvas.SetTop(outlineRect, 0);

            // バーのエフェクトの色を設定する
            this.BarEffect.Color = this.BarBrush.Color.ChangeBrightness(1.05d);

            this.ProgressBarCanvas.Width = backRect.Width;
            this.ProgressBarCanvas.Height = backRect.Height;

#if false
            sw.Stop();
            Debug.WriteLine("Spell Refresh -> " + sw.ElapsedMilliseconds.ToString("N0") + "ms");
#endif
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
    }
}
