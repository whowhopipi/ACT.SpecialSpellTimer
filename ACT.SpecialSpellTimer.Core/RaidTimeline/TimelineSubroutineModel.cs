using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [XmlType(TypeName = "s")]
    [XmlInclude(typeof(TimelineDefaultModel))]
    [XmlInclude(typeof(TimelineActivityModel))]
    [XmlInclude(typeof(TimelineTriggerModel))]
    [XmlInclude(typeof(TimelineSubroutineModel))]
    public class TimelineSubroutineModel :
        TimelineBase
    {
        [XmlIgnore]
        public override TimelineElementTypes TimelineType => TimelineElementTypes.Subroutine;

        private List<TimelineBase> statements = new List<TimelineBase>();

        [XmlIgnore]
        public IReadOnlyList<TimelineBase> Statements => this.statements;

        [XmlElement(ElementName = "a")]
        public TimelineActivityModel[] Activities
        {
            get => this.statements.Where(x => x.TimelineType == TimelineElementTypes.Activity).Cast<TimelineActivityModel>().ToArray();
            set => this.AddRange(value);
        }

        [XmlElement(ElementName = "t")]
        public TimelineTriggerModel[] Triggers
        {
            get => this.statements.Where(x => x.TimelineType == TimelineElementTypes.Trigger).Cast<TimelineTriggerModel>().ToArray();
            set => this.AddRange(value);
        }

        #region Methods

        public void Add(TimelineBase timeline)
        {
            timeline.Parent = this;
            this.statements.Add(timeline);
        }

        public void AddRange(IEnumerable<TimelineBase> timelines)
        {
            foreach (var tl in timelines)
            {
                this.Add(tl);
            }
        }

        #endregion Methods
    }
}
