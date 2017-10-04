using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ACT.SpecialSpellTimer.Utility;
using Microsoft.VisualBasic.FileIO;

namespace ACT.SpecialSpellTimer.Sound
{
    public class TTSDictionary
    {
        private const string SourceFileName = @"TTSDictionary.txt";

        #region Singleton

        private static TTSDictionary instance = new TTSDictionary();

        public static TTSDictionary Instance => instance;

        #endregion Singleton

        private string SourceFile => Path.Combine(this.ResourcesDirectory, SourceFileName);

        private readonly object locker = new object();
        private readonly Dictionary<string, string> ttsDictionary = new Dictionary<string, string>();

        public string ResourcesDirectory
        {
            get
            {
                // ACT‚ÌƒpƒX‚ðŽæ“¾‚·‚é
                var asm = Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    var actDirectory = Path.GetDirectoryName(asm.Location);
                    var resourcesUnderAct = Path.Combine(actDirectory, @"resources");

                    if (Directory.Exists(resourcesUnderAct))
                    {
                        return resourcesUnderAct;
                    }
                }

                // Ž©g‚ÌêŠ‚ðŽæ“¾‚·‚é
                var selfDirectory = PluginCore.Instance.Location ?? string.Empty;
                var resourcesUnderThis = Path.Combine(selfDirectory, @"resources");

                if (Directory.Exists(resourcesUnderThis))
                {
                    return resourcesUnderThis;
                }

                return string.Empty;
            }
        }

        public string ReplaceWordsTTS(
            string textToSpeak)
        {
            lock (this.locker)
            {
                foreach (var item in this.ttsDictionary)
                {
                    textToSpeak = textToSpeak.Replace(item.Key, item.Value);
                }
            }

            return textToSpeak;
        }

        public string ReplaceTTS(
            string textToSpeak)
        {
            lock (this.locker)
            {
                if (this.ttsDictionary.ContainsKey(textToSpeak))
                {
                    textToSpeak = this.ttsDictionary[textToSpeak];
                }

                return textToSpeak;
            }
        }

        public void Load()
        {
            if (!File.Exists(this.SourceFile))
            {
                return;
            }

            using (var sr = new StreamReader(this.SourceFile, new UTF8Encoding(false)))
            using (var tf = new TextFieldParser(sr)
            {
                CommentTokens = new string[] { "#" },
                Delimiters = new string[] { "\t", " " },
                TextFieldType = FieldType.Delimited,
                HasFieldsEnclosedInQuotes = true,
                TrimWhiteSpace = true
            })
            {
                lock (this.locker)
                {
                    this.ttsDictionary.Clear();
                }

                while (!tf.EndOfData)
                {
                    var fields = tf.ReadFields()
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToArray();

                    if (fields.Length <= 0)
                    {
                        continue;
                    }

                    var key = fields.Length > 0 ? fields[0] : string.Empty;
                    var value = fields.Length > 1 ? fields[1] : string.Empty;

                    if (!string.IsNullOrEmpty(key))
                    {
                        lock (this.locker)
                        {
                            this.ttsDictionary[key] = value;
                        }
                    }
                }
            }

            Logger.Write($"TTSDictionary loaded. {this.SourceFile}");
        }
    }
}
