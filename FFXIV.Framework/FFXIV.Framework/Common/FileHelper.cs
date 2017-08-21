using System.IO;

namespace FFXIV.Framework.Common
{
    public class FileHelper
    {
        public static void CreateDirectory(
            string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
