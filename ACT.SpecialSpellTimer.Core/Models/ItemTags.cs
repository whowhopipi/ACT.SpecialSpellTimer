using System;
using System.Windows.Data;
using System.Linq;
using System.Xml.Serialization;

namespace ACT.SpecialSpellTimer.Models
{
    [Serializable]
    public class ItemTags
    {
        public ItemTags()
        {
        }

        public ItemTags(
            Guid itemID,
            Guid tagID)
        {
            this.ItemID = itemID;
            this.TagID = tagID;
        }

        public Guid ItemID { get; set; }
        public Guid TagID { get; set; }

        private ITreeItem item;
        private Tag tag;

        [XmlIgnore]
        public ItemTypes ItemType
        {
            get
            {
                switch (this.Item)
                {
                    case Tag tag:
                        return ItemTypes.Tag;
                    case SpellPanel p:
                        return ItemTypes.SpellPanel;
                    case Spell s:
                        return ItemTypes.Spell;
                    case Ticker t:
                        return ItemTypes.Ticker;
                    default:
                        return ItemTypes.Root;
                }
            }
        }

        [XmlIgnore]
        public ITreeItem Item
        {
            get
            {
                if (this.item != null)
                {
                    return this.item;
                }

                this.item = SpellPanelTable.Instance.Table.FirstOrDefault(x => x.ID == this.ItemID);
                if (this.item != null)
                {
                    return this.item;
                }

                this.item = SpellTable.Instance.Table.FirstOrDefault(x => x.Guid == this.ItemID);
                if (this.item != null)
                {
                    return this.item;
                }

                this.item = TickerTable.Instance.Table.FirstOrDefault(x => x.Guid == this.ItemID);
                if (this.item != null)
                {
                    return this.item;
                }

                this.item = TagTable.Instance.Tags.FirstOrDefault(x => x.ID == this.ItemID);

                return this.item;
            }
        }

        [XmlIgnore]
        public Tag Tag
        {
            get
            {
                if (this.tag != null)
                {
                    return this.tag;
                }

                this.tag = TagTable.Instance.Tags.FirstOrDefault(x => x.ID == this.TagID);
                return this.tag;
            }
        }
    }
}
