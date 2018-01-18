using System.Windows.Data;
using ACT.SpecialSpellTimer.Models;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.Models
{
    public class TriggersTreeRoot :
        BindableBase,
        ITreeItem
    {
        private bool isExpanded = false;
        private string displayText = string.Empty;
        private CollectionViewSource children;

        public string DisplayText
        {
            get => this.displayText;
            set => this.SetProperty(ref this.displayText, value);
        }

        public CollectionViewSource Children
        {
            get => this.children;
            set => this.SetProperty(ref this.children, value);
        }

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        public bool IsEnabled
        {
            get => false;
            set { }
        }
    }
}
