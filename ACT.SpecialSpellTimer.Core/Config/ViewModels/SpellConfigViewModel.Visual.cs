using System.Windows.Input;
using ACT.SpecialSpellTimer.Forms;
using Advanced_Combat_Tracker;
using Prism.Commands;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public partial class SpellConfigViewModel
    {
        private ICommand selectIconCommand;

        public ICommand SelectIconCommand =>
            this.selectIconCommand ?? (this.selectIconCommand = new DelegateCommand(() =>
            {
                var result = SelectIconForm.ShowDialog(
                    this.Model?.SpellIcon,
                    ActGlobals.oFormActMain,
                    this.Model);

                if (result.Result)
                {
                    this.Model.SpellIcon = result.Icon;
                }
            }));
    }
}
