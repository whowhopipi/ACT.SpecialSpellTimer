using ACT.SpecialSpellTimer.Models;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public class TickerConfigViewModel :
        BindableBase
    {
        public TickerConfigViewModel() : this(new Ticker())
        {
        }

        public TickerConfigViewModel(
            Ticker model)
            => this.Model = model;

        private Ticker model;

        public Ticker Model
        {
            get => this.model;
            set => this.SetProperty(ref this.model, value);
        }
    }
}
