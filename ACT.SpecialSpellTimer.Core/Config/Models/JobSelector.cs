using System;
using FFXIV.Framework.FFXIVHelper;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.Models
{
    public class JobSelector :
        BindableBase
    {
        private bool isSelected;
        private Action selectedChangedDelegate;

        public JobSelector(
            Job job,
            bool isSelected = false,
            Action selectedChangedDelegate = null)
        {
            this.Job = job;
            this.isSelected = isSelected;
            this.selectedChangedDelegate = selectedChangedDelegate;
        }

        public Job Job { get; set; }

        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                if (this.SetProperty(ref this.isSelected, value))
                {
                    this.selectedChangedDelegate?.Invoke();
                }
            }
        }

        public string Text => Job.GetName(Settings.Default.UILocale);

        public override string ToString() => this.Text;
    }
}
