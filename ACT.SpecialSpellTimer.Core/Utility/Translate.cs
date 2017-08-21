using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.resources.strings;

namespace ACT.SpecialSpellTimer.Utility
{
    internal class Translate
    {
        private static ResourceManager Language;

        public static String Get(String name)
        {
            InitializeIfNeeded();

            if (name == "")
            {
                return name;
            }

            if (name.StartsWith("__"))
            {
                return name.Substring(2);
            }

            double a;
            if (double.TryParse(name, out a))
            {
                return name;
            }

            var s = string.Empty;

            try
            {
                s = Language.GetString(name);
            }
            catch (Exception)
            {
                s = "N/A: " + name;
            }

            if (string.IsNullOrEmpty(s))
            {
                s = "N/A: " + name;
            }

            return s.Replace("\\n", Environment.NewLine);
        }

        public static ResourceManager GetTranslationsFor(string name)
        {
            switch (name)
            {
                case "EN":
                    return Strings_EN.ResourceManager;

                case "JP":
                    return Strings_JP.ResourceManager;

                case "KR":
                    return Strings_KR.ResourceManager;
            }

            Logger.Write("Unknown language: " + Settings.Default.Language + " -> " + name);

            return Strings_JP.ResourceManager;
        }

        public static void TranslateControls(Control control)
        {
            var setterList = new List<Action>();

            try
            {
                setterList.Add(() => control.Text = Get(control.Text));

                foreach (Control c in control.Controls.AsParallel())
                {
                    TranslateControls(c);
                }

                // Controls may have a context menu, these are not controls but they do have Text.
                if (control.ContextMenuStrip != null)
                {
                    foreach (ToolStripItem c in control.ContextMenuStrip.Items.AsParallel())
                    {
                        setterList.Add(() => c.Text = Get(c.Text));
                    }
                }

                switch (control)
                {
                    case ListView listView:
                        foreach (ColumnHeader c in listView.Columns.AsParallel())
                        {
                            setterList.Add(() => c.Text = Get(c.Text));
                        }
                        break;

                    case ComboBox combo:
                        for (int i = 0; i < combo.Items.Count; ++i)
                        {
                            if (combo.Items[i] is string)
                            {
                                setterList.Add(() => combo.Items[i] = Get((String)combo.Items[i]));
                            }
                        }
                        break;

                    case MenuStrip menu:
                        foreach (ToolStripItem c in menu.Items.AsParallel())
                        {
                            setterList.Add(() => c.Text = Get(c.Text));
                        }
                        break;

                    case ContextMenuStrip contextMenu:
                        foreach (ToolStripItem c in contextMenu.Items.AsParallel())
                        {
                            setterList.Add(() => c.Text = Get(c.Text));
                        }
                        break;
                }

                foreach (var action in setterList.AsParallel())
                {
                    if (control.InvokeRequired)
                    {
                        control.Invoke((MethodInvoker)delegate { action(); });
                    }
                    else
                    {
                        action();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        private static void InitializeIfNeeded()
        {
            if (Language != null)
            {
                return;
            }

            Language = GetTranslationsFor(Settings.Default.Language);
        }
    }
}