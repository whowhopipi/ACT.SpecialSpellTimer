using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;

using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// テキストコマンド Controller
    /// </summary>
    public static class TextCommandController
    {
        /// <summary>
        /// コマンド解析用の正規表現
        /// </summary>
        private readonly static Regex regexCommand = new Regex(
            @".*/spespe (?<command>refresh|changeenabled|analyze|set|clear|on|off) ?(?<target>spells|telops|me|pt|pet|on|off|placeholder|$) ?(?<windowname>"".*""|all)? ?(?<value>.*)",
            RegexOptions.Compiled |
            RegexOptions.IgnoreCase);

        /// <summary>
        /// ログ1行とマッチングする
        /// </summary>
        /// <param name="logLine">ログ行</param>
        /// <returns>
        /// コマンドを実行したか？</returns>
        public static bool MatchCommandCore(
            string logLine)
        {
            var r = false;

            // 正規表現の前にキーワードがなければ抜けてしまう
            if (!logLine.ToLower().Contains("/spespe"))
            {
                return r;
            }

            var match = regexCommand.Match(logLine);
            if (!match.Success)
            {
                return r;
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
                            PluginCore.Instance.ConfigPanel.CombatAnalyzerEnabled = true;
                            r = true;
                            break;

                        case "off":
                            PluginCore.Instance.ConfigPanel.CombatAnalyzerEnabled = false;
                            r = true;
                            break;
                    }

                    break;

                case "refresh":
                    switch (target)
                    {
                        case "spells":
                            SpellsController.Instance.ClosePanels();
                            r = true;
                            break;

                        case "telops":
                            TickersController.Instance.CloseTelops();
                            r = true;
                            break;

                        case "pt":
                            TableCompiler.Instance.RefreshPlayerPlacceholder();
                            TableCompiler.Instance.RefreshPartyPlaceholders();
                            TableCompiler.Instance.RecompileSpells();
                            TableCompiler.Instance.RecompileTickers();
                            r = true;
                            break;

                        case "pet":
                            TableCompiler.Instance.RefreshPetPlaceholder();
                            TableCompiler.Instance.RecompileSpells();
                            TableCompiler.Instance.RecompileTickers();
                            r = true;
                            break;
                    }

                    break;

                case "changeenabled":
                    var changed = false;
                    switch (target)
                    {
                        case "spells":
                            foreach (var spell in SpellTable.Instance.Table)
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
                                    PluginCore.Instance.ConfigPanel.LoadSpellTimerTable();
                                });

                                r = true;
                            }

                            break;

                        case "telops":
                            foreach (var telop in TickerTable.Instance.Table)
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
                                    PluginCore.Instance.ConfigPanel.LoadTelopTable();
                                });

                                r = true;
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
                                TableCompiler.Instance.SetCustomPlaceholder(windowname.Trim(), valueAsText.Trim());

                                r = true;
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
                                TableCompiler.Instance.ClearCustomPlaceholderAll();

                                r = true;
                            }
                            else if (windowname.Trim() != string.Empty)
                            {
                                TableCompiler.Instance.ClearCustomPlaceholder(windowname.Trim());

                                r = true;
                            }

                            break;
                    }

                    break;

                case "on":
                    PluginCore.Instance.ChangeSwitchVisibleButton(true);
                    r = true;
                    break;

                case "off":
                    PluginCore.Instance.ChangeSwitchVisibleButton(false);
                    r = true;
                    break;
            }

            return r;
        }

        /// <summary>
        /// Commandとマッチングする
        /// </summary>
        /// <param name="logLines">
        /// ログ行</param>
        public static void MatchCommand(
            IReadOnlyList<string> logLines)
        {
            var commandDone = false;
            logLines.AsParallel().ForAll(log =>
            {
                commandDone |= MatchCommandCore(log);
            });

            // コマンドを実行したらシステム音を鳴らす
            if (commandDone)
            {
                SystemSounds.Asterisk.Play();
            }
        }
    }
}
