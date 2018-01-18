using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
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

        public SpellPanel()
        {
            this.SetupChildrenSource();
        }

        public Guid ID { get; set; } = Guid.NewGuid();

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

        [XmlIgnore]
        public TriggerTypes TriggerType => TriggerTypes.SpellPanel;

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
            get => this.Spells.Count == this.Spells.Where(x => x.IsEnabled).Count();
            set
            {
                foreach (var spell in this.Spells)
                {
                    spell.IsEnabled = value;
                }

                this.RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public CollectionViewSource Children
        {
            get;
            private set;
        } = new CollectionViewSource()
        {
            Source = SpellTable.Instance.Table
        };

        private void SetupChildrenSource()
        {
            this.Children.Filter += (x, y) =>
            {
                var spell = y.Item as Spell;
                y.Accepted = spell.Panel == this.PanelName;
            };

            this.Children.SortDescriptions.AddRange(new SortDescription[]
            {
                new SortDescription()
                {
                    PropertyName = nameof(Spell.DisplayNo),
                    Direction = ListSortDirection.Ascending
                },
                new SortDescription()
                {
                    PropertyName = nameof(Spell.SpellTitle),
                    Direction = ListSortDirection.Ascending
                },
            });
        }

        #endregion ITreeItem
    }
}
