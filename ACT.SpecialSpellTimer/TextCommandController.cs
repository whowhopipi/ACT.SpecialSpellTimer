namespace ACT.SpecialSpellTimer
{
    using System.Collections.Generic;
    using System.Media;
    using System.Text.RegularExpressions;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// テキストコマンド Controller
    /// </summary>
    public static class TextCommandController
    {
        /// <summary>
        /// コマンド解析用の正規表現
        /// </summary>
        private static Regex regexCommand = new Regex(
            @".*/spespe (?<command>refresh|changeenabled|analyze|set|clear|on|off) ?(?<target>spells|telops|me|pt|pet|on|off|placeholder|$) ?(?<windowname>"".*""|all)? ?(?<value>.*)",
            RegexOptions.Compiled |
            RegexOptions.IgnoreCase);

        /// <summary>
        /// Commandとマッチングする
        /// </summary>
        /// <param name="logLines">
        /// ログ行</param>
        public static void MatchCommand(
            IReadOnlyList<string> logLines)
        {
            var commandDone = false;
            foreach (var log in logLines)
            {
                // 正規表現の前にキーワードがなければ抜けてしまう
                if (!log.ToLower().Contains("/spespe"))
                {
                    continue;
                }

                var match = regexCommand.Match(log);
                if (!match.Success)
                {
                    continue;
                }

                var command = match.Groups["command"].ToString().ToLower();
                var target = match.Groups["target"].ToString().ToLower();
                var windowname = match.Groups["windowname"].ToString().Replace(@"""", string.Empty);
                var valueAsText = match.Groups["value"].ToString();
                var value = false;
                if (!bool.TryParse(valueAsText, out value))
                {
                    value = false;
                }

                switch (command)
                {
                    case "analyze":
                        switch (target)
                        {
                            case "on":
                                SpecialSpellTimerPlugin.ConfigPanel.CombatAnalyzerEnabled = true;
                                commandDone = true;
                                break;

                            case "off":
                                SpecialSpellTimerPlugin.ConfigPanel.CombatAnalyzerEnabled = false;
                                commandDone = true;
                                break;
                        }

                        break;

                    case "refresh":
                        switch (target)
                        {
                            case "spells":
                                SpellTimerCore.Default.ClosePanels();
                                commandDone = true;
                                break;

                            case "telops":
                                OnePointTelopController.CloseTelops();
                                commandDone = true;
                                break;

                            case "pt":
                                LogBuffer.RefreshPartyList();
                                commandDone = true;
                                break;

                            case "pet":
                                LogBuffer.RefreshPetID();
                                commandDone = true;
                                break;
                        }

                        break;

                    case "changeenabled":
                        var changed = false;
                        switch (target)
                        {
                            case "spells":
                                foreach (var spell in SpellTimerTable.Table)
                                {
                                    if (spell.Panel.Trim().ToLower() == windowname.Trim().ToLower() ||
                                        spell.SpellTitle.Trim().ToLower() == windowname.Trim().ToLower() ||
                                        windowname.Trim().ToLower() == "all")
                                    {
                                        changed = true;
                                        spell.Enabled = value;
                                    }
                                }

                                if (changed)
                                {
                                    ActInvoker.Invoke(() =>
                                    {
                                        SpecialSpellTimerPlugin.ConfigPanel.LoadSpellTimerTable();
                                    });

                                    commandDone = true;
                                }

                                break;

                            case "telops":
                                foreach (var telop in OnePointTelopTable.Default.Table)
                                {
                                    if (telop.Title.Trim().ToLower() == windowname.Trim().ToLower() ||
                                        windowname.Trim().ToLower() == "all")
                                    {
                                        changed = true;
                                        telop.Enabled = value;
                                    }
                                }

                                if (changed)
                                {
                                    ActInvoker.Invoke(() =>
                                    {
                                        SpecialSpellTimerPlugin.ConfigPanel.LoadTelopTable();
                                    });

                                    commandDone = true;
                                }

                                break;
                        }

                        break;

                    case "set":
                        switch (target)
                        {
                            case "placeholder":
                                if (windowname.Trim().ToLower() != "all" &&
                                    windowname.Trim() != string.Empty &&
                                    valueAsText.Trim() != string.Empty)
                                {
                                    LogBuffer.SetCustomPlaceholder(windowname.Trim(), valueAsText.Trim());

                                    commandDone = true;
                                }

                                break;
                        }

                        break;

                    case "clear":
                        switch (target)
                        {
                            case "placeholder":
                                if (windowname.Trim().ToLower() == "all")
                                {
                                    LogBuffer.ClearCustomPlaceholderAll();

                                    commandDone = true;
                                }
                                else if (windowname.Trim() != string.Empty)
                                {
                                    LogBuffer.ClearCustomPlaceholder(windowname.Trim());

                                    commandDone = true;
                                }

                                break;
                        }

                        break;

                    case "on":
                        SpecialSpellTimerPlugin.ChangeSwitchVisibleButton(true);
                        commandDone = true;
                        break;

                    case "off":
                        SpecialSpellTimerPlugin.ChangeSwitchVisibleButton(false);
                        commandDone = true;
                        break;
                }
            }   // loop end logLines

            // コマンドを実行したらシステム音を鳴らす
            if (commandDone)
            {
                SystemSounds.Asterisk.Play();
            }
        }   // method end
    }
}