using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;

namespace ACT.SpecialSpellTimer.Views
{
    /// <summary>
    /// ワンポイントテロップWindow
    /// </summary>
    public partial class OnePointTelopWindow : Window
    {
        /// <summary>
        /// ドラッグ終了
        /// </summary>
        private Action<MouseEventArgs> DragOff;

        /// <summary>
        /// ドラッグ開始
        /// </summary>
        private Action<MouseEventArgs> DragOn;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OnePointTelopWindow()
        {
            this.InitializeComponent();

            this.MessageTextBlock.Text = string.Empty;

            this.Loaded += this.OnePointTelopWindow_Loaded;
            this.MouseLeftButtonDown += (s1, e1) => this.DragMove();

            this.DragOn = new Action<MouseEventArgs>((mouse) =>
            {
                if (mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.IsDragging = true;
                }
            });

            this.DragOff = new Action<MouseEventArgs>((mouse) =>
            {
                if (mouse.LeftButton == MouseButtonState.Released)
                {
                    this.IsDragging = false;
                }
            });

            this.MouseDown += (s1, e1) => this.DragOn(e1);
            this.MouseUp += (s1, e1) => this.DragOff(e1);
            this.MessageTextBlock.MouseDown += (s1, e1) => this.DragOn(e1);
            this.MessageTextBlock.MouseUp += (s1, e1) => this.DragOff(e1);
            this.ProgressBarCanvas.MouseDown += (s1, e1) => this.DragOn(e1);
            this.ProgressBarCanvas.MouseUp += (s1, e1) => this.DragOff(e1);

            // アニメーションを準備する
            this.SetupAnimation();
        }

        /// <summary>
        /// 表示するデータソース
        /// </summary>
        public OnePointTelop DataSource { get; set; }

        /// <summary>
        /// ドラッグ中か？
        /// </summary>
        public bool IsDragging { get; private set; }

        /// <summary>背景色のBrush</summary>
        private SolidColorBrush BackgroundBrush { get; set; }

        /// <summary>バーの背景のBrush</summary>
        private SolidColorBrush BarBackBrush { get; set; }

        /// <summary>バーのBrush</summary>
        private SolidColorBrush BarBrush { get; set; }

        /// <summary>バーのアウトラインのBrush</summary>
        private SolidColorBrush BarOutlineBrush { get; set; }

        /// <summary>フォントのBrush</summary>
        private SolidColorBrush FontBrush { get; set; }

        /// <summary>フォントのアウトラインBrush</summary>
        private SolidColorBrush FontOutlineBrush { get; set; }

        /// <summary>
        /// 描画を更新する
        /// </summary>
        public void Refresh()
        {
            if (this.IsDragging)
            {
                return;
            }

            if (this.DataSource == null)
            {
                this.HideOverlay();
                return;
            }

            // Brushを生成する
            var fontColor = this.DataSource.FontColor.FromHTML().ToWPF();
            var fontOutlineColor = string.IsNullOrWhiteSpace(this.DataSource.FontOutlineColor) ?
                Settings.Default.FontOutlineColor.ToWPF() :
                this.DataSource.FontOutlineColor.FromHTMLWPF();
            var barColor = fontColor;
            var barBackColor = barColor.ChangeBrightness(0.4d);
            var barOutlineColor = fontOutlineColor;
            var c = this.DataSource.BackgroundColor.FromHTML().ToWPF();
            var backGroundColor = Color.FromArgb(
                (byte)this.DataSource.BackgroundAlpha,
                c.R,
                c.G,
                c.B);

            this.FontBrush = this.GetBrush(fontColor);
            this.FontOutlineBrush = this.GetBrush(fontOutlineColor);
            this.BarBrush = this.GetBrush(barColor);
            this.BarBackBrush = this.GetBrush(barBackColor);
            this.BarOutlineBrush = this.GetBrush(barOutlineColor);
            this.BackgroundBrush = this.GetBrush(backGroundColor);

            // 背景色を設定する
            var nowbackground = this.BaseColorRectangle.Fill as SolidColorBrush;
            if (nowbackground == null ||
                nowbackground.Color != this.BackgroundBrush.Color)
            {
                this.BaseColorRectangle.Fill = this.BackgroundBrush;
            }

            var message = Settings.Default.TelopAlwaysVisible ?
                this.DataSource.Message.Replace(",", Environment.NewLine) :
                this.DataSource.MessageReplaced.Replace(",", Environment.NewLine);

            // カウントダウンプレースホルダを置換する
            var count = (
                this.DataSource.MatchDateTime.AddSeconds(DataSource.Delay + DataSource.DisplayTime) -
                DateTime.Now).TotalSeconds;

            if (count < 0.0d)
            {
                count = 0.0d;
            }

            if (Settings.Default.TelopAlwaysVisible)
            {
                count = 0.0d;
            }

            var countAsText = count.ToString("N1");
            var displayTimeAsText = this.DataSource.DisplayTime.ToString("N1");
            countAsText = countAsText.PadLeft(displayTimeAsText.Length, '0');

            var count0AsText = count.ToString("N0");
            var displayTime0AsText = this.DataSource.DisplayTime.ToString("N0");
            count0AsText = count0AsText.PadLeft(displayTime0AsText.Length, '0');

            message = message.Replace("{COUNT}", countAsText);
            message = message.Replace("{COUNT0}", count0AsText);

            // テキストブロックにセットする
            var textBlock = this.MessageTextBlock;
            if (textBlock.Text != message) textBlock.Text = message;
            if (textBlock.Fill != this.FontBrush) textBlock.Fill = this.FontBrush;
            if (textBlock.Stroke != this.FontOutlineBrush) textBlock.Stroke = this.FontOutlineBrush;
            textBlock.SetFontInfo(this.DataSource.Font);
            textBlock.SetAutoStrokeThickness();

            // プログレスバーを表示しない？
            if (!this.DataSource.ProgressBarEnabled ||
                this.DataSource.DisplayTime <= 0)
            {
                this.ProgressBarCanvas.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeProgressBar()
        {
            // アニメーションを停止させる
            this.BarRectangle.BeginAnimation(
                Rectangle.WidthProperty,
                null);

            var baseHeight = Settings.Default.ProgressBarSize.Height;
            var baseWidth = this.MessageTextBlock.ActualWidth;

            var barRect = this.BarRectangle;
            if (barRect.Fill != this.BarBrush) barRect.Fill = this.BarBrush;
            if (barRect.Width != baseWidth) barRect.Width = baseWidth;
            if (barRect.Height != baseHeight) barRect.Height = baseHeight;

            var backRect = this.BarBackRectangle;
            if (backRect.Fill != this.BarBackBrush) backRect.Fill = this.BarBackBrush;
            if (backRect.Width != baseWidth) backRect.Width = baseWidth;

            var outlineRect = this.BarOutlineRectangle;
            if (outlineRect.Stroke != this.BarOutlineBrush) outlineRect.Stroke = this.BarOutlineBrush;

            // バーのエフェクトの色を設定する
            var barEffectColor = this.BarBrush.Color.ChangeBrightness(1.05d);
            if (this.BarEffect.Color != barEffectColor) this.BarEffect.Color = barEffectColor;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void OnePointTelopWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataSource != null)
            {
                this.Left = this.DataSource.Left;
                this.Top = this.DataSource.Top;
            }

            this.Refresh();
        }

        #region Animation

        private DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
        private LinearDoubleKeyFrame keyframe1 = new LinearDoubleKeyFrame();

        public void StartProgressBar()
        {
            if (this.DataSource == null ||
                !this.DataSource.ProgressBarEnabled)
            {
                this.ProgressBarCanvas.Visibility = Visibility.Collapsed;
                return;
            }

            this.InitializeProgressBar();
            this.ProgressBarCanvas.Visibility = Visibility.Visible;

            var matchDateTime = this.DataSource.MatchDateTime;
            if (matchDateTime <= DateTime.MinValue)
            {
                matchDateTime = DateTime.Now;
            }

            var timeToHide = matchDateTime.AddSeconds(
                this.DataSource.Delay + this.DataSource.DisplayTime);

            var timeToLive = (timeToHide - DateTime.Now).TotalMilliseconds;

            if (timeToLive >= 0)
            {
                this.keyframe1.Value = 0;
                this.keyframe1.KeyTime = TimeSpan.FromMilliseconds(timeToLive);

                this.BarRectangle.BeginAnimation(
                    Rectangle.WidthProperty,
                    this.animation);
            }
        }

        private void SetupAnimation()
        {
            this.animation.KeyFrames.Add(this.keyframe1);
        }

        #endregion Animation

        #region フォーカスを奪わない対策

        private const int GWL_EXSTYLE = -20;

        private const int WS_EX_NOACTIVATE = 0x08000000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion フォーカスを奪わない対策
    }
}
