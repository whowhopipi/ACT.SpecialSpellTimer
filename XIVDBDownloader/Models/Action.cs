using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XIVDBDownloader.Models
{
    [DataContract]
    public class ActionData
    {
        public string ClassJob => this.ClassJobID.ToString();
        public string ClassJobCategory => this.ClassJobCategoryID.ToString();

        [DataMember(Name = "classjob_category", Order = 2)]
        public int ClassJobCategoryID { get; set; }

        [DataMember(Name = "classjob", Order = 1)]
        public int ClassJobID { get; set; }

        [DataMember(Name = "icon", Order = 3)]
        public int IconID { get; set; }

        [DataMember(Name = "id", Order = 4)]
        public int ID { get; set; }

        [DataMember(Name = "name", Order = 5)]
        public string Name { get; set; }
    }

    public class ActionModel :
        XIVDBApiBase<IList<ActionData>>
    {
        public override string Uri =>
            @"https://api.xivdb.com/action?columns=id,name,icon,classjob,classjob_category";

        public void SaveToCSV(
            string file)
        {
            if (this.ResultList == null ||
                this.ResultList.Count < 1)
            {
                return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            }

            var buffer = new StringBuilder(5120);

            using (var sw = new StreamWriter(file, false, new UTF8Encoding(false)))
            {
                sw.WriteLine(
                    $"ID,ID_HEX,ClassJob,ClassJobCategory,Name");

                var orderd =
                    from x in this.ResultList
                    orderby
                    x.ClassJobCategoryID,
                    x.ID
                    select
                    x;

                foreach (var action in orderd)
                {
                    buffer.AppendLine(
                        $"{action.ID},{action.ID:X4},{action.ClassJob},{action.ClassJobCategory},{action.Name}");

                    if (buffer.Length >= 5120)
                    {
                        sw.Write(buffer.ToString());
                        buffer.Clear();
                    }
                }

                if (buffer.Length > 0)
                {
                    sw.Write(buffer.ToString());
                }
            }
        }
    }
}
