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
    public partial class SpellConfigViewModel :
        BindableBase
    {
        public SpellConfigViewModel() : this(new Spell())
        {
        }

        public SpellConfigViewModel(
            Spell model)
            => this.Model = model;

        private Spell model;

        public Spell Model
        {
            get => this.model;
            set
            {
                if (this.SetProperty(ref this.model, value))
                {
                    try
                    {
                        this.isInitialize = true;

                        // ジョブ・ゾーン・前提条件のセレクタを初期化する
                        this.SetJobSelectors();
                        this.SetZoneSelectors();
                        PreconditionSelectors.Instance.SetModel(this.model);

                        // Designモード？（Visualタブがアクティブか？）
                        this.model.IsDesignMode = this.IsActiveVisualTab;
                        Task.Run(() => TableCompiler.Instance.CompileSpells());
                        this.SwitchDesignGrid();
                    }
                    finally
                    {
                        this.isInitialize = false;
                    }

                    this.RaisePropertyChanged(nameof(this.IsJobFiltered));
                    this.RaisePropertyChanged(nameof(this.IsZoneFiltered));
                }
            }
        }

        private bool isInitialize = false;

        private ICommand simulateMatchCommand;

        public ICommand SimulateMatchCommand =>
            this.simulateMatchCommand ?? (this.simulateMatchCommand = new DelegateCommand(() =>
            {
                this.Model.SimulateMatch();
            }));

        private bool isActiveVisualTab;

        public bool IsActiveVisualTab
        {
            get => this.isActiveVisualTab;
            set
            {
                if (this.SetProperty(ref this.isActiveVisualTab, value))
                {
                    this.Model.IsDesignMode = this.isActiveVisualTab;
                    Task.Run(() => TableCompiler.Instance.CompileSpells());
                    this.SwitchDesignGrid();
                }
            }
        }

        private void SwitchDesignGrid()
        {
            var showGrid =
                TableCompiler.Instance.SpellList.Any(x => x.IsDesignMode) ||
                TableCompiler.Instance.TickerList.Any(x => x.IsDesignMode);

            Settings.Default.VisibleDesignGrid = showGrid;
        }

        #region Job filter

        public bool IsJobFiltered => !string.IsNullOrEmpty(this.Model?.JobFilter);

        private static List<JobSelector> jobSelectors;

        public List<JobSelector> JobSelectors => jobSelectors;

        private void SetJobSelectors()
        {
            if (jobSelectors == null)
            {
                jobSelectors = new List<JobSelector>();

                foreach (var job in
                    from x in Jobs.List
                    where
                    x.ID != JobIDs.Unknown &&
                    x.ID != JobIDs.ADV
                    orderby
                    x.Role.ToSortOrder(),
                    x.ID
                    select
                    x)
                {
                    jobSelectors.Add(new JobSelector(job));
                }
            }

            var jobFilters = this.Model.JobFilter?.Split(',');
            foreach (var selector in this.JobSelectors)
            {
                selector.IsSelected = jobFilters.Contains(((int)selector.Job.ID).ToString());
                selector.SelectedChangedDelegate = this.JobFilterChanged;
            }
        }

        private void JobFilterChanged()
        {
            if (!this.isInitialize)
            {
                this.Model.JobFilter = string.Join(",",
                    this.JobSelectors
                        .Where(x => x.IsSelected)
                        .Select(x => ((int)x.Job.ID).ToString())
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

        private static List<ZoneSelector> zoneSelectors;

        public List<ZoneSelector> ZoneSelectors => zoneSelectors;

        private void SetZoneSelectors()
        {
            if (zoneSelectors == null ||
                zoneSelectors.Count <= 0)
            {
                zoneSelectors = new List<ZoneSelector>();

                foreach (var zone in
                    from x in FFXIVPlugin.Instance?.ZoneList
                    orderby
                    x.IsAddedByUser ? 0 : 1,
                    x.Rank,
                    x.ID descending
                    select
                    x)
                {
                    var selector = new ZoneSelector(
                        zone.ID.ToString(),
                        zone.Name);

                    zoneSelectors.Add(selector);
                }
            }

            var zoneFilters = this.Model.ZoneFilter?.Split(',');
            foreach (var selector in this.ZoneSelectors)
            {
                selector.IsSelected = zoneFilters.Contains(selector.ID);
                selector.SelectedChangedDelegate = this.ZoneFilterChanged;
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

        #region Precondition selector

        public PreconditionSelectors PreconditionSelectors => PreconditionSelectors.Instance;

        private ICommand clearPreconditionsCommand;

        public ICommand ClearPreconditionsCommand =>
            this.clearPreconditionsCommand ?? (this.clearPreconditionsCommand = new DelegateCommand(() =>
            {
                PreconditionSelectors.Instance.ClearSelect();
            }));

        #endregion Precondition selector
    }
}
