using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ACT.SpecialSpellTimer.Utility;
using FFXIV.Framework.Common;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    public class TimelineManager
    {
        #region Logger

        private NLog.Logger AppLogger => FFXIV.Framework.Common.AppLog.DefaultLogger;

        #endregion Logger

        #region Singleton

        private static TimelineManager instance;

        public static TimelineManager Instance =>
            instance ?? (instance = new TimelineManager());

        #endregion Singleton

        public string TimelineDirectory => DirectoryHelper.FindSubDirectory(@"resources\timeline");

        private ObservableCollection<TimelineModel> timelineModels = new ObservableCollection<TimelineModel>();

        public ObservableCollection<TimelineModel> TimelineModels => this.timelineModels;

        public void LoadTimelineModels()
        {
            var dir = this.TimelineDirectory;
            if (!Directory.Exists(dir))
            {
                return;
            }

            var list = new List<TimelineModel>();
            foreach (var file in Directory.GetFiles(dir, "*.xml"))
            {
                try
                {
                    var tl = TimelineModel.Load(file);
                    list.Add(tl);
                }
                catch (Exception ex)
                {
                    this.AppLogger.Error(
                        ex,
                        $"[TL] Load error. file={file}");

                    throw new FileLoadException(
                        $"Timeline file Load error.\n{Path.GetFileName(file)}",
                        ex);
                }
            }

            WPFHelper.Invoke(() =>
            {
                foreach (var tl in this.TimelineModels)
                {
                    if (tl.IsActive)
                    {
                        tl.IsActive = false;
                        tl.Controller.Unload();
                    }
                }

                this.TimelineModels.Clear();
                this.TimelineModels.AddRange(list.OrderBy(x => x.FileName));
            });

            return;
        }
    }
}
