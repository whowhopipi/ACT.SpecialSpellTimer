using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Models;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer.Views
{
    /// <summary>
    /// ワンポイントテロップWindow
    /// </summary>
    public partial class TickerWindow :
        Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TickerWindow()
        {
            this.InitializeComponent();

            this.Loaded += this.OnePointTelopWindow_Loaded;
            this.MouseLeftButtonDown += (s1, e1) => this.DragMove();
        }

        /// <summary>
        /// 表示するデータソース
        /// </summary>
        public OnePointTelop DataSource { get; set; }

        /// <summary>背景色のBrush</summary>
        private SolidColorBrush BackgroundBrush { get; set; }

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
            this.BarOutlineBrush = this.GetBrush(barOutlineColor);
            this.BackgroundBrush = this.GetBrush(backGroundColor);

            // 背景色を設定する
            var nowbackground = this.BaseColorRectangle.Fill as SolidColorBrush;
            if (nowbackground == null ||
                nowbackground.Color != this.BackgroundBrush.Color)
            {
                this.BaseColorRectangle.Fill = this.BackgroundBrush;
            }

            var forceVisible =
                Settings.Default.TelopAlwaysVisible ||
                this.DataSource.IsTemporaryDisplay;

            var message = forceVisible ?
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
            this.TickerControl.Message = message;
            this.TickerControl.Font = this.DataSource.Font;
            this.TickerControl.FontBrush = this.FontBrush;
            this.TickerControl.FontOutlineBrush = this.FontOutlineBrush;

            // プログレスバーを表示しない？
            if (!this.DataSource.ProgressBarEnabled ||
                this.DataSource.DisplayTime <= 0)
            {
                this.TickerControl.BarVisible = false;
            }
            else
            {
                this.TickerControl.BarVisible = true;
            }

            // プログレスバーを初期化する
            this.TickerControl.BarHeight = Settings.Default.ProgressBarSize.Height;
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

        public void StartProgressBar()
        {
            if (this.DataSource == null ||
                !this.DataSource.ProgressBarEnabled)
            {
                this.TickerControl.BarVisible = false;
                return;
            }

            var matchDateTime = this.DataSource.MatchDateTime;
            if (matchDateTime <= DateTime.MinValue)
            {
                matchDateTime = DateTime.Now;
            }

            var timeToHide = matchDateTime.AddSeconds(
                this.DataSource.Delay + this.DataSource.DisplayTime);
            var timeToLive = (timeToHide - DateTime.Now).TotalMilliseconds;

            this.TickerControl.StartProgressBar(timeToLive);
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
