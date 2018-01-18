using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Views;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Models
{
    [Serializable]
    [XmlType(TypeName = "PanelSettings")]
    public class SpellPanel :
        BindableBase,
        ITrigger
    {
        private double left = 0;
        private double top = 0;

        public double Left
        {
            get => this.left;
            set => this.SetProperty(ref this.left, value);
        }

        public double Top
        {
            get => this.top;
            set => this.SetProperty(ref this.top, value);
        }

        public string PanelName { get; set; } = string.Empty;

        public bool FixedPositionSpell { get; set; } = false;

        public bool Horizontal { get; set; } = false;

        public double Margin { get; set; } = 0;

        [XmlIgnore]
        public bool ToClose { get; set; } = false;

        [XmlIgnore]
        public SpellTimerListWindow PanelWindow { get; set; } = null;

        [XmlIgnore]
        public IReadOnlyList<Spell> Spells
            => SpellTable.Instance.Table.Where(x => x.Panel == this.PanelName).ToList();

        #region ITrigger

        private ObservableCollection<Guid> tags = new ObservableCollection<Guid>();

        [XmlIgnore]
        public TriggerTypes TriggerType => TriggerTypes.SpellPanel;

        public ObservableCollection<Guid> Tags
        {
            get => this.tags;
            set => this.SetProperty(ref this.tags, value);
        }

        public void MatchTrigger(string logLine)
        {
        }

        #endregion ITrigger

        #region ITreeItem

        private bool isExpanded = false;

        [XmlIgnore]
        public string DisplayText => this.PanelName;

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        [XmlIgnore]
        public bool IsEnabled
        {
            get => this.Children.Count == this.Children.Where(x => x.IsEnabled).Count();
            set
            {
                foreach (var spell in this.Children)
                {
                    spell.IsEnabled = value;
                }

                this.RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public IReadOnlyList<ITreeItem> Children => this.Spells;

        #endregion ITreeItem
    }
}
