using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using FFXIV.Framework.Common;

namespace ACT.SpecialSpellTimer.Views
{
    /// <summary>
    /// TickerControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TickerControl : UserControl
    {
        public TickerControl()
        {
            this.InitializeComponent();

            if (WPFHelper.IsDesignMode)
            {
                this.Message = "サンプルテロップ";
                this.Font = new FontInfo(
                    "メイリオ",
                    30.0,
                    "Normal",
                    "Bold",
                    "Normal");

                this.FontBrush = new SolidColorBrush(Colors.Red);
                this.FontOutlineBrush = new SolidColorBrush(Colors.White);
                this.BarVisible = true;
            }
        }

        public string Message
        {
            get => this.MessageTextBlock.Text;
            set
            {
                if (this.MessageTextBlock.Text != value)
                {
                    this.MessageTextBlock.Text = value;

                    if (string.IsNullOrEmpty(this.MessageTextBlock.Text))
                    {
                        this.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public FontInfo Font
        {
            get => this.MessageTextBlock.GetFontInfo();
            set
            {
                if (this.MessageTextBlock.SetFontInfo(value))
                {
                    this.MessageTextBlock.SetAutoStrokeThickness();
                }
            }
        }

        /// <summary>フォントのBrush</summary>
        public SolidColorBrush FontBrush
        {
            get => this.MessageTextBlock.Fill as SolidColorBrush;
            set
            {
                if (this.MessageTextBlock.Fill != value)
                {
                    this.MessageTextBlock.Fill = value;
                }
            }
        }

        /// <summary>フォントのアウトラインBrush</summary>
        public SolidColorBrush FontOutlineBrush
        {
            get => this.MessageTextBlock.Stroke as SolidColorBrush;
            set
            {
                if (this.MessageTextBlock.Stroke != value)
                {
                    this.MessageTextBlock.Stroke = value;
                }
            }
        }

        public double BarHeight
        {
            get => this.BarRectangle.Height;
            set
            {
                if (this.BarRectangle.Height != value)
                {
                    this.BarRectangle.Height = value;
                }
            }
        }

        public bool BarVisible
        {
            get => this.ProgressBarCanvas.Visibility == Visibility.Visible;
            set
            {
                var visibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (this.ProgressBarCanvas.Visibility != visibility)
                {
                    this.ProgressBarCanvas.Visibility = visibility;
                }
            }
        }

        #region Animation

        private DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames()
        {
            AutoReverse = false,
            KeyFrames = new DoubleKeyFrameCollection()
            {
                new LinearDoubleKeyFrame()
            }
        };

        private LinearDoubleKeyFrame KeyFrame => (LinearDoubleKeyFrame)this.animation.KeyFrames[0];

        public void ResetProgressBar()
        {
            this.BarRectangle.Width = this.BarBackRectangle.Width;
        }

        public void StartProgressBar(
            double timeToCount)
        {
            if (!this.BarVisible)
            {
                return;
            }

            if (timeToCount >= 0)
            {
                this.BarRectangle.BeginAnimation(
                    Rectangle.WidthProperty,
                    null);

                this.ResetProgressBar();

                this.KeyFrame.Value = 0;
                this.KeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeToCount));

                Timeline.SetDesiredFrameRate(this.animation, 30);

                this.BarRectangle.BeginAnimation(
                    Rectangle.WidthProperty,
                    this.animation);
            }
        }

        #endregion Animation
    }
}
