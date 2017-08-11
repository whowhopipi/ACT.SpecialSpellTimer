using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACT.SpecialSpellTimer.Image;
using ACT.SpecialSpellTimer.Models;

namespace ACT.SpecialSpellTimer
{
    public partial class SelectIconForm : Form
    {
        #region Singleton

        private static SelectIconForm instance = new SelectIconForm();

        public static SelectIconForm Instance => instance;

        #endregion Singleton

        public SelectIconForm()
        {
            this.InitializeComponent();

            this.Load += this.SelectIconForm_Load;
            this.FolderTreeView.AfterSelect += this.FolderTreeView_AfterSelect;
            this.ClearButton.Click += (s, e) =>
            {
                this.SelectedIconRelativePath = string.Empty;
                this.DialogResult = DialogResult.OK;
            };
        }

        private void FolderTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.ShowIcons(e.Node);
        }

        public string SelectedIconRelativePath { get; set; }
        public SpellTimer ParrentSpell { get; set; }

        public static Task<IconDialogResult> ShowDialogAsync(
            string iconRelativePath,
            IWin32Window owner = null,
            SpellTimer spell = null)
        {
            return Task.Run(() =>
            {
                var result = new IconDialogResult()
                {
                    Result = false,
                    Icon = iconRelativePath,
                };

                if (owner != null)
                {
                    var font = (owner as Control)?.Font;
                    instance.Font = font;
                    instance.FolderTreeView.Font = font;
                    instance.FolderTreeView.ItemHeight = (int)(font.Size * 2);
                    instance.ClearButton.Font = font;
                    instance.CloseButton.Font = font;
                    instance.IconsFlowLayoutPanel.Font = font;
                }

                instance.SelectedIconRelativePath = iconRelativePath;
                instance.ParrentSpell = spell;

                if (instance.ShowDialog(owner) == 
                    DialogResult.OK)
                {
                    result.Result = true;
                    result.Icon = instance.SelectedIconRelativePath;
                }

                return result;
            });
        }

        private void SelectIconForm_Load(object sender, EventArgs e)
        {
            var iconsByDirectory =
                from x in IconController.Instance.EnumlateIcon()
                where
                !string.IsNullOrEmpty(x.Directory)
                group x by
                x.Directory;

            var tree = this.FolderTreeView;
            var selectedNode = default(TreeNode);

            try
            {
                tree.SuspendLayout();
                tree.Nodes.Clear();

                foreach (var dir in iconsByDirectory)
                {
                    var text = string.Empty;
                    text = dir.Key.Split('\\').LastOrDefault();

                    var node = new TreeNode()
                    {
                        Text = text,
                        Tag = dir.ToList(),
                    };

                    if (selectedNode == null)
                    {
                        if (string.IsNullOrEmpty(
                            this.SelectedIconRelativePath))
                        {
                            if (this.ParrentSpell != null)
                            {
                                var q =
                                    from x in dir
                                    where
                                    !string.IsNullOrEmpty(x.SkillName) &&
                                    (
                                        this.ParrentSpell.Keyword.Contains(x.SkillName) ||
                                        this.ParrentSpell.SpellTitle.Contains(x.SkillName)
                                    )
                                    select
                                    x;

                                if (q.Any())
                                {
                                    selectedNode = node;
                                }
                            }
                        }
                        else
                        {
                            if (dir.AsParallel().Any(x =>
                                x.RelativePath.ToLower() == 
                                this.SelectedIconRelativePath.ToLower()))
                            {
                                selectedNode = node;
                            }
                        }
                    }

                    tree.Nodes.Add(node);
                }
            }
            finally
            {
                tree.ResumeLayout();
            }

            if (selectedNode != null)
            {
                tree.SelectedNode = selectedNode;
            }
            else
            {
                if (tree.Nodes.Count > 0)
                {
                    tree.SelectedNode = tree.Nodes[0];
                }
            }

            Application.DoEvents();
            tree.Focus();
        }

        private void ShowIcons(
            TreeNode node)
        {
            var iconList = node.Tag as List<IconController.IconFile>;

            var panel = this.IconsFlowLayoutPanel;
            var selectedCtrl = default(SelectIconUserControl);

            try
            {
                panel.SuspendLayout();
                panel.Controls.Clear();

                foreach (var icon in iconList)
                {
                    var ctrl = new SelectIconUserControl();

                    ctrl.Button.BackgroundImageLayout = ImageLayout.Zoom;
                    ctrl.Button.BackgroundImage = System.Drawing.Image.FromFile(
                        icon.FullPath);

                    ctrl.Label.Text = icon.Name;
                    ctrl.Icon = icon;

                    ctrl.Button.Font = panel.Font;
                    ctrl.Label.Font = panel.Font;

                    ctrl.Button.Click += (s, e) =>
                    {
                        this.SelectIcon(ctrl);
                    };

                    panel.Controls.Add(ctrl);

                    if (selectedCtrl == null)
                    {
                        if (string.IsNullOrEmpty(
                            this.SelectedIconRelativePath))
                        {
                            if (this.ParrentSpell != null)
                            {
                                if (!string.IsNullOrEmpty(icon.SkillName))
                                {
                                    if (this.ParrentSpell.Keyword.Contains(icon.SkillName) ||
                                        this.ParrentSpell.SpellTitle.Contains(icon.SkillName))
                                    {
                                        selectedCtrl = ctrl;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (icon.RelativePath.ToLower() ==
                                this.SelectedIconRelativePath.ToLower())
                            {
                                selectedCtrl = ctrl;
                            }
                        }
                    }
                }
            }
            finally
            {
                panel.ResumeLayout();
            }

            selectedCtrl?.Focus();
            Application.DoEvents();
        }

        private void SelectIcon(
            SelectIconUserControl selectedControl)
        {
            this.SelectedIconRelativePath = selectedControl?.Icon.RelativePath;
            this.DialogResult = DialogResult.OK;
        }

        #region Sub classes

        public class IconDialogResult
        {
            public bool Result { get; set; }

            public string Icon { get; set; }
        }

        #endregion
    }
}
