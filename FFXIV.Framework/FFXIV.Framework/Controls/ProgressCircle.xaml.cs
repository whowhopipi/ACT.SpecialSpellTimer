#if false
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FFXIV.Framework.Controls
{
    /// <summary>
    /// ProgressCircle.xaml の相互作用ロジック
    /// </summary>
    public partial class ProgressCircle : UserControl
    {
#region Radius 依存関係プロパティ

        /// <summary>
        /// Radius 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty RadiusProperty
            = DependencyProperty.Register(
            nameof(Radius),
            typeof(double),
            typeof(ProgressCircle));

        /// <summary>
        /// Radius
        /// </summary>
        public double Radius
        {
            get => this.BackInnerCircle.Radius;
            set => this.BackInnerCircle.Radius = value;
        }

#endregion Radius 依存関係プロパティ

#region Fore 依存関係プロパティ

        /// <summary>
        /// Fore 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty ForeProperty
            = DependencyProperty.Register(
            nameof(Fore),
            typeof(SolidColorBrush),
            typeof(ProgressCircle));

        /// <summary>
        /// Fore
        /// </summary>
        public SolidColorBrush Fore
        {
            get => this.ForeInnerCircle.Stroke as SolidColorBrush;
            set => this.ForeInnerCircle.Stroke = value;
        }

#endregion Fore 依存関係プロパティ

#region Back 依存関係プロパティ

        /// <summary>
        /// Back 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty BackProperty
            = DependencyProperty.Register(
            nameof(Back),
            typeof(SolidColorBrush),
            typeof(ProgressCircle));

        /// <summary>
        /// Back
        /// </summary>
        public SolidColorBrush Back
        {
            get => this.BackInnerCircle.Stroke as SolidColorBrush;
            set => this.BackInnerCircle.Stroke = value;
        }

#endregion Back 依存関係プロパティ

#region StrokeThickness 依存関係プロパティ

        /// <summary>
        /// StrokeThickness 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty
            = DependencyProperty.Register(
            nameof(StrokeThickness),
            typeof(double),
            typeof(ProgressCircle));

        /// <summary>
        /// StrokeThickness
        /// </summary>
        public double StrokeThickness
        {
            get => this.ForeInnerCircle.StrokeThickness;
            set => this.ForeInnerCircle.StrokeThickness = value;
        }

#endregion StrokeThickness 依存関係プロパティ

#region StartAngle 依存関係プロパティ

        /// <summary>
        /// StartAngle 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty
            = DependencyProperty.Register(
            nameof(StartAngle),
            typeof(double),
            typeof(ProgressCircle));

        /// <summary>
        /// StartAngle
        /// </summary>
        public double StartAngle
        {
            get => this.ForeInnerCircle.StartAngle;
            set => this.ForeInnerCircle.StartAngle = value;
        }

#endregion StartAngle 依存関係プロパティ

#region EndAngle 依存関係プロパティ

        /// <summary>
        /// EndAngle 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty
            = DependencyProperty.Register(
            nameof(EndAngle),
            typeof(double),
            typeof(ProgressCircle));

        /// <summary>
        /// EndAngle
        /// </summary>
        public double EndAngle
        {
            get => this.ForeInnerCircle.EndAngle;
            set => this.ForeInnerCircle.EndAngle = value;
        }

#endregion EndAngle 依存関係プロパティ

        public ProgressCircle()
        {
            this.InitializeComponent();
        }
    }
}
#endif
