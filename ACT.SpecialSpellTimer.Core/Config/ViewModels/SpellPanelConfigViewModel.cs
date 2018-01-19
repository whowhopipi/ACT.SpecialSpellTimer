using ACT.SpecialSpellTimer.Models;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public class SpellPanelConfigViewModel :
        BindableBase
    {
        public SpellPanelConfigViewModel() : this(new SpellPanel())
        {
        }

        public SpellPanelConfigViewModel(
            SpellPanel model)
            => this.Model = model;

        public SpellPanel Model { get; private set; }
    }
}
