using System;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Views;

namespace ACT.SpecialSpellTimer.Models
{
    [Serializable]
    [XmlType(TypeName = "PanelSettings")]
    public class SpellPanel
    {
        public bool FixedPositionSpell { get; set; } = false;
        public bool Horizontal { get; set; } = false;
        public double Left { get; set; } = 0;
        public double Margin { get; set; } = 0;
        public string PanelName { get; set; } = string.Empty;
        public double Top { get; set; } = 0;

        [XmlIgnore]
        public bool ToClose { get; set; } = false;

        [XmlIgnore]
        public SpellTimerListWindow PanelWindow { get; set; } = null;
    }
}
