using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Sound;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Bridge;
using Prism.Mvvm;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Models
{
    [Serializable]
    public class AdvancedNoticeConfig :
        BindableBase,
        ICloneable
    {
        private bool isEnabled;

        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetProperty(ref this.isEnabled, value);
        }

        private bool toMainDevice = true;

        public bool ToMainDevice
        {
            get => this.toMainDevice;
            set => this.SetProperty(ref this.toMainDevice, value);
        }

        private bool toSubDevice;

        public bool ToSubDevice
        {
            get => this.toSubDevice;
            set => this.SetProperty(ref this.toSubDevice, value);
        }

        private bool toDicordTextChat;

        public bool ToDicordTextChat
        {
            get => this.toDicordTextChat;
            set => this.SetProperty(ref this.toDicordTextChat, value);
        }

        public void PlayWave(string wave) => PlayWaveCore(wave, this);

        public void Speak(string tts) => SyncSpeak(tts, this);

        public const string SyncKeyword = "/sync";

        [XmlIgnore]
        private static readonly Regex SyncRegex = new Regex(
            $@"{SyncKeyword} (?<priority>\d+?) (?<text>.*)",
            RegexOptions.Compiled |
            RegexOptions.Multiline);

        [XmlIgnore]
        private static readonly List<SyncTTS> SyncList = new List<SyncTTS>();

        [XmlIgnore]
        private static Timer syncSpeakTimer;

        [XmlIgnore]
        private static AdvancedNoticeConfig lastConfig;

        private static void PlayWaveCore(
            string wave,
            AdvancedNoticeConfig config)
        {
            if (string.IsNullOrEmpty(wave))
            {
                return;
            }

            if (!File.Exists(wave))
            {
                return;
            }

            if (!config.IsEnabled)
            {
                ActGlobals.oFormActMain.PlaySound(wave);
                return;
            }

            if (config.ToMainDevice)
            {
                PlayBridge.Instance.PlayMainDeviceDelegate?.Invoke(wave);
            }

            if (config.ToSubDevice)
            {
                PlayBridge.Instance.PlaySubDeviceDelegate?.Invoke(wave);
            }
        }

        private static void SpeakCore(
            string tts,
            AdvancedNoticeConfig config)
        {
            if (string.IsNullOrEmpty(tts))
            {
                return;
            }

            tts = TTSDictionary.Instance.ReplaceWordsTTS(tts);

            if (!config.IsEnabled)
            {
                ActGlobals.oFormActMain.TTS(tts);
                return;
            }

            if (config.ToMainDevice)
            {
                PlayBridge.Instance.PlayMainDeviceDelegate?.Invoke(tts);
            }

            if (config.ToSubDevice)
            {
                PlayBridge.Instance.PlaySubDeviceDelegate?.Invoke(tts);
            }

            if (config.ToDicordTextChat)
            {
                DiscordBridge.Instance.SendMessageDelegate?.Invoke(tts);
            }
        }

        private static void SyncSpeak(
            string tts,
            AdvancedNoticeConfig config)
        {
            // シンクロTTSじゃない？
            if (!tts.Contains(SyncKeyword))
            {
                SpeakCore(tts, config);
                return;
            }

            var match = SyncRegex.Match(tts);
            if (!match.Success)
            {
                // シンクロTTSじゃない
                SpeakCore(tts, config);
                return;
            }

            var value = string.Empty;
            value = match.Groups["priority"].Value;

            double priority;
            if (!double.TryParse(value, out priority))
            {
                SpeakCore(tts, config);
                return;
            }

            var speakText = match.Groups["text"].Value;
            if (string.IsNullOrEmpty(speakText))
            {
                return;
            }

            var period = Settings.Default.UILocale == Locales.JA ? "。" : ".";
            if (speakText.EndsWith(period))
            {
                speakText += period;
            }

            lock (SyncList)
            {
                if (syncSpeakTimer == null)
                {
                    syncSpeakTimer = new Timer(50);
                    syncSpeakTimer.Elapsed += SyncSpeakTimerOnElapsed;
                }

                syncSpeakTimer.Stop();
                SyncList.Add(new SyncTTS(priority, speakText));
                lastConfig = config;
                syncSpeakTimer.Start();
            }
        }

        private static void SyncSpeakTimerOnElapsed(
            object sender,
            ElapsedEventArgs e)
        {
            lock (SyncList)
            {
                var text = string.Empty;

                var texts =
                    from x in SyncList
                    where
                    !string.IsNullOrEmpty(x.Text)
                    orderby
                    x.Priority
                    select
                    x.Text;

                text = string.Join(Environment.NewLine, texts);

                SyncList.Clear();

                var config = lastConfig;
                if (config == null)
                {
                    return;
                }

                SpeakCore(text, config);
            }
        }

        public object Clone() => this.MemberwiseClone();

        public class SyncTTS
        {
            public SyncTTS()
            {
            }

            public SyncTTS(
                double priority,
                string text)
            {
                this.Priority = priority;
                this.Text = text;
            }

            public double Priority { get; set; }

            public string Text { get; set; }
        }
    }
}
