namespace ACT.SpecialSpellTimer.Utility
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// MetroスタイルWindow
    /// </summary>
    public partial class MetroWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MetroWindow()
        {
            this.ShowInTaskbar = true;
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState != MouseButtonState.Pressed)
                {
                    return;
                }

                this.DragMove();
            };
        }
    }
}