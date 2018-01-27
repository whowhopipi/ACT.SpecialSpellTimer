using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Config.Models;
using ACT.SpecialSpellTimer.Views;

namespace ACT.SpecialSpellTimer.Models
{
    public enum SpellOrders
    {
        None = 0,
        SortRecastTimeASC,
        SortRecastTimeDESC,
        SortPriority,
        SortMatchTime,
        Fixed
    }

    [Serializable]
    [XmlType(TypeName = "PanelSettings")]
    public class SpellPanel :
        TreeItemBase
    {
        #region プリセットパネル

        private static SpellPanel generalPanel = new SpellPanel()
        {
            PanelName = "+General",
            SortPriority = 100,
        };

        public static SpellPanel GeneralPanel => generalPanel;

        public static void SetGeneralPanel(
            SpellPanel panel)
        {
            panel.SortPriority = generalPanel.SortPriority;
            generalPanel = panel;
        }

        #endregion プリセットパネル

        private double left = 0;
        private double top = 0;
        private string panelName = string.Empty;

        public SpellPanel()
        {
            this.SetupChildrenSource();
        }

        [XmlIgnore]
        public override ItemTypes ItemType => ItemTypes.SpellPanel;

        public Guid ID { get; set; } = Guid.NewGuid();

        public double Left
        {
            get => this.left;
            set => this.SetProperty(ref this.left, Math.Round(value));
        }

        public double Top
        {
            get => this.top;
            set => this.SetProperty(ref this.top, Math.Round(value));
        }

        public string PanelName
        {
            get => this.panelName;
            set
            {
                if (this.SetProperty(ref this.panelName, value))
                {
                    this.RaisePropertyChanged(nameof(this.DisplayText));
                }
            }
        }

        private SpellOrders sortOrder = SpellOrders.None;

        public SpellOrders SortOrder
        {
            get => this.sortOrder;
            set => this.SetProperty(ref this.sortOrder, value);
        }

        [XmlIgnore]
        public bool IsDesignMode
        {
            get =>
                this.Spells.Count < 1 ?
                false :
                this.Spells.Count == this.Spells.Where(x => x.IsDesignMode).Count();
            set
            {
                foreach (var spell in this.Spells)
                {
                    spell.IsDesignMode = value;
                }

                this.RaisePropertyChanged();
            }
        }

        public bool FixedPositionSpell { get; set; } = false;

        public bool Horizontal { get; set; } = false;

        public double Margin { get; set; } = 0;

        [XmlIgnore]
        public bool ToClose { get; set; } = false;

        [XmlIgnore]
        public SpellTimerListWindow PanelWindow { get; set; } = null;

        [XmlIgnore]
        public IReadOnlyList<Spell> Spells
            => SpellTable.Instance.Table.Where(x => x.PanelID == this.ID).ToList();

        #region ITreeItem

        private bool isExpanded = false;

        [XmlIgnore]
        public override string DisplayText => this.PanelName;

        public override int SortPriority { get; set; }

        public override bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        [XmlIgnore]
        public override bool Enabled
        {
            get =>
                this.Spells.Count < 1 ?
                false :
                this.Spells.Count == this.Spells.Where(x => x.Enabled).Count();
            set
            {
                foreach (var spell in this.Spells)
                {
                    spell.Enabled = value;
                }

                this.RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public override ICollectionView Children => this.childrenSource?.View;

        private CollectionViewSource childrenSource;

        public void SetupChildrenSource()
        {
            this.childrenSource = new CollectionViewSource()
            {
                Source = SpellTable.Instance.Table,
                IsLiveFilteringRequested = true,
                IsLiveSortingRequested = true,
            };

            this.childrenSource.Filter += (x, y) =>
             {
                 var spell = y.Item as Spell;
                 y.Accepted = spell.PanelID == this.ID;
             };

            this.childrenSource.SortDescriptions.AddRange(new SortDescription[]
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
                new SortDescription()
                {
                    PropertyName = nameof(Spell.ID),
                    Direction = ListSortDirection.Ascending
                },
            });
        }

        #endregion ITreeItem
    }
}
