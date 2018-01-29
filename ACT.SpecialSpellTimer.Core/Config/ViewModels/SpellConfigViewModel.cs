using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ACT.SpecialSpellTimer.Config.Models;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using FFXIV.Framework.FFXIVHelper;
using Prism.Commands;
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
        {
            this.Model = model;
            this.SetJobSeelectors();
            this.SetZoneSeelectors();
        }

        public Spell Model { get; private set; }

        private bool isInitialize = false;

        #region Job filter

        public bool IsJobFiltered => !string.IsNullOrEmpty(this.Model?.JobFilter);

        public List<JobSelector> JobSelectors { get; private set; } = new List<JobSelector>();

        private void SetJobSeelectors()
        {
            var jobFilter = this.Model.JobFilter.Split(',');
            foreach (var job in Jobs.List.OrderBy(x => x.Role))
            {
                if (job.ID == JobIDs.Unknown ||
                    job.ID == JobIDs.ADV)
                {
                    continue;
                }

                var selector = new JobSelector(
                    job,
                    jobFilter.Contains(job.ID.ToString()),
                    () => this.JobFilterChanged());

                this.JobSelectors.Add(selector);
            }
        }

        private void JobFilterChanged()
        {
            if (!this.isInitialize)
            {
                this.Model.JobFilter = string.Join(",",
                    this.JobSelectors
                        .Where(x => x.IsSelected)
                        .Select(x => x.Job.ID.ToString())
                        .ToArray());

                this.RaisePropertyChanged(nameof(this.IsJobFiltered));
                Task.Run(() => TableCompiler.Instance.CompileSpells());
            }
        }

        private ICommand clearJobFilterCommand;

        public ICommand ClearJobFilterCommand =>
            this.clearJobFilterCommand ?? (this.clearJobFilterCommand = new DelegateCommand(() =>
            {
                try
                {
                    this.isInitialize = true;
                    foreach (var selector in this.JobSelectors)
                    {
                        selector.IsSelected = false;
                    }

                    this.Model.JobFilter = string.Empty;
                    this.RaisePropertyChanged(nameof(this.IsJobFiltered));
                    Task.Run(() => TableCompiler.Instance.CompileSpells());
                }
                finally
                {
                    this.isInitialize = false;
                }
            }));

        #endregion Job filter

        #region Zone filter

        public bool IsZoneFiltered => !string.IsNullOrEmpty(this.Model?.ZoneFilter);

        public List<ZoneSelector> ZoneSelectors { get; private set; } = new List<ZoneSelector>();

        private void SetZoneSeelectors()
        {
            var zoneFilter = this.Model.ZoneFilter.Split(',');
            foreach (var zone in
                from x in FFXIVPlugin.Instance.ZoneList
                orderby
                x.IsAddedByUser ? 0 : 1,
                x.Rank,
                x.ID descending
                select
                x)
            {
                var selector = new ZoneSelector(
                    zone.ID.ToString(),
                    zone.Name,
                    zoneFilter.Contains(zone.ID.ToString()),
                    () => this.ZoneFilterChanged());

                this.ZoneSelectors.Add(selector);
            }
        }

        private void ZoneFilterChanged()
        {
            if (!this.isInitialize)
            {
                this.Model.ZoneFilter = string.Join(",",
                    this.ZoneSelectors
                        .Where(x => x.IsSelected)
                        .Select(x => x.ID)
                        .ToArray());

                this.RaisePropertyChanged(nameof(this.IsZoneFiltered));
                Task.Run(() => TableCompiler.Instance.CompileSpells());
            }
        }

        private ICommand clearZoneFilterCommand;

        public ICommand ClearZoneFilterCommand =>
            this.clearZoneFilterCommand ?? (this.clearZoneFilterCommand = new DelegateCommand(() =>
            {
                try
                {
                    this.isInitialize = true;
                    foreach (var selector in this.ZoneSelectors)
                    {
                        selector.IsSelected = false;
                    }

                    this.Model.ZoneFilter = string.Empty;
                    this.RaisePropertyChanged(nameof(this.IsZoneFiltered));
                    Task.Run(() => TableCompiler.Instance.CompileSpells());
                }
                finally
                {
                    this.isInitialize = false;
                }
            }));

        #endregion Zone filter
    }
}
