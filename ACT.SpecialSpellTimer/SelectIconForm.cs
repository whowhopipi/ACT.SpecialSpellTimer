using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACT.SpecialSpellTimer.Image;

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

        public static Task<IconDialogResult> ShowDialogAsync(
            string iconRelativePath,
            IWin32Window owner = null)
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
                    instance.Font = (owner as Form)?.Font;
                }

                instance.SelectedIconRelativePath = iconRelativePath;

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
                    var text = dir.Key.Split('\\').LastOrDefault();
                    var node = new TreeNode()
                    {
                        Text = text,
                        Tag = dir.ToList(),
                    };

                    if (selectedNode == null)
                    {
                        if (dir.AsParallel().Any(x =>
                            x.RelativePath == this.SelectedIconRelativePath))
                        {
                            selectedNode = node;
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
                tree.Focus();
                Application.DoEvents();
            }
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

                    ctrl.Button.Click += (s, e) =>
                    {
                        this.SelectIcon(ctrl);
                    };

                    panel.Controls.Add(ctrl);

                    if (icon.RelativePath ==
                        this.SelectedIconRelativePath)
                    {
                        selectedCtrl = ctrl;
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
