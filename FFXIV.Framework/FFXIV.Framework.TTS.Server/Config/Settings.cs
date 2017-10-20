using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Prism.Mvvm;

namespace FFXIV.Framework.TTS.Server.Config
{
    [Serializable]
    [XmlRoot(ElementName = "FFXIV.Framework.TTS.Server.Settings", Namespace = "")]
    public class Settings :
        BindableBase
    {
        /// <summary>
        /// LOCKオブジェクト
        /// </summary>
        private static readonly object locker = new object();

        #region Singleton

        private static Settings instance;

        public static Settings Instance =>
            instance ?? (instance = new Settings());

        private Settings()
        {
        }

        #endregion Singleton

        #region Serializer

        /// <summary>
        /// 保存先ファイル名
        /// </summary>
        public readonly string FileName = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\FFXIV.Framework.TTS.Server.config");

        /// <summary>
        /// シリアライザ
        /// </summary>
        private readonly XmlSerializer Serializer = new XmlSerializer(typeof(Settings));

        /// <summary>
        /// XMLライターSettings
        /// </summary>
        private readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings()
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
        };

        /// <summary>
        /// Load
        /// </summary>
        public void Load()
        {
            lock (locker)
            {
                var file = this.FileName;
                if (!File.Exists(file))
                {
                    this.Reset();
                    this.Save();
                    return;
                }

                var fi = new FileInfo(file);
                if (fi.Length <= 0)
                {
                    this.Reset();
                    this.Save();
                    return;
                }

                using (var xr = XmlReader.Create(file))
                {
                    var data = this.Serializer.Deserialize(xr) as Settings;
                    if (data != null)
                    {
                        instance = data;
                    }
                }
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        public void Save()
        {
            lock (locker)
            {
                if (!Directory.Exists(Path.GetDirectoryName(this.FileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(this.FileName));
                }

                using (var xw = XmlWriter.Create(
                    this.FileName,
                    this.XmlWriterSettings))
                {
                    this.Serializer.Serialize(xw, instance);
                }
            }
        }

        #endregion Serializer

        #region Data

        private float cevioVolumeGain;

        public float CevioVolumeGain
        {
            get => this.cevioVolumeGain;
            set => this.SetProperty(ref this.cevioVolumeGain, value);
        }

        #endregion Data

        #region Data - Default

        /// <summary>
        /// 初期値
        /// </summary>
        public static readonly Dictionary<string, object> DefaultValues = new Dictionary<string, object>()
        {
            { nameof(Settings.CevioVolumeGain), 2.1f },
        };

        /// <summary>
        /// 初期値に戻す
        /// </summary>
        public void Reset()
        {
            lock (locker)
            {
                var pis = this.GetType().GetProperties();
                foreach (var pi in pis)
                {
                    try
                    {
                        var defaultValue =
                            DefaultValues.ContainsKey(pi.Name) ?
                            DefaultValues[pi.Name] :
                            null;

                        if (defaultValue != null)
                        {
                            pi.SetValue(this, defaultValue);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine($"Settings Reset Error: {pi.Name}");
                    }
                }
            }
        }

        #endregion Data - Default
    }
}
