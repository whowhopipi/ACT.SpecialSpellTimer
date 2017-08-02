using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Image;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;

namespace ACT.SpecialSpellTimer
{
    [Serializable]
    public class ColorSet
    {
        public int BackgroundAlpha { get; set; }
        public string BackgroundColor { get; set; }
        public string BarColor { get; set; }
        public string BarOutlineColor { get; set; }
        public string FontColor { get; set; }
        public string FontOutlineColor { get; set; }
        public string WarningFontColor { get; set; }
        public string WarningFontOutlineColor { get; set; }
    }

    /// <summary>
    /// 見た目設定用コントロール
    /// </summary>
    public partial class VisualSettingControl : UserControl
    {
        private VisualSettingControlBackgoundColorForm alphaDialog = new VisualSettingControlBackgoundColorForm();
        private Color backgroundColor;
        private bool barEnabled;
        private FontInfo fontInfo = FontInfo.DefaultFont;

        public VisualSettingControl()
        {
            this.InitializeComponent();

            this.components.Add(this.alphaDialog);

            if (Settings.Default.Font != null)
            {
                this.SetFontInfo(Settings.Default.Font.ToFontInfo());
            }

            this.FontColor = Settings.Default.FontColor;
            this.FontOutlineColor = Settings.Default.FontOutlineColor;
            this.WarningFontColor = Settings.Default.WarningFontColor;
            this.WarningFontOutlineColor = Settings.Default.WarningFontOutlineColor;
            this.BarColor = Settings.Default.ProgressBarColor;
            this.BarOutlineColor = Settings.Default.ProgressBarOutlineColor;
            this.BarSize = Settings.Default.ProgressBarSize;

            this.SpellIcon = string.Empty;

            this.BarEnabled = true;

            this.Load += this.VisualSettingControl_Load;
        }

        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                this.backgroundColor = value;
                this.alphaDialog.Alpha = this.backgroundColor.A;
            }
        }

        public Color BarColor { get; set; }

        public bool BarEnabled
        {
            get { return this.barEnabled; }
            set
            {
                this.barEnabled = value;

                if (this.barEnabled)
                {
                    this.WidthNumericUpDown.Visible = true;
                    this.HeightNumericUpDown.Visible = true;
                    this.BarSizeLabel.Visible = true;
                    this.BarSizeXLabel.Visible = true;
                    this.ChangeBarColorItem.Enabled = true;
                    this.ChangeBarOutlineColorItem.Enabled = true;
                    this.ResetSpellBarSizeItem.Enabled = true;
                }
                else
                {
                    this.WidthNumericUpDown.Visible = false;
                    this.HeightNumericUpDown.Visible = false;
                    this.BarSizeLabel.Visible = false;
                    this.BarSizeXLabel.Visible = false;
                    this.ChangeBarColorItem.Enabled = false;
                    this.ChangeBarOutlineColorItem.Enabled = false;
                    this.ResetSpellBarSizeItem.Enabled = false;
                }

                this.RefreshSampleImage();
            }
        }

        public Color BarOutlineColor { get; set; }

        public Size BarSize
        {
            get
            {
                return new Size(
                    (int)this.WidthNumericUpDown.Value,
                    (int)this.HeightNumericUpDown.Value);
            }
            set
            {
                this.WidthNumericUpDown.Value = value.Width;
                this.HeightNumericUpDown.Value = value.Height;
            }
        }

        public Color FontColor { get; set; }

        public Color FontOutlineColor { get; set; }

        public string GetColorSetDirectory
        {
            get
            {
                // ACTのパスを取得する
                var asm = Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    var actDirectory = Path.GetDirectoryName(asm.Location);
                    var resourcesUnderAct = Path.Combine(actDirectory, @"resources\color");

                    if (Directory.Exists(resourcesUnderAct))
                    {
                        return resourcesUnderAct;
                    }
                }

                // 自身の場所を取得する
                var selfDirectory = SpecialSpellTimerPlugin.Location ?? string.Empty;
                var resourcesUnderThis = Path.Combine(selfDirectory, @"resources\color");

                if (Directory.Exists(resourcesUnderThis))
                {
                    return resourcesUnderThis;
                }

                return string.Empty;
            }
        }

        public bool HideSpellName { get; set; }
        public bool OverlapRecastTime { get; set; }
        public String SpellIcon { get; set; }
        public int SpellIconSize { get; set; }
        public Color WarningFontColor { get; set; }

        public Color WarningFontOutlineColor { get; set; }

        public FontInfo GetFontInfo()
        {
            return fontInfo;
        }

        /// <summary>
        /// サンプルイメージを描画する
        /// </summary>
        public void RefreshSampleImage()
        {
            var font = this.GetFontInfo().ToFontForWindowsForm();
            var fontColor = this.FontColor;
            var fontOutlineColor = this.FontOutlineColor;
            var barColor = this.BarColor;
            var barOutlineColor = this.BarOutlineColor;
            var barSize = this.BarEnabled ?
                this.BarSize :
                this.SamplePictureBox.Size;
            var barLocation = new Point(
                (this.SamplePictureBox.Width / 2) - (barSize.Width / 2),
                this.SamplePictureBox.Height - barSize.Height - 12);

            var spellWidth = barSize.Width > this.SpellIconSize ? barSize.Width : this.SpellIconSize;
            var spellX = barSize.Width > this.SpellIconSize ? barLocation.X : (this.SamplePictureBox.Size.Width / 2) - (this.SpellIconSize / 2);

            var bmp = new Bitmap(this.SamplePictureBox.Width, this.SamplePictureBox.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // 背景色を描く
                var backgroundRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                var backgroundBrush = new SolidBrush(this.backgroundColor);
                g.FillRectangle(backgroundBrush, backgroundRect);

                if (this.BarEnabled)
                {
                    // バーの暗を描く
                    var backRect = new Rectangle(barLocation, barSize);
                    var backBrush = new SolidBrush(barColor.ChangeBrightness(0.4d));
                    g.FillRectangle(backBrush, backRect);

                    // バーの明を描く
                    var foreRect = new Rectangle(barLocation, new Size((int)(barSize.Width * 0.6), barSize.Height));
                    var foreBrush = new SolidBrush(barColor);
                    g.FillRectangle(foreBrush, foreRect);

                    // バーのアウトラインを描く
                    var outlineRect = new Rectangle(barLocation, barSize);
                    var outlinePen = new Pen(barOutlineColor, 1.0f);
                    g.DrawRectangle(outlinePen, outlineRect);

                    // 後片付け
                    backBrush.Dispose();
                    foreBrush.Dispose();
                    outlinePen.Dispose();
                }

                // アイコンを描く
                var hasIcon = false;
                if (this.BarEnabled)
                {
                    var spellIcon = IconController.Default.getIconFile(this.SpellIcon);
                    if (spellIcon != null)
                    {
                        var image = System.Drawing.Image.FromFile(spellIcon.FullPath);
                        g.DrawImage(
                            image,
                            spellX,
                            barLocation.Y - this.SpellIconSize,
                            (float)this.SpellIconSize,
                            (float)this.SpellIconSize);
                        hasIcon = true;
                    }
                }

                // フォントのペンを生成する
                var fontBrush = new SolidBrush(fontColor);
                var fontOutlinePen = new Pen(fontOutlineColor, 0.2f);
                var fontHeight = font.Size * 2; // 正しくない計算
                var fontRect = new Rectangle(
                    hasIcon ? spellX + this.SpellIconSize : spellX,
                    barLocation.Y - 2 - (int)fontHeight,
                    hasIcon ? spellWidth - this.SpellIconSize : spellWidth,
                    barLocation.Y - 2);

                if (!this.BarEnabled)
                {
                    fontRect = new Rectangle(
                        barLocation.X,
                        6,
                        barSize.Width,
                        this.SamplePictureBox.Height - 6);
                }

                // フォントを描く
                var spellSf = new StringFormat()
                {
                    Alignment = StringAlignment.Near
                };

                var recastSf = new StringFormat()
                {
                    Alignment = this.OverlapRecastTime ? StringAlignment.Near : StringAlignment.Far
                };

                var telopSf = new StringFormat()
                {
                    Alignment = StringAlignment.Center
                };

                var path = new GraphicsPath();

                if (this.BarEnabled)
                {
                    if (!this.HideSpellName)
                    {
                        path.AddString(
                            Translate.Get("SampleSpell"),
                            font.FontFamily,
                            (int)font.Style,
                            (float)font.ToFontSizeWPF(),
                            fontRect,
                            spellSf);
                    }

                    if (this.OverlapRecastTime)
                    {
                        fontRect.X = spellX;
                        fontRect.Width = this.SpellIconSize;
                        recastSf.Alignment = StringAlignment.Center;
                    }

                    path.AddString(
                        Settings.Default.EnabledSpellTimerNoDecimal ? "120" : "120.0",
                        font.FontFamily,
                        (int)font.Style,
                        (float)font.ToFontSizeWPF(),
                        fontRect,
                        recastSf);
                }
                else
                {
                    path.AddString(
                        Translate.Get("SampleTelop"),
                        font.FontFamily,
                        (int)font.Style,
                        (float)font.ToFontSizeWPF(),
                        fontRect,
                        telopSf);
                }

                g.FillPath(fontBrush, path);
                g.DrawPath(fontOutlinePen, path);

                // まとめて後片付け
                fontOutlinePen.Dispose();
                path.Dispose();
                spellSf.Dispose();
                recastSf.Dispose();
                telopSf.Dispose();
            }

            if (this.SamplePictureBox.Image != null)
            {
                this.SamplePictureBox.Image.Dispose();
                this.SamplePictureBox.Image = null;
            }

            this.SamplePictureBox.Image = bmp;
        }

        public void SetFontInfo(
            FontInfo fontInfo)
        {
            this.fontInfo = fontInfo;
        }

        private void VisualSettingControl_Load(object sender, EventArgs e)
        {
            var colorSetDirectory = this.GetColorSetDirectory;
            if (Directory.Exists(colorSetDirectory))
            {
                this.OpenFileDialog.InitialDirectory = colorSetDirectory;
                this.SaveFileDialog.InitialDirectory = colorSetDirectory;
            }

            this.WidthNumericUpDown.Value = this.BarSize.Width;
            this.HeightNumericUpDown.Value = this.BarSize.Height;

            this.RefreshSampleImage();

            this.ChangeFontItem.Click += (s1, e1) =>
            {
                var f = new FontDialogWindow();
                f.SetOwner(this.ParentForm);

                f.FontInfo = this.GetFontInfo();
                if (f.ShowDialog().Value)
                {
                    this.SetFontInfo(f.FontInfo);
                    this.RefreshSampleImage();
                }
            };

            this.ChangeFontColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.FontColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.FontColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeFontOutlineColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.FontOutlineColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.FontOutlineColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeWarningFontColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.WarningFontColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.WarningFontColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeWarningFontOutlineColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.WarningFontOutlineColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.WarningFontOutlineColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeBarColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.BarColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.BarColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeBarOutlineColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.BarOutlineColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.BarOutlineColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeBackgoundColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.backgroundColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.backgroundColor = Color.FromArgb(
                        this.alphaDialog.Alpha,
                        this.ColorDialog.Color);
                    this.RefreshSampleImage();
                }
            };

            this.ChangeBackgroundAlphaItem.Click += (s1, e1) =>
            {
                this.alphaDialog.Alpha = this.backgroundColor.A;
                if (this.alphaDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.backgroundColor = Color.FromArgb(
                        this.alphaDialog.Alpha,
                        this.backgroundColor);
                    this.RefreshSampleImage();
                }
            };

            this.LoadColorSetItem.Click += (s1, e1) =>
            {
                if (this.OpenFileDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    using (var sr = new StreamReader(this.OpenFileDialog.FileName, new UTF8Encoding(false)))
                    {
                        var xs = new XmlSerializer(typeof(ColorSet));
                        var colorSet = xs.Deserialize(sr) as ColorSet;
                        if (colorSet != null)
                        {
                            this.FontColor = colorSet.FontColor.FromHTML();
                            this.FontOutlineColor = colorSet.FontOutlineColor.FromHTML();
                            this.WarningFontColor = colorSet.WarningFontColor.FromHTML();
                            this.WarningFontOutlineColor = colorSet.WarningFontOutlineColor.FromHTML();
                            this.BarColor = colorSet.BarColor.FromHTML();
                            this.BarOutlineColor = colorSet.BarOutlineColor.FromHTML();
                            this.backgroundColor = string.IsNullOrWhiteSpace(colorSet.BackgroundColor) ?
                                Color.Transparent :
                                Color.FromArgb(colorSet.BackgroundAlpha, colorSet.BackgroundColor.FromHTML());

                            this.RefreshSampleImage();

                            // カラーパレットに登録する
                            this.ColorDialog.CustomColors = new int[]
                            {
                                ColorTranslator.ToOle(colorSet.FontColor.FromHTML()),
                                ColorTranslator.ToOle(colorSet.FontOutlineColor.FromHTML()),
                                ColorTranslator.ToOle(colorSet.BarColor.FromHTML()),
                                ColorTranslator.ToOle(colorSet.BarOutlineColor.FromHTML()),
                            };
                        }
                    }
                }
            };

            this.SaveColorSetItem.Click += (s1, e1) =>
            {
                if (string.IsNullOrWhiteSpace(this.SaveFileDialog.FileName))
                {
                    this.SaveFileDialog.FileName = "スペスペ配色セット.xml";
                }

                if (this.SaveFileDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    var colorSet = new ColorSet()
                    {
                        FontColor = this.FontColor.ToHTML(),
                        FontOutlineColor = this.FontOutlineColor.ToHTML(),
                        WarningFontColor = this.WarningFontColor.ToHTML(),
                        WarningFontOutlineColor = this.WarningFontOutlineColor.ToHTML(),
                        BarColor = this.BarColor.ToHTML(),
                        BarOutlineColor = this.BarOutlineColor.ToHTML(),
                        BackgroundColor = this.backgroundColor.ToHTML(),
                        BackgroundAlpha = this.backgroundColor.A,
                    };

                    using (var sw = new StreamWriter(this.SaveFileDialog.FileName, false, new UTF8Encoding(false)))
                    {
                        var xs = new XmlSerializer(typeof(ColorSet));
                        xs.Serialize(sw, colorSet);
                    }
                }
            };

            this.WidthNumericUpDown.ValueChanged += (s1, e1) =>
            {
                this.RefreshSampleImage();
            };

            this.HeightNumericUpDown.ValueChanged += (s1, e1) =>
            {
                this.RefreshSampleImage();
            };

            this.ResetSpellFontItem.Click += (s1, e1) =>
            {
                foreach (var s in SpellTimerTable.Instance.Table)
                {
                    s.Font = this.GetFontInfo();
                }

                SpellTimerCore.Default.ClosePanels();
                SpellTimerTable.Instance.Save();
            };

            this.ResetSpellBarSizeItem.Click += (s1, e1) =>
            {
                foreach (var s in SpellTimerTable.Instance.Table)
                {
                    s.BarWidth = this.BarSize.Width;
                    s.BarHeight = this.BarSize.Height;
                }

                SpellTimerCore.Default.ClosePanels();
                SpellTimerTable.Instance.Save();
            };

            this.ResetSpellColorItem.Click += (s1, e1) =>
            {
                foreach (var s in SpellTimerTable.Instance.Table)
                {
                    s.FontColor = this.FontColor.ToHTML();
                    s.FontOutlineColor = this.FontOutlineColor.ToHTML();
                    s.WarningFontColor = this.WarningFontColor.ToHTML();
                    s.WarningFontOutlineColor = this.WarningFontOutlineColor.ToHTML();
                    s.BarColor = this.BarColor.ToHTML();
                    s.BarOutlineColor = this.BarOutlineColor.ToHTML();
                    s.BackgroundColor = this.backgroundColor.ToHTML();
                    s.BackgroundAlpha = this.backgroundColor.A;
                }

                SpellTimerCore.Default.ClosePanels();
                SpellTimerTable.Instance.Save();
            };

            this.ResetTelopFontItem.Click += (s1, e1) =>
            {
                foreach (var s in OnePointTelopTable.Instance.Table)
                {
                    s.Font = this.GetFontInfo();
                }

                OnePointTelopController.CloseTelops();
                OnePointTelopTable.Instance.Save();
            };

            this.ResetTelopColorItem.Click += (s1, e1) =>
            {
                foreach (var s in OnePointTelopTable.Instance.Table)
                {
                    s.FontColor = this.FontColor.ToHTML();
                    s.FontOutlineColor = this.FontOutlineColor.ToHTML();
                    s.BackgroundColor = this.backgroundColor.ToHTML();
                    s.BackgroundAlpha = this.backgroundColor.A;
                }

                OnePointTelopController.CloseTelops();
                OnePointTelopTable.Instance.Save();
            };
        }
    }
}
