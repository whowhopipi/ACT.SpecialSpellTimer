using System;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Views;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Models
{
    [Serializable]
    [XmlType(TypeName = "PanelSettings")]
    public class SpellPanel :
        BindableBase
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
    }
}
