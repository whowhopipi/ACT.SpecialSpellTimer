namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Config;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// Configパネル オプション
    /// </summary>
    public partial class ConfigPanel
    {
        /// <summary>
        /// 設定を適用する
        /// </summary>
        private void ApplySettingsOption()
        {
            Settings.Default.Language = ((Utility.Language)this.LanguageComboBox.SelectedItem).Value;

            Settings.Default.OverlayForceVisible = this.OverlayForceVisibleCheckBox.Checked;
            Settings.Default.HideWhenNotActive = this.HideWhenNotActiceCheckBox.Checked;
            Settings.Default.UseOtherThanFFXIV = this.UseOtherThanFFXIVCheckbox.Checked;

            Settings.Default.ProgressBarSize = this.DefaultVisualSetting.BarSize;
            Settings.Default.ProgressBarColor = this.DefaultVisualSetting.BarColor;
            Settings.Default.ProgressBarOutlineColor = this.DefaultVisualSetting.BarOutlineColor;
            Settings.Default.Font = this.DefaultVisualSetting.GetFontInfo().ToFontForWindowsForm();
            Settings.Default.FontColor = this.DefaultVisualSetting.FontColor;
            Settings.Default.FontOutlineColor = this.DefaultVisualSetting.FontOutlineColor;
            Settings.Default.WarningFontColor = this.DefaultVisualSetting.WarningFontColor;
            Settings.Default.WarningFontOutlineColor = this.DefaultVisualSetting.WarningFontOutlineColor;
            Settings.Default.BackgroundColor = this.DefaultVisualSetting.BackgroundColor;

            Settings.Default.Opacity = (int)this.OpacityNumericUpDown.Value;
            Settings.Default.ReduceIconBrightness = (int)this.ReduceIconBrightnessNumericUpDown.Value;
            Settings.Default.ClickThroughEnabled = this.ClickThroughCheckBox.Checked;
            Settings.Default.AutoSortEnabled = this.AutoSortCheckBox.Checked;
            Settings.Default.AutoSortReverse = this.AutoSortReverseCheckBox.Checked;
            Settings.Default.TimeOfHideSpell = (double)this.TimeOfHideNumericUpDown.Value;
            Settings.Default.RefreshInterval = (long)this.RefreshIntervalNumericUpDown.Value;
            Settings.Default.LogPollSleepInterval = (long)this.LogPollSleepNumericUpDown.Value;
            Settings.Default.EnabledPartyMemberPlaceholder = this.EnabledPTPlaceholderCheckBox.Checked;
            Settings.Default.EnabledSpellTimerNoDecimal = this.EnabledSpellTimerNoDecimalCheckBox.Checked;

            Settings.Default.ReadyText = this.ReadyTextBox.Text;
            Settings.Default.OverText = this.OverTextBox.Text;

            Settings.Default.SaveLogEnabled = this.SaveLogCheckBox.Checked;
            Settings.Default.SaveLogDirectory = this.SaveLogTextBox.Text;

            Settings.Default.ResetOnWipeOut = this.ResetOnWipeOutCheckBox.Checked;
            Settings.Default.WipeoutNotifyToACT = this.NotifyToACTCheckBox.Checked;
            Settings.Default.SimpleRegex = this.SimpleRegexCheckBox.Checked;
            Settings.Default.RemoveTooltipSymbols = this.RemoveTooltipSymbolsCheckBox.Checked;
            Settings.Default.DetectPacketDump = this.DetectPacketDumpcheckBox.Checked;

            Settings.Default.TextOutlineThicknessRate = (double)this.TextOutlineThicknessRateNumericUpDown.Value;
            Settings.Default.TextBlurRate = (double)this.TextBlurRateNumericUpDown.Value;

            SpellTimerCore.Default.InvalidateSettings();

            // 有効状態から無効状態に変化する場合は、標準のスペルタイマーから設定を削除する
            if (Settings.Default.EnabledNotifyNormalSpellTimer &&
                !this.EnabledNotifyNormalSpellTimerCheckBox.Checked)
            {
                SpellTimerCore.Default.ClearNormalSpellTimer(true);
            }

            Settings.Default.EnabledNotifyNormalSpellTimer = this.EnabledNotifyNormalSpellTimerCheckBox.Checked;

            // 標準のスペルタイマーへ設定を反映する
            SpellTimerCore.Default.ApplyToNormalSpellTimer();

            // 設定を保存する
            Settings.Default.Save();
        }

        /// <summary>
        /// オプションのLoad
        /// </summary>
        private void LoadOption()
        {
            this.LoadSettingsOption();

            this.OverlayForceVisibleCheckBox.CheckedChanged += (s1, e1) =>
            {
                Settings.Default.OverlayForceVisible = this.OverlayForceVisibleCheckBox.Checked;
                Settings.Default.Save();
            };

            this.UseOtherThanFFXIVCheckbox.CheckedChanged += (s1, e1) =>
            {
                if (this.UseOtherThanFFXIVCheckbox.Checked)
                {
                    this.OverlayForceVisibleCheckBox.Checked = true;
                    this.EnabledPTPlaceholderCheckBox.Checked = false;
                    this.ResetOnWipeOutCheckBox.Checked = false;

                    this.OverlayForceVisibleCheckBox.Enabled = false;
                    this.EnabledPTPlaceholderCheckBox.Enabled = false;
                    this.ResetOnWipeOutCheckBox.Enabled = false;
                }
                else
                {
                    this.OverlayForceVisibleCheckBox.Enabled = true;
                    this.EnabledPTPlaceholderCheckBox.Enabled = true;
                    this.ResetOnWipeOutCheckBox.Enabled = true;
                }
            };

            this.SwitchOverlayButton.Click += (s1, e1) =>
            {
                Settings.Default.OverlayVisible = !Settings.Default.OverlayVisible;
                Settings.Default.Save();
                this.LoadSettingsOption();

                if (Settings.Default.OverlayVisible)
                {
                    SpellTimerCore.Default.ActivatePanels();
                    OnePointTelopController.ActivateTelops();
                }
            };

            this.SwitchTelopButton.Click += (s1, e1) =>
            {
                Settings.Default.TelopAlwaysVisible = !Settings.Default.TelopAlwaysVisible;
                Settings.Default.Save();
                this.LoadSettingsOption();
            };

            this.LanguageComboBox.SelectedValueChanged += (s1, e1) =>
            {
                Language language = (Language)this.LanguageComboBox.SelectedItem;
                this.LanguageRestartLabel.Text = Utility.Translate.GetTranslationsFor(language.Value).GetString("RequiresRestart");
                Settings.Default.Language = language.Value;
                Settings.Default.Save();
                this.LoadSettingsOption();
            };

            this.SaveLogCheckBox.CheckedChanged += (s1, e1) =>
            {
                this.SaveLogTextBox.Enabled = this.SaveLogCheckBox.Checked;
                this.SaveLogButton.Enabled = this.SaveLogCheckBox.Checked;
            };

            this.SaveLogButton.Click += (s1, e1) =>
            {
                if (!string.IsNullOrWhiteSpace(this.SaveLogTextBox.Text))
                {
                    this.FolderBrowserDialog.SelectedPath = this.SaveLogTextBox.Text;
                }

                if (this.FolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.SaveLogTextBox.Text = this.FolderBrowserDialog.SelectedPath;
                }
            };

            Action action = new Action(() =>
            {
                if (Settings.Default.OverlayVisible)
                {
                    this.SwitchOverlayButton.Text = Translate.Get("OverlayDisplaySwitchIsOn");
                }
                else
                {
                    this.SwitchOverlayButton.Text = Translate.Get("OverlayDisplaySwitchIsOff");
                }
            });

            this.OptionTabPage.MouseHover += (s1, e1) => action();
            this.SwitchOverlayButton.MouseHover += (s1, e1) => action();
        }

        /// <summary>
        /// オプション設定をロードする
        /// </summary>
        private void LoadSettingsOption()
        {
            foreach (Language lang in this.LanguageComboBox.Items)
            {
                if (lang.Value == Settings.Default.Language)
                    this.LanguageComboBox.SelectedItem = lang;
            }

            this.OverlayForceVisibleCheckBox.Checked = Settings.Default.OverlayForceVisible;
            this.HideWhenNotActiceCheckBox.Checked = Settings.Default.HideWhenNotActive;
            this.UseOtherThanFFXIVCheckbox.Checked = Settings.Default.UseOtherThanFFXIV;

            if (Settings.Default.OverlayVisible)
            {
                this.SwitchOverlayButton.Text = Translate.Get("OverlayDisplaySwitchIsOn");
            }
            else
            {
                this.SwitchOverlayButton.Text = Translate.Get("OverlayDisplaySwitchIsOff");
            }

            if (Settings.Default.TelopAlwaysVisible)
            {
                this.SwitchTelopButton.Text = Translate.Get("TelopDisplaySwitchIsOn");
            }
            else
            {
                this.SwitchTelopButton.Text = Translate.Get("TelopDisplaySwitchIsOff");
            }

            this.DefaultVisualSetting.BarSize = Settings.Default.ProgressBarSize;
            this.DefaultVisualSetting.BarColor = Settings.Default.ProgressBarColor;
            this.DefaultVisualSetting.BarOutlineColor = Settings.Default.ProgressBarOutlineColor;
            this.DefaultVisualSetting.SetFontInfo(Settings.Default.Font.ToFontInfo());
            this.DefaultVisualSetting.FontColor = Settings.Default.FontColor;
            this.DefaultVisualSetting.FontOutlineColor = Settings.Default.FontOutlineColor;
            this.DefaultVisualSetting.WarningFontColor = Settings.Default.WarningFontColor;
            this.DefaultVisualSetting.WarningFontOutlineColor = Settings.Default.WarningFontOutlineColor;
            this.DefaultVisualSetting.BackgroundColor = Settings.Default.BackgroundColor;
            this.DefaultVisualSetting.RefreshSampleImage();

            this.OpacityNumericUpDown.Value = Settings.Default.Opacity;
            this.ReduceIconBrightnessNumericUpDown.Value = Settings.Default.ReduceIconBrightness;
            this.ClickThroughCheckBox.Checked = Settings.Default.ClickThroughEnabled;
            this.AutoSortCheckBox.Checked = Settings.Default.AutoSortEnabled;
            this.AutoSortReverseCheckBox.Checked = Settings.Default.AutoSortReverse;
            this.TimeOfHideNumericUpDown.Value = (decimal)Settings.Default.TimeOfHideSpell;
            this.RefreshIntervalNumericUpDown.Value = Settings.Default.RefreshInterval;
            this.LogPollSleepNumericUpDown.Value = Settings.Default.LogPollSleepInterval;
            this.EnabledPTPlaceholderCheckBox.Checked = Settings.Default.EnabledPartyMemberPlaceholder;
            this.EnabledSpellTimerNoDecimalCheckBox.Checked = Settings.Default.EnabledSpellTimerNoDecimal;
            this.EnabledNotifyNormalSpellTimerCheckBox.Checked = Settings.Default.EnabledNotifyNormalSpellTimer;

            this.ReadyTextBox.Text = Settings.Default.ReadyText;
            this.OverTextBox.Text = Settings.Default.OverText;

            this.SaveLogCheckBox.Checked = Settings.Default.SaveLogEnabled;
            this.SaveLogTextBox.Text = Settings.Default.SaveLogDirectory;

            this.ResetOnWipeOutCheckBox.Checked = Settings.Default.ResetOnWipeOut;
            this.NotifyToACTCheckBox.Checked = Settings.Default.WipeoutNotifyToACT;
            this.SimpleRegexCheckBox.Checked = Settings.Default.SimpleRegex;
            this.RemoveTooltipSymbolsCheckBox.Checked = Settings.Default.RemoveTooltipSymbols;
            this.DetectPacketDumpcheckBox.Checked = Settings.Default.DetectPacketDump;

            this.TextOutlineThicknessRateNumericUpDown.Value = (decimal)Settings.Default.TextOutlineThicknessRate;
            this.TextBlurRateNumericUpDown.Value = (decimal)Settings.Default.TextBlurRate;

            var sw1 = this.SaveLogCheckBox.Checked;
            this.SaveLogTextBox.Enabled = sw1;
            this.SaveLogButton.Enabled = sw1;

            var sw2 = !this.UseOtherThanFFXIVCheckbox.Checked;
            this.OverlayForceVisibleCheckBox.Enabled = sw2;
            this.EnabledPTPlaceholderCheckBox.Enabled = sw2;
            this.ResetOnWipeOutCheckBox.Enabled = sw2;

            // 標準のスペルタイマーへ設定を反映する
            SpellTimerCore.Default.ApplyToNormalSpellTimer();
        }
    }
}