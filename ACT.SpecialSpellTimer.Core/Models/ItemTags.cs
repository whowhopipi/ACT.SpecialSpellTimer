using System;

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
    }
}
