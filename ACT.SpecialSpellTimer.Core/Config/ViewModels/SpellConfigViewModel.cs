using ACT.SpecialSpellTimer.Models;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public class SpellConfigViewModel :
        BindableBase
    {
        public SpellConfigViewModel() : this(new Spell())
        {
        }

        public SpellConfigViewModel(
            Spell model)
            => this.Model = model;

        public Spell Model { get; set; }
    }
}
