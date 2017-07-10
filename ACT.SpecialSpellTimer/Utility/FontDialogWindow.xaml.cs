namespace ACT.SpecialSpellTimer.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml.Serialization;

    using ACT.SpecialSpellTimer.Views;

    public static class ControlExtension
    {
        public static FontInfo GetFontInfo(
            this Control control)
        {
            return new FontInfo(
                control.FontFamily,
                control.FontSize,
                control.FontStyle,
                control.FontWeight,
                control.FontStretch);
        }

        public static void SetFontInfo(
                    this Control control,
            FontInfo fontInfo)
        {
            if (control.GetFontInfo().ToString() != fontInfo.ToString())
            {
                control.FontFamily = fontInfo.Family;
                control.FontSize = fontInfo.Size;
                control.FontStyle = fontInfo.Style;
                control.FontWeight = fontInfo.Weight;
                control.FontStretch = fontInfo.Stretch;
            }
        }

        internal static FontInfo GetFontInfo(
            this OutlineTextBlock control)
        {
            return new FontInfo(
                control.FontFamily,
                control.FontSize,
                control.FontStyle,
                control.FontWeight,
                control.FontStretch);
        }

        internal static FontInfo GetFontInfo(
            this TextBlock control)
        {
            return new FontInfo(
                control.FontFamily,
                control.FontSize,
                control.FontStyle,
                control.FontWeight,
                control.FontStretch);
        }

        internal static void SetFontInfo(
                            this OutlineTextBlock control,
            FontInfo fontInfo)
        {
            if (control.GetFontInfo().ToString() != fontInfo.ToString())
            {
                control.FontFamily = fontInfo.Family;
                control.FontSize = fontInfo.Size;
                control.FontStyle = fontInfo.Style;
                control.FontWeight = fontInfo.Weight;
                control.FontStretch = fontInfo.Stretch;
            }
        }

        internal static void SetFontInfo(
            this TextBlock control,
            FontInfo fontInfo)
        {
            if (control.GetFontInfo().ToString() != fontInfo.ToString())
            {
                control.FontFamily = fontInfo.Family;
                control.FontSize = fontInfo.Size;
                control.FontStyle = fontInfo.Style;
                control.FontWeight = fontInfo.Weight;
                control.FontStretch = fontInfo.Stretch;
            }
        }
    }

    public partial class FontDialogWindow : MetroWindow
    {
        private FontInfo fontInfo = new FontInfo();

        public FontDialogWindow()
        {
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);

            this.InitializeComponent();

            this.Loaded += (s, e) =>
            {
                this.ShowFontInfo();

                // リストボックスにフォーカスを設定する
                ListBox box;

                box = this.FontStyleListBox;
                if (box.SelectedItem != null)
                {
                    var item =
                        box.ItemContainerGenerator.ContainerFromItem(box.SelectedItem)
                        as ListBoxItem;

                    if (item != null)
                    {
                        item.Focus();
                    }
                }

                box = this.FontFamilyListBox;
                if (box.SelectedItem != null)
                {
                    var item =
                        box.ItemContainerGenerator.ContainerFromItem(box.SelectedItem)
                        as ListBoxItem;

                    if (item != null)
                    {
                        item.Focus();
                    }
                }
            };

            this.FontSizeTextBox.PreviewKeyDown += this.FontSizeTextBox_PreviewKeyDown;
            this.FontSizeTextBox.LostFocus += (s, e) =>
            {
                const double MinSize = 5.0;

                var t = (s as TextBox).Text;

                double d;
                if (double.TryParse(t, out d))
                {
                    if (d < MinSize)
                    {
                        d = MinSize;
                    }

                    (s as TextBox).Text = d.ToString("N1");
                }
                else
                {
                    (s as TextBox).Text = MinSize.ToString("N0");
                }
            };

            this.FontFamilyListBox.SelectionChanged += this.FontFamilyListBox_SelectionChanged;
            this.OKBUtton.Click += this.OKBUtton_Click;
            this.CancelBUtton.Click += this.CancelBUtton_Click;
        }

        public FontInfo FontInfo
        {
            get
            {
                return this.fontInfo;
            }
            set
            {
                this.fontInfo = value;
                this.ShowFontInfo();
            }
        }

        public void SetOwner(
            System.Windows.Forms.Control windowsControl)
        {
            var helper = new WindowInteropHelper(this);
            helper.Owner = windowsControl.Handle;
        }

        private void CancelBUtton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void FontFamilyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FontStyleListBox.SelectedIndex = 0;
        }

        private void FontSizeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var t = sender as TextBox;

            decimal d;

            if (e.Key == Key.Up)
            {
                if (decimal.TryParse(t.Text, out d))
                {
                    t.Text = (d + 0.5m).ToString("N1");
                }
            }

            if (e.Key == Key.Down)
            {
                if (decimal.TryParse(t.Text, out d))
                {
                    if ((d - 0.5m) >= 1.0m)
                    {
                        t.Text = (d - 0.5m).ToString("N1");
                    }
                }
            }
        }

        private void OKBUtton_Click(object sender, RoutedEventArgs e)
        {
            this.fontInfo = this.PreviewTextBlock.GetFontInfo();

            this.DialogResult = true;
            this.Close();
        }

        private void ShowFontInfo()
        {
            this.FontSizeTextBox.Text = this.fontInfo.Size.ToString("N1");

            int i = 0;
            foreach (FontFamily item in this.FontFamilyListBox.Items)
            {
                if (this.fontInfo.Family != null)
                {
                    if (item.Source == this.fontInfo.Family.Source ||
                        item.FamilyNames.Any(x => x.Value == this.fontInfo.Family.Source))
                    {
                        break;
                    }
                }

                i++;
            }

            if (i < this.FontFamilyListBox.Items.Count)
            {
                this.FontFamilyListBox.SelectedIndex = i;
                this.FontFamilyListBox.ScrollIntoView(this.FontFamilyListBox.Items[i]);
            }

            this.FontStyleListBox.SelectedItem = this.fontInfo.Typeface;
        }
    }

    public class FontFamilyToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as FontFamily;
            var currentLang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            return v.FamilyNames.FirstOrDefault(o => o.Key == currentLang).Value ?? v.Source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [Serializable]
    public class FontInfo
    {
        [XmlIgnore]
        private static Dictionary<string, FontFamily> fontFamilyDictionary = new Dictionary<string, FontFamily>();

        [XmlIgnore]
        private static FontStretchConverter stretchConverter = new FontStretchConverter();

        [XmlIgnore]
        private static FontStyleConverter styleConverter = new FontStyleConverter();

        [XmlIgnore]
        private static FontWeightConverter weightConverter = new FontWeightConverter();

        public FontInfo()
        {
            this.Family = GetFontFamily(string.Empty);
            this.Size = 11.25;
        }

        public FontInfo(
            string family,
            double size,
            string style,
            string weight,
            string stretch)
        {
            this.Family = GetFontFamily(family);
            this.Size = size;
            this.StyleString = style;
            this.WeightString = weight;
            this.StretchString = stretch;
        }

        public FontInfo(
            FontFamily family,
            double size,
            FontStyle style,
            FontWeight weight,
            FontStretch stretch)
        {
            this.Family = family;
            this.Size = size;
            this.Style = style;
            this.Weight = weight;
            this.Stretch = stretch;
        }

        [XmlIgnore]
        public FontFamily Family { get; set; }

        [XmlAttribute("FontFamily")]
        public string FamilyName
        {
            get
            {
                return
                    this.Family != null ?
                    this.Family.Source ?? string.Empty :
                    string.Empty;
            }
            set
            {
                this.Family = GetFontFamily(value);
            }
        }

        [XmlAttribute("Size")]
        public double Size { get; set; }

        [XmlIgnore]
        public FontStretch Stretch { get; set; }

        [XmlAttribute("Stretch")]
        public string StretchString
        {
            get
            {
                return stretchConverter.ConvertToString(this.Stretch);
            }
            set
            {
                this.Stretch = (FontStretch)stretchConverter.ConvertFromString(value);
            }
        }

        [XmlIgnore]
        public FontStyle Style { get; set; }

        [XmlAttribute("Style")]
        public string StyleString
        {
            get
            {
                return styleConverter.ConvertToString(this.Style);
            }
            set
            {
                this.Style = (FontStyle)styleConverter.ConvertFromString(value);
            }
        }

        [XmlIgnore]
        public FamilyTypeface Typeface
        {
            get
            {
                var ftf = new FamilyTypeface();
                ftf.Stretch = this.Stretch;
                ftf.Weight = this.Weight;
                ftf.Style = this.Style;
                return ftf;
            }
        }

        [XmlIgnore]
        public FontWeight Weight { get; set; }

        [XmlAttribute("Weight")]
        public string WeightString
        {
            get
            {
                return weightConverter.ConvertToString(this.Weight);
            }
            set
            {
                this.Weight = (FontWeight)weightConverter.ConvertFromString(value);
            }
        }

        public System.Drawing.Font ToFontForWindowsForm()
        {
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;

            if (this.Style == FontStyles.Italic ||
                this.Style == FontStyles.Oblique)
            {
                style |= System.Drawing.FontStyle.Italic;
            }

            if (this.Weight > FontWeights.Normal)
            {
                style |= System.Drawing.FontStyle.Bold;
            }

            System.Drawing.Font f = new System.Drawing.Font(
                this.FamilyName,
                (float)(this.Size * 72.0d / 96.0d),
                style);

            return f;
        }

        public override string ToString()
        {
            var t = string.Empty;

            t += "{ ";
            t += "\"Family\":" + "\"" + this.FamilyName + "\"" + ", ";
            t += "\"Size\":" + this.Size + ", ";
            t += "\"Style\":" + "\"" + this.StyleString + "\"" + ", ";
            t += "\"Weight\":" + "\"" + this.WeightString + "\"" + ", ";
            t += "\"Streth\":" + "\"" + this.StretchString + "\"";
            t += " }";

            return t;
        }

        private static FontFamily GetFontFamily(
            string source)
        {
            if (!fontFamilyDictionary.ContainsKey(source))
            {
                fontFamilyDictionary[source] = new FontFamily(source);
            }

            return fontFamilyDictionary[source];
        }
    }
}