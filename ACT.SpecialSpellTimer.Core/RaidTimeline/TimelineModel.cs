using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Config.Views;
using ACT.SpecialSpellTimer.Utility;
using FFXIV.Framework.Common;
using FFXIV.Framework.Globalization;
using Prism.Commands;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [XmlType(TypeName = "timeline")]
    [XmlInclude(typeof(TimelineDefaultModel))]
    [XmlInclude(typeof(TimelineActivityModel))]
    [XmlInclude(typeof(TimelineTriggerModel))]
    [XmlInclude(typeof(TimelineSubroutineModel))]
    public partial class TimelineModel :
        BindableBase
    {
        private string name = string.Empty;

        [XmlElement(ElementName = "name")]
        public string Name
        {
            get => this.name;
            set
            {
                if (this.SetProperty(ref this.name, value))
                {
                    this.RaisePropertyChanged(nameof(this.DisplayName));
                }
            }
        }

        private string revision = null;

        [XmlElement(ElementName = "rev")]
        public string Revision
        {
            get => this.revision;
            set => this.SetProperty(ref this.revision, value);
        }

        private string description = null;

        [XmlElement(ElementName = "description")]
        public string Description
        {
            get => this.description;
            set
            {
                var text = string.Empty;

                using (var sr = new StringReader(value))
                {
                    while (true)
                    {
                        var line = sr.ReadLine();
                        if (line == null)
                        {
                            break;
                        }

                        line = line.Trim();
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        text += !string.IsNullOrEmpty(text) ?
                            Environment.NewLine + line :
                            line;
                    }
                }

                this.SetProperty(ref this.description, text);
            }
        }

        private string zone = string.Empty;

        [XmlElement(ElementName = "zone")]
        public string Zone
        {
            get => this.zone;
            set
            {
                if (this.SetProperty(ref this.zone, value))
                {
                    this.RaisePropertyChanged(nameof(this.DisplayName));
                }
            }
        }

        private Locales locale = Locales.JA;

        [XmlElement(ElementName = "locale")]
        public Locales Locale
        {
            get => this.locale;
            set => this.SetProperty(ref this.locale, value);
        }

        [XmlIgnore]
        public string LocaleText => this.Locale.ToText();

        private string entry = null;

        [XmlElement(ElementName = "entry")]
        public string Entry
        {
            get => this.entry;
            set => this.SetProperty(ref this.entry, value);
        }

        private string startTrigger = null;

        [XmlElement(ElementName = "start")]
        public string StartTrigger
        {
            get => this.startTrigger;
            set
            {
                if (this.SetProperty(ref this.startTrigger, value))
                {
                    if (string.IsNullOrEmpty(this.startTrigger))
                    {
                        this.StartTriggerRegex = null;
                    }
                    else
                    {
                        this.StartTriggerRegex = new Regex(
                            this.startTrigger,
                            RegexOptions.Compiled |
                            RegexOptions.ExplicitCapture |
                            RegexOptions.IgnoreCase);
                    }
                }
            }
        }

        private Regex startTriggerRegex = null;

        [XmlIgnore]
        public Regex StartTriggerRegex
        {
            get => this.startTriggerRegex;
            private set => this.SetProperty(ref this.startTriggerRegex, value);
        }

        private string file = string.Empty;

        [XmlIgnore]
        public string File
        {
            get => this.file;
            private set
            {
                if (this.SetProperty(ref this.file, value))
                {
                    this.RaisePropertyChanged(nameof(this.FileName));
                }
            }
        }

        private bool isActive = false;

        [XmlIgnore]
        public bool IsActive
        {
            get => this.isActive;
            set => this.SetProperty(ref this.isActive, value);
        }

        [XmlIgnore]
        public string FileName => Path.GetFileName(this.File);

        private List<TimelineBase> elements = new List<TimelineBase>();

        [XmlIgnore]
        public IReadOnlyList<TimelineBase> Elements => this.elements;

        [XmlElement(ElementName = "default")]
        public TimelineDefaultModel[] Defaults
        {
            get => this.Elements.Where(x => x.TimelineType == TimelineElementTypes.Default).Cast<TimelineDefaultModel>().ToArray();
            set => this.AddRange(value);
        }

        [XmlElement(ElementName = "a")]
        public TimelineActivityModel[] Activities
        {
            get => this.Elements.Where(x => x.TimelineType == TimelineElementTypes.Activity).Cast<TimelineActivityModel>().OrderBy(x => x.Time).ToArray();
            set => this.AddRange(value);
        }

        [XmlElement(ElementName = "t")]
        public TimelineTriggerModel[] Triggers
        {
            get => this.Elements.Where(x => x.TimelineType == TimelineElementTypes.Trigger).Cast<TimelineTriggerModel>().ToArray();
            set => this.AddRange(value);
        }

        [XmlElement(ElementName = "s")]
        public TimelineSubroutineModel[] Subroutines
        {
            get => this.Elements.Where(x => x.TimelineType == TimelineElementTypes.Subroutine).Cast<TimelineSubroutineModel>().ToArray();
            set => this.AddRange(value);
        }

        private TimelineController controller;

        /// <summary>
        /// タイムラインの実行を制御するオブジェクト
        /// </summary>
        [XmlIgnore]
        public TimelineController Controller =>
            this.controller = (this.controller ?? new TimelineController(this));

        #region Methods

        public void Add(TimelineBase timeline)
        {
            timeline.Parent = this;
            this.elements.Add(timeline);
        }

        public void AddRange(IEnumerable<TimelineBase> timelines)
        {
            if (timelines != null)
            {
                foreach (var tl in timelines)
                {
                    this.Add(tl);
                }
            }
        }

        public static TimelineModel Load(
            string file)
        {
            if (!System.IO.File.Exists(file))
            {
                return null;
            }

            var tl = default(TimelineModel);

            using (var sr = new StreamReader(file, new UTF8Encoding(false)))
            {
                if (sr.BaseStream.Length > 0)
                {
                    var xs = new XmlSerializer(typeof(TimelineModel));
                    var data = xs.Deserialize(sr) as TimelineModel;
                    if (data != null)
                    {
                        tl = data;
                        tl.File = file;
                    }
                }
            }

            if (tl != null)
            {
                // 既定値を適用する
                tl.SetDefaultValues();
            }

            return tl;
        }

        public void Save(
            string file)
        {
            lock (this)
            {
                FileHelper.CreateDirectory(file);

                var ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                var sb = new StringBuilder();
                using (var sw = new StringWriter(sb))
                {
                    var xs = new XmlSerializer(this.GetType());
                    xs.Serialize(sw, this, ns);
                }

                sb.Replace("utf-16", "utf-8");
                sb.Replace("True", "true");
                sb.Replace("False", "false");

                System.IO.File.WriteAllText(
                    file,
                    sb.ToString() + Environment.NewLine,
                    new UTF8Encoding(false));
            }
        }

        #endregion Methods

        #region Default Values

        /// <summary>
        /// 規定の既定値の定義リスト
        /// </summary>
        private static readonly IList<TimelineDefaultModel> SuperDefaultValues = new[]
        {
            // アクティビティ
            NewDefault(TimelineElementTypes.Activity, "Enabled", true),
            NewDefault(TimelineElementTypes.Activity, "SyncOffsetStart", -12),
            NewDefault(TimelineElementTypes.Activity, "SyncOffsetEnd", 12),
            NewDefault(TimelineElementTypes.Activity, "NoticeDevice", NoticeDevices.Both),
            NewDefault(TimelineElementTypes.Activity, "NoticeOffset", -6),

            // トリガ
            NewDefault(TimelineElementTypes.Trigger, "Enabled", true),
            NewDefault(TimelineElementTypes.Trigger, "SyncCount", 0),
            NewDefault(TimelineElementTypes.Trigger, "NoticeDevice", NoticeDevices.Both),

            // サブルーチン
            NewDefault(TimelineElementTypes.Subroutine, "Enabled", true),
            NewDefault(TimelineElementTypes.Subroutine, "SyncCount", 0),
            NewDefault(TimelineElementTypes.Subroutine, "NoticeDevice", NoticeDevices.Both),

            // Load
            NewDefault(TimelineElementTypes.Load, "Enabled", true),
        };

        private void SetDefaultValues()
        {
            var defaults = this.Defaults.Union(SuperDefaultValues)
                .Where(x => (x.Enabled ?? true));

            foreach (var el in this.Elements)
            {
                setDefaultValuesToElement(el);

                if (el.TimelineType == TimelineElementTypes.Subroutine)
                {
                    var sub = el as TimelineSubroutineModel;
                    foreach (var el2 in sub.Statements)
                    {
                        setDefaultValuesToElement(el2);

                        if (el2.TimelineType == TimelineElementTypes.Trigger)
                        {
                            var tri = el2 as TimelineTriggerModel;
                            foreach (var el3 in tri.Statements)
                            {
                                setDefaultValuesToElement(el3);
                            }
                        }
                    }
                }

                if (el.TimelineType == TimelineElementTypes.Trigger)
                {
                    var tri = el as TimelineTriggerModel;
                    foreach (var el2 in tri.Statements)
                    {
                        setDefaultValuesToElement(el2);
                    }
                }
            }

            void setDefaultValuesToElement(TimelineBase element)
            {
                try
                {
                    foreach (var def in defaults
                        .Where(x => x.TargetElement == element.TimelineType))
                    {
                        var pi = GetPropertyInfo(element, def.TargetAttribute);
                        if (pi == null)
                        {
                            continue;
                        }

                        var value = pi.GetValue(element);
                        if (value == null)
                        {
                            object defValue = null;

                            var type = pi.PropertyType;

                            if (type.IsGenericType &&
                                type.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                type = Nullable.GetUnderlyingType(type);
                            }

                            if (!type.IsEnum)
                            {
                                defValue = Convert.ChangeType(def.Value, type);
                            }
                            else
                            {
                                defValue = Enum.Parse(type, def.Value, true);
                            }

                            if (defValue != null)
                            {
                                pi.SetValue(element, defValue);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("[TL] Load default values error.", ex);
                }
            }
        }

        private static PropertyInfo GetPropertyInfo(
            TimelineBase element,
            string fieldName)
        {
            const BindingFlags flag =
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.IgnoreCase;

            var info = default(PropertyInfo);

            var type = element.GetType();
            if (type == null)
            {
                return info;
            }

            // 通常のフィールド名からフィールド情報を取得する
            info = type.GetProperty(fieldName, flag);

            if (info != null)
            {
                return info;
            }

            // XML属性名からフィールド情報を取得する
            var pis = type.GetProperties(flag);

            foreach (var pi in pis)
            {
                var attr = Attribute.GetCustomAttributes(pi, typeof(XmlAttributeAttribute))
                    .FirstOrDefault() as XmlAttributeAttribute;
                if (attr != null)
                {
                    if (string.Equals(
                        attr.AttributeName,
                        fieldName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        info = pi;
                        break;
                    }
                }
            }

            return info;
        }

        private static TimelineDefaultModel NewDefault(
            TimelineElementTypes element,
            string attr,
            object value)
            => new TimelineDefaultModel()
            {
                TargetElement = element,
                TargetAttribute = attr,
                Value = value.ToString(),
                Enabled = true,
            };

        #endregion Default Values

        #region To View

        private CollectionViewSource activitySource;

        private CollectionViewSource ActivitySource =>
            this.activitySource ?? (this.activitySource = this.CreateActivityView());

        public ICollectionView ActivityView => this.ActivitySource?.View;

        private string subName = string.Empty;

        public string SubName
        {
            get => this.subName;
            set => this.SetProperty(ref this.subName, value);
        }

        public string DisplayName =>
            !string.IsNullOrEmpty(this.Name) ?
            this.Name :
            this.Zone;

        private bool isActivitiesVisible = true;

        public bool IsActivitiesVisible
        {
            get => this.isActivitiesVisible;
            set => this.SetProperty(ref this.isActivitiesVisible, value);
        }

        public void StopLive()
        {
            this.IsActivitiesVisible = false;
            /*
            this.ActivitySource.IsLiveFilteringRequested = false;
            this.ActivitySource.IsLiveSortingRequested = false;
            */
        }

        public void ResumeLive()
        {
            this.IsActivitiesVisible = true;
            /*
            this.ActivitySource.IsLiveFilteringRequested = true;
            this.ActivitySource.IsLiveSortingRequested = true;
            */
        }

        private CollectionViewSource CreateActivityView()
        {
            var cvs = new CollectionViewSource()
            {
                Source = this.Controller.ActivityLine,
                IsLiveSortingRequested = true,
                IsLiveFilteringRequested = true,
            };

            cvs.Filter += (x, y) =>
                y.Accepted = (y.Item as TimelineActivityModel).IsVisible;

            cvs.LiveFilteringProperties.Add(nameof(TimelineActivityModel.IsVisible));

            cvs.SortDescriptions.AddRange(new[]
            {
                new SortDescription()
                {
                    PropertyName = nameof(TimelineActivityModel.Seq),
                    Direction = ListSortDirection.Ascending,
                }
            });

            return cvs;
        }

        #endregion To View

        #region Commands

        private ICommand changeActiveCommand;

        public ICommand ChangeActiveCommand =>
            this.changeActiveCommand ?? (this.changeActiveCommand = new DelegateCommand<bool?>((isChecked) =>
            {
                if (!isChecked.HasValue)
                {
                    return;
                }

                if (isChecked.Value)
                {
                    var old = TimelineManager.Instance.TimelineModels.FirstOrDefault(x =>
                        x != this &&
                        x.IsActive);

                    if (old != null)
                    {
                        old.IsActive = false;
                        old.Controller.Unload();
                    }

                    this.Controller.Load();
                }
                else
                {
                    this.Controller.Unload();
                }
            }));

        private ICommand editCommand;

        public ICommand EditCommand =>
            this.editCommand ?? (this.editCommand = new DelegateCommand(() =>
            {
                if (System.IO.File.Exists(this.File))
                {
                    Process.Start(this.File);
                }
            }));

        private ICommand reloadCommand;

        public ICommand ReloadCommand =>
            this.reloadCommand ?? (this.reloadCommand = new DelegateCommand(async () =>
            {
                if (!System.IO.File.Exists(this.File))
                {
                    return;
                }

                try
                {
                    var tl = default(TimelineModel);
                    await Task.Run(() =>
                    {
                        tl = TimelineModel.Load(this.File);

                        if (tl != null)
                        {
                            this.elements.Clear();
                            this.AddRange(tl.Elements);
                        }
                    });

                    if (tl == null)
                    {
                        return;
                    }

                    this.File = tl.File;
                    this.Name = tl.Name;
                    this.Revision = tl.Revision;
                    this.Description = tl.Description;
                    this.Zone = tl.Zone;
                    this.Locale = tl.Locale;
                    this.Entry = tl.Entry;
                    this.StartTrigger = tl.StartTrigger;

                    if (this.IsActive)
                    {
                        this.Controller.Unload();
                        this.Controller.Load();
                    }

                    ModernMessageBox.ShowDialog(
                        "Timeline reloaded.",
                        "Timeline Manager");
                }
                catch (Exception ex)
                {
                    ModernMessageBox.ShowDialog(
                        "Timeline reload error !",
                        "Timeline Manager",
                        MessageBoxButton.OK,
                        ex);
                }
            }));

        #endregion Commands

        #region Dummy Timeline

        private static TimelineModel dummyTimeline;

        public static TimelineModel DummyTimeline =>
            dummyTimeline ?? (dummyTimeline = CreateDummyTimeline());

        public static TimelineModel CreateDummyTimeline(
            TimelineStyle testStyle = null)
        {
            var tl = new TimelineModel();

            tl.Name = "サンプルタイムライン";
            tl.Zone = "Hojoring Zone v1.0 (Ultimate)";

            if (testStyle == null)
            {
                testStyle = TimelineStyle.SuperDefaultStyle;
                if (!WPFHelper.IsDesignMode)
                {
                    testStyle = TimelineSettings.Instance.DefaultStyle;
                }
            }

            var act1 = new TimelineActivityModel()
            {
                Enabled = true,
                Seq = 1,
                Text = "デスセンテンス",
                Time = TimeSpan.FromSeconds(10.1),
                Parent = new TimelineSubroutineModel()
                {
                    Name = "PHASE-1"
                },
                StyleModel = testStyle,
            };

            var act2 = new TimelineActivityModel()
            {
                Enabled = true,
                Seq = 2,
                Text = "ツイスター",
                Time = TimeSpan.FromSeconds(16.1),
                StyleModel = testStyle,
            };

            var act3 = new TimelineActivityModel()
            {
                Enabled = true,
                Seq = 3,
                Text = "メガフレア",
                Time = TimeSpan.FromSeconds(20.1),
                CallTarget = "PHASE-2",
                StyleModel = testStyle,
            };

            tl.Controller.ActivityLine.Add(act1);
            tl.Controller.ActivityLine.Add(act2);
            tl.Controller.ActivityLine.Add(act3);

            for (int i = 1; i <= 13; i++)
            {
                var a = new TimelineActivityModel()
                {
                    Enabled = true,
                    Seq = act3.Seq + i,
                    Text = "アクション" + i,
                    Time = TimeSpan.FromSeconds(30 + i),
                    StyleModel = testStyle,
                };

                tl.Controller.ActivityLine.Add(a);
            }

            return tl;
        }

        #endregion Dummy Timeline
    }
}
