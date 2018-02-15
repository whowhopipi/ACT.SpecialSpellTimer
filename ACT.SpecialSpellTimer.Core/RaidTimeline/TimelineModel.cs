using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FFXIV.Framework.Common;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [XmlType(TypeName = "timeline")]
    [XmlInclude(typeof(TimelineDefaultModel))]
    [XmlInclude(typeof(TimelineActivityModel))]
    [XmlInclude(typeof(TimelineTriggerModel))]
    [XmlInclude(typeof(TimelineSubroutineModel))]
    public class TimelineModel :
        BindableBase
    {
        private string zone = string.Empty;

        [XmlAttribute(AttributeName = "zone")]
        public string Zone
        {
            get => this.zone;
            set => this.SetProperty(ref this.zone, value);
        }

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
            get => this.Elements.Where(x => x.TimelineType == TimelineElementTypes.Activity).Cast<TimelineActivityModel>().ToArray();
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

        #region Methods

        public void Add(TimelineBase timeline)
        {
            timeline.Parent = this;
            this.elements.Add(timeline);
        }

        public void AddRange(IEnumerable<TimelineBase> timelines)
        {
            foreach (var tl in timelines)
            {
                this.Add(tl);
            }
        }

        public static TimelineModel Load(
            string file)
        {
            if (!File.Exists(file))
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
                    }
                }
            }

            if (tl == null)
            {
                return tl;
            }

            void setSubroutinesParent(
                TimelineSubroutineModel sub)
            {
                foreach (var state in sub.Statements
                    .Where(x =>
                        x.TimelineType == TimelineElementTypes.Default ||
                        x.TimelineType == TimelineElementTypes.Activity ||
                        x.TimelineType == TimelineElementTypes.Trigger))
                {
                    state.Parent = sub;
                }

                foreach (var subsub in sub.Statements
                    .Where(x =>
                        x.TimelineType == TimelineElementTypes.Subroutine))
                {
                    subsub.Parent = sub;
                    setSubroutinesParent(subsub as TimelineSubroutineModel);
                }
            }

            foreach (var el in tl.Elements)
            {
                switch (el.TimelineType)
                {
                    case TimelineElementTypes.Default:
                    case TimelineElementTypes.Activity:
                    case TimelineElementTypes.Trigger:
                        el.Parent = tl;
                        break;

                    case TimelineElementTypes.Subroutine:
                        el.Parent = tl;
                        setSubroutinesParent(el as TimelineSubroutineModel);
                        break;
                }
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

                File.WriteAllText(
                    file,
                    sb.ToString() + Environment.NewLine,
                    new UTF8Encoding(false));
            }
        }

        #endregion Methods
    }
}
